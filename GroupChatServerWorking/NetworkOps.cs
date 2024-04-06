using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GroupChatServerWorking
{
    public class NetworkOps
    {
        private static NetworkOps _listener_instance = null;
        private TcpListener server = null;
        private Object logBoxlock;
        private TextBox logBox;
        private NetworkOps(Int32 port,IPAddress ip,TextBox logBox,Object logBoxLock)
        {
            this.logBoxlock = logBoxLock;
            this.logBox = logBox;
            try
            {
                server = new TcpListener(ip, port);
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);    
            }
            server.Start();
            lock (logBoxlock)
            {
                logBox.AppendText($"{DateTime.Now.ToString("HH:mm")} | Listening for incoming connections...{Environment.NewLine}");
                logBox.AppendText($"{DateTime.Now.ToString("HH:mm")} | IP: {ip.ToString()}, Port: {port}{Environment.NewLine}");
            }

        }
        public void Run(CancellationToken token)
        {
            while(!token.IsCancellationRequested) 
            {
                TcpClient client = server.AcceptTcpClient();
                lock(logBoxlock) 
                {
                    logBox.AppendText($"{DateTime.Now.ToString("HH:mm")} | New client... Authorizing {Environment.NewLine}");
                }
                // perform authorization

            }
        }
    //    static bool PerformAuthorization(TcpClient client)
    //    {
            

    //        try
    //        {
    //            // Odbieranie danych od klienta
    //            NetworkStream stream = client.GetStream();
    //            byte[] buffer = new byte[1024];
    //            int bytesRead = stream.Read(buffer, 0, buffer.Length);
    //            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

    //            // Sprawdzenie czy odebrana wiadomość to Messages.Authorization
    //            if (message == Messages.Authorization)
    //            {
    //                // Tutaj możesz umieścić logikę autoryzacji, np. porównanie kluczy

    //                // W przypadku sukcesu zwróć true
    //                return true;
    //            }
    //            else
    //            {
    //                // W przypadku niezgodności kluczy zwróć false
    //                return false;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"An error occurred during authorization: {ex.Message}");
    //            return false;
    //        }
    //    }
    //}

    public static NetworkOps GetInstance(string port,string ipAddress,TextBox logBox,Object logBoxLock)
        {
            if(_listener_instance == null) 
            {
                try
                {
                    IPAddress ip;
                    if(ipAddress == "localhost")
                    {
                        string host = Dns.GetHostName();
                        IPHostEntry IP = Dns.GetHostEntry(host);
                        ip = IP.AddressList[0];
                       
                    }
                    else
                    {
                        ip = IPAddress.Parse(ipAddress);
                   
                    }
        
                    _listener_instance = new NetworkOps(Int32.Parse(port), ip,logBox,logBoxLock);
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return _listener_instance;
        }
        public void CloseConnection()
        {
            if(server != null)
            {
                server.Stop();
                _listener_instance = null;
                lock(logBoxlock)
                {
                    logBox.AppendText($"{DateTime.Now.ToString("HH:mm")} | Disconnected {Environment.NewLine}");
                }
            }
            else
            {
                MessageBox.Show("already disconnected");
            }
        }
    }
}
