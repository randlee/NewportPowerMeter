using System;
using System.Windows.Forms;
using Newport.Usb;

namespace Visyn.Newport
{
    /// <summary>
    /// This class defines the main form for the C# Sample Application.
    /// </summary>
    public partial class MainForm : Form
    {
        private NewportUsbPowerMeter _powerMeter;
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm ()
        {
            InitializeComponent ();
            OnConnected (false);
            _powerMeter = new NewportUsbPowerMeter();
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
                // Open all devices on the USB bus
                var connected = _powerMeter.Connect();

                OnConnected(connected);
                if (connected)
                {
                    txtCmd.Focus();
                    txtCmd.Text = NewportScpiCommands.Identity;
                    OnSend(this,null);
                    OnRead(this,null);
                }
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show($"Could not connect.\r\n{ex.Message}", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                OnConnected(false);
                _powerMeter?.Disconnect();
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show ($"Could not disconnect.\r\n{ex.Message}", "Disconnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void OnRead(object sender, EventArgs e)
        {
            rtbResponse.Text = _powerMeter.Read();
        }

        private void OnSend(object sender, EventArgs e)
        {
            rtbResponse.Text = _powerMeter.Write(txtCmd.Text);
        }
    }
}