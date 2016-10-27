// Decompiled with JetBrains decompiler
// Type: Newport.USBComm.USB
// Assembly: UsbDllWrap, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF673B4-A5B6-4BF0-8499-DD5C94CDB07F
// Assembly location: C:\Customers\Orb\NewportPowerMeter\NewportUsbWrapper\USBWrap.dll

namespace Newport.USBComm
{
    public class NewportDll //: INewportDll
    {
#if false
        private ArrayList c = new ArrayList();
        public const int m_knIOError = -1;
        public const int m_knUSBAddrNotFound = -2;
        public const int m_knWDStatusInvalidWDHandle = -1;
        public const int m_knWDDeviceNotFound = 536870927;
        public const uint m_knWDDeviceNotResponding = 3221225477;
        public const uint m_knWDStatusEndpointHalted = 3221225520;
        private USB.a a;
        private string[] d;
        private Hashtable e;
        private Hashtable f;
        private bool g;
        private bool h;

        public string LogFilePath => global::c.b();

        public bool Logging
        {
            get
            {
                return this.h;
            }
            set
            {
                this.h = value;
            }
        }

        public event USB.DeviceStateChangedDelegate DeviceStateChanged;

        public NewportDll()
        {
            this.c();
        }

        public NewportDll(bool bLogging)
        {
            this.h = bLogging;
            this.c();
        }

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_init_system();

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_init_product(int A_0);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_open_devices(int A_0, bool A_1, ref int A_2);

        [DllImport("usbdll.dll")]
        private static extern void newp_usb_uninit_system();

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_event_init(int A_0, USB.a A_1);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_event_assign_key(string A_0, int A_1);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_event_remove_key(string A_0);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_event_get_attached_devices([In, Out] string[] A_0, [Out] int[] A_1);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_event_get_key_from_handle(int A_0, StringBuilder A_1);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_get_device_info(StringBuilder A_0);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_get_model_serial_keys([In, Out] string[] A_0);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_get_ascii(int A_0, StringBuilder A_1, int A_2, ref uint A_3);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_get_ascii(int A_0, IntPtr A_1, int A_2, ref uint A_3);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_send_ascii(int A_0, string A_1, int A_2);

        [DllImport("usbdll.dll")]
        private static extern int newp_usb_send_binary(int A_0, IntPtr A_1, int A_2);

        private void c()
        {
            this.b();
            this.a = new USB.a(this.a);
        }

        private void b()
        {
            try
            {
                if (!this.h && !File.Exists(global::c.b()))
                    return;
                global::c.a("********** Entry Path = {0} **********", (object)global::b.d());
                global::c.a("OS: {0} Version: {1}", new object[2]
                {
          (object) this.a(),
          (object) Environment.OSVersion.Version
                });
                bool flag1 = global::a.a();
                bool flag2 = global::a.b();
                global::c.a("{0}-bit OS, {1}-bit mode.", new object[2]
                {
          flag1 ? (object) "64" : (object) "32",
          flag2 ? (object) "64" : (object) "32"
                });
                string str = Environment.GetFolderPath(Environment.SpecialFolder.System);
                if (flag1 && !flag2)
                {
                    StringBuilder stringBuilder = new StringBuilder(128);
                    stringBuilder.AppendFormat("{0}{1}", (object)global::b.a(str), (object)"SysWOW64");
                    str = stringBuilder.ToString();
                }
                global::c.a("System Folder: {0}", (object)str);
                StringBuilder stringBuilder1 = new StringBuilder(128);
                stringBuilder1.AppendFormat("{0}\\UsbDll.dll", (object)str);
                if (File.Exists(stringBuilder1.ToString()))
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(stringBuilder1.ToString());
                    global::c.a("{0} Version {1}", new object[2]
                    {
            (object) stringBuilder1,
            (object) versionInfo.FileVersion
                    });
                }
                else
                    global::c.a("The USB driver is not installed.  {0} does not exist.", (object)stringBuilder1);
                DirectoryInfo directoryInfo = new DirectoryInfo(str);
                if (directoryInfo == null)
                    return;
                foreach (FileInfo file in directoryInfo.GetFiles("wdapi*.dll", SearchOption.TopDirectoryOnly))
                    global::c.a("{0} is installed.", (object)file.FullName);
            }
            catch (Exception ex)
            {
                global::c.a("LogInfo exception.  {0}", (object)ex.Message);
            }
        }

