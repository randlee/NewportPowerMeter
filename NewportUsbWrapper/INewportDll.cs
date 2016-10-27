using System;
using System.Collections;
using Newport.USBComm;

namespace Newport.Usb
{
    public interface INewportDll : INewportInstrument
    {
        event USB.DeviceStateChangedDelegate DeviceStateChanged;
        bool EventInit(int productID);
        bool ParseDeviceKey(string id, ref string deviceKey);
        Hashtable GetAttachedDevices();

        int ReadBinary(string deviceKey, IntPtr buffer, ref uint bytesRead);
        int ReadBinary(int deviceID, IntPtr buffer, ref uint bytesRead);

        int WriteBinary(string deviceKey, IntPtr buffer, int length);
        int WriteBinary(int deviceID, IntPtr buffer, int length);

        Hashtable GetDeviceTable();
        int GetDeviceKeys(out string[] deviceKeys);
        ArrayList GetDevInfoList();
        int GetDevInfoArrays(out int[] devIDs, out string[] descriptions);
        int NumProductsConnected(string keyVendorProduct);
    }
}