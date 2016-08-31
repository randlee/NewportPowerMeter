namespace Newport.Usb
{
    public static class NewportScpiCommands
    {
        public static string Identity => "*IDN?";
        public static string Reset => "*RST";

        public static string DataStoreEnable => "PM:DS:ENABLE 1\r";
        public static string DataStoreDisable => "PM:DS:ENABLE 0\r";

        public static string DataStoreClear => "PM:DS:CLEAR\r";
        public static string DataStoreCountQuery => "PM:DS:COUNT?\r";
        public static string DataStoreBuffer(uint index) { return $"PM:DS:BUFFER {index}\r"; }
        public static string DataStoreInterval(uint index) { return $"PM:DS:INTERVAL {index}\r"; }

        public static string DataStoreSize(uint sampleSize) { return $"PM:DS:SIZE {sampleSize}\r"; }
    }
}
