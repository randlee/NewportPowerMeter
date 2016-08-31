using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newport.Usb;
using Newport.USBComm;

namespace DataStoreSample
{
    /// <summary>
    /// This class defines the main form for the Data Store Sample Application.
    /// </summary>
    public partial class DataStoreForm : Form
    {
        /// <summary>
        /// The maximum string length for an I/O transfer.
        /// </summary>
        private const int m_kMaxXferLen = 64;

        /// <summary>
        /// The USB communication object.
        /// </summary>
        private readonly NewportUsbPowerMeter _newport;

        ///// <summary>
        ///// The USB address of the device that is being communicated with.
        ///// </summary>
        // private int m_nDeviceID;
        /// <summary>
        /// True if the devices are connected, otherwise false.
        /// </summary>
        private bool _connected = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataStoreForm()
        {
            InitializeComponent();
            _newport = new NewportUsbPowerMeter();
        }

        /// <summary>
        /// This method attempts to connect to the devices attached via USB cable.
        /// </summary>
        private void ConnectDevices()
        {
            try
            {
                if (_newport != null)
                {
                    _connected = _newport.Connect();

// If a device is attached via USB or if it cannot be determined
                    // var count = _newport.NumProductsConnected(Program.m_kstrVendorProductID);
                    // if (count != 0)
                    // {
                    // // Open all devices on the USB bus with the specified product ID
                    // if (_newport.OpenDevices(Program.m_knProductID))
                    // {
                    // // Get the device ID of the first device in the list
                    // ArrayList alDevInfoList = _newport.GetDevInfoList();
                    // var devInfo = (DevInfo) alDevInfoList[0];
                    // m_nDeviceID = devInfo.ID;
                    // _connected = true;
                    // }
                    // }
                }

                if (!_connected)
                {
                    MessageBox.Show("Could not establish communication with the power meter.", "Connect",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show($"Could not establish communication with the power meter.\n{ex.Message}", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method handles the click event for the DSGet button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClick_btnGet(object sender, EventArgs e)
        {
            try
            {
                // If the devices are not connected
                if (!_connected)
                {   // Attempt to connect the devices
                    ConnectDevices();
                }

                // If the devices are not connected
                if (!_connected)
                {
                    return;
                }

                // Get the sample size from the edit box on the form
                var nSampleSize = GetSampleSize();

                // If the sample size is not valid
                if (nSampleSize <= 0) { return; }

                var sbCmd = new StringBuilder();
//                var sbResponse = new StringBuilder();
                var nSamples = 0;
                rtbResponse.Text = string.Empty;

                var status = _newport.Write("pm:ds:buffer 0\r");

                if (string.IsNullOrEmpty(status))
                {
                    status = _newport.Write("pm:ds:clear\r");
                }

                if (string.IsNullOrEmpty(status))
                {
                    status = _newport.Write("pm:ds:interval 1\r");
                }

                if (string.IsNullOrEmpty(status))
                {
                    sbCmd.Append($"pm:ds:size {nSampleSize}\r");
                    status = _newport.Write(sbCmd.ToString());
                }

                if (string.IsNullOrEmpty(status))
                {
                    _newport.Flush();
                    status = _newport.Write("pm:ds:enable 1\r");
                }

                if (string.IsNullOrEmpty(status))
                {
                    nSamples = GetSampleCount(nSampleSize);
                    status = _newport.Write("pm:ds:enable 0\r");
                }

                if (string.IsNullOrEmpty(status))
                {
                    List<double> data;
                    var ioStatus = GetDataStoreValues(nSamples, out data);
                    if (ioStatus == 0)
                    {
                        rtbResponse.Text = $"Count={data.Count} Min={data.Min()} Max={data.Max()}";
                    }
                    else
                    {
                        rtbResponse.Text += $"\rStatus = {ioStatus}";
                    }
                }

                if (!string.IsNullOrEmpty(status))
                {
                    rtbResponse.Text += $"\rStatus = {status}";
                }
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show($"Could not complete the DS:GET? query.\n{ex.Message}", "DS:GET?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method gets the sample size from the edit box on the form
        /// </summary>
        /// <returns>The sample size.</returns>
        private int GetSampleSize()
        {
            var nSampleSize = 0;

            try
            {
                // If the edit box is not empty
                if (txtSampleSize.Text.Length > 0)
                {
                    nSampleSize = Convert.ToInt32(txtSampleSize.Text);
                }

                // If the sample size is within the valid range
                if (nSampleSize >= 1 && nSampleSize <= 250000)
                {
                    return nSampleSize;
                }
                else
                {
                    MessageBox.Show("The sample size must be from 1 to 250,000.", "DS:GET?", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSampleSize.SelectAll();
                    txtSampleSize.Focus();
                }
            }
            catch (Exception ex)
            {
                // Display the exception message
                var sbMsg = new StringBuilder(256);
                sbMsg.AppendFormat("Could not read the sample size.\n{0}", ex.Message);
                MessageBox.Show(sbMsg.ToString(), "DS:GET?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSampleSize.SelectAll();
                txtSampleSize.Focus();
            }

            return 0;
        }

        /// <summary>
        /// This method repeatedly queries the sample count until it matches the passed in sample size,
        /// or until a timeout occurs.
        /// </summary>
        /// <param name="sampleSize">The sample size that the sample count should match.</param>
        /// <returns>The sample count.</returns>
        private int GetSampleCount(int sampleSize)
        {
            var nSamples = 0;
            var nStatus = 0;
            var sbResponse = new StringBuilder();
            var dtStart = DateTime.Now;
            var dtEnd = dtStart;

            // Repeat until an error occurs
            while (nStatus == 0)
            {
                Thread.Sleep(1000);
                
                // Query the sample count
                var writeResponse = _newport.Write("pm:ds:count?\r");

                if (string.IsNullOrEmpty(writeResponse))
                {
                    // Read the sample count
                    sbResponse.Capacity = m_kMaxXferLen;
                    string response;
                    if (_newport.TryRead(out response, out nStatus))
                    {
                        sbResponse.Append(response);
                        nSamples = Convert.ToInt32(sbResponse.ToString(), 10);
                        rtbResponse.Text = "Samples = " + nSamples;
                        rtbResponse.Update();

                        // If the sample count matches the sample size
                        if (nSamples == sampleSize)
                        {
                            break;
                        }
                    }
                }

                dtEnd = DateTime.Now;
            }

            return nSamples;
        }

        /// <summary>
        /// This method retrieves the data store values and writes them to a file.
        /// </summary>
        /// <param name="sampleCount"></param>
        /// <returns>The I/O status.</returns>
        private int GetDataStoreValues(int sampleCount, out List<double> data)
        {
            StreamWriter writer = null;
            data = new List<double>(sampleCount);
            int status = -1;
            try
            {
                var writeResponse = _newport.Write($"pm:ds:get? +{sampleCount}\r");
                var startTime = DateTime.Now;
                var sbWriteBuf = new StringBuilder(5120);
                var nIdx = -1;
                string response;
                if (_newport.ReadBinaryUntil(NewportUsbPowerMeter.END_OF_HEADER, out response, out status))
                {
                    int markerIndex = response.IndexOf(NewportUsbPowerMeter.END_OF_HEADER) + NewportUsbPowerMeter.END_OF_HEADER.Length;

                    string bufferData = response.Substring(markerIndex);
                    if (_newport.ReadBinaryUntil(NewportUsbPowerMeter.END_OF_DATA, out response, out status))
                    {
                        markerIndex = response.IndexOf(NewportUsbPowerMeter.END_OF_DATA);
                        bufferData += response.Substring(0, markerIndex);
                        using (writer = new StreamWriter("DataStore.txt", false))
                        {
                            rtbResponse.Text = "See DataStore.txt for the results.";
                            //var strEndOfData = "End of Data\r\n";
                            //var sbCompareBuf = new StringBuilder();
                            //var sbResponse = new StringBuilder();

                            sbWriteBuf.AppendFormat("{0}\r\n", sampleCount);
                            sbWriteBuf.Append(bufferData);
                            writer.Write(sbWriteBuf.ToString());

                            var split = bufferData.Split(new[] { "\r\n" } ,StringSplitOptions.RemoveEmptyEntries);
                            if(split.Length != sampleCount)
                            {
                                Debug.WriteLine($"Retrieved count {split.Length} != sample count {sampleCount}");
                            }
                            else
                            {
                                foreach(var str in split)
                                {
                                    int result;
                                    if(int.TryParse(str,out result))
                                    {
                                        data.Add(result);
                                    }
                                }
                            }
                            
                            writer.Write(sbWriteBuf.ToString());
                        }
                    }
                }
                // Update the elapsed time
                UpdateDSGetTime(startTime);
            }
            catch
            {
                throw;
            }
            finally
            {
                writer?.Close();
            }
            return status;
        }

        /// <summary>
        /// This method updates the elapsed time display for the DS:Get query.
        /// </summary>
        /// <param name="startTime"></param>
        private void UpdateDSGetTime(DateTime startTime)
        {
            // Update the elapsed time
            var dSeconds = ElapsedMilliseconds(startTime, DateTime.Now)*1000;
            txtTime.Text = FormatTimeData("{0:f}", dSeconds);
            txtTime.Update();
        }

        /// <summary>
        /// This method returns the number of elapsed milliseconds between the passed in start and end times.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>The number of elapsed milliseconds.</returns>
        private double ElapsedMilliseconds(DateTime startTime, DateTime endTime)
        {
            var tsDiff = endTime.Subtract(startTime);
            return tsDiff.TotalMilliseconds;
        }

        /// <summary>
        /// This method gets the time data and formats it according to the specified format.
        /// </summary>
        /// <param name="format">The format string for the time data.</param>
        /// <param name="time">The number of microseconds.</param>
        /// <returns>The formatted string.</returns>
        private string FormatTimeData(string format, double time)
        {
            var sbTime = new StringBuilder(32);
            var strUnits = GetTimeUnits(ref time);
            sbTime.AppendFormat(format, time);
            sbTime.AppendFormat(" {0}", strUnits);
            return sbTime.ToString();
        }

        /// <summary>
        /// This method converts the passed in time from microseconds to the appropriate units.  
        /// It then returns the time and unit specifier.
        /// </summary>
        /// <param name="time">Input is the number of microseconds.  Output is the converted time.</param>
        /// <returns>The unit specifier.</returns>
        private string GetTimeUnits(ref double time)
        {
            if (time < 1000)
            {
                return "us";
            }
            else if (time < 1000000)
            {
                time /= 1000;
                return "ms";
            }
            else
            {
                time /= 1000000;
                return "sec";
            }
        }
    }
}