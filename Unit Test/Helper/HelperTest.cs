namespace Unit_Test.Helper
{
    [TestClass]
    public class HelperTest
    {
        [TestMethod]
        public void FakeServerFunctionTest()
        {
            string serverURL = "http://localhost:5000";
            HttpClient client = new HttpClient();
            FakeServer fakeServer = new FakeServer(serverURL);

            List<string> requests = new List<string>
            {
                "Test",
                "Hello",
                "World"
            };

            fakeServer.Start();
            foreach (string request in requests) 
            {
                client.PostAsync(serverURL, new StringContent(request)).Wait();
            }

            Assert.AreEqual(requests.Count, fakeServer.ReceivedRequests.Count);
            foreach (string request in requests)
            {
                Assert.AreEqual(request, fakeServer.ReceivedRequests.Dequeue());
            }

            fakeServer.Stop();
        }

        [TestMethod]
        public void UDPFunctionTest()
        {
            UDP udp1 = new UDP(5001);
            UDP udp2 = new UDP(5002);

            UDP.ProcessPacket ack = new UDP.ProcessPacket((byte[] bytes) =>
            {
                byte sum = 0;
                foreach (byte b in bytes) { sum += b; }
                byte[] ackBytes = new byte[] {sum};
                udp2.SendBackBytes(ackBytes);
            });
            udp2.onProcessPacket += ack;

            List<byte[]> sendBytes = new List<byte[]>
            {
                new byte[] { 0, 1, 2 },
                new byte[] { 3, 4, 5, 6 },
                new byte[] { 7, 8, 9, 10, 11 }
            };

            udp1.Start();
            udp2.Start();

            udp1.Connect("localhost", udp2.Port);

            foreach (byte[] send in sendBytes)
            {
                udp1.SendBytes(send);
            }

            Thread.Sleep(50);

            Assert.AreEqual(sendBytes.Count, udp1.ReceivedPackets.Count);
            Assert.AreEqual(sendBytes.Count, udp2.ReceivedPackets.Count);

            foreach (byte[] send in sendBytes)
            {
                byte[] received = udp2.ReceivedPackets.Dequeue();
                CollectionAssert.AreEqual(send, received);
            }

            foreach (byte[] send in sendBytes)
            {
                byte sum = 0;
                foreach (byte b in send) { sum += b; }
                byte[] ackBytes = new byte[] { sum };

                byte[] received = udp1.ReceivedPackets.Dequeue();
                CollectionAssert.AreEqual(ackBytes, received);
            }

            udp1.Stop();
            udp2.Stop();
        }
    }
}
