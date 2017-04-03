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
using System.Threading;
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
            Console.WriteLine("Porta su cui ascoltare: ");
            string portno = Console.ReadLine();
            int port = Convert.ToInt32(portno);
            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                Socket handler;
                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    handler = listener.Accept();
                    Console.WriteLine("Connected.");
                    //System.Threading.Thread.Sleep(5000);
                    Console.WriteLine("Starting to send data..");
                    data = null;

                    // An incoming connection needs to be processed.  
                    int i = 0;
                    while (i < listOfApps.Count && listOfApps != null)
                    {
                        if (i >= 4)
                        {   //per mandare il resto quando premo invio o un qualsiasi tasto (serve solo per debug del client)
                            Console.WriteLine("Tap for sending activities.");
                            Console.Read();
                        }

                        //serializzo il json e ottengo una stringa
                        string serializedJson = JsonConvert.SerializeObject(listOfApps.ElementAt(i));
                        //Console.WriteLine("JSON: {0}", serializedJson);
                        
                        //sempre per debug del client
                        if (i == 3) Thread.Sleep(10000);
                        sendJSON(handler, serializedJson);
                        i++;
                        /*if (i >= 4)
                        {
                            Application a4 = JsonConvert.DeserializeObject<Application>(serializedJson);
                            Image img = Base64ToImage(a4.Icon);
                        }*/
                    }

                    // Show the data on the console.  
                    //Console.WriteLine("Text received : {0}", data);

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

        //converte la stringa in byte[] e spedisce
        public static void sendJSON(Socket handler, string serializedJson)
        {
            byte[] msg = Encoding.ASCII.GetBytes(serializedJson);
            byte[] msglength = BitConverter.GetBytes(msg.Length);
            Console.WriteLine("msg: {0}, msglength: {1}", msg.Length, msglength.Length);
            handler.Send(msglength);
            handler.Send(msg);
            Console.WriteLine(msg);
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
                Image img2 = Image.FromFile("C:/Users/fab/Documents/Visual Studio 2015/Projects/ConsoleApplication2/ConsoleApplication2/22x22_alert.png");
                Image img3 = Image.FromFile("C:/Users/fab/Documents/Visual Studio 2015/Projects/ConsoleApplication2/ConsoleApplication2/32x32_world.png");

                var a1 = new Application
                {
                    Icon = ImageToBase64(img1, ImageFormat.Png),
                    WindowName = "Processo1",
                    Status = "NewWindow"
                };
                var a2 = new Application
                {
                    Icon = ImageToBase64(img2, ImageFormat.Png),
                    WindowName = "Processo2",
                    Status = "OnFocus"
                };
                var a3 = new Application
                {
                    Icon = ImageToBase64(img3, ImageFormat.Png),
                    WindowName = "Processo3",
                    Status = "NewWindow"
                };
                var a4 = new Application
                {
                    Icon = "",
                    WindowName = "Processo1",
                    Status = "OnFocus"
                };
                var a5 = new Application
                {
                    Icon = "",
                    WindowName = "Processo1",
                    Status = "Closed"
                };
                var a6 = new Application
                {
                    Icon = "",
                    WindowName = "Processo2",
                    Status = "OnFocus"
                };
                var a7 = new Application
                {
                    Icon = "",
                    WindowName = "Processo2",
                    Status = "Closed"
                };
                LinkedList<Application> apps = new LinkedList<Application>();
                apps.AddFirst(a3);
                apps.AddFirst(a2);
                apps.AddFirst(a1);
                apps.AddLast(a4);
                apps.AddLast(a5);
                apps.AddLast(a6);
                apps.AddLast(a7);

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