        private string a()
        {
            string str = string.Empty;
            switch (Environment.OSVersion.Version.Major)
            {
                case 5:
                    str = Environment.OSVersion.Version.Minor != 0 ? "Windows XP" : "Windows 2000";
                    break;
                case 6:
                    if (Environment.OSVersion.Version.Minor == 0)
                    {
                        str = "Windows Vista";
                        break;
                    }
                    if (Environment.OSVersion.Version.Minor == 1)
                    {
                        str = "Windows 7";
                        break;
                    }
                    break;
            }
            return str;
        }

        public bool EventInit(int productID)
        {
            try
            {
                this.e = new Hashtable();
                this.f = new Hashtable();
                if (this.h)
                    global::c.a("EventInit:  ProductID = '{0}' (0x{1})", new object[2]
                    {
            (object) productID,
            (object) productID.ToString("X")
                    });
                return USB.newp_usb_event_init(productID, this.a) == 0;
            }
            catch (Exception ex)
            {
                global::c.a("EventInit exception.  {0}", (object)ex.Message);
                return false;
            }
        }

        private void a(int A_0, int A_1)
        {
            try
            {
                if (A_1 == 1)
                {
                    string deviceKey = string.Empty;
                    if (this.CreateDeviceKey(A_0, ref deviceKey))
                    {
                        if (this.h)
                            global::c.a("OnDeviceStateChanged:  Attach DeviceKey = {0}, Handle = {1}", new object[2]
                            {
                (object) deviceKey,
                (object) A_0
                            });
                        USB.newp_usb_event_assign_key(deviceKey, A_0);
                        this.e.Add((object)deviceKey, (object)A_0);
                        this.f.Add((object)deviceKey, (object)A_0);
                        if (this.b == null)
                            return;
                        this.b(deviceKey, USB.eDeviceState.Attached);
                    }
                    else
                    {
                        if (!this.h)
                            return;
                        global::c.a("OnDeviceStateChanged:  Attach Failed.  Could not create DeviceKey from Handle '{0}'.", (object)A_0);
                    }
                }
                else
                {
                    StringBuilder A_1_1 = new StringBuilder();
                    if (USB.newp_usb_event_get_key_from_handle(A_0, A_1_1) == 0)
                    {
                        if (this.h)
                            global::c.a("OnDeviceStateChanged:  Detach DeviceKey = {0}, Handle = {1}", new object[2]
                            {
                (object) A_1_1,
                (object) A_0
                            });
                        if (USB.newp_usb_event_remove_key(A_1_1.ToString()) == 0)
                        {
                            this.e.Remove((object)A_1_1.ToString());
                            this.f.Remove((object)A_1_1.ToString());
                            if (this.b == null)
                                return;
                            this.b(A_1_1.ToString(), USB.eDeviceState.Detached);
                        }
                        else
                        {
                            if (!this.h)
                                return;
                            global::c.a("OnDeviceStateChanged:  Detach Failed.  Could not remove DeviceKey '{0}'.", (object)A_1_1);
                        }
                    }
                    else
                    {
                        if (!this.h)
                            return;
                        global::c.a("OnDeviceStateChanged:  Detach Failed.  Could not get DeviceKey from Handle '{0}'.", (object)A_0);
                    }
                }
            }
            catch (Exception ex)
            {
                global::c.a("OnDeviceStateChanged exception.  {0}", (object)ex.Message);
            }
        }

