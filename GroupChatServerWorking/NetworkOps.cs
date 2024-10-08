﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroupChatServerWorking
{
    public class NetworkOps
    {
        private static NetworkOps _listener_instance = null;
        private TcpListener server = null;
        private Object logBoxlock;
        private TextBox logBox;
        string key;
        string UserName;
        private Object keylock;
        Task OlgierdTask;
        Form1 form;
        public List<ConnectionClient> connections;
        public int row = 0;
        private NetworkOps(Int32 port,IPAddress ip,TextBox logBox,Object logBoxLock,string key,Object keyLock,string UserName, Form1 form)
        {
            this.logBoxlock = logBoxLock;
            this.logBox = logBox;
            this.key = key; 
            this.keylock = keyLock;
            this.UserName = UserName;
            this.form = form;
            connections = new List<ConnectionClient>();
            OlgierdTask = Task.Factory.StartNew(() => { });
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
        public async void Run(CancellationToken token)
        {
            List<Task> Clienttasks = new List<Task>();
            while(!token.IsCancellationRequested) 
            {
                try
                {
                    
                    ConnectionClient client = new ConnectionClient(await server.AcceptTcpClientAsync(token));
                    Clienttasks.Add(Task.Run((Action)(() => Connection(client))));
                    lock (logBoxlock)
                    {
                        logBox.AppendText($"{DateTime.Now.ToString("HH:mm")} | New client... Authorizing {Environment.NewLine}");
                    }
                }catch (Exception ex) 
                {
                    
                }

            }
            //server.Stop();
            //form.AppendLogBox($"{DateTime.Now.ToString("HH:mm")} | Shutting connection {Environment.NewLine}");
        }
        private async void Connection(ConnectionClient client)
        {
            if(PerformAuthorization(client)) 
            {
                connections.Add(client);
                //logBox.AppendText($"{DateTime.Now.ToString("HH:mm")} Has Connected {client.Name} {Environment.NewLine}");
                form.AppendLogBox($"{DateTime.Now.ToString("HH:mm")} Has Connected {client.Name} {Environment.NewLine}");
                // grid logic
                while(client.Connected && !client.Cancellation.IsCancellationRequested) 
                {
                    try
                    {
                        string RecivedMessage = ((TextReader)client.Reader).ReadLine();
                        if(!(RecivedMessage is null))
                        {
                            Messages.Message recived = SerialOps.DeserializeFromJsonMes(RecivedMessage);
                            if(recived.Text == "Disconnected")
                            {
                                client.TcpClient.Close();
                                connections.Remove(client);
                                continue;
                            }
                            form.AppendLogBox($"{recived.Time.ToString("HH:mm")} | Recived {recived.Text} From {recived.Sender} {Environment.NewLine}");
                            foreach(var cl in connections)
                            {
                                if(cl.Id != client.Id)
                                {
                                    AsyncOlgierd(recived, cl);
                                    form.AppendLogBox($"{recived.Time.ToString("HH:mm")} | Send {recived.Text} To {cl.Name} {Environment.NewLine}");
                                }
                            }
                        }
                    }
                    catch (Exception ex) 
                    {
                    }
                }
            }
        }
        private bool PerformAuthorization(ConnectionClient client) 
        {
            try
            {
                string NetMessage = ((TextReader)client.Reader).ReadLine();
                if (NetMessage is null)
                    return false;
                Messages.Authorization auth = SerialOps.DeserializeFromJsonAuth(NetMessage);
                if (auth is null)
                    return false;
                client.Name = auth.Sender;
                // idk jak zblokowoać
                if(auth.Key == key)
                {
                    AsyncOlgierd(new Messages.Message(UserName, "Authorized", DateTime.Now), client);
                    form.AppendLogBox($"{DateTime.Now.ToString("HH:mm")} Authorized {client.Name} {Environment.NewLine}");
                    form.Invoke(new Action(() => form.dataGridView1.Rows.Add(client.Id, client.Name)));
                    var disconnectButton = new DataGridViewButtonCell();
                    disconnectButton.Value = "Disconnect";
                    form.Invoke(new Action(() => form.dataGridView1.Rows[row].Cells["Disconnecth"] = disconnectButton));
                    row++;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) 
            {
                lock(logBoxlock) 
                {
                    form.AppendLogBox($"{DateTime.Now.ToString("HH:mm")} | Auth Errror {Environment.NewLine}");
                    return false;
                }
            }
        }
        private void SendMesagePackage(Messages.Message message, ConnectionClient client)
        {
            try
            {
                string og_olgierd = SerialOps.SerializeToJson(message);
                ((TextWriter)client.Writer).WriteLine(og_olgierd);
                ((TextWriter)client.Writer).Flush();
            }
            catch(Exception ex)
            {
                
                
                //logBox.AppendText(ex.Message);
                
            }
        }
        public async Task AsyncOlgierd(Messages.Message message, ConnectionClient client)
        {
            await OlgierdTask.ContinueWith((Action<Task>)(task => SendMesagePackage(message, client)));
        }
        public void SendToAll(Messages.Message mes)
        {
            foreach(ConnectionClient client in connections)
            {
                AsyncOlgierd(mes, client); 
                logBox.AppendText($"{DateTime.Now.ToString("HH:mm")} | Send {mes.Text} to {client.Name} {Environment.NewLine}");
            }
        }
 

    public static NetworkOps GetInstance(string port,string ipAddress,TextBox logBox,Object logBoxLock,string key,Object keyLock,string UserName,Form1 form)
        {
            if(_listener_instance == null) 
            {
                try
                {
                    IPAddress ip;
                    if(ipAddress == "localhost")
                    {
                        string host = Dns.GetHostName();
                        IPHostEntry IP = Dns.GetHostEntry(ipAddress);
                        //idk is localhost ok
                        ip = IP.AddressList[0];
                       
                    }
                    else
                    {
                        ip = IPAddress.Parse(ipAddress);
                   
                    }
        
                    _listener_instance = new NetworkOps(Int32.Parse(port), ip,logBox,logBoxLock,key,keyLock,UserName,form);
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
                SendToAll(new Messages.Message(UserName, "disconnected", DateTime.Now));
                foreach(var client in connections)
                {
                    logBox.AppendText($"{DateTime.Now.ToString("HH:mm")} | Disconnecting client {client.Name} {Environment.NewLine}");
                    client.TcpClient.Close();
                }
                form.Invoke(new Action(() => form.dataGridView1.Rows.Clear()));
                connections.Clear();
                server.Stop();
                _listener_instance = null;
                lock(logBoxlock)
                {
                    logBox.AppendText($"{DateTime.Now.ToString("HH:mm")} | Disconnected {Environment.NewLine}");
                }
                server = null;
            }
            else
            {
                MessageBox.Show("already disconnected");
            }
        }
    }
}
