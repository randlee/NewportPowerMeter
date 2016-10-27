using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newport.Usb;
using Newport.Usb.Util;
using Visyn.Newport.Log;

namespace DataStoreSample
{
    /// <summary>
    /// This class defines the main form for the Data Store Sample Application.
    /// </summary>
    public partial class DataStoreForm : Form
    {
        /// <summary>
        /// The USB communication object.
        /// </summary>
        private INewportInstrument _newport;

        private NewportPowerMeter _newportPowerMeter => _newport as NewportPowerMeter;

        ///// <summary>
        ///// The USB address of the device that is being communicated with.
        ///// </summary>
        // private int m_nDeviceID;
        /// <summary>
        /// True if the devices are connected, otherwise false.
        /// </summary>
        private bool _connected = false;

        private CommLogger Logger { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataStoreForm()
        {
            InitializeComponent();
            Logger = new CommLogger();
        }

        private void DataStoreForm_Load(object sender, EventArgs e)
        {
            buttonSaveLog.Visible = Logger != null;
            buttonSaveLog.Enabled = Logger != null;
        }

        /// <summary>
        /// This method handles the click event for the DSGet button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGet_OnClick(object sender, EventArgs e)
        {
            if (_newportPowerMeter == null) return;
            // Get the sample size from the edit box on the form
            var sampleSize = GetSampleSize();
            var interval = GetInterval();

            rtbResponse.Text = sampleSize > 0 ? $"Samples = {sampleSize}" : "Sample Size must be > 0!";
            rtbResponse.Update();
            GetMeasurements(sampleSize,interval);
        }

        private void GetMeasurements(uint sampleSize, uint interval)
        {
            if (sampleSize <= 0) return;
            try
            {
                int ioStatus;
                var data = _newportPowerMeter.GetMeasurements(sampleSize, interval,out ioStatus);
                if (ioStatus == 0)
                {
                    rtbResponse.Text += data.Count > 0 ? $"\rCount={data.Count} Min={data.Min()} Max={data.Max()}" : $"\rCount={data.Count}";
                }
                else
                {
                    rtbResponse.Text += $"\rStatus = {ioStatus}";;
                }
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show($"Could not complete the DS:GET? query.\n{ex.Message}", "DS:GET?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonContinuous_Click(object sender, EventArgs e)
        {
            if (_newportPowerMeter == null) { return; }
 // device not connected...
            try
            {
                // Get the sample size from the edit box on the form
                var nSampleSize = GetSampleSize();
                var interval = GetInterval();

// If the sample size is not valid
                if (nSampleSize <= 0 || interval <= 0) { return; }

                rtbResponse.Text = string.Empty;

                var status = _newportPowerMeter.ContinuousMeasurementSetup(0, interval);
                if (string.IsNullOrEmpty(status))
                {
                    _newportPowerMeter.ContinuousReading(nSampleSize);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    rtbResponse.Text += $"\rStatus = {status}";
                }
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show($"Could not complete the DS:GET? query.\n{ex.Message}", "DS:GET?", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void buttonTriggeredSoftkey_Click(object sender, EventArgs e)
        {
            if(_newportPowerMeter == null) return;
            // device connected...
            try
            {
                var settings = NewportMeasurementSettings.SoftkeyTriggered(GetChannel(), GetInterval(), GetSampleSize(), true);
                rtbResponse.Text = $"Initializing softkey trigger {settings.Samples} samples, {settings.DurationSeconds} sec ";
                _newportPowerMeter.TriggeredMeasurement(settings);
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show($"Could not complete the DS:GET? query.\n{ex.Message}", "DS:GET?", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void buttonTriggerTtlDuration_Click(object sender, EventArgs e)
        {
            if (_newportPowerMeter == null) return;
            // device connected...
            try
            {
#if false
                rtbResponse.Text = string.Empty;
                
// Get the sample size from the edit box on the form
                var sampleSize = GetSampleSize();
                _newport.ResetMeasurement();
                _newport.TriggeredMeasurement(true, 0, sampleSize/10000.0, TriggerStartEvent.ExternalTrigger);
                updateButtonState(true);
#endif
                var settings = NewportMeasurementSettings.TtlTriggered(GetChannel(), GetInterval(), GetSampleSize(), true, true);
                rtbResponse.Text = $"Initializing ttl-ttl trigger {settings.Samples} samples, {settings.DurationSeconds} sec ";
                _newportPowerMeter.TriggeredMeasurement(settings);
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show($"Could not complete the DS:GET? query.\n{ex.Message}", "DS:GET?", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void buttonTtlTriggerEdge2Edge_Click(object sender, EventArgs e)
        {
            if(_newportPowerMeter == null) return;
            // device connected...
            try
            {
                var settings = NewportMeasurementSettings.TtlTriggerToTrigger(0, 1, true, true);
                rtbResponse.Text = $"Initializing ttl-ttl trigger max samples: {settings.Samples} samples, max duration: {settings.DurationSeconds} sec ";
                _newportPowerMeter.TriggeredMeasurement(settings);
            }
            catch (Exception ex)
            { // Display the exception message
                MessageBox.Show($"Could not complete the DS:GET? query.\n{ex.Message}", "DS:GET?", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void buttonSCPITriggerCommand_Click(object sender, EventArgs e)
        {
            if (_newportPowerMeter == null) return;
            // device connected...
            try
            {
                var settings = NewportMeasurementSettings.CommandTriggered(GetChannel(), GetInterval(), GetSampleSize(), true);
                rtbResponse.Text = $"Initializing SCPI trigger {settings.Samples} samples, {settings.DurationSeconds} sec ";
                _newportPowerMeter.TriggeredMeasurement(settings);
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show($"Could not complete the DS:GET? query.\n{ex.Message}", "DS:GET?", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void buttonSendScpiTrigger_Click(object sender, EventArgs e)
        {
            _newport.Write(NewportScpi.TriggerStateTriggered);
        }

        private List<double> _data;

        private void _newport_ContinuousData(List<double> data)
        {
            if (rtbResponse.InvokeRequired)
            {
                BeginInvoke(new Action(() => _newport_ContinuousData(data)), null);
                return;
            }

            var displayText = rtbResponse.Text;
            if (!string.IsNullOrEmpty(displayText))
                displayText += $"\r\n{DateTime.Now} Rx {data.Count}/{_newportPowerMeter.SamplesRead}";
            else displayText = $"{DateTime.Now} Rx {data.Count}/{_newportPowerMeter.SamplesRead}";
            if (_data == null) _data = data;
            else _data.AddRange(data);
            rtbResponse.Text = displayText;
        }

        private uint GetChannel() => 0;

        /// <summary>
        /// This method gets the sample size from the edit box on the form
        /// </summary>
        /// <returns>The sample size.</returns>
        private uint GetSampleSize() { return (uint)GetTextInt(txtSampleSize, 1, (int)NewportUsbPowerMeter.DATASTORE_SIZE_MAX); }

        public uint GetInterval() {  return (uint)GetTextInt(textBoxInterval, 1, (int)NewportUsbPowerMeter.DATASTORE_SIZE_MAX); }

        private int GetTextInt(TextBox textBox, int min, int max)
        {
            try
            {
                // If the edit box is not empty
                if (textBox.Text.Length > 0)
                {
                    var samplesRequested = Convert.ToInt32(textBox.Text);

// If the sample size is within the valid range
                    if (samplesRequested >= min && samplesRequested <= max)
                    {
                        return samplesRequested;
                    }
                }

                MessageBox.Show($"The sample size must be between {min} and {max}.", "DS:GET?", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Display the exception message
                MessageBox.Show($"Could not read the sample size.\n{ex.Message}", "DS:GET?", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }

            textBox.SelectAll();
            textBox.Focus();
            return min;
        }


        /// <summary>
        /// This method retrieves the data store values and writes them to a file.
        /// </summary>
        /// <param name="sampleCount"></param>
        /// <returns>The I/O status.</returns>
        private int GetDataStoreValues(uint sampleCount, out List<double> data)
        {
            var count = _newportPowerMeter.ReadDataStoreValues(sampleCount, out data);
            var status = saveFile(sampleCount, data);

            return status;
        }

        private int saveFile(uint sampleCount, List<double> data)
        {
            StreamWriter writer = null;

            var status = -0;
            try
            {
                var sbWriteBuf = new StringBuilder(5120);

                using (writer = new StreamWriter("DataStore.txt", false))
                {
                    rtbResponse.Text +=
                        $"{sampleCount} samples requested\r\n{data?.Count ?? 0} samples read.\r\nSee DataStore.txt for the results.";

                    sbWriteBuf.AppendFormat("{0}\r\n", sampleCount);
                    if (data != null)
                        foreach (var item in data)
                        {
                            sbWriteBuf.AppendLine(item.ToString(CultureInfo.InvariantCulture));
                        }

                    writer.Write(sbWriteBuf.ToString());
                }
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
        /// <param name="time">The number of microseconds.</param>
        /// <returns>The formatted string.</returns>
        private string FormatTimeData(TimeSpan time)
        {
            if (time.TotalSeconds > 1.0) return $" {time.TotalSeconds} sec";
            var ms = time.TotalMilliseconds;
            return ms > 1.0 ? $" {ms} ms" : $" {ms*1000.0} us";
        }



        private void updateButtonState(NewportState measuring)
        {
            buttonAbort.Enabled = measuring.Measuring;
            buttonContinuous.Enabled = !measuring.Measuring;
            buttonTriggeredSoftkey.Enabled = !measuring.Measuring;
            buttonTriggerCommand.Enabled = !measuring.Measuring;
            buttonTriggerTtlDuration.Enabled = !measuring.Measuring;
            buttonTtlTriggerEdge2Edge.Enabled = !measuring.Measuring;

            buttonSendScpiTrigger.Enabled = measuring.Armed;
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            _newportPowerMeter?.ResetMeasurement();
        }

        private NewportState _previousState = new NewportState();
        private void _newport_MeasuringChanged(NewportState state)
        {
            if (rtbResponse.InvokeRequired)
            {
                BeginInvoke(new Action(() => _newport_MeasuringChanged(state)), null);
                return;
            }

            updateButtonState(state);
            if(state.Measuring)
            {
                if (_previousState.Measuring == false) _data = null;
                if (state.Armed && !_previousState.Armed)
                    rtbResponse.Text += "\r\nTrigger armed";
                if (state.Triggered && !_previousState.Triggered)
                    rtbResponse.Text += "\r\nTriggered";
            }
            else if (_previousState.Measuring)
            {
                rtbResponse.Text += "\r\nMeasurement Complete";
                saveFile(0, _data);
            }

            _previousState = state;
        }


        private void _newport_Profile(string status)
        {
            if (rtbResponse.InvokeRequired)
            {
                BeginInvoke(new Action(() => _newport_Profile(status)), null);
                return;
            }

            rtbResponse.Text += $"\r\n\t{status}";
        }

#region TextBox validation
        private void textBoxInterval_TextChanged(object sender, EventArgs e)
        {
            validateTextboxInt(sender as TextBox, 1, (int)NewportUsbPowerMeter.INTERVAL_MAX);

        }

        private void txtSampleSize_TextChanged(object sender, EventArgs e)
        {
            validateTextboxInt(sender as TextBox, 1, (int)NewportUsbPowerMeter.DATASTORE_SIZE_MAX);
        }

        private bool validateTextboxInt(TextBox textBox, int min, int max)
        {
            var result = 0;
            if (int.TryParse(textBoxInterval.Text, out result))
            {
                if (result >= min && result <= max)
                {
                    textBoxInterval.ForeColor = Color.Black;
                    return true;
                }
            }

            textBoxInterval.ForeColor = Color.Red;
            return false;
        }

        #endregion

        private void buttonSaveLog_Click(object sender, EventArgs e)
        {
            if (Logger == null) return;

            var xmlFile = FileUtil.GetTempFileName("xml");
            var csvFile = FileUtil.GetTempFileName("csv");
            FileUtil.XmlSerialize(Logger, xmlFile);
            FileUtil.SaveDelimitedFileToDisk(csvFile, Logger, null);
            System.Diagnostics.Process.Start(xmlFile);
            System.Diagnostics.Process.Start(csvFile);
        }

        private void buttonConnectUsb_Click(object sender, EventArgs e)
        {
            connect(new NewportDllWrap(Logger));
        }

        void connect(INewportInstrument meter)
        {
            var failText = $"Connection to {meter?.Name} failed!";
            try
            {
                OnDisconnect();
                // Open all devices on the USB bus
                if (meter.OpenDevices())
                {
                    _newport = meter;
                    _newportPowerMeter.Profile += _newport_Profile;
                    _newportPowerMeter.ContinuousData += _newport_ContinuousData;
                    _newportPowerMeter.MeasuringChanged += _newport_MeasuringChanged;
                }
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show($"{failText}\r\n{ex.Message}", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            rtbResponse.Text = _newport != null ? $"Connected to {_newport.Name}" : failText;
            buttonConnectUsb.Enabled = _newport == null;
            buttonConnectSerial.Enabled = _newport == null;
            buttonDisconnect.Enabled = _newport != null;
        }

        private void buttonConnectSerial_Click(object sender, EventArgs e)
        {
            connect(new NewportSerialPowerMeter(new NewportSerialWrap(Logger)));
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            OnDisconnect();
        }

        /// <summary>
        /// This method disconnects the devices attached via USB cable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisconnect()
        {
            try
            {
                _newport?.CloseDevices();
                _newport = null;
                //OnConnected(false);
                //_powerMeter?.CloseDevices();
                //_powerMeter = null;
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show($"Could not disconnect.\r\n{ex.Message}", "Disconnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            buttonConnectUsb.Enabled = _newport == null;
            buttonConnectSerial.Enabled = _newport == null;
            buttonDisconnect.Enabled = _newport != null;
        }
    }
}