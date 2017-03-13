using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Client
{
    public class Server
    {
        //private TcpClient _TCPclient;
        private Socket _socket;
        private string _address;
        private Int32 _port;
        private DateTime _connectionTime;   //tempo in cui la connessione è attiva
        private LinkedList<OpenWindow> _openWindows;    //linkedList può aggiungere in testa o in coda o in punti specifici
        
        public Server()
        {
            IsValid = false;

        }

        public Boolean IsValid
        {
            get;
            set;
        }

        public void SetAddressAndPort(string address, Int32 port)
        {
            _address = address;
            _port = port;
        }

        public Task<string> Startup()
        {
            //MessageBox.Show("server is connecting", "Prova", MessageBoxButton.OK, MessageBoxImage.None);
            try
            {
                //IPHostEntry hostInfo = Dns.GetHostEntry(_address);
                //IPAddress clientAddr = hostInfo.AddressList[0];
                //First permette di prendere il primo ip di tipo ipv4
                IPAddress clientAddr = Dns.GetHostEntry(_address).AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                var clientEndPoint = new IPEndPoint(clientAddr, _port);

                //clientAddr.AddressFamily serve a specificare il socket giusto
                _socket = new Socket(clientAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(clientEndPoint);
                //_TCPclient = new TcpClient();
                //_TCPclient.Connect(clientAddr, _port);
                _connectionTime = DateTime.Now;
            }
            catch (SocketException se)
            {   //An error occurred when attempting to access the socket.
                return Task.FromResult(se.Message);
            }
            catch (ObjectDisposedException ode)
            {   //The Socket has been closed 
                return Task.FromResult(ode.Message);
            }
            catch (Exception e)
            {
                return Task.FromResult(e.Message);
            }
            return Task.FromResult("Connected");
        }

        /** Riceve una stringa json da cui estrae nome processo, icona e tipo di evento (nuova finestra, chiusura finestra, cambio focus)
        *   Ritorna true se l'operazione e' andata a buon fine, false se ci sono errori
        **/
        public Task<bool> Receive(out StringBuilder json)
        {
            try {
                NetworkStream nws = new NetworkStream(_socket);
                byte[] lengthData = new byte[4];
                
                //leggo la dimensione del json            
                int bytesRead = nws.Read(lengthData, 0, 4);
                //leggo i byte del file
                int bytesToRead = BitConverter.ToInt32(lengthData, 0);
                //int totalBytes = 0;
                byte[] buffer = new byte[bytesToRead];

                //leggo i dati in arrivo
                json = new StringBuilder();
                //int dataRead = 0;
                nws.Read(buffer, 0, (int) bytesToRead);
                string data = Encoding.ASCII.GetString(buffer.ToArray());
                json.Append(data);
                    //totalBytes += dataRead;
            }
            catch (ObjectDisposedException ode)
            {
                //il socket è chiuso o è impossibile leggere dalla rete
                //la finestra deve diventare grigia e il client deve riprovare a connettersi al server
                MsgException = ode.Message;
                json = null;
                return Task.FromResult<bool>(false);
            }
            catch (Exception e)
            {
                MsgException = e.Message;
                json = null;
                return Task.FromResult<bool>(false);
            }

            //Console.WriteLine("json = " + json.ToString());
            return Task.FromResult(true);
        }

        /** Converte l'img codificata in base64 in oggetto Image
        */
        public Image Base64ToImage(string base64String)
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

        public void Close()
        {
            _socket.Close();
        }

        public void SetSocketOptions()
        {
            if (_socket.Poll(1000, SelectMode.SelectRead)) {
                try {
                    //per chiudere la connessione in modo "grazioso" ma senza attendere la fine delle spedizioni
                    _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                    //per mantenere attiva la connessione, spedendo pacchetti ogni tanto
                    _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                }
                catch (Exception e)
                {
                    //qui provo a riconnettermi per 3 volte e intanto metto grigia la schermata serverElement
                }
            }
        }

        public string Name()
        {
            return _address;
        }

        public string MsgException
        {
            get;
            set;
        }
    }
}