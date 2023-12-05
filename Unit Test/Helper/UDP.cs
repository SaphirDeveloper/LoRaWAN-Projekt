using System.Net;
using System.Net.Sockets;

namespace Unit_Test.Helper
{
    internal class UDP
    {
        private Thread _receiveUDPThread;
        private UdpClient _udpClient;
        private IPEndPoint _groupEP;
        public Queue<byte[]> ReceivedPackets { get; private set; }
        public int Port { get; private set; }
        public delegate void ProcessPacket(byte[] packet);
        public ProcessPacket onProcessPacket;

        public UDP(int port) 
        {
            Port = port;
            _receiveUDPThread = new Thread(AwaitData);
            _groupEP = new IPEndPoint(IPAddress.Any, port);
            ReceivedPackets = new Queue<byte[]>();
        }

        public void AwaitData()
        {
            while (_udpClient.Client != null)
            {
                try
                {
                    byte[] bytes = _udpClient.Receive(ref _groupEP);
                    ReceivedPackets.Enqueue(bytes);
                    onProcessPacket?.Invoke(bytes);
                }
                catch (Exception)
                {
                    // Ignore Exception
                }
            }
        }

        public void Connect(string hostname, int port)
        {
            _udpClient.Connect(hostname, port);
        }

        public void SendBytes(byte[] bytes) 
        {
            _udpClient.Send(bytes); 
        }

        public void SendBackBytes(byte[] bytes)
        {
            _udpClient.Send(bytes, _groupEP);
        }

        public void Start()
        {
            _udpClient = new UdpClient(Port);
            _receiveUDPThread.Start();
        }

        public void Stop()
        {
            _udpClient.Close();
            _udpClient.Dispose();
            _receiveUDPThread.Join();
        }
    }
}
