﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GroupChatServerWorking
{
    public class ConnectionClient
    {
        public ConnectionClient(TcpClient tcpClient)
        {
            this.Id = Interlocked.Increment(ref ConnectionClient.id);
            this.TcpClient = tcpClient;
            this.NetworkStream = this.TcpClient.GetStream();
            this.Reader = new StreamReader((Stream)this.NetworkStream);
            this.Writer = new StreamWriter((Stream)this.NetworkStream);
        }
        public static long id;
        public long Id;
        public string Name = "Unauthorized";
        public TcpClient TcpClient;
        public bool Connected => this.TcpClient.Connected;
        public NetworkStream NetworkStream;
        public StreamReader Reader;
        public StreamWriter Writer;
        public CancellationTokenSource Cancellation = new CancellationTokenSource();
        
    }
}
