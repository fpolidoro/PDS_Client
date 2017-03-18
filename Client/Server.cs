using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;

namespace Client
{
    public class Server : IDisposable
    {
        //private TcpClient _TCPclient;
        private Socket _socket;
        private string _address;
        private Int32 _port;
        private DateTime _connectionTime;   //tempo in cui la connessione è attiva
        //private LinkedList<OpenWindow> _openWindows;    //linkedList può aggiungere in testa o in coda o in punti specifici
        private ServerElement _parentGUIElement;

        public Server()
        {
            IsValid = false;
        }

        public Boolean IsValid
        {
            get;
            set;
        }

        public void SetGUIParentElement(ServerElement srv) {
            _parentGUIElement = srv;
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
                SetSocketOptions();
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
        public void Receive()
        {
            Console.WriteLine("Receive è partita.");
            /*try
            {*/
                NetworkStream nws = new NetworkStream(_socket);
                StringBuilder json = new StringBuilder();
                string received;
                byte[] bufferLength = new byte[4];
                byte[] buffer;
                int length;
                int totalRead;
                int read;
                Action<string> addToList = _parentGUIElement.pendingJSONs.Add;
                while(nws.DataAvailable)
                {//ciclo fino a quando ricevo uno stop dall'esterno oppure ottengo un errore dal socket
                    totalRead = 0;
                    length = 0;
                    Console.WriteLine("Dentro al while di receive.");
                    //ricevo la stringa json
                    read = nws.Read(bufferLength, 0, 4);
                    length = BitConverter.ToInt32(bufferLength, 0);
                    buffer = new byte[length];
                    read = 0;
                    while (totalRead < buffer.Length)
                    {
                        read = nws.Read(buffer, totalRead, buffer.Length - totalRead);
                        totalRead += read;
                        received = Encoding.ASCII.GetString(buffer);
                        json.Append(received);
                    }/* while (nws.DataAvailable);*/
                     //il json è stato ricevuto completamente, quindi lo aggiungo alla lista dei json
                     //(che è nel thread della GUI) da elaborare e visualizzare
                     /*_parentGUIElement.Dispatcher.BeginInvoke((Action)delegate ()
                     {
                         _parentGUIElement.pendingJSONs.Add(json.ToString());
                     });*/

                    _parentGUIElement.Dispatcher.BeginInvoke(addToList, json.ToString());
                    //_parentGUIElement.pendingJSONs.Add(json.ToString());
                    //Console.WriteLine("Received string: {0}", received);
                    Console.WriteLine("pendingJSONs ha {0} elementi.", _parentGUIElement.pendingJSONs.Count);
                    //Console.WriteLine("elemento: {0}", _parentGUIElement.pendingJSONs.Last());
                }
            /*}
            catch (ObjectDisposedException ode)
            {
                //il socket è chiuso o è impossibile leggere dalla rete
                //la finestra deve diventare grigia e il client deve riprovare a connettersi al server
                MsgException = ode.Message;
                return;
            }
            catch (Exception e)
            {
                MsgException = e.Message;
                return;
            }*/
            ////Console.WriteLine("json = " + json.ToString());
            return;
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
            _socket.Dispose();
            _socket.Close();
        }

        public void SetSocketOptions()
        {
            if (_socket.Poll(1000, SelectMode.SelectRead)) {
                //per chiudere la connessione in modo "grazioso" ma senza attendere la fine delle spedizioni
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                //per mantenere attiva la connessione, spedendo pacchetti ogni tanto
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            }
        }

        public string Name()
        {
            return _address + ": " + _port;
        }

        public string MsgException
        {
            get;
            set;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Per rilevare chiamate ridondanti

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminare lo stato gestito (oggetti gestiti).
                    if (_socket != null)
                    {
                        _socket.Dispose();
                    }
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di un finalizzatore.
                // TODO: impostare campi di grandi dimensioni su Null.

                disposedValue = true;
            }
        }

        // TODO: eseguire l'override di un finalizzatore solo se Dispose(bool disposing) include il codice per liberare risorse non gestite.
        // ~Server() {
        //   // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
        //   Dispose(false);
        // }

        // Questo codice viene aggiunto per implementare in modo corretto il criterio Disposable.
        public void Dispose()
        {
            // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
            Dispose(true);
            // TODO: rimuovere il commento dalla riga seguente se è stato eseguito l'override del finalizzatore.
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}