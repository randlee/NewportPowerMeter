using System;
using System.Collections;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using Newport.USBComm;
using Visyn.Newport.Log;

namespace Newport.Usb
{
    public class NewportSerialWrap : INewportInstrument
    {
        public CommLogger Logger { get; }
        public string Name => (_serialPort.IsOpen ? $"Serial Port : {_serialPort.PortName}" : "Serial Port : Closed");
        readonly SerialPort _serialPort = new SerialPort();

        public int m_knIOError => USB.m_knIOError;
        public int m_knUSBAddrNotFound => USB.m_knUSBAddrNotFound;
        public int m_knWDStatusInvalidWDHandle => USB.m_knWDStatusInvalidWDHandle;
        public int m_knWDDeviceNotFound => USB.m_knWDDeviceNotFound;
        public uint m_knWDDeviceNotResponding => USB.m_knWDDeviceNotResponding;
        public uint m_knWDStatusEndpointHalted => USB.m_knWDStatusEndpointHalted;

        public NewportSerialWrap(CommLogger logger)
        {
            Logger = logger;
        }
        #region Implementation of INewportDll

        public string LogFilePath { get; private set; }

        public bool Logging { get; set; }

        public bool OpenDevices()
        {
            var ports = SerialPort.GetPortNames();
            return OpenDevices(ports.Last().Last() - '0');
        }

        public bool OpenDevices(int port)
        {
            _serialPort.PortName = $"COM{port}";
            _serialPort.BaudRate = 38400;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataBits = 8;
            _serialPort.DtrEnable = false;
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 10000;
            _serialPort.Open();
            if (_serialPort.IsOpen)
            {
                Logger?.Open();
                Write(NewportScpi.Echo(false));
                Thread.Sleep(100);
                _serialPort.ReadExisting();
            }
            return _serialPort.IsOpen;
        }

        public bool OpenDevices(int port, bool usingDeviceKey)
        {
            return OpenDevices(port);
        }

        public void CloseDevices()
        {
            _serialPort.Close();
            Logger?.Disconnect();
        }

        public void Flush()
        {
            while (Read()?.Length > 0) { }
        }

        public int Read(string deviceKey, StringBuilder buffer) => read(buffer);

        public int Read(int deviceID, StringBuilder buffer) => read(buffer);

        public int ReadBinary(string deviceKey, StringBuilder buffer) => read(buffer);

        public int ReadBinary(int deviceID, StringBuilder buffer) => read(buffer);
        public int ReadBinary(StringBuilder buffer) => read(buffer);

        public int ReadBinary(string deviceKey, IntPtr buffer, ref uint bytesRead)
        {
            throw new NotImplementedException();
        }

        public int ReadBinary(int deviceID, IntPtr buffer, ref uint bytesRead)
        {
            throw new NotImplementedException();
        }


        public string Read()
        {
            var builder = new StringBuilder(1024);
            read(builder);
            return builder.ToString();
        }

        private int read(StringBuilder buffer)
        {
            try
            {
                var bytes = _serialPort.BytesToRead;
                if (bytes == 0) return 0;
                var chars = new char[bytes];
                var bytesRead = _serialPort.Read(chars, 0, bytes);
                buffer.Append(chars);
                if (bytesRead > 0) Logger?.Read(buffer.ToString());
                return bytesRead;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int Write(string deviceKey, string cmd) => Write(cmd);

        public int Write(int deviceID, string cmd) => Write(cmd);

        public int WriteBinary(string deviceKey, IntPtr buffer, int length) => writeBinary(buffer, length);



        public int WriteBinary(int deviceID, IntPtr buffer, int length) => writeBinary(buffer, length);


        public bool TryRead(out string response, out int ioStatus)
        {
            ioStatus = 0;
            if (_serialPort?.BytesToRead > 0)
            {
                response = _serialPort.ReadExisting();
                return true;
            }
            response = null;

            return false;
        }

        public bool TryReadBinary(out string response, out int ioStatus) => TryRead(out response, out ioStatus);

        //public bool ReadBinaryUntil(string match, uint expectedLength, out string response, out int ioStatus)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool ReadDataUntil(string match, uint expectedLength, string preamble, out int ioStatus, out List<double> data)
        //{
        //    throw new NotImplementedException();
        //}

        private int writeBinary(IntPtr buffer, int length)
        {
            var str = new char[length];
            for (var i = 0; i < length; i++)
            {
                str[i] = Convert.ToChar((buffer + i).ToInt32());
            }
            return Write(new string(str));
        }

        public int Query(string deviceKey, string cmd, StringBuilder buffer) => query(cmd, buffer);

        public int Query(int deviceID, string cmd, StringBuilder buffer) => query(cmd, buffer);


        private int query(string cmd, StringBuilder buffer)
        {
            var respoonse = Write(cmd);
            if (respoonse == 0) respoonse = read(buffer);
            return respoonse;
        }



        public Hashtable GetDeviceTable() => null;

        public int GetDeviceKeys(out string[] deviceKeys)
        {
            deviceKeys = new string[0];
            return 0;
        }

        public ArrayList GetDevInfoList() => new ArrayList(0);

        public int GetDevInfoArrays(out int[] devIDs, out string[] descriptions)
        {
            devIDs = (_serialPort?.IsOpen == true) ? new int[] { _serialPort.PortName.Last() - '0' } : new int[0];
            descriptions = (_serialPort?.IsOpen == true) ? new string[] { _serialPort.PortName } : new string[0];
            return 0;
        }

        public int NumProductsConnected(string keyVendorProduct) => (_serialPort?.IsOpen == true) ? 1 : 0;

        public virtual int Write(string command)
        {
            if (_serialPort == null) return m_knUSBAddrNotFound;
            _serialPort.Write(command);
            Logger.Send(command);
            Thread.Sleep(250);
            return 0;
        }

        public int Read(StringBuilder sbResponse)
        {
            if (_serialPort == null) return m_knUSBAddrNotFound;
            var result = this.Read();
            sbResponse.Append(result);
            return 0;
        }

        #endregion
    }
}
