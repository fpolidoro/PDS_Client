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

        public DateTime ConnectionTime {
            get { return _connectionTime; }
        }

        public Server()
        {
            IsValid = false;
            _connectionsCounter = 0;
        }

        public int Counter
        {
            get { return _connectionsCounter; }
            set { _connectionsCounter = value; }
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
            CloseEvent.Reset(); //altrimenti il while della receive si interrompe subito
            _connectionsCounter = 0;
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
                if(_socket != null) _socket.Close();
                return Task.FromResult(se.Message);
            }
            catch (ObjectDisposedException ode)
            {   //The Socket has been closed
                if (_socket != null) _socket.Close(); 
                return Task.FromResult(ode.Message);
            }
            catch (Exception e)
            {
                if (_socket != null) _socket.Close();
                return Task.FromResult(e.Message);
            }
            _connectionsCounter++;
            return Task.FromResult("Connected");
        }

        private string TryReconnect() {
            bool tryAgain = true;
            string msg;
#if (DEBUG)
            Debug.WriteLine("Dentro a TryReconnect");
#endif
            while (tryAgain && _connectionsCounter < 4 && !CloseEvent.WaitOne(0))
            {
                try
                {
                    //First permette di prendere il primo ip di tipo ipv4
                    IPAddress clientAddr = Dns.GetHostEntry(_address).AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                    var clientEndPoint = new IPEndPoint(clientAddr, _port);

                    //clientAddr.AddressFamily serve a specificare il socket giusto
                    _socket = new Socket(clientAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    if (_connectionsCounter > 2)
                        Thread.Sleep(3000); //attendo un po' prima di riconnettere, per dare tempo all'altro eventualmente di tornare online
                    _socket.Connect(clientEndPoint);
                    tryAgain = false;
#if (DEBUG)
                    Debug.WriteLine("Riconnessione OK");
#endif
                }
                catch (SocketException)
                {   //An error occurred while attempting to access the socket.
                    if (_socket != null) _socket.Close();
#if (DEBUG)
                    Debug.WriteLine("TryReconnect: SocketException");
#endif
                    tryAgain = true;
                }
                catch (ObjectDisposedException)
                {   //The Socket has been closed
                    if (_socket != null) _socket.Close();
#if (DEBUG)
                    Debug.WriteLine("TryReconnect: ObjectDisposedException");
#endif
                    tryAgain = true; 
                }
                catch (Exception)
                {
                    if (_socket != null) _socket.Close();
#if (DEBUG)
                    Debug.WriteLine("TryReconnect: Exception");
#endif
                    tryAgain = true;
                }
                _connectionsCounter++;
            }
            if (tryAgain) //vuol dire che ho superato il # di tentativi
            {
#if (DEBUG)
                Debug.WriteLine("TryReconnect: fuori dal while, ReconnectionLimitExceeded");
#endif
                msg = "ReconnectionLimitExceeded";
            }
            else
            {
                _connectionsCounter = 1;
#if (DEBUG)
                Debug.WriteLine("TryReconnect: Connected");
#endif
                msg = "Connected";
            }
            return msg;
        }


        public void Receive()
        {
#if (DEBUG)
            Debug.WriteLine("Receive è partita.");
#endif
            Action<string> notifySocketStatus = _parentGUIElement.SocketStatusChanged;
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

                while (!CloseEvent.WaitOne(0))
                {//ciclo fino a quando ricevo uno stop dall'esterno oppure ottengo un errore dal socket
                    totalRead = 0;
                    length = 0;
                    //Debug.WriteLine("Dentro al while di receive.");
                    try
                    {
                        Debug.Assert(_socket.Connected, "_socket IS connected");
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
                                //Debug.WriteLine("length = {0}", length);
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
#if (DEBUG)
                                Debug.WriteLine("pendingJSONs ha" + _parentGUIElement.pendingJSONs.Count + "elementi.");
#endif
                            }
                            else
                            {   //la dimensione del json è eccessivamente grande o negativa, quindi chiudo il socket
#if (DEBUG)
                                Debug.WriteLine("La dimensione del json è eccessivamente grande o negativa, quindi chiudo il socket");
#endif
                                _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "SocketClosedByUs");
                                CloseEvent.Set();
                                break;
                            }
                        }
                    }
                    catch (IOException ioe) //qui è probabile che sia crashato il server, quindi bisogna tentare le 3 riconnessioni
                    {   //càpita quando il timeout scade senza che siano stati ricevuti dati
                        Debug.WriteLine("Connessione chiusa per timeout:\n" + MsgException);
                        MsgException = ioe.Message;
                        _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "IOException");
                        nws.Dispose();
                        String msg = TryReconnect();
                        if (msg.Equals("ReconnectionLimitExceeded")) {
                            _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "ReconnectionLimitExceeded");
                            CloseEvent.Set();
                            break;
                        }
                        if (msg.Equals("Connected"))
                        {
                            _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "Connected");
                            //CloseEvent.Reset();
                            nws = new NetworkStream(_socket);
                            continue;
                        }
                    }
                    catch (ObjectDisposedException ode) {
                        Debug.WriteLine("Connessione chiusa per objectDisposed:" + MsgException);
                        MsgException = ode.Message;
                        _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "ObjectDisposedException");
                        nws.Dispose();
                        String msg = TryReconnect();
                        if (msg.Equals("ReconnectionLimitExceeded"))
                        {
                            _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "ReconnectionLimitExceeded");
                            CloseEvent.Set();
                            break;
                        }
                        if (msg.Equals("Connected"))
                        {
                            _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "Connected");
                            //CloseEvent.Reset();
                            nws = new NetworkStream(_socket);
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "GenericException");
                        CloseEvent.Set();
                        break;
                    }
                }
