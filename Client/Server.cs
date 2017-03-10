using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.IO;

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
            MessageBox.Show("server is connecting", "Prova", MessageBoxButton.OK, MessageBoxImage.None);
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

        public Task<bool> Receive(ref string json)
        {
            try {
                NetworkStream nws = new NetworkStream(_socket);
                //byte[] lengthData = new byte[4];
                
                //leggo la dimensione del json            
                //int bytesRead = nws.Read(lengthData, 0, 4);
                //if(bytesRead <= 0)  //non ho ricevuto nulla
                /*{
                    return Task.FromResult<bool>(false);
                }*/
                //leggo i byte del file
                //int bytesToRead = BitConverter.ToInt32(lengthData, 0);
                //int totalBytes = 0;
                byte[] buffer = new byte[256];

                //leggo i dati in arrivo
                StringBuilder sbjson = new StringBuilder();
                int dataRead = 0;
                do
                {
                    nws.Read(buffer, 0, 256);
                    List<byte> actualBuffer = (new List<byte>(buffer)).GetRange(0, dataRead);
                    string data = Encoding.UTF8.GetString(actualBuffer.ToArray());
                    Console.WriteLine("Raw data: {0}", data);
                    sbjson.Append(data);
                 } while (nws.DataAvailable);
                json = sbjson.ToString();
            }
            catch (ObjectDisposedException ode)
            {
                //il socket è chiuso o è impossibile leggere dalla rete
                //la finestra deve diventare grigia e il client deve riprovare a connettersi al server
                MsgException = ode.Message;
                return Task.FromResult<bool>(false);
            }
            catch (Exception e)
            {
                MsgException = e.Message;
                return Task.FromResult<bool>(false);
            }
          
            return Task.FromResult<bool>(true);
        }

        public Task<bool> ReadReceived(string filename)
        {
            return Task.FromResult<bool>(false);
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