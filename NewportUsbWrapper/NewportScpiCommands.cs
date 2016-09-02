namespace Newport.Usb
{
    public enum PmMode
    {
        DcContinuous = 0,
        DcSingle = 1,
        Integrate = 2,
        PeakToPeakContinuous = 3,
        PeakToPeakSingle = 4,
        PulseContinuous = 5,
        PulseSingle = 6,
        RMS = 7
    }
    public static class NewportScpiCommands
    {
        public static string Identity => "*IDN?";
        public static string Reset => "*RST";

        public static string AutoRange(bool enable) => $"PM:AUTO {(enable ? 1 : 0)}";
        public static string Mode(PmMode mode) => $"PM:MODE {(int) mode}\r";

        public static string DataStoreEnable => "PM:DS:ENABLE 1\r";
        public static string DataStoreDisable => "PM:DS:ENABLE 0\r";

        public static string DataStoreClear => "PM:DS:CLEAR\r";
        public static string DataStoreCountQuery => "PM:DS:COUNT?\r";
        public static string DataStoreBuffer(uint index) { return $"PM:DS:BUFFER {index}\r"; }
        public static string DataStoreInterval(uint index) { return $"PM:DS:INTERVAL {index}\r"; }

        public static string DataStoreSize(uint sampleSize) { return $"PM:DS:SIZE {sampleSize}\r"; }
    }
}
