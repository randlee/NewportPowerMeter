using System;
using System.Threading;
using System.Windows.Forms;
using Newport.Usb;
using Visyn.Newport.Log;

namespace Visyn.Newport
{
    /// <summary>
    /// This class defines the main form for the C# Sample Application.
    /// </summary>
    public partial class MainForm : Form
    {
        private INewportInstrument _powerMeter;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm ()
        {
            InitializeComponent ();
            OnConnected (false);
            _powerMeter = new NewportUsbPowerMeter(null);
        }

        /// <summary>
        /// This method enables / disables buttons according to the current connection state.
        /// </summary>
        /// <param name="isConnected">True if connected, false if disconnected.</param>
        private void OnConnected (bool isConnected)
        {
            btnConnectUsb.Enabled = !isConnected;
            buttonConnectSerial.Enabled = !isConnected;
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
                if(_powerMeter != null) OnDisconnect(this, null);
                var meter = new NewportDllWrap(ComLogger);
                _powerMeter = meter;
                // Open all devices on the USB bus
                var connected = meter.OpenDevices();

                OnConnected(connected);
                if (connected)
                {
                    txtCmd.Focus();
                    txtCmd.Text = NewportScpi.Identity;
                    OnQuery(this,null);
                }
                else
                {
                    rtbResponse.Text = "Connection failed";
                }
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show($"Could not connect.\r\n{ex.Message}", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private CommLogger ComLogger { get; } = new CommLogger();

        private void buttonConnectSerial_Click(object sender, EventArgs e)
        {
            connect(new NewportSerialPowerMeter(new NewportSerialWrap(ComLogger))); 
        }

        private void connect(INewportInstrument meter)
        {
            try
            {
                if (_powerMeter != null) OnDisconnect(this, null);
                _powerMeter = meter;
                // Open all devices on the USB bus
                var connected = meter.OpenDevices();

                OnConnected(connected);
                if (connected)
                {
                    txtCmd.Focus();
                    txtCmd.Text = NewportScpi.Identity;
                    OnSend(this, null);
                    OnRead(this, null);
                }
            }
            catch (Exception ex)
            {
                // Display the exception message
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
                _powerMeter?.CloseDevices();
                _powerMeter = null;
            }
            catch (Exception ex)
            {   // Display the exception message
                MessageBox.Show ($"Could not disconnect.\r\n{ex.Message}", "Disconnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void OnRead(object sender, EventArgs e)
        {
            rtbResponse.Text += $": {_powerMeter.Read()}\r\n";
        }

        private void OnSend(object sender, EventArgs e)
        {
            if(_powerMeter.Write(txtCmd.Text + '\r') == 0) rtbResponse.Text = txtCmd.Text;
        }

        private void OnQuery(object sender, EventArgs e)
        {
            OnSend(this,e);
            Thread.Sleep(250);
            OnRead(this,e);
        }
    }
}