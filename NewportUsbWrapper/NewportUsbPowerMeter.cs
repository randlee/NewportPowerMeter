using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Newport.Usb
{
    public class NewportUsbPowerMeter : NewportPowerMeter, INewportInstrument
    {
        private INewportDll _newportDll => _newport as INewportDll;
        public string Name => $"Newport Power Meter - {_newport?.Name}";

        /// <summary>
        /// The USB address of the device that is being communicated with.
        /// </summary>
        private int _deviceID = -1;

        /// <summary>
        /// DeviceKey used to communicate with Newport device
        /// </summary>
        private string _deviceKey = null;
		


        public NewportUsbPowerMeter(INewportInstrument newportUsb) : base(newportUsb ?? new NewportDllWrap(null))
        {
        }

        public bool OpenDevices() => Connect();
        public bool OpenDevices(int port) => Connect();
        public bool Connect()
        {
            if (_newport == null) return false;
            var open = false;
#if false

            // If a device is connected via USB or if it cannot be determined
            if (USB.NumProductsConnected(Program.m_kstrVendorProductID) != 0)
            {
                // Open all devices on the USB bus with the specified product ID
                open = USB.OpenDevices(Program.m_knProductID);
                if (open)
                {
                    // Get the device ID of the first device in the list
                    ArrayList alDevInfoList = USB.GetDevInfoList();
                    DevInfo devInfo = (DevInfo) alDevInfoList[0];
                    _deviceID = devInfo.ID;
                    OnConnected(true);
                    this.txtCmd.Focus();
                }
            }
#else
            open = _newport.OpenDevices(0, true);

            // If the devices were not opened successfully
            if (!open)
            {
                Debug.WriteLine("\n***** Error:  Could not open the devices. *****\n\n");
                Debug.WriteLine("Check the log file for details.\n");
                if(_newport.Logging)
                {
                    _newport.Write(_deviceKey, NewportScpi.ErrorString);
                    Debug.WriteLine($"ERRSTR: {Read()}");
                    Process.Start(_newport.LogFilePath);
                }
            }
            else
            {
                // Display the device table
                var nDeviceCount = DisplayDeviceTable();
                DisplayDeviceInfo();

                // If there is at lease one instrument that is open
                if (nDeviceCount <= 0) return open;
                _deviceID = -1;

// Select a device key from the list
                var table = _newportDll?.GetDeviceTable();
                if (table != null)
                {
                    foreach (var item in table.Keys)
                    {
                        _deviceKey = item as string;
                        if (!string.IsNullOrEmpty(_deviceKey)) break;
                    }
                }

                Flush();
                resetMeasurement();
            }

#endif
            return open;
        }



        public void Disconnect()
        {
            // Close all devices on the USB bus
            _newport?.CloseDevices();
            _deviceID = -1;
            _deviceKey = null;
        }

        protected int DisplayDeviceTable()
        {
            if (_newportDll == null) return 0;  // Not USB device
            var hashTable = _newportDll.GetDeviceTable();

            if (hashTable != null)
            {
                Debug.WriteLine($"{hashTable.Count} devices in DeviceTable");
                foreach (var item in hashTable.Keys)
                {
                    Debug.WriteLine(item);
                }

                return hashTable.Count;
            }

            Debug.WriteLine($"No USB devices found in DeviceTable!");
            return 0;
        }

        protected int DisplayDeviceInfo()
        {
            if (_newportDll == null) return 0;  // Not USB device
            var devices = _newportDll?.GetDevInfoList();
            if (devices != null)
            {
                Debug.WriteLine($"{devices.Count} devices in DevInfoList");

                foreach (var device in devices)
                {
                    Debug.WriteLine(device);
                }

                return devices.Count;
            }

            Debug.WriteLine($"No USB devices found in DevInfoList!");
            return 0;
        }


        /// <summary>
        /// This method reads data from the communication port and then 
        /// displays the response and read status.
        /// </summary>
        public string Read()
        {
            try
            {
                if (_newport == null) return ErrorString(_newport.m_knUSBAddrNotFound);
                var nIOStatus = _newport.m_knUSBAddrNotFound;

                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                var sbResponse = new StringBuilder(64);
                nIOStatus = _newport.Read(sbResponse);
                //if (_deviceID > 0) nIOStatus = _newport.Read(_deviceID, sbResponse);
                //else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = _newport.Read(_deviceKey, sbResponse);

                if (nIOStatus == 0)
                {
                    var response = sbResponse.ToString();
                    //                    Profile?.Invoke($"{DateTime.Now} Read-{response}");
                    Debug.WriteLine($"[{response.Length}] '{response}'");
                    return response;
                }

                return ErrorString(nIOStatus);
            }
            catch (Exception ex)
            {
                return $"Could not read the command response.\r\n{ex.Message}";
            }
        }

 



        //public bool ReadBinaryUntil(INewportInstrument newport, string match, uint expectedLength, out string response, out int ioStatus)
        //{
        //    ioStatus = newport.m_knUSBAddrNotFound;
        //    try
        //    {
        //        // The firmware limits the transfer size to the maximum packet size of 64 bytes
        //        var readCount = 0;
        //        var start = DateTime.Now;
        //        var sbResponse = new StringBuilder((int)expectedLength);
        //        if (newport != null)
        //        {
        //            ioStatus = 0;
        //            while (ioStatus == 0 && !sbResponse.ToString().Contains(match))
        //            {
        //                var sbTemp = new StringBuilder((int)expectedLength);
        //                if (_deviceID > 0) ioStatus = newport.ReadBinary(_deviceID, sbTemp);
        //                else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = newport.ReadBinary(_deviceKey, sbTemp);
        //                sbResponse.Append(sbTemp);
        //                readCount++;
        //            }
        //        }

        //        if (ioStatus == 0)
        //        {
        //            Debug.WriteLine($"{readCount} reads in {(DateTime.Now - start).TotalSeconds} sec");
        //            Debug.WriteLine($"Response Length={sbResponse.Length}");
        //            response = sbResponse.ToString();
        //            return true;
        //        }

        //        response = $"Error Code = {ioStatus}, 0x{ioStatus.ToString("X")}";
        //    }
        //    catch (Exception ex)
        //    {
        //        response = $"Could not read the command response.\r\n{ex.Message}";
        //    }

        //    Debug.WriteLine(response);
        //    return false;
        //}

 

        /// <summary>
        /// This method sends the specified command to the communication port and 
        /// then displays the write status.
        /// </summary>
        /// <param name="command">Command to send to power meter</param>
        public override int Write(string command)
        {
            try
            {
                var nIOStatus = _newport.m_knUSBAddrNotFound;
                if (_newport == null) return _newport.m_knWDDeviceNotFound;
                Debug.WriteLine(command);
                if (_deviceID > 0) nIOStatus = _newport.Write(_deviceID, command);
                else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = _newport.Write(_deviceKey, command);
//                Profile?.Invoke($"{DateTime.Now}Write-{command}");
                // If no error status
                return nIOStatus;
            }
            catch (Exception ex)
            {
                return _newport.m_knWDDeviceNotFound;
            }
        }

        public int Read(StringBuilder sbResponse) => _newport.Read(sbResponse);

        #region Implementation of INewportInstrument

        public int m_knIOError => _newport.m_knIOError;

        public int m_knUSBAddrNotFound => _newport.m_knUSBAddrNotFound;

        public int m_knWDStatusInvalidWDHandle => _newport.m_knWDStatusInvalidWDHandle;

        public int m_knWDDeviceNotFound => _newport.m_knWDDeviceNotFound;

        public uint m_knWDDeviceNotResponding => _newport.m_knWDDeviceNotResponding;

        public uint m_knWDStatusEndpointHalted => _newport.m_knWDStatusEndpointHalted;

        public bool Logging
        {
            get { return _newport.Logging; }
            set { _newport.Logging = value; }
        }

        public string LogFilePath => _newport.LogFilePath;

        public bool OpenDevices(int port, bool usingDeviceKey)
        {
            return _newport.OpenDevices(port, usingDeviceKey);
        }

        public void CloseDevices()
        {
            _newport.CloseDevices();
        }

        public int Write(string deviceKey, string cmd)
        {
            return _newport.Write(deviceKey, cmd);
        }

        public int Write(int deviceID, string cmd)
        {
            return _newport.Write(deviceID, cmd);
        }

        public int Read(string deviceKey, StringBuilder buffer)
        {
            return _newport.Read(deviceKey, buffer);
        }

        public int Read(int deviceID, StringBuilder buffer)
        {
            return _newport.Read(deviceID, buffer);
        }

        public int ReadBinary(string deviceKey, StringBuilder buffer)
        {
            return _newport.ReadBinary(deviceKey, buffer);
        }

        public int ReadBinary(int deviceID, StringBuilder buffer)
        {
            return _newport.ReadBinary(deviceID, buffer);
        }

        public int ReadBinary(StringBuilder buffer)
        {
            if (_deviceID > 0) return _newport.ReadBinary(_deviceID, buffer);
            else if (!string.IsNullOrEmpty(_deviceKey)) return _newport.ReadBinary(_deviceKey, buffer);
            return m_knWDDeviceNotFound;
        }

        public int Query(string deviceKey, string cmd, StringBuilder buffer)
        {
            return _newport.Query(deviceKey, cmd, buffer);
        }

        public int Query(int deviceID, string cmd, StringBuilder buffer)
        {
            return _newport.Query(deviceID, cmd, buffer);
        }

        #endregion
    }
}
