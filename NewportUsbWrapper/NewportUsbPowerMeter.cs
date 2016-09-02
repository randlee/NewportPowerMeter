using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Newport.USBComm;

namespace Newport.Usb
{
    public class NewportUsbPowerMeter
    {
        /// <summary>
        /// The USB communication object.
        /// </summary>
        private readonly USB _usb;

        /// <summary>
        /// The USB address of the device that is being communicated with.
        /// </summary>
        private int _deviceID = -1;

        /// <summary>
        /// DeviceKey used to communicate with Newport device
        /// </summary>
        private string _deviceKey = null;

        /// <summary>
        /// The maximum string length for an I/O transfer.
        /// </summary>
        private const int maxTransferLength = 64;

        public const string END_OF_HEADER = "End of Header\r\n";
        public const string END_OF_DATA = "End of Data\r\n";

        public NewportUsbPowerMeter()
        {
            _usb = new USB();
        }

        public bool Connect()
        {
            if (_usb == null) return false;
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
            open = _usb.OpenDevices(0, true);

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
                var table = _usb.GetDeviceTable();
                foreach (var item in table.Keys)
                {
                    _deviceKey = item as string;
                    if (!string.IsNullOrEmpty(_deviceKey)) break;
                }
                Flush();
            }
#endif
            return open;
        }



        public void Disconnect()
        {
            // Close all devices on the USB bus
            _usb?.CloseDevices();
            _deviceID = -1;
            _deviceKey = null;
        }

