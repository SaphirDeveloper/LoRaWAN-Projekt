namespace JoinServer
{
    public class EndDevice
    {
        public string DevEUI { get; private set; }
        public string AppKey { get; private set; }
        public string AppSKey { get; set; }
        public string NwkSKey { get; set; }

        private EndDevice(string devEUI, string appKey)
        {
            DevEUI = devEUI;
            AppKey = appKey;
        }


        public static List<EndDevice> ReadEndDeviceCSVList()
        {
            var list = new List<EndDevice>();

            StreamReader reader = new StreamReader("./end_devices.csv");
            string[] head = reader.ReadLine().Split(';');
            int indexDevEUI = Array.IndexOf(head, "DevEUI");
            int indexAppKey = Array.IndexOf(head, "AppKey");

            while (!reader.EndOfStream)
            {
                string[] row = reader.ReadLine().Split(';');
                list.Add(new EndDevice(row[indexDevEUI], row[indexAppKey]));
            }

            return list;
        }
    }
}
