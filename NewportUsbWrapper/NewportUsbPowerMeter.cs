using System;
using System.Diagnostics;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public string Write(string command)
        {
            try
            {
                var nIOStatus = USB.m_knUSBAddrNotFound;
                if (USB != null)
                {
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public string Read()
        {
            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                var sbResponse = new StringBuilder(64);

                var nIOStatus = USB.m_knUSBAddrNotFound;
                if (USB != null)
                {
                    if (_deviceID > 0) nIOStatus = USB.Read(_deviceID, sbResponse);
                    else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = USB.Read(_deviceKey, sbResponse);
                }

                return nIOStatus == 0 ? sbResponse.ToString() : $"Error Code = {nIOStatus}, 0x{nIOStatus.ToString("X")}";
            }
            catch (Exception ex)
            {
                return $"Could not read the command response.\r\n{ex.Message}";
            }
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
