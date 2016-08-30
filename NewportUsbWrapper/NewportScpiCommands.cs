namespace Newport.Usb
{
    public static class NewportScpiCommands
    {
        public static string Identity => "*IDN?";
        public static string Reset => "*RST";
    }
}
