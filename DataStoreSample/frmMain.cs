using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newport.USBComm;

namespace DataStoreSample
{
    /// <summary>
    /// This class defines the main form for the Data Store Sample Application.
    /// </summary>
    public partial class frmMain : Form
    {
        /// <summary>
        /// The maximum string length for an I/O transfer.
        /// </summary>
        private const int m_kMaxXferLen = 64;

        /// <summary>
        /// The USB communication object.
        /// </summary>
        private USB m_USB;
        /// <summary>
        /// The USB address of the device that is being communicated with.
        /// </summary>
        private int m_nDeviceID;
        /// <summary>
        /// True if the devices are connected, otherwise false.
        /// </summary>
        private bool m_bConnected = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public frmMain ()
        {
            InitializeComponent ();
            m_USB = new USB ();
        }

        /// <summary>
        /// This method attempts to connect to the devices attached via USB cable.
        /// </summary>
        private void ConnectDevices ()
        {
            try
            {
                if (m_USB != null)
                {
                    // If a device is attached via USB or if it cannot be determined
                    var count = m_USB.NumProductsConnected(Program.m_kstrVendorProductID);
                    if (count != 0)
                    {
                        // Open all devices on the USB bus with the specified product ID
                        if (m_USB.OpenDevices (Program.m_knProductID))
                        {
                            // Get the device ID of the first device in the list
                            ArrayList alDevInfoList = m_USB.GetDevInfoList ();
                            DevInfo devInfo = (DevInfo) alDevInfoList[0];
                            m_nDeviceID = devInfo.ID;
                            m_bConnected = true;
                        }
                    }
                }

                if (!m_bConnected)
                {
                    MessageBox.Show ("Could not establish communication with the power meter.", "Connect", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Display the exception message
                StringBuilder sbMsg = new StringBuilder (256);
                sbMsg.AppendFormat ("Could not establish communication with the power meter.\n{0}", ex.Message);
                MessageBox.Show (sbMsg.ToString (), "Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method handles the click event for the DSGet button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClick_btnGet (object sender, EventArgs e)
        {
            try
            {
                // If the devices are not connected
                if (!m_bConnected)
                {
                    // Attempt to connect the devices
                    ConnectDevices ();
                }

                // If the devices are not connected
                if (!m_bConnected)
                {
                    return;
                }

                // Get the sample size from the edit box on the form
                int nSampleSize = GetSampleSize ();

                // If the sample size is not valid
                if (nSampleSize == 0)
                {
                    return;
                }

                StringBuilder sbCmd = new StringBuilder ();
                StringBuilder sbResponse = new StringBuilder ();
                int nSamples = 0;
                this.rtbResponse.Text = "";

                int nStatus = m_USB.Write (m_nDeviceID, "pm:ds:buffer 0\r");

                if (nStatus == 0)
                {
                    nStatus = m_USB.Write (m_nDeviceID, "pm:ds:clear\r");
                }
                if (nStatus == 0)
                {
                    nStatus = m_USB.Write (m_nDeviceID, "pm:ds:interval 1\r");
                }
                if (nStatus == 0)
                {
                    sbCmd.AppendFormat ("pm:ds:size {0}\r", nSampleSize);
                    nStatus = m_USB.Write (m_nDeviceID, sbCmd.ToString ());
                }
                if (nStatus == 0)
                {
                    nStatus = m_USB.Write (m_nDeviceID, "pm:ds:enable 1\r");
                }

                if (nStatus == 0)
                {
                    nSamples = GetSampleCount (nSampleSize);
                    nStatus = m_USB.Write (m_nDeviceID, "pm:ds:enable 0\r");
                }

                if (nStatus == 0)
                {
                    nStatus = GetDataStoreValues (nSamples);
                }

                if (nStatus != 0)
                {
                    this.rtbResponse.Text += "\rStatus = " + nStatus.ToString ();
                }
            }
            catch (Exception ex)
            {
                // Display the exception message
                StringBuilder sbMsg = new StringBuilder (256);
                sbMsg.AppendFormat ("Could not complete the DS:GET? query.\n{0}", ex.Message);
                MessageBox.Show (sbMsg.ToString (), "DS:GET?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method gets the sample size from the edit box on the form
        /// </summary>
        /// <returns>The sample size.</returns>
        private int GetSampleSize ()
        {
            int nSampleSize = 0;

            try
            {
                // If the edit box is not empty
                if (txtSampleSize.Text.Length > 0)
                {
                    nSampleSize = Convert.ToInt32 (txtSampleSize.Text);
                }

                // If the sample size is within the valid range
                if (nSampleSize >= 1 && nSampleSize <= 250000)
                {
                    return nSampleSize;
                }
                else
                {
                    MessageBox.Show ("The sample size must be from 1 to 250,000.", "DS:GET?",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txtSampleSize.SelectAll ();
                    this.txtSampleSize.Focus ();
                }
            }
            catch (Exception ex)
            {
                // Display the exception message
                StringBuilder sbMsg = new StringBuilder (256);
                sbMsg.AppendFormat ("Could not read the sample size.\n{0}", ex.Message);
                MessageBox.Show (sbMsg.ToString (), "DS:GET?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtSampleSize.SelectAll ();
                this.txtSampleSize.Focus ();
            }

            return 0;
        }

        /// <summary>
        /// This method repeatedly queries the sample count until it matches the passed in sample size,
        /// or until a timeout occurs.
        /// </summary>
        /// <param name="sampleSize">The sample size that the sample count should match.</param>
        /// <returns>The sample count.</returns>
        private int GetSampleCount (int sampleSize)
        {
            int nSamples = 0;
            int nStatus = 0;
            StringBuilder sbResponse = new StringBuilder ();
            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = dtStart;

            // Repeat until an error occurs
            while (nStatus == 0)
            {
                Thread.Sleep (1000);
                // Query the sample count
                nStatus = m_USB.Write (m_nDeviceID, "pm:ds:count?\r");

                if (nStatus == 0)
                {
                    // Read the sample count
                    sbResponse.Capacity = m_kMaxXferLen;
                    nStatus = m_USB.Read (m_nDeviceID, sbResponse);

                    if (nStatus == 0)
                    {
                        nSamples = Convert.ToInt32 (sbResponse.ToString (), 10);
                        this.rtbResponse.Text = "Samples = " + nSamples.ToString ();
                        this.rtbResponse.Update ();

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
        private int GetDataStoreValues (int sampleCount)
        {
            int nStatus = 0;
            StreamWriter swDataStore = null;

            try
            {
                StringBuilder sbCmd = new StringBuilder ();
                sbCmd.AppendFormat ("pm:ds:get? +{0}\r", sampleCount);
                nStatus = m_USB.Write (m_nDeviceID, sbCmd.ToString ());

                int nIdx = -1;
                swDataStore = new StreamWriter ("DataStore.txt", false);
                this.rtbResponse.Text = "See DataStore.txt for the results.";
                string strEndOfData = "End of Data\r\n";
                StringBuilder sbCompareBuf = new StringBuilder ();
                StringBuilder sbResponse = new StringBuilder ();

                StringBuilder sbWriteBuf = new StringBuilder (5120);
                sbWriteBuf.AppendFormat ("{0}\r\n", sampleCount);
                DateTime dtStart = DateTime.Now;

                while (nStatus == 0 && nIdx <= 0)
                {
                    // Set the compare buffer to the previous response data
                    sbCompareBuf.Remove (0, sbCompareBuf.Length);
                    sbCompareBuf.Append (sbResponse.ToString ());

                    // Read the response data
                    sbResponse.Remove (0, sbResponse.Length);
                    sbResponse.Capacity = m_kMaxXferLen;
                    nStatus = m_USB.ReadBinary (m_nDeviceID, sbResponse);

                    if (nStatus == 0)
                    {
                        // Append the current response data to the compare buffer
                        sbCompareBuf.Append (sbResponse.ToString ());
                        nIdx = sbCompareBuf.ToString ().IndexOf (strEndOfData);

                        sbWriteBuf.Append (sbResponse.ToString ());

                        if (sbWriteBuf.Length > 5000)
                        {
                            // Write to the output file
                            swDataStore.Write (sbWriteBuf.ToString ());
                            sbWriteBuf.Remove (0, sbWriteBuf.Length);

                            // Update the elapsed time
                            UpdateDSGetTime (dtStart);
                        }
                    }
                }

                // Write to the output file
                swDataStore.Write (sbWriteBuf.ToString ());

                // Update the elapsed time
                UpdateDSGetTime (dtStart);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (swDataStore != null)
                {
                    swDataStore.Close ();
                }
            }

            return nStatus;
        }

        /// <summary>
        /// This method updates the elapsed time display for the DS:Get query.
        /// </summary>
        /// <param name="startTime"></param>
        private void UpdateDSGetTime (DateTime startTime)
        {
            // Update the elapsed time
            double dSeconds = ElapsedMilliseconds (startTime, DateTime.Now) * 1000;
            this.txtTime.Text = FormatTimeData ("{0:f}", dSeconds);
            this.txtTime.Update ();
        }

        /// <summary>
        /// This method returns the number of elapsed milliseconds between the passed in start and end times.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>The number of elapsed milliseconds.</returns>
        private double ElapsedMilliseconds (DateTime startTime, DateTime endTime)
        {
            TimeSpan tsDiff = endTime.Subtract (startTime);
            return tsDiff.TotalMilliseconds;
        }

        /// <summary>
        /// This method gets the time data and formats it according to the specified format.
        /// </summary>
        /// <param name="format">The format string for the time data.</param>
        /// <param name="time">The number of microseconds.</param>
        /// <returns>The formatted string.</returns>
        private string FormatTimeData (string format, double time)
        {
            StringBuilder sbTime = new StringBuilder (32);
            string strUnits = GetTimeUnits (ref time);
            sbTime.AppendFormat (format, time);
            sbTime.AppendFormat (" {0}", strUnits);
            return sbTime.ToString ();
        }

        /// <summary>
        /// This method converts the passed in time from microseconds to the appropriate units.  
        /// It then returns the time and unit specifier.
        /// </summary>
        /// <param name="time">Input is the number of microseconds.  Output is the converted time.</param>
        /// <returns>The unit specifier.</returns>
        private string GetTimeUnits (ref double time)
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