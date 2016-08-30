using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Newport.USBComm;

namespace Visyn.Newport
{
    /// <summary>
    /// This class defines the main form for the C# Sample Application.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The USB communication object.
        /// </summary>
        private readonly USB USB;

        /// <summary>
        /// The USB address of the device that is being communicated with.
        /// </summary>
        private int _deviceID=-1;

        private string _deviceKey = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm ()
        {
            InitializeComponent ();
            OnConnected (false);
            USB = new USB ();
        }

        /// <summary>
        /// This method enables / disables buttons according to the current connection state.
        /// </summary>
        /// <param name="isConnected">True if connected, false if disconnected.</param>
        private void OnConnected (bool isConnected)
        {
            btnConnect.Enabled = !isConnected;
            btnDisconnect.Enabled = isConnected;
            txtCmd.Enabled = isConnected;
            btnSend.Enabled = isConnected;
            btnRead.Enabled = isConnected;

            if(isConnected)
            {
                
            }
            else
            {
                _deviceID = -1;
                _deviceKey = null;
            }
        }

        /// <summary>
        /// This method attempts to connect to the devices attached via USB cable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnect (object sender, EventArgs e)
        {
            try
            {
                if (USB != null)
                {
#if false

// If a device is connected via USB or if it cannot be determined
                    if (USB.NumProductsConnected(Program.m_kstrVendorProductID) != 0)
                    {
                        // Open all devices on the USB bus with the specified product ID
                        if (USB.OpenDevices(Program.m_knProductID))
                        {
                            // Get the device ID of the first device in the list
                            ArrayList alDevInfoList = USB.GetDevInfoList();
                            DevInfo devInfo = (DevInfo) alDevInfoList[0];
                            _deviceID = devInfo.ID;
                            OnConnected(true);
                            this.txtCmd.Focus();
                        }
                    }
                    else
                    {
                        var openDevices = USB.OpenDevices();
                        Debug.WriteLine($"OpenDevices()={openDevices}");
                        
                        DisplayDeviceTable();
                        DisplayDeviceInfo();

                        if (openDevices)
                        {
                            OnConnected(true);
                            this.txtCmd.Focus();
                        }
                    }

#else

// Open all devices on the USB bus
                    var bOpen = USB.OpenDevices(0, true);

                    // If the devices were not opened successfully
                    if (!bOpen)
                    {
                        Debug.WriteLine("\n***** Error:  Could not open the devices. *****\n\n");
                        Debug.WriteLine("Check the log file for details.\n");
                    }
                    else
                    {
                        OnConnected(true);
                        txtCmd.Focus();

                        // Display the device table
                        var nDeviceCount = DisplayDeviceTable();
                        DisplayDeviceInfo();

// Get the device keys
                        // string[] strDeviceKeyList=null;
                        // int nDeviceCount = USB.GetDeviceKeys(out strDeviceKeyList);

                        // If there is at lease one instrument that is open
                        if (nDeviceCount > 0)
                        {
                            _deviceID = -1;
                            // Select a device key from the list
                            var table = USB.GetDeviceTable();
                            foreach (var item in table.Keys)
                            {
                                _deviceKey = item as string;
                                if (!string.IsNullOrEmpty(_deviceKey)) break;
                            }
                        }

                        // Display the command to be sent to the device
                        Debug.WriteLine("\nSend Command = '*IDN?'\n");

                        // Send the command to the device
                        var nStatus = USB.Write(_deviceKey, "*IDN?");

                        // If there was a Write error
                        if (nStatus != 0)
                        {
                            Debug.WriteLine($"\n***** Error:  Device Write Error Code = {nStatus}. *****\n\n");
                        }
                        else
                        {
                            var sbResponse = new StringBuilder(64);

                            // Read the command response from the device
                            nStatus = USB.Read(_deviceKey, sbResponse);

                            // If there was a Read error
                            if (nStatus != 0)
                            {
                                Debug.WriteLine($"\n***** Error:  Device Read Error Code = {nStatus}. *****\n\n");
                            }
                            else
                            {
                                // Display the data that was read from the device
                                Debug.WriteLine($"Response = '{sbResponse}'\n\n");
                            }
                        }
                    }
#endif
                }
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show($"Could not connect.\r\n{ex.Message}", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /// <summary>
        /// This method disconnects the devices attached via USB cable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisconnect (object sender, EventArgs e)
        {
            try
            {
                if (USB == null) return;

                // Close all devices on the USB bus
                USB.CloseDevices();
                OnConnected(false);
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show ($"Could not disconnect.\r\n{ex.Message}", "Disconnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method sends the specified command to the communication port and 
        /// then displays the write status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSend (object sender, EventArgs e)
        {
            try
            {
                var nIOStatus = USB.m_knUSBAddrNotFound;
                if (USB != null)
                {
                    if (_deviceID > 0) nIOStatus = USB.Write(_deviceID, txtCmd.Text);
                    else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = USB.Write(_deviceKey, txtCmd.Text);
                }
                // If no error status
                rtbResponse.Text = nIOStatus == 0 ? string.Empty : $"Error Code = {nIOStatus}, 0x{nIOStatus.ToString("X")}";
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show ($"Could not send the specified command.\r\n{ex.Message}", "Send", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method reads data from the communication port and then 
        /// displays the response and read status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRead (object sender, EventArgs e)
        {
            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                var sbResponse = new StringBuilder (64);

                var nIOStatus = USB.m_knUSBAddrNotFound;
                if (USB != null)
                {
                    if(_deviceID > 0) nIOStatus = USB.Read(_deviceID, sbResponse);
                    else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = USB.Read(_deviceKey, sbResponse);
                }

                rtbResponse.Text = nIOStatus == 0 ? sbResponse.ToString () : $"Error Code = {nIOStatus}, 0x{nIOStatus.ToString("X")}";
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show ($"Could not read the command response.\r\n{ex.Message}", "Read", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}