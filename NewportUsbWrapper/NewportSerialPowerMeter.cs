using System.Text;

namespace Newport.Usb
{
    public class NewportSerialPowerMeter : NewportPowerMeter , INewportInstrument
    {
        public string Name => $"Newport Power Meter - {_newport?.Name}";
        public NewportSerialPowerMeter(INewportInstrument newport) : base(newport)
        {
        }

        #region Implementation of INewportInstrument



        public bool Logging
        {
            get { return _newport.Logging; }
            set { _newport.Logging = value; }
        }

        public string LogFilePath => _newport.LogFilePath;

        public bool OpenDevices() => _newport.OpenDevices();

        public bool OpenDevices(int port) => _newport.OpenDevices(port);

        public bool OpenDevices(int port, bool usingDeviceKey) => _newport.OpenDevices(port, usingDeviceKey);

        public void CloseDevices()
        {
            _newport.CloseDevices();
        }

        public int Write(string deviceKey, string cmd) => _newport.Write(deviceKey, cmd);

        public int Write(int deviceID, string cmd) => _newport.Write(deviceID, cmd);

        public int Read(string deviceKey, StringBuilder buffer) => _newport.Read(deviceKey, buffer);

        public int Read(int deviceID, StringBuilder buffer) => _newport.Read(deviceID, buffer);

        public int ReadBinary(string deviceKey, StringBuilder buffer) => _newport.ReadBinary(deviceKey, buffer);

        public int ReadBinary(int deviceID, StringBuilder buffer) => _newport.ReadBinary(deviceID, buffer);

        public int ReadBinary(StringBuilder buffer) => _newport.ReadBinary(buffer);

        public int Query(string deviceKey, string cmd, StringBuilder buffer) => _newport.Query(deviceKey, cmd, buffer);

        public int Query(int deviceID, string cmd, StringBuilder buffer) => _newport.Query(deviceID, cmd, buffer);

        public string Read() => _newport.Read();

        public int Read(StringBuilder sbResponse) => _newport.Read(sbResponse);

        #endregion
    }
}