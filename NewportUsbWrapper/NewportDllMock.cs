using System;
using System.Collections;
using System.Text;
using Newport.USBComm;
using Visyn.Newport;
using Visyn.Newport.Log;

namespace Newport.Usb
{
    public class NewportDllMock : INewportDll
    {
        private readonly ICommunicationsChannel _interceptor;
        public CommLogger Logger { get; }

        public NewportDllMock(CommLogger logger = null) 
        {
            _interceptor = new Interceptor();
            Logger = logger;
        }

        #region Implementation of INewportDll

        public string Name => "Newport Dll Mock";
        public int m_knIOError => USB.m_knIOError;
        public int m_knUSBAddrNotFound => USB.m_knUSBAddrNotFound;
        public int m_knWDStatusInvalidWDHandle => USB.m_knWDStatusInvalidWDHandle;
        public int m_knWDDeviceNotFound => USB.m_knWDDeviceNotFound;
        public uint m_knWDDeviceNotResponding => USB.m_knWDDeviceNotResponding;
        public uint m_knWDStatusEndpointHalted => USB.m_knWDStatusEndpointHalted;

        public string LogFilePath { get; }
        public bool Logging { get; set; }
        public event USB.DeviceStateChangedDelegate DeviceStateChanged;
        public bool EventInit(int productID)
        {
            throw new NotImplementedException();
        }

        public bool ParseDeviceKey(string id, ref string deviceKey)
        {
            throw new NotImplementedException();
        }

        public Hashtable GetAttachedDevices()
        {
            throw new NotImplementedException();
        }

        public bool OpenDevices()
        {
            throw new NotImplementedException();
        }

        public bool OpenDevices(int port)
        {
            throw new NotImplementedException();
        }

        public bool OpenDevices(int port, bool usingDeviceKey)
        {
            _interceptor.Open();
            return _interceptor.Connected;
        }

        public void CloseDevices()
        {
            _interceptor.Disconnect();
        }

        public void Flush()
        {
            while(Read()?.Length > 0) { }
        }

        public string Read()
        {
            var builder = new StringBuilder(
                );
            read(builder);
            return builder.ToString();
        }
        public int Read(string deviceKey, StringBuilder buffer)
        {
            return read(buffer);
        }


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

        public int Write(string deviceKey, string cmd) => Write(cmd);

        public int Write(int deviceID, string cmd) => Write(cmd);

        public int WriteBinary(string deviceKey, IntPtr buffer, int length)
        {
            throw new NotImplementedException();
        }

        public int WriteBinary(int deviceID, IntPtr buffer, int length)
        {
            throw new NotImplementedException();
        }

        public int Query(string deviceKey, string cmd, StringBuilder buffer) => query(cmd, buffer);


        public int Query(int deviceID, string cmd, StringBuilder buffer) => query(cmd, buffer);

        public Hashtable GetDeviceTable()
        {
            return new Hashtable {{"NewportDllMock Key", "NewportDllMock Value" } };
        }

        public int GetDeviceKeys(out string[] deviceKeys)
        {
            throw new NotImplementedException();
        }

        public ArrayList GetDevInfoList()
        {
            return new ArrayList();
        }

        public int GetDevInfoArrays(out int[] devIDs, out string[] descriptions)
        {
            throw new NotImplementedException();
        }

        public int NumProductsConnected(string keyVendorProduct)
        {
            throw new NotImplementedException();
        }

        #endregion

        private int read(StringBuilder buffer)
        {
            if (!_interceptor.Connected) return m_knUSBAddrNotFound;
            var data = _interceptor.Read();
            Logger?.Read(data);
            buffer.Append(data);
            return data.Length > 0 ? 0 : m_knIOError;
        }

        public int Write(string cmd)
        {
            if (!_interceptor.Connected) return m_knUSBAddrNotFound;
            Logger?.Send(cmd);
            _interceptor.Send(cmd);
            return 0;
        }

        public int Read(StringBuilder sbResponse)
        {
            throw new NotImplementedException();
        }

        private int query(string cmd, StringBuilder buffer)
        {
            if (!_interceptor.Connected) return m_knUSBAddrNotFound;
            _interceptor.Send(cmd);
            var response = _interceptor.Read();
            Logger?.Query(cmd,response);

            return response.Length > 0 ? 0 : m_knIOError; ;
        }
    }
}
