using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newport.Usb;

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
        private const int maxTransferLength = 64;

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
                _connected = _newport != null && _newport.Connect();

                if (!_connected)
                {
                    MessageBox.Show("Could not establish communication with the power meter.", "Connect",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show($"Could not establish communication with the power meter.\n{ex.Message}", "Connect Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (!_connected) {  return; }

                // Get the sample size from the edit box on the form
                var nSampleSize = GetSampleSize();

                // If the sample size is not valid
                if (nSampleSize <= 0)
                {
                    return;
                }

                var sbCmd = new StringBuilder();

// var sbResponse = new StringBuilder();
                var nSamples = 0;
                rtbResponse.Text = string.Empty;

                var status = _newport.Write(NewportScpiCommands.DataStoreBuffer(0));

                if (string.IsNullOrEmpty(status))
                {
                    status = _newport.Write(NewportScpiCommands.DataStoreClear);
                }

                if (string.IsNullOrEmpty(status))
                {
                    status = _newport.Write(NewportScpiCommands.DataStoreInterval(1));
                }

                if (string.IsNullOrEmpty(status))
                {
                    status = _newport.Write(NewportScpiCommands.DataStoreSize(nSampleSize));
                }

                if (string.IsNullOrEmpty(status))
                {
                    _newport.Flush();
                    status = _newport.Write(NewportScpiCommands.DataStoreEnable);
                }

                if (string.IsNullOrEmpty(status))
                {
                    nSamples = GetSampleCount(nSampleSize);
                    status = _newport.Write(NewportScpiCommands.DataStoreDisable);
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
        private uint GetSampleSize()
        {
            try
            {   // If the edit box is not empty
                if (txtSampleSize.Text.Length > 0)
                {
                    var nSampleSize = Convert.ToUInt32(txtSampleSize.Text);
                    // If the sample size is within the valid range
                    if (nSampleSize >= 1 && nSampleSize <= 250000)
                    {
                        return nSampleSize;
                    }
                }
                MessageBox.Show("The sample size must be between 1 and 250,000.", "DS:GET?",  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show($"Could not read the sample size.\n{ex.Message}", "DS:GET?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            txtSampleSize.SelectAll();
            txtSampleSize.Focus();
            return 0;
        }

        /// <summary>
        /// This method repeatedly queries the sample count until it matches the passed in sample size,
        /// or until a timeout occurs.
        /// </summary>
        /// <param name="sampleSize">The sample size that the sample count should match.</param>
        /// <returns>The sample count.</returns>
        private int GetSampleCount(uint sampleSize)
        {
            var nSamples = 0;
            var nStatus = 0;
            var sbResponse = new StringBuilder();
            var startTime = DateTime.Now;
            var endTime = startTime;

            // Repeat until an error occurs
            while (nStatus == 0)
            {
                Thread.Sleep(1000);
                
                // Query the sample count
                var writeResponse = _newport.Write(NewportScpiCommands.DataStoreCountQuery);

                if (string.IsNullOrEmpty(writeResponse))
                {
                    // Read the sample count
                    sbResponse.Capacity = maxTransferLength;
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

                endTime = DateTime.Now;
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
            var status = -1;
            try
            {
                _newport.Write($"pm:ds:get? +{sampleCount}\r");
                var startTime = DateTime.Now;
                var sbWriteBuf = new StringBuilder(5120);

                string response;
                if (_newport.ReadBinaryUntil(NewportUsbPowerMeter.END_OF_HEADER, out response, out status))
                {
                    var markerIndex = response.IndexOf(NewportUsbPowerMeter.END_OF_HEADER, StringComparison.Ordinal) + NewportUsbPowerMeter.END_OF_HEADER.Length;

                    var bufferData = response.Substring(markerIndex);
                    if (_newport.ReadBinaryUntil(NewportUsbPowerMeter.END_OF_DATA, out response, out status))
                    {
                        markerIndex = response.IndexOf(NewportUsbPowerMeter.END_OF_DATA, StringComparison.Ordinal);
                        bufferData += response.Substring(0, markerIndex);
                        using (writer = new StreamWriter("DataStore.txt", false))
                        {
                            rtbResponse.Text = "See DataStore.txt for the results.";

// var strEndOfData = "End of Data\r\n";
                            // var sbCompareBuf = new StringBuilder();
                            // var sbResponse = new StringBuilder();
                            sbWriteBuf.AppendFormat("{0}\r\n", sampleCount);
                            sbWriteBuf.Append(bufferData);
                            writer.Write(sbWriteBuf.ToString());

                            var split = bufferData.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
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

                            writer.Write(sbWriteBuf.ToString());
                        }
                    }
                }

// Update the elapsed time
                UpdateDSGetTime(startTime);
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
            txtTime.Text = FormatTimeData(DateTime.Now.Subtract(startTime));
            txtTime.Update();
        }


        /// <summary>
        /// This method gets the time data and formats it according to the specified format.
        /// </summary>
        /// <param name="format">The format string for the time data.</param>
        /// <param name="time">The number of microseconds.</param>
        /// <returns>The formatted string.</returns>
        private string FormatTimeData(TimeSpan time)
        {
            if (time.TotalSeconds > 1.0) return $" {time.TotalSeconds} sec";
            var ms = time.TotalMilliseconds;
            return ms > 1.0 ? $" {ms} ms" : $" {ms*1000.0} us";
        }
    }
}