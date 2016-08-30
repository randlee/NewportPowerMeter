using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Newport.USBComm;

namespace Newport.Usb
{
    public class NewportUsbPowerMeter
    {
        /// <summary>
        /// The USB communication object.
        /// </summary>
        private readonly USB USB;

        /// <summary>
        /// The USB address of the device that is being communicated with.
        /// </summary>
        private int _deviceID = -1;

        /// <summary>
        /// DeviceKey used to communicate with Newport device
        /// </summary>
        private string _deviceKey = null;

        public NewportUsbPowerMeter()
        {
            USB = new USB();
        }

        public bool Connect()
        {
            if (USB == null) return false;
            bool open = false;
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
            open = USB.OpenDevices(0, true);

            // If the devices were not opened successfully
            if (!open)
            {
                Debug.WriteLine("\n***** Error:  Could not open the devices. *****\n\n");
                Debug.WriteLine("Check the log file for details.\n");
            }
            else
            {   // Display the device table
                var nDeviceCount = DisplayDeviceTable();
                DisplayDeviceInfo();

                // If there is at lease one instrument that is open
                if (nDeviceCount <= 0) return open;
                _deviceID = -1;
                // Select a device key from the list
                var table = USB.GetDeviceTable();
                foreach (var item in table.Keys)
                {
                    _deviceKey = item as string;
                    if (!string.IsNullOrEmpty(_deviceKey)) break;
                }
                string response;
                int status;
                while(TryRead(out response, out status))
                {
                }
            }
#endif
            return open;
        }

        public void Disconnect()
        {
            // Close all devices on the USB bus
            USB?.CloseDevices();
            _deviceID = -1;
            _deviceKey = null;
        }

        /// <summary>
        /// This method sends the specified command to the communication port and 
        /// then displays the write status.
        /// </summary>
        /// <param name="command">Command to send to power meter</param>
        public string Write(string command)
        {
            try
            {
                var nIOStatus = USB.m_knUSBAddrNotFound;
                if (USB != null)
                {
                    Debug.WriteLine(command);
                    if (_deviceID > 0) nIOStatus = USB.Write(_deviceID, command);
                    else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = USB.Write(_deviceKey, command);
                }
                // If no error status
                return nIOStatus == 0 ? string.Empty : $"Error Code = {nIOStatus}, 0x{nIOStatus.ToString("X")}";
            }
            catch (Exception ex)
            {
                return $"Could not send the specified command.\r\n{ex.Message}";
            }
        }

        /// <summary>
        /// This method reads data from the communication port and then 
        /// displays the response and read status.
        /// </summary>
        public string Read()
        {
            try
            {
                var nIOStatus = USB.m_knUSBAddrNotFound;
                if (USB != null)
                {
                    // The firmware limits the transfer size to the maximum packet size of 64 bytes
                    var sbResponse = new StringBuilder(64);
                    if (_deviceID > 0) nIOStatus = USB.Read(_deviceID, sbResponse);
                    else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = USB.Read(_deviceKey, sbResponse);

                    if (nIOStatus == 0)
                    {
                        var response = sbResponse.ToString();
                        Debug.WriteLine($"[{response.Length}] '{response}'");
                        return response;
                    }
                }

                return $"Error Code = {nIOStatus}, 0x{nIOStatus.ToString("X")}";
            }
            catch (Exception ex)
            {
                return $"Could not read the command response.\r\n{ex.Message}";
            }
        }

        public bool TryRead(out string response, out int ioStatus)
        {
            ioStatus = USB.m_knUSBAddrNotFound;
            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                var sbResponse = new StringBuilder(64);
                if (USB != null)
                {
                    if (_deviceID > 0) ioStatus = USB.Read(_deviceID, sbResponse);
                    else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = USB.Read(_deviceKey, sbResponse);
                }
                if (ioStatus == 0)
                {
                    response = sbResponse.ToString();
                    Debug.WriteLine($"[{response.Length}] '{response}'");
                    return true;
                }
                response = $"Error Code = {ioStatus}, 0x{ioStatus.ToString("X")}";
            }
            catch (Exception ex)
            {
                response = $"Could not read the command response.\r\n{ex.Message}";
            }
            Debug.WriteLine(response);
            return false;
        }
        public bool TryReadBinary(out string response, out int ioStatus)
        {
            ioStatus = USB.m_knUSBAddrNotFound;
            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                var sbResponse = new StringBuilder(64);
                if (USB != null)
                {
                    if (_deviceID > 0) ioStatus = USB.ReadBinary(_deviceID, sbResponse);
                    else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = USB.ReadBinary(_deviceKey, sbResponse);
                }
                if (ioStatus == 0)
                {
                    response = sbResponse.ToString();
                    Debug.WriteLine($"[{response.Length}] '{response}'");
                    return true;
                }
                response = $"Error Code = {ioStatus}, 0x{ioStatus.ToString("X")}";
            }
            catch (Exception ex)
            {
                response = $"Could not read the command response.\r\n{ex.Message}";
            }
            Debug.WriteLine(response);
            return false;
        }
        private int DisplayDeviceTable()
        {
            var hashTable = USB.GetDeviceTable();

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

        private int DisplayDeviceInfo()
        {
            var devices = USB.GetDevInfoList();
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
    }
}
