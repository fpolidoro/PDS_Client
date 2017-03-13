using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    //[DataContract]
    public class Application
    {
        //[DataMember]
        public String Icon { get; set; }
        public String WindowName { get; set; }
        public String Status { get; set; }
    }

    public class SynchronousSocketListener
    {

        // Incoming data from the client.  
        public static string data = null;

        public static void StartListening()
        {
            //creo le finestre da mandare
            LinkedList <Application> listOfApps = CreateJSONWindows();

            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 3301);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    Console.WriteLine("Connected.");
                    //System.Threading.Thread.Sleep(5000);
                    Console.WriteLine("Starting to send data..");
                    data = null;

                    // An incoming connection needs to be processed.  
                    int i = 0;
                    //while (i < 3 && listOfApps != null)
                    //{
                        //bytes = new byte[1024];
                        //int bytesRec = handler.Receive(bytes);
                        string serializedJson = JsonConvert.SerializeObject(listOfApps.ElementAt(i));
                   
                        i++;
                        byte[] msg = Encoding.ASCII.GetBytes(serializedJson);
                        byte[] msglength = BitConverter.GetBytes(msg.Length);
                        Console.WriteLine("msg: {0}, msglength: {1}", msg.Length, msglength.Length);
                        handler.Send(msglength);
                        handler.Send(msg);
                        //data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        //send del JSON
                        /*if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }*/
                        Console.WriteLine(msg);
                    //}

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);

                    // Echo the data back to the client.  
                    //byte[] msg = Encoding.ASCII.GetBytes(data);

                    //handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to base 64 string
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }

        public static LinkedList<Application> CreateJSONWindows()
        {
            try {
                Image img1 = Image.FromFile("C:/Users/fab/Documents/Visual Studio 2015/Projects/ConsoleApplication2/ConsoleApplication2/ico16_Computer.png");
                Image img2 = Image.FromFile("C:/Users/fab/Documents/Visual Studio 2015/Projects/ConsoleApplication2/ConsoleApplication2/ico16_Computer48x48.png");
                Image img3 = Image.FromFile("C:/Users/fab/Documents/Visual Studio 2015/Projects/ConsoleApplication2/ConsoleApplication2/ico16_ComputerMicro.png");

            var a1 = new Application
            {
                Icon = ImageToBase64(img1, ImageFormat.Png),
                WindowName = "Processo1",
                Status = "Evento1"
            };
            var a2 = new Application
            {
                Icon = ImageToBase64(img2, ImageFormat.Png),
                WindowName = "Processo2",
                Status = "Evento2"
            };
            var a3 = new Application
            {
                Icon = ImageToBase64(img3, ImageFormat.Png),
                WindowName = "Processo3",
                Status = "Evento3"
            };

            LinkedList<Application> apps = new LinkedList<Application>();
            apps.AddFirst(a3);
            apps.AddFirst(a2);
            apps.AddFirst(a1);
            return apps;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
