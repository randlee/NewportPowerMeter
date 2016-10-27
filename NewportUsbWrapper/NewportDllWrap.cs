using System;
using System.Collections;
using System.Text;
using Newport.USBComm;
using Visyn.Newport.Log;

namespace Newport.Usb
{

    public class NewportDllWrap : INewportInstrument
    {
        public CommLogger Logger { get; }

        public int m_knIOError => USB.m_knIOError;
        public int m_knUSBAddrNotFound => USB.m_knUSBAddrNotFound;
        public int m_knWDStatusInvalidWDHandle => USB.m_knWDStatusInvalidWDHandle;
        public int m_knWDDeviceNotFound => USB.m_knWDDeviceNotFound;
        public uint m_knWDDeviceNotResponding => USB.m_knWDDeviceNotResponding;
        public uint m_knWDStatusEndpointHalted => USB.m_knWDStatusEndpointHalted;

        readonly USB _newportDll;

        public string Name => "Newport Dll";

        public NewportDllWrap(CommLogger logger)
        {
            _newportDll = new USB();
#if DEBUG
            _newportDll.Logging = true;
#endif
            Logger = logger;
        }

        #region Implementation of INewportDll

        public string LogFilePath => _newportDll.LogFilePath;

        public bool Logging
        {
            get { return _newportDll.Logging; }
            set { _newportDll.Logging = value; }
        }

        public event USB.DeviceStateChangedDelegate DeviceStateChanged
        {
            add { _newportDll.DeviceStateChanged += value; }
            remove { _newportDll.DeviceStateChanged -= value; }
        }

        public bool EventInit(int productID)
        {
            return _newportDll.EventInit(productID);
        }

        public bool ParseDeviceKey(string id, ref string deviceKey)
        {
            return _newportDll.ParseDeviceKey(id, ref deviceKey);
        }

        public Hashtable GetAttachedDevices() => _newportDll.GetAttachedDevices();

        public bool OpenDevices()
        {
            var open = _newportDll.OpenDevices();
            if(open)    Logger?.Open();
            return open;
        }

        public bool OpenDevices(int port)
        {
            var open =  _newportDll.OpenDevices(port);
            if (open) Logger?.Open();
            return open;
        }

        public bool OpenDevices(int port, bool usingDeviceKey)
        {
            var open = _newportDll.OpenDevices(port, usingDeviceKey);
            if (open) Logger?.Open();
            return open;
        }

        public void CloseDevices()
        {
            _newportDll.CloseDevices();
            Logger?.Disconnect();
        }

        public void Flush()
        {
            while (Read()?.Length > 0) { }
        }

        public string Read()
        {
            throw new NotImplementedException();
        }

        public int Write(string text)
        {
            throw new NotImplementedException();
        }

        public int Read(StringBuilder sbResponse)
        {
            throw new NotImplementedException();
        }

        public int Read(string deviceKey, StringBuilder buffer)
        {
            var result = _newportDll.Read(deviceKey, buffer);
            if (result == 0) Logger?.Read(buffer.ToString());
            return result;
        }

        public int Read(int deviceID, StringBuilder buffer)
        {
            var result = _newportDll.Read(deviceID, buffer);
            if (result == 0) Logger?.Read(buffer.ToString());
            return result;
        }

        public int ReadBinary(string deviceKey, StringBuilder buffer)
        {
            var result = _newportDll.ReadBinary(deviceKey, buffer);
            if (result == 0) Logger?.Read(buffer.ToString());
            return result;
        }

        public int ReadBinary(int deviceID, StringBuilder buffer)
        {
            var result = _newportDll.ReadBinary(deviceID, buffer);
            if (result == 0) Logger?.Read(buffer.ToString());
            return result;
        }

        public int ReadBinary(StringBuilder buffer)
        {
            throw new NotImplementedException();
        }


        public int ReadBinary(string deviceKey, IntPtr buffer, ref uint bytesRead)
        {
            var result = _newportDll.ReadBinary(deviceKey, buffer, ref bytesRead);
            if (result == 0) Logger?.Read(buffer.ToString());
            return result;
        }

        public int ReadBinary(int deviceID, IntPtr buffer, ref uint bytesRead)
        {
            var result = _newportDll.ReadBinary(deviceID, buffer, ref bytesRead);
            if (result == 0) Logger?.Read(buffer.ToString());
            return result;
        }

        public int Write(string deviceKey, string cmd)
        {
            Logger?.Send(cmd);
            return _newportDll.Write(deviceKey, cmd);
        }

        public int Write(int deviceID, string cmd)
        {
            Logger?.Send(cmd);
            return _newportDll.Write(deviceID, cmd);
        }

        public int WriteBinary(string deviceKey, IntPtr buffer, int length)
        {
            Logger?.Send(buffer);
            return _newportDll.WriteBinary(deviceKey, buffer, length);
        }

        public int WriteBinary(int deviceID, IntPtr buffer, int length)
        {
            Logger?.Send(buffer);
            return _newportDll.WriteBinary(deviceID, buffer, length);
        }

        public int Query(string deviceKey, string cmd, StringBuilder buffer)
        {
            var result = _newportDll.Query(deviceKey, cmd, buffer);
            Logger?.Query(cmd, result == 0 ? buffer.ToString() : null);
            return result;
        }

        public int Query(int deviceID, string cmd, StringBuilder buffer)
        {
            var result = _newportDll.Query(deviceID, cmd, buffer);
            Logger?.Query(cmd, result == 0 ? buffer.ToString() : null);
            return result;
        }


        public Hashtable GetDeviceTable() => _newportDll?.GetDeviceTable();

        public int GetDeviceKeys(out string[] deviceKeys) =>_newportDll.GetDeviceKeys(out deviceKeys);

        public ArrayList GetDevInfoList() => _newportDll.GetDevInfoList();

        public int GetDevInfoArrays(out int[] devIDs, out string[] descriptions)
        {
            return _newportDll.GetDevInfoArrays(out devIDs, out descriptions);
        }

        public int NumProductsConnected(string keyVendorProduct)
        {
            return _newportDll.NumProductsConnected(keyVendorProduct);
        }

        #endregion
    }
}
