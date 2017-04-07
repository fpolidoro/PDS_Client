using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace Client
{
    public class Server : IDisposable
    {
        //private TcpClient _TCPclient;
        private Socket _socket;
        private string _address;
        private Int32 _port;
        private DateTime _connectionTime;   //tempo in cui la connessione è attiva
        private int _connectionsCounter; //contatore per il # di volte di tentata riconnessione al server
        //private LinkedList<OpenWindow> _openWindows;    //linkedList può aggiungere in testa o in coda o in punti specifici
        private ServerElement _parentGUIElement;
        private ManualResetEvent CloseEvent = new ManualResetEvent(false); //permette di fermare la ricezione

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

        public string GetAddress() {
            return _address;
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
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
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
            Debug.WriteLine("Receive è partita.");
            try
            {
                NetworkStream nws = new NetworkStream(_socket);
                StringBuilder json = new StringBuilder();
                string received;
                byte[] bufferLength = new byte[4];
                byte[] buffer;
                int length;
                int totalRead;
                int read;
                Action<string> addToList = _parentGUIElement.pendingJSONs.Add;
                Action<string> notifySocketStatus = _parentGUIElement.SocketStatusChanged;

                while (!CloseEvent.WaitOne(0))
                {//ciclo fino a quando ricevo uno stop dall'esterno oppure ottengo un errore dal socket
                    totalRead = 0;
                    length = 0;
                    //Debug.WriteLine("Dentro al while di receive.");
                    try
                    {
                        /*if (!nws.DataAvailable)
                        {
                            Thread.Sleep(1);
                        }
                        else */if ((read = nws.Read(bufferLength, 0, 4)) >= 0)
                        {
                            if (read == 0)
                            {//il server ha chiuso la connessione, non ha senso riconnettersi perchè ha proprio fatto socket close
                                Debug.WriteLine("Connessione chiusa gentilmente dalla controparte");
                                _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "SocketGentlyDisposed");
                                CloseEvent.Set();
                                //devo ancora specificare un flag per dire che il server si è disconnesso gentilmente e quindi non si
                                //tenteranno le 3 riconnessioni, che invece si cercheranno di fare nel caso di objectDisposedException
                                break;
                            }
                            // Raise the DataReceived event w/ data...
                            length = BitConverter.ToInt32(bufferLength, 0);
                            if (length < 10000 && length > 0)
                            {
                                Debug.WriteLine("length = {0}", length);
                                buffer = new byte[length];
                                read = 0;
                                while (totalRead < buffer.Length)
                                {
                                    read = nws.Read(buffer, totalRead, buffer.Length - totalRead);
                                    totalRead += read;
                                    received = Encoding.ASCII.GetString(buffer);
                                    json.Append(received);
                                }
                                _parentGUIElement.Dispatcher.BeginInvoke(addToList, json.ToString());
                                json.Clear();   //ripulisco la stringa contenente il json
                                Debug.WriteLine("pendingJSONs ha {0} elementi.", _parentGUIElement.pendingJSONs.Count);
                            }
                            else
                            {   //la dimensione del json è eccessivamente grande o negativa, quindi chiudo il socket
                                Debug.WriteLine("La dimensione del json è eccessivamente grande o negativa, quindi chiudo il socket");
                                CloseEvent.Set();
                            }
                        }
                        /*else
                        {
                            // The connection has closed gracefully, so stop the
                            // thread.
                            Debug.WriteLine("Connessione chiusa dalla controparte");
                            CloseEvent.Set();
                        }*/
                    }
                    catch (IOException ioe) //qui è probabile che sia crashato il server, quindi bisogna tentare le 3 riconnessioni
                    {   //càpita quando il timeout scade senza che siano stati ricevuti dati
                        Debug.WriteLine("Connessione chiusa per timeout: {0}", MsgException);
                        MsgException = ioe.Message;
                        _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "IOException");
                        break;
                    }
                    catch (ObjectDisposedException ode) {
                        Debug.WriteLine("Connessione chiusa per objectDisposed: {0}", MsgException);
                        MsgException = ode.Message;
                        _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "ObjectDisposedException");
                        break;
                    }
                }
                Debug.WriteLine("uscito dal while.waitone");
            }
            catch (Exception e)
            {
                MsgException = e.Message;
                Debug.WriteLine("Eccezione nel try-catch esterno: {0}",MsgException);
            }
            finally
            {
                Debug.WriteLine("Finally");
                Close();
            }
            ////Debug.WriteLine("json = " + json.ToString());
            return;
        }

        public void StopReceive() {
            CloseEvent.Set();
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