        public void Flush()
        {
            if (_usb == null) return;
            string response;
            int status;
            while (TryRead(out response, out status))
            {
            }
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
                if (_usb != null)
                {
                    Debug.WriteLine(command);
                    if (_deviceID > 0) nIOStatus = _usb.Write(_deviceID, command);
                    else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = _usb.Write(_deviceKey, command);
                }
                // If no error status
                return nIOStatus == 0 ? string.Empty : $"Write Error Code = {nIOStatus}, 0x{nIOStatus.ToString("X")}";
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
                if (_usb != null)
                {
                    // The firmware limits the transfer size to the maximum packet size of 64 bytes
                    var sbResponse = new StringBuilder(64);
                    if (_deviceID > 0) nIOStatus = _usb.Read(_deviceID, sbResponse);
                    else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = _usb.Read(_deviceKey, sbResponse);

                    if (nIOStatus == 0)
                    {
                        var response = sbResponse.ToString();
                        Debug.WriteLine($"[{response.Length}] '{response}'");
                        return response;
                    }
                }

                return $"Read Error Code = {nIOStatus}, 0x{nIOStatus.ToString("X")}";
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
                if (_usb != null)
                {
                    if (_deviceID > 0) ioStatus = _usb.Read(_deviceID, sbResponse);
                    else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = _usb.Read(_deviceKey, sbResponse);
                }
                if (ioStatus == 0)
                {
                    response = sbResponse.ToString();
//                    Debug.WriteLine($"[{response.Length}] '{response}'");
                    return true;
                }
                response = $"TryRead Error Code = {ioStatus}, 0x{ioStatus.ToString("X")}";
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
                if (_usb != null)
                {
                    if (_deviceID > 0) ioStatus = _usb.ReadBinary(_deviceID, sbResponse);
                    else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = _usb.ReadBinary(_deviceKey, sbResponse);
                }
                if (ioStatus == 0)
                {
                    response = sbResponse.ToString();
//                    Debug.WriteLine($"Read {response.Length} bytes");
//                    Debug.WriteLine($"[{response.Length}] '{response}'");
                    return true;
                }
                response = $"TryReadBinary Error Code = {ioStatus}, 0x{ioStatus.ToString("X")}";
            }
            catch (Exception ex)
            {
                response = $"Could not read the command response.\r\n{ex.Message}";
            }
            Debug.WriteLine(response);
            return false;
        }
        
        public bool ReadBinaryUntil(string match, out string response, out int ioStatus)
        {
            ioStatus = USB.m_knUSBAddrNotFound;
            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                int readCount=0;
                DateTime start = DateTime.Now;
                var sbResponse = new StringBuilder(1024);
                if (_usb != null)
                {
                    ioStatus = 0;
                    while (ioStatus == 0 && !sbResponse.ToString().Contains(match))
                    {
                        var sbTemp = new StringBuilder(64);
                        if (_deviceID > 0) ioStatus = _usb.ReadBinary(_deviceID, sbTemp);
                        else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = _usb.ReadBinary(_deviceKey, sbTemp);
                        sbResponse.Append(sbTemp);
                        readCount++;
                    }
                }
                if (ioStatus == 0)
                {
                    Debug.WriteLine($"{readCount} reads in {(DateTime.Now - start).TotalSeconds} sec");
                    response = sbResponse.ToString();
//                    Debug.WriteLine($"Read {response.Length} bytes");
//                    Debug.WriteLine($"[{response.Length}] '{response}'");
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

        /// <summary>
        /// This method repeatedly queries the sample count until it matches the passed in sample size,
        /// or until a timeout occurs.
        /// </summary>
        /// <param name="sampleSize">The sample size that the sample count should match.</param>
        /// <returns>The sample count.</returns>
        public uint WaitForDataStore(uint sampleSize)
        {
            uint nSamples = 0;
            var nStatus = 0;
            var sbResponse = new StringBuilder(maxTransferLength);
            var startTime = DateTime.Now;
            var endTime = startTime;

            // Repeat until an error occurs
            while (nStatus == 0)
            {
                Thread.Sleep(100);
                sbResponse.Length = 0;
                // Query the sample count
                var writeResponse = Write(NewportScpiCommands.DataStoreCountQuery);

                if (string.IsNullOrEmpty(writeResponse))
                {
                    // Read the sample count
                    sbResponse.Capacity = maxTransferLength;
                    string response;
                    if (TryRead(out response, out nStatus))
                    {
                        sbResponse.Append(response);
                        nSamples = Convert.ToUInt32(sbResponse.ToString(), 10);

                        // If the sample count matches the sample size
                        if (nSamples == sampleSize)
                        {
                            break;
                        }
                    }
                }
                
                endTime = DateTime.Now;
                Debug.WriteLine($"{nSamples} Samples ready.  {(endTime - startTime).TotalSeconds}");
            }

            return nSamples;
        }

        public int ReadDataStoreValues(uint sampleCount, out List<double> data)
        {
            data = new List<double>((int)sampleCount);
            var status = -1;

            {
                Write($"pm:ds:get? +{sampleCount}\r");

                // Read header....
                string response;
                if (!ReadBinaryUntil(NewportUsbPowerMeter.END_OF_HEADER, out response, out status)) return status;

                // Copy remaining data (after 'End Of Header'), which is actual data, to receive buffer. 
                var markerIndex = response.IndexOf(NewportUsbPowerMeter.END_OF_HEADER, StringComparison.Ordinal) + NewportUsbPowerMeter.END_OF_HEADER.Length;

                var receivedString = response.Substring(markerIndex);
                // Read remaining data
                if (!ReadBinaryUntil(NewportUsbPowerMeter.END_OF_DATA, out response, out status)) return status;

                markerIndex = response.IndexOf(NewportUsbPowerMeter.END_OF_DATA, StringComparison.Ordinal);
                // Concatinate received data
                receivedString += response.Substring(0, markerIndex);

                // split on delimiter
                var split = receivedString.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != sampleCount)
                {
                    Debug.WriteLine($"Retrieved count {split.Length} != sample count {sampleCount}");
                }
                else
                {
                    foreach (var str in split)
                    {
                        double result;
                        if (double.TryParse(str, out result))
                        {
                            data.Add(result);
                        }
                    }
                }
            }
            return status;
        }

        private int DisplayDeviceTable()
        {
            var hashTable = _usb.GetDeviceTable();

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
            var devices = _usb.GetDevInfoList();
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

        private uint _samples = 0;
        public void ContinuousReading(uint samples)
        {
            if (samples == _samples) return;
            if (_samples > 0)
            {   // already running
                _samples = samples;
            }
            else
            {   // Not currently running
                if (samples == 0) return;
                _samples = samples;
                ThreadPool.QueueUserWorkItem(RunContinuousTask, null);
            }
        }



        public virtual event Action<List<double>> ContinuousData;
        public int SamplesRead { get; private set; }

        protected virtual void RunContinuousTask(object unused)
        {
            SamplesRead = 0;
            while (true)
            {
                var samples = _samples;
                if (samples <= 0) break;
                Flush();
                WaitForDataStore(samples);

                var samplesReady = WaitForDataStore(samples);
                if (samplesReady > samples)
                {
                    var result = new List<double>((int) _samples);
                    ReadDataStoreValues(samples, out result);
                    SamplesRead += result.Count;
                    ContinuousData?.Invoke(result);
                }
            }
        }
    }
}