#if (DEBUG)
                Debug.WriteLine("uscito dal while.waitone");
#endif
            }
            catch (Exception e)
            {
                MsgException = e.Message;
#if(DEBUG)
                Debug.WriteLine("Eccezione nel try-catch esterno: {0}",MsgException);
#endif
                _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "GenericException");
            }
            finally
            {
#if (DEBUG)
                Debug.WriteLine("Finally");
#endif
                Close();
            }
            ////Debug.WriteLine("json = " + json.ToString());
            return;
        }

        public void StopReceive() {
            CloseEvent.Set();
        }

        public async Task Send(string json)
        {
#if (DEBUG)
            Debug.WriteLine("Send è partita.");
#endif
            Action<string> notifySocketStatus = _parentGUIElement.SocketStatusChanged;
            try
            {
                NetworkStream nws = new NetworkStream(_socket);
                string received;
                byte[] bufferLength = new byte[4];
                byte[] buffer;
                byte[] concatBuffer;
                int length;
                int totalWritten;
                int written;
                

                while (!CloseEvent.WaitOne(0))
                {//ciclo fino a quando ricevo uno stop dall'esterno oppure ottengo un errore dal socket
                    totalWritten = 0;
                    length = 0;
                    //Debug.WriteLine("Dentro al while di receive.");
                    try
                    {
                        Debug.Assert(_socket.Connected, "_socket IS connected");
                        /*if (!nws.DataAvailable)
                        {
                            Thread.Sleep(1);
                        }
                        else */
                        buffer = Encoding.ASCII.GetBytes(json);
                        bufferLength = BitConverter.GetBytes(json.Length);

                        //concateno il buffer contenente il json al buffer contenente la dimensione del json
                        var s = new MemoryStream();
                        s.Write(bufferLength, 0, bufferLength.Length);
                        s.Write(buffer, 0, buffer.Length);
                        concatBuffer = s.ToArray();

                        written = 0;
                        totalWritten = 0;
                        while (totalWritten < buffer.Length)
                        {
                            written = _socket.Send(concatBuffer, totalWritten, concatBuffer.Length - totalWritten, SocketFlags.None);
                            totalWritten += written;
                        }
                        break;
                    }
                    catch (SocketException se) //errore del sistema operativo nel tentare di accedere al socket
                    {   //chiudo tutto e metto in grigio la finestra
                        MsgException = se.Message;
                        Debug.WriteLine("Connessione chiusa per SocketException:\n" + MsgException);
                        _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "SocketException");
                        CloseEvent.Set();
                        break;
                    }
                    catch (ObjectDisposedException ode)
                    {
                        MsgException = ode.Message;
                        Debug.WriteLine("Connessione chiusa per objectDisposed:" + MsgException);
                        _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "ObjectDisposedException");
                        nws.Dispose();
                        String msg = TryReconnect();
                        if (msg.Equals("ReconnectionLimitExceeded"))
                        {
                            _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "ReconnectionLimitExceeded");
                            CloseEvent.Set();
                            break;
                        }
                        if (msg.Equals("Connected"))
                        {
                            _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "Connected");
                            //CloseEvent.Reset();
                            nws = new NetworkStream(_socket);
                            continue;
                        }
                    }
                    catch(Exception e)
                    {
                        _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "GenericException");
                        CloseEvent.Set();
                        break;
                    }
                }
#if (DEBUG)
                Debug.WriteLine("uscito dal while.waitone Send");
#endif
            }
            catch (Exception e)
            {
                MsgException = e.Message;
#if (DEBUG)
                Debug.WriteLine("Eccezione nel try-catch esterno: " + MsgException);
#endif
                _parentGUIElement.Dispatcher.BeginInvoke(notifySocketStatus, "GenericException");
            }

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
            if (_socket != null)
            {
                _socket.Dispose();
                _socket.Close();
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