        protected virtual bool CreateDeviceKey(int handle, ref string deviceKey)
        {
            int num = this.Write(handle, "*IDN?");
            if (num == 0)
            {
                StringBuilder buffer = new StringBuilder(64);
                num = this.Read(handle, buffer);
                if (num == 0)
                    return this.ParseDeviceKey(buffer.ToString(), ref deviceKey);
            }
            return num == 0;
        }

        public virtual bool ParseDeviceKey(string ID, ref string deviceKey)
        {
            string[] strArray = ID.Split(' ');
            if (strArray.Length >= 5)
            {
                StringBuilder stringBuilder = new StringBuilder(ID.Length);
                stringBuilder.Append(strArray[4]);
                for (int index = 5; index < strArray.Length; ++index)
                    stringBuilder.AppendFormat(" {0}", (object)strArray[index]);
                stringBuilder.Insert(0, " ");
                stringBuilder.Insert(0, strArray[1]);
                deviceKey = stringBuilder.ToString();
                return true;
            }
            if (this.h)
                global::c.a("ParseDeviceKey:  Number of tokens '{0}' exceeds the max '{1}'.", new object[2]
                {
          (object) strArray.Length,
          (object) 5
                });
            return false;
        }

        public Hashtable GetAttachedDevices()
        {
            Hashtable hashtable = (Hashtable)null;
            try
            {
                hashtable = new Hashtable();
                if (this.e != null)
                {
                    string[] A_0 = new string[this.e == null ? 0 : this.e.Count];
                    int[] A_1 = new int[this.e == null ? 0 : this.e.Count];
                    for (int index = 0; index < this.e.Count; ++index)
                    {
                        A_0[index] = new string(' ', 32);
                        A_1[index] = 0;
                    }
                    USB.newp_usb_event_get_attached_devices(A_0, A_1);
                    for (int index = 0; index < A_0.GetLength(0); ++index)
                    {
                        hashtable.Add((object)A_0[index], (object)A_1[index]);
                        if (this.h)
                            global::c.a("GetAttachedDevices: DeviceKey #{0} = {1}", new object[2]
                            {
                (object) (index + 1),
                (object) A_0[index]
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                global::c.a("GetAttachedDevices exception.  {0}", (object)ex.Message);
            }
            return hashtable;
        }

        public bool OpenDevices()
        {
            return this.OpenDevices(0);
        }

        public bool OpenDevices(int productId)
        {
            if (this.g)
            {
                if (this.h)
                    global::c.a("OpenDevices: Devices with ProductID '{0}' (0x{1}) are already open.", new object[2]
                    {
            (object) productId,
            (object) productId.ToString("X")
                    });
                return true;
            }
            try
            {
                if (this.h)
                    global::c.a("OpenDevices: Open devices with ProductID '{0}' (0x{1}).", new object[2]
                    {
            (object) productId,
            (object) productId.ToString("X")
                    });
                int num = USB.newp_usb_init_product(productId);
                this.g = num == 0;
                if (this.g || num == 1)
                {
                    StringBuilder A_0 = new StringBuilder(256);
                    int deviceInfo = USB.newp_usb_get_device_info(A_0);
                    if (deviceInfo == 0)
                    {
                        this.a(A_0.ToString());
                        if (this.h)
                        {
                            if (num == 1)
                                global::c.a("OpenDevices:  Error - instruments have duplicate USB addresses.  Change the USB address or open using Device Key.", new object[0]);
                        }
                    }
                    else
                    {
                        if (this.h)
                            global::c.a("OpenDevices:  Could not retrieve the device information.  Error Code = {0}, 0x{1}", new object[2]
                            {
                (object) deviceInfo,
                (object) deviceInfo.ToString("X")
                            });
                        this.CloseDevices();
                    }
                }
                else if (this.h)
                    global::c.a("OpenDevices:  Error Code = {0}, 0x{1}", new object[2]
                    {
            (object) num,
            (object) num.ToString("X")
                    });
            }
            catch (Exception ex)
            {
                global::c.a("OpenDevices exception.  {0}", (object)ex.Message);
                this.CloseDevices();
            }
            return this.g;
        }

        public bool OpenDevices(int productId, bool usingDeviceKey)
        {
            if (!usingDeviceKey)
                return this.OpenDevices(productId);
            if (this.g)
            {
                if (this.h)
                    global::c.a("OpenDevices (By Key): Devices with ProductID '{0}' (0x{1}) are already open.", new object[2]
                    {
            (object) productId,
            (object) productId.ToString("X")
                    });
                return true;
            }
            try
            {
                int A_2 = 0;
                if (this.h)
                    global::c.a("OpenDevices (By Key): Open devices with ProductID '{0}' (0x{1}).", new object[2]
                    {
            (object) productId,
            (object) productId.ToString("X")
                    });
                int num = USB.newp_usb_open_devices(productId, !usingDeviceKey, ref A_2);
                this.g = num == 0;
                if (this.g)
                {
                    this.d = new string[A_2];
                    for (int index = 0; index < A_2; ++index)
                        this.d[index] = new string(' ', 32);
                    int modelSerialKeys = USB.newp_usb_get_model_serial_keys(this.d);
                    if (modelSerialKeys == 0)
                    {
                        this.e = new Hashtable(A_2);
                        this.f = new Hashtable(A_2);
                        for (int index = 0; index < A_2; ++index)
                        {
                            if (this.h)
                                global::c.a("OpenDevices (By Key):  DeviceKey #{0} = '{1}'", new object[2]
                                {
                  (object) (index + 1),
                  (object) this.d[index]
                                });
                            this.e.Add((object)this.d[index], (object)index);
                            this.f.Add((object)this.d[index], (object)index);
                        }
                    }
                    else
                    {
                        if (this.h)
                            global::c.a("OpenDevices (By Key):  Could not retrieve the device information.  Error Code = {0}, 0x{1}", new object[2]
                            {
                (object) modelSerialKeys,
                (object) modelSerialKeys.ToString("X")
                            });
                        this.CloseDevices();
                    }
                }
                else if (this.h)
                    global::c.a("OpenDevices (By Key):  Error Code = {0}, 0x{1}", new object[2]
                    {
            (object) num,
            (object) num.ToString("X")
                    });
            }
            catch (Exception ex)
            {
                global::c.a("OpenDevices (By Key) exception.  {0}", (object)ex.Message);
                this.CloseDevices();
            }
            return this.g;
        }

        public void CloseDevices()
        {
            if (!this.g)
                return;
            this.g = false;
            try
            {
                if (this.c != null)
                    this.c.Clear();
                if (this.e != null)
                    this.e.Clear();
                if (this.f != null)
                    this.f.Clear();
                USB.newp_usb_uninit_system();
            }
            catch (Exception ex)
            {
                global::c.a("CloseDevices exception.  {0}", (object)ex.Message);
            }
        }

        public int Read(string deviceKey, StringBuilder buffer)
        {
            try
            {
                if (deviceKey != null && this.e.ContainsKey((object)deviceKey))
                    return this.Read((int)this.e[(object)deviceKey], buffer);
                if (this.h)
                    global::c.a("Read:  DeviceKey '{0}' cannot be found in the Device Table.", (object)deviceKey);
                return -1;
            }
            catch (Exception ex)
            {
                global::c.a("Read exception (DeviceKey = {0}).  {1}", new object[2]
                {
          (object) deviceKey,
          (object) ex.Message
                });
                return -1;
            }
        }

        public int Read(int deviceID, StringBuilder buffer)
        {
            try
            {
                uint A_3 = 0;
                int ascii = USB.newp_usb_get_ascii(deviceID, buffer, buffer.Capacity, ref A_3);
                int startIndex = buffer.ToString().IndexOfAny(new char[2] { '\r', '\n' });
                if (startIndex < 0)
                    startIndex = (int)A_3;
                buffer.Remove(startIndex, buffer.Length - startIndex);
                if (this.h)
                    global::c.a("Read:  Status = {0}, Bytes read = {1}, Response length = {2}, Response = '{3}'", (object)ascii, (object)A_3, (object)buffer.Length, (object)buffer);
                return ascii;
            }
            catch (Exception ex)
            {
                global::c.a("Read exception (DeviceID = {0}).  {1}", new object[2]
                {
          (object) deviceID,
          (object) ex.Message
                });
                return -1;
            }
        }

        public int ReadBinary(string deviceKey, StringBuilder buffer)
        {
            try
            {
                if (deviceKey != null && this.e.ContainsKey((object)deviceKey))
                    return this.ReadBinary((int)this.e[(object)deviceKey], buffer);
                if (this.h)
                    global::c.a("ReadBinary:  DeviceKey '{0}' cannot be found in the Device Table.", (object)deviceKey);
                return -1;
            }
            catch (Exception ex)
            {
                global::c.a("ReadBinary exception (DeviceKey = {0}).  {1}", new object[2]
                {
          (object) deviceKey,
          (object) ex.Message
                });
                return -1;
            }
        }

        public int ReadBinary(int deviceID, StringBuilder buffer)
        {
            try
            {
                uint A_3 = 0;
                int ascii = USB.newp_usb_get_ascii(deviceID, buffer, buffer.Capacity, ref A_3);
                buffer.Remove((int)A_3, (int)((long)buffer.Length - (long)A_3));
                if (this.h)
                {
                    StringBuilder stringBuilder = new StringBuilder(buffer.Capacity);
                    for (int index = 0; (long)index < (long)A_3; ++index)
                        stringBuilder.Append(Convert.ToUInt32(buffer[index]));
                    global::c.a("ReadBinary:  Status = {0}, Bytes read = {1}, Response = '{2}'", (object)ascii, (object)A_3, (object)stringBuilder);
                }
                return ascii;
            }
            catch (Exception ex)
            {
                global::c.a("ReadBinary exception (DeviceID = {0}).  {1}", new object[2]
                {
          (object) deviceID,
          (object) ex.Message
                });
                return -1;
            }
        }

        public int ReadBinary(string deviceKey, IntPtr buffer, ref uint bytesRead)
        {
            try
            {
                if (deviceKey != null && this.e.ContainsKey((object)deviceKey))
                    return this.ReadBinary((int)this.e[(object)deviceKey], buffer, ref bytesRead);
                if (this.h)
                    global::c.a("ReadBinary:  DeviceKey '{0}' cannot be found in the Device Table.", (object)deviceKey);
                return -1;
            }
            catch (Exception ex)
            {
                global::c.a("ReadBinary exception (DeviceKey = {0}).  {1}", new object[2]
                {
          (object) deviceKey,
          (object) ex.Message
                });
                return -1;
            }
        }

        public int ReadBinary(int deviceID, IntPtr buffer, ref uint bytesRead)
        {
            try
            {
                int ascii = USB.newp_usb_get_ascii(deviceID, buffer, 64, ref bytesRead);
                if (this.h)
                    global::c.a("ReadBinary:  Status = {0}, Bytes read = {1}, Response = '{2}'", (object)ascii, (object)bytesRead, (object)buffer.ToString("X"));
                return ascii;
            }
            catch (Exception ex)
            {
                global::c.a("ReadBinary exception (DeviceID = {0}).  {1}", new object[2]
                {
          (object) deviceID,
          (object) ex.Message
                });
                return -1;
            }
        }

        public int Write(string deviceKey, string cmd)
        {
            try
            {
                if (deviceKey != null && this.e.ContainsKey((object)deviceKey))
                    return this.Write((int)this.e[(object)deviceKey], cmd);
                if (this.h)
                    global::c.a("Write:  DeviceKey '{0}' cannot be found in the Device Table.  Cmd = '{1}'", new object[2]
                    {
            (object) deviceKey,
            (object) cmd
                    });
                return -1;
            }
            catch (Exception ex)
            {
                global::c.a("Write exception (DeviceKey = {0}, Cmd = '{1}').  {2}", (object)deviceKey, (object)cmd, (object)ex.Message);
                return -1;
            }
        }

        public int Write(int deviceID, string cmd)
        {
            try
            {
                if (this.h)
                    global::c.a("Write:  Cmd = '{0}'", (object)cmd);
                return USB.newp_usb_send_ascii(deviceID, cmd, cmd.Length);
            }
            catch (Exception ex)
            {
                global::c.a("Write exception (DeviceID = {0}, Cmd = '{1}').  {2}", (object)deviceID, (object)cmd, (object)ex.Message);
                return -1;
            }
        }

        public int WriteBinary(string deviceKey, IntPtr buffer, int length)
        {
            try
            {
                if (deviceKey != null && this.e.ContainsKey((object)deviceKey))
                    return this.WriteBinary((int)this.e[(object)deviceKey], buffer, length);
                if (this.h)
                    global::c.a("WriteBinary:  DeviceKey '{0}' cannot be found in the Device Table.", (object)deviceKey);
                return -1;
            }
            catch (Exception ex)
            {
                global::c.a("WriteBinary exception (DeviceKey = {0}).  {1}", new object[2]
                {
          (object) deviceKey,
          (object) ex.Message
                });
                return -1;
            }
        }

        public int WriteBinary(int deviceID, IntPtr buffer, int length)
        {
            try
            {
                if (this.h)
                    global::c.a("WriteBinary:  Cmd = '{0}'", (object)buffer.ToString("X"));
                return USB.newp_usb_send_binary(deviceID, buffer, length);
            }
            catch (Exception ex)
            {
                global::c.a("WriteBinary exception (DeviceID = {0}).  {1}", new object[2]
                {
          (object) deviceID,
          (object) ex.Message
                });
                return -1;
            }
        }

        public int Query(string deviceKey, string cmd, StringBuilder buffer)
        {
            try
            {
                int num = this.Write(deviceKey, cmd);
                if (num == 0)
                    return this.Read(deviceKey, buffer);
                return num;
            }
            catch (Exception ex)
            {
                global::c.a("Query exception (DeviceKey = {0}, Cmd = '{1}').  {2}", (object)deviceKey, (object)cmd, (object)ex.Message);
                return -1;
            }
        }

        public int Query(int deviceID, string cmd, StringBuilder buffer)
        {
            try
            {
                int num = this.Write(deviceID, cmd);
                if (num == 0)
                    return this.Read(deviceID, buffer);
                return num;
            }
            catch (Exception ex)
            {
                global::c.a("Query exception (DeviceID = {0}, Cmd = '{1}').  {2}", (object)deviceID, (object)cmd, (object)ex.Message);
                return -1;
            }
        }

        public Hashtable GetDeviceTable()
        {
            return this.f;
        }

        public int GetDeviceKeys(out string[] deviceKeys)
        {
            deviceKeys = new string[this.e.Count];
            this.e.Keys.CopyTo((Array)deviceKeys, 0);
            return this.e.Count;
        }

        public ArrayList GetDevInfoList()
        {
            return ArrayList.ReadOnly(this.c);
        }

        private void a(string A_0)
        {
            string[] strArray1 = A_0.Split(';');
            for (int index = 0; index < strArray1.Length - 1; ++index)
            {
                try
                {
                    string[] strArray2 = strArray1[index].Split(',');
                    DevInfo devInfo = new DevInfo();
                    devInfo.ID = int.Parse(strArray2[0]);
                    devInfo.Description = strArray2[1].TrimEnd('\r', '\n');
                    if (this.h)
                        global::c.a("FillDevInfoList:  Device #{0} ID = {1}, Description = '{2}'", (object)(index + 1), (object)devInfo.ID, (object)devInfo.Description);
                    this.c.Add((object)devInfo);
                }
                catch (Exception ex)
                {
                    global::c.a("FillDevInfoList exception.  {0}", (object)ex.Message);
                }
            }
        }

        public int GetDevInfoArrays(out int[] devIDs, out string[] descriptions)
        {
            devIDs = new int[this.c.Count];
            descriptions = new string[this.c.Count];
            for (int index = 0; index < this.c.Count; ++index)
            {
                devIDs[index] = ((DevInfo)this.c[index]).ID;
                descriptions[index] = ((DevInfo)this.c[index]).Description;
            }
            return this.c.Count;
        }

        public int NumProductsConnected(string keyVendorProduct)
        {
            int num1 = -1;
            try
            {
                using (RegistryKey registryKey1 = Registry.LocalMachine)
                {
                    StringBuilder stringBuilder = new StringBuilder(256);
                    stringBuilder.AppendFormat("SYSTEM\\CurrentControlSet\\Enum\\USB\\{0}", (object)keyVendorProduct);
                    using (RegistryKey registryKey2 = registryKey1.OpenSubKey(stringBuilder.ToString()))
                    {
                        string[] subKeyNames1 = registryKey2.GetSubKeyNames();
                        string @string = stringBuilder.ToString();
                        for (int index1 = 0; index1 < subKeyNames1.Length; ++index1)
                        {
                            stringBuilder.Remove(0, stringBuilder.Length);
                            stringBuilder.AppendFormat("{0}\\{1}\\Device Parameters", (object)@string, (object)subKeyNames1[index1]);
                            if (this.h)
                                global::c.a("NumProductsConnected:  RegKey = {0}", (object)stringBuilder.ToString());
                            using (RegistryKey registryKey3 = registryKey1.OpenSubKey(stringBuilder.ToString()))
                            {
                                string str1 = (string)registryKey3.GetValue("SymbolicName", (object)"");
                                if (str1.Length > 0)
                                {
                                    string str2 = str1.Replace("??\\", "##?#");
                                    int num2 = str2.IndexOf('}');
                                    if (str2.Length > num2 + 1)
                                        str2 = str2.Remove(num2 + 1);
                                    int startIndex = str2.IndexOf('{');
                                    string str3 = str2.Substring(startIndex);
                                    stringBuilder.Remove(0, stringBuilder.Length);
                                    stringBuilder.AppendFormat("SYSTEM\\CurrentControlSet\\Control\\DeviceClasses\\{0}{1}", (object)str3, (object)str2);
                                    using (RegistryKey registryKey4 = registryKey1.OpenSubKey(stringBuilder.ToString()))
                                    {
                                        if (registryKey4.SubKeyCount > 1)
                                        {
                                            string[] subKeyNames2 = registryKey4.GetSubKeyNames();
                                            for (int index2 = 0; index2 < subKeyNames2.Length; ++index2)
                                            {
                                                if (subKeyNames2[index2] == "Control")
                                                {
                                                    using (RegistryKey registryKey5 = registryKey4.OpenSubKey(subKeyNames2[index2]))
                                                    {
                                                        num1 += (int)registryKey5.GetValue("ReferenceCount", (object)0);
                                                        if (this.h)
                                                            global::c.a("NumProductsConnected:  NumProdConn = {0}, Key = {1}", new object[2]
                                                            {
                                (object) (num1 + 1),
                                (object) registryKey5.ToString()
                                                            });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        ++num1;
                    }
                }
            }
            catch (Exception ex)
            {
                global::c.a("NumProductsConnected exception.  {0}", (object)ex.Message);
            }
            if (this.h)
                global::c.a("NumProductsConnected:  {0}", (object)num1);
            return num1;
        }

        public enum eDeviceState
        {
            Detached,
            Attached,
        }

        private delegate void a(int A_0, int A_1);

        public delegate void DeviceStateChangedDelegate(string deviceKey, USB.eDeviceState state);
#endif
    }
}

