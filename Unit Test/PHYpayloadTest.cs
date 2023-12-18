using LoRaWAN;
using LoRaWAN.PHYPayload;

namespace Unit_Test
{
    [TestClass]
    public class PHYpayloadTest
    {
        [TestMethod]
        public void JoinRequestBase64Decode()
        {
            string base64 = "AAAAAAAASLYEIHdQAAxItgToswnOhxg=";

            PHYpayload payload = PHYpayloadFactory.DecodePHYPayloadFromBase64(base64);
            
            Assert.IsNotNull(payload);
            Assert.IsTrue(payload.MACpayload is MACpayloadJoinRequest);

            Assert.AreEqual("00000000000048B604207750000C48B604E8B309CE8718", payload.Hex.ToUpper());
            Assert.AreEqual("00", payload.MHDR.ToUpper());
            Assert.AreEqual("000000000048B604207750000C48B604E8B3", payload.MACpayload.Hex.ToUpper());
            Assert.AreEqual("09CE8718", payload.MIC.ToUpper());

            MACpayloadJoinRequest joinRequest = (MACpayloadJoinRequest)payload.MACpayload;
            Assert.AreEqual("04B6480000000000", joinRequest.AppEUI.ToUpper());
            Assert.AreEqual("04B6480C00507720", joinRequest.DevEUI.ToUpper());
            Assert.AreEqual("B3E8", joinRequest.DevNonce.ToUpper());
        }

        [TestMethod]
        public void JoinRequestHexDecode()
        {
            string hex = "00010000000fcba05810a0000010cba0587b00fefcd456";

            PHYpayload payload = PHYpayloadFactory.DecodePHYPayloadFromHex(hex);

            Assert.IsNotNull(payload);
            Assert.IsTrue(payload.MACpayload is MACpayloadJoinRequest);

            Assert.AreEqual("00010000000FCBA05810A0000010CBA0587B00FEFCD456", payload.Hex.ToUpper());
            Assert.AreEqual("00", payload.MHDR.ToUpper());
            Assert.AreEqual("010000000FCBA05810A0000010CBA0587B00", payload.MACpayload.Hex.ToUpper());
            Assert.AreEqual("FEFCD456", payload.MIC.ToUpper());

            MACpayloadJoinRequest joinRequest = (MACpayloadJoinRequest)payload.MACpayload;
            Assert.AreEqual("58A0CB0F00000001", joinRequest.AppEUI.ToUpper());
            Assert.AreEqual("58A0CB100000A010", joinRequest.DevEUI.ToUpper());
            Assert.AreEqual("007B", joinRequest.DevNonce.ToUpper());
        }

        [TestMethod]
        public void JoinAcceptBase64Decode()
        {
            string base64 = "IIE/R/UI/6JnC24j4B+EueJdnEEV8C7qCz3T4gs+ypLa";

            PHYpayload payload = PHYpayloadFactory.DecodePHYPayloadFromBase64(base64);

            Assert.IsNotNull(payload);
            Assert.IsTrue(payload.MACpayload is MACpayloadJoinAccept);

            Assert.AreEqual("20813F47F508FFA2670B6E23E01F84B9E25D9C4115F02EEA0B3DD3E20B3ECA92DA", payload.Hex.ToUpper());
            Assert.AreEqual("20", payload.MHDR.ToUpper());
            Assert.AreEqual("813F47F508FFA2670B6E23E01F84B9E25D9C4115F02EEA0B3DD3E20B", payload.MACpayload.Hex.ToUpper());
            Assert.AreEqual("3ECA92DA", payload.MIC.ToUpper());

            MACpayloadJoinAccept joinAccept = (MACpayloadJoinAccept)payload.MACpayload;
            Assert.AreEqual("473F81", joinAccept.AppNonce.ToUpper());
            Assert.AreEqual("FF08F5", joinAccept.NetID.ToUpper());
            Assert.AreEqual("6E0B67A2", joinAccept.DevAddr.ToUpper());
            Assert.AreEqual("23", joinAccept.DLSettings.ToUpper());
            Assert.AreEqual("E0", joinAccept.RxDelay.ToUpper());
            Assert.AreEqual("1F84B9E25D9C4115F02EEA0B3DD3E20B", joinAccept.CFList.ToUpper());
        }

        [TestMethod]
        public void JoinAcceptHexDecode()
        {
            string hex = "20c9b0e8a8ab33316c93a2943573373778";

            PHYpayload payload = PHYpayloadFactory.DecodePHYPayloadFromHex(hex);

            Assert.IsNotNull(payload);
            Assert.IsTrue(payload.MACpayload is MACpayloadJoinAccept);

            Assert.AreEqual("20C9B0E8A8AB33316C93A2943573373778", payload.Hex.ToUpper());
            Assert.AreEqual("20", payload.MHDR.ToUpper());
            Assert.AreEqual("C9B0E8A8AB33316C93A29435", payload.MACpayload.Hex.ToUpper());
            Assert.AreEqual("73373778", payload.MIC.ToUpper());

            MACpayloadJoinAccept joinAccept = (MACpayloadJoinAccept)payload.MACpayload;
            Assert.AreEqual("E8B0C9", joinAccept.AppNonce.ToUpper());
            Assert.AreEqual("33ABA8", joinAccept.NetID.ToUpper());
            Assert.AreEqual("A2936C31", joinAccept.DevAddr.ToUpper());
            Assert.AreEqual("94", joinAccept.DLSettings.ToUpper());
            Assert.AreEqual("35", joinAccept.RxDelay.ToUpper());
            Assert.AreEqual("", joinAccept.CFList.ToUpper());
        }

        [TestMethod]
        public void JoinAcceptCreate()
        {
            PHYpayload exampleEncryptedJoinAccept = PHYpayloadFactory.DecodePHYPayloadFromBase64("ILbqJeHi2DjxnM1avwwg0cuod6OHrlHhC8Wkx4geNZ/m");

            // Decrypt example join accept
            byte[] temp = Utils.HexStringToByteArray(exampleEncryptedJoinAccept.Hex[2..]);
            temp = Cryptography.AESEncrypt(Utils.HexStringToByteArray("03D3C29C7AAE3F87483D60AB33F2EA86"), temp);
            PHYpayload exampleDecryptedJoinAccept = PHYpayloadFactory.DecodePHYPayloadFromHex("20" + BitConverter.ToString(temp).Replace("-", ""));

            // Get join accept mac payload
            MACpayloadJoinAccept joinAccept = (MACpayloadJoinAccept)exampleDecryptedJoinAccept.MACpayload;

            // Create custom join accept (encrypted)
            PHYpayload actualEncryptedJoinAccept = PHYpayloadFactory.CreatePHYpayloadJoinAccept(joinAccept.AppNonce, joinAccept.NetID, joinAccept.DevAddr, joinAccept.DLSettings, joinAccept.RxDelay, joinAccept.CFList, "03D3C29C7AAE3F87483D60AB33F2EA86");

            // Test Hex
            Assert.AreEqual(exampleEncryptedJoinAccept.Hex, actualEncryptedJoinAccept.Hex);
        }
    }
}
