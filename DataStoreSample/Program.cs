using System;
using System.Windows.Forms;

namespace DataStoreSample
{
    static class Program
    {
        /// <summary>
        /// The unique Vendor ID / Product ID (from the .inf file)
        /// </summary>
        internal const string m_kstrVendorProductID = "VID_104D&PID_CEC7";
        /// <summary>
        /// The unique Product ID (from the .inf file)
        /// </summary>
        internal const int m_knProductID = 0xCEC7;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main ()
        {
            Application.EnableVisualStyles ();
            Application.SetCompatibleTextRenderingDefault (false);
            Application.Run (new DataStoreForm ());
        }
    }
}