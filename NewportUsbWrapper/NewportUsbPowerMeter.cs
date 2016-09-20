using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

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
		
        public const uint DATASTORE_SIZE_MAX = 250000;
        public const string END_OF_HEADER = "End of Header\r\n";
        public const string END_OF_DATA = "End of Data";

        private NewportState _measuring = new NewportState(false,false,false);

        public NewportState Measuring
        {
            get { return _measuring; }
            protected set
            {
                if (_measuring.Equals(value)) return;
                _measuring = value;
                MeasuringChanged?.Invoke(_measuring);
            }
        }

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
                resetMeasurement();
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
            while (TryRead(out response, out status))  { }
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
                if (_usb == null) return ErrorString(USB.m_knWDDeviceNotFound,"Write");
                Debug.WriteLine(command);
                if (_deviceID > 0) nIOStatus = _usb.Write(_deviceID, command);
                else if (!string.IsNullOrEmpty(_deviceKey)) nIOStatus = _usb.Write(_deviceKey, command);
                // If no error status
                return ErrorString(nIOStatus,"Write");
            }
            catch (Exception ex)
            {
                return $"Could not send the specified command.\r\n{ex.Message}";
            }
        }

#if true // .net 4.5.2 or greater
        private static string ErrorString(int nIOStatus, [CallerMemberName] string callingMemberName = null)
#else
        private static string ErrorString(int nIOStatus, string callingMemberName)
#endif
        {
            if (nIOStatus == 0) return string.Empty;
            var response = $"{callingMemberName} Error Code = {nIOStatus}, 0x{nIOStatus.ToString("X")}";
            Debug.WriteLine(response);
            return response;
        }

        /// <summary>
        /// This method reads data from the communication port and then 
        /// displays the response and read status.
        /// </summary>
        public string Read()
        {
            try
            {
                if (_usb == null) return ErrorString(USB.m_knUSBAddrNotFound);
                var nIOStatus = USB.m_knUSBAddrNotFound;

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

                return ErrorString(nIOStatus);
            }
            catch (Exception ex)
            {
                return $"Could not read the command response.\r\n{ex.Message}";
            }
        }

        public bool TryRead(out string response, out int ioStatus)
        {
            ioStatus = USB.m_knUSBAddrNotFound;
            if (_usb != null)
            {
                try
                {
                    var sbResponse = new StringBuilder(64);
                    if (_deviceID > 0) ioStatus = _usb.Read(_deviceID, sbResponse);
                    else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = _usb.Read(_deviceKey, sbResponse);
                    if (ioStatus == 0)
                    {
                        response = sbResponse.ToString();
                        return true;
                    }
                    response = ErrorString(ioStatus);
                }
                catch (Exception ex)
                {
                    response = $"Could not read the command response.\r\n{ex.Message}";
                }
            }
            else
            {
                response = ErrorString(USB.m_knUSBAddrNotFound);
            }

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

                    if (ioStatus == 0)
                    {
                        response = sbResponse.ToString();
                        return true;
                    }
                }
                response = ErrorString(ioStatus);
            }
            catch (Exception ex)
            {
                response = $"Could not read the command response.\r\n{ex.Message}";
            }
            return false;
        }
        
        public bool ReadBinaryUntil(string match, uint expectedLength, out string response, out int ioStatus)
        {
            ioStatus = USB.m_knUSBAddrNotFound;
            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                var readCount=0;
                var start = DateTime.Now;
                var sbResponse = new StringBuilder((int)expectedLength);
                if (_usb != null)
                {
                    ioStatus = 0;
                    while (ioStatus == 0 && !sbResponse.ToString().Contains(match))
                    {
                        var sbTemp = new StringBuilder((int)expectedLength);
                        if (_deviceID > 0) ioStatus = _usb.ReadBinary(_deviceID, sbTemp);
                        else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = _usb.ReadBinary(_deviceKey, sbTemp);
                        sbResponse.Append(sbTemp);
                        readCount++;
                    }
                }
                if (ioStatus == 0)
                {
                    Debug.WriteLine($"{readCount} reads in {(DateTime.Now - start).TotalSeconds} sec");
                    Debug.WriteLine($"Response Length={sbResponse.Length}");
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



        public bool ReadDataUntil(string match, uint expectedLength,string preamble, out int ioStatus, out List<double> data)
        {
            ioStatus = USB.m_knUSBAddrNotFound;
            data = new List<double>((int)expectedLength);
            var readSize = new List<int>(128);
            
            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                int readCount = 0;
                DateTime start = DateTime.Now;
//                var sbResponse = new StringBuilder((int)expectedLength);
                if (_usb != null)
                {
                    ioStatus = 0;
                    var sbTemp = new StringBuilder((int)expectedLength * 16);
                    while (ioStatus == 0)
                    {
                        sbTemp.Length = 0;
                        if (_deviceID > 0) ioStatus = _usb.ReadBinary(_deviceID, sbTemp);
                        else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = _usb.ReadBinary(_deviceKey, sbTemp);
                        readCount++;
                        if (ioStatus != 0)
                        {
                            break;
                        }
                        preamble += sbTemp.ToString();
                        var split = preamble.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                        preamble = "";
                        var length = split.Length;
                        if (length <= 0) continue;
                        readSize.Add(length);
                        if (length > 1)
                        {
                            for (var i = 0; i < length - 1; i++)
                            {
                                double result;
                                if (double.TryParse(split[i], out result))
                                {
                                    data.Add(result);
                                }
                            }
                        }

                        if(split[length-1].Contains(match))
                        {
                            var seconds = (DateTime.Now - start).TotalSeconds;
                            var min = int.MaxValue;
                            var max = 0;
                            foreach(var size in readSize)
                            {
                                if (size < min) min = size;
                                if (size > max) max = size;
                            }
                            Debug.WriteLine($"{readCount} reads in {seconds} sec. Min={min} Max={max}");
                            Debug.WriteLine($"Response Length={data.Count} {seconds/data.Count} samples/sec");
                            return true;
                        }

                        if (data.Count >= expectedLength)
                        {
                            Debug.WriteLine($"Count:{data.Count} >= {expectedLength}");
                        }
                        preamble = split[length - 1];
                    }
                }
                //if (ioStatus == 0)
                //{
                //    Debug.WriteLine($"{readCount} reads in {(DateTime.Now - start).TotalSeconds} sec");
                //    Debug.WriteLine($"Response Length={data.Count}");
                //    return true;
                //}
                Debug.WriteLine($"Error Code = {ioStatus}, 0x{ioStatus.ToString("X")}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not read the command response.\r\n{ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// This method repeatedly queries the sample count until it matches the passed in sample size,
        /// or until a timeout occurs.
        /// </summary>
        /// <param name="samplesRequested">The sample size that the sample count should match.</param>
        /// <returns>The number of samples ready for read.</returns>
        public uint WaitForDataStore(uint samplesRequested)
        {
            uint samplesReady = 0;
            var nStatus = 0;
            var startTime = DateTime.Now;

            // Repeat until an error occurs
            while (nStatus == 0)
            {
                // Query the sample count
                var writeResponse = Write(NewportScpi.DataStoreCountQuery);

                if (string.IsNullOrEmpty(writeResponse))
                {
                    // Read the sample count
                    string response;
                    if (TryRead(out response, out nStatus))
                    {
                        samplesReady = Convert.ToUInt32(response, 10);
                        Debug.WriteLine($"{samplesReady} samples ready.");
                        // If the sample count matches the sample size
                        if (samplesReady >= samplesRequested)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"{NewportScpi.DataStoreCountQuery} Error response '{writeResponse}'");
                    Write(NewportScpi.DataStoreEnableQuery);
                    if(TryRead(out writeResponse,out nStatus))
                    {
                        Debug.WriteLine($"{NewportScpi.DataStoreEnableQuery} {writeResponse}");
                    }
                    else
                    {
                        Debug.WriteLine($"{NewportScpi.DataStoreEnableQuery} Error response '{writeResponse}'");
                    }
                }
                Thread.Sleep(10);
            }
            Debug.WriteLine($"{samplesReady} Samples ready.  {(DateTime.Now - startTime).TotalSeconds}");

            return samplesReady;
        }

        public int ReadDataStoreValues(uint sampleCount, out List<double> data)
        {
            data = new List<double>((int)sampleCount);
            var status = -1;
            var errorString = Write(NewportScpi.DataStoreGetLatest(sampleCount));
            if(string.IsNullOrEmpty(errorString))
            {
                if (ReadDataStorePacket(sampleCount, ref data, ref status)) return status;
            }
            return status;
        }

        public int ReadDataStoreValues(uint start, uint end, out List<double> data)
        {
            Debug.Assert(end > start);
            var sampleCount = end - start;
            //data = new List<double>((int)(sampleCount));
            var status = -1;

            data = new List<double>((int)sampleCount);

            var errorString = Write(NewportScpi.DataStoreGetRange(start, end));
            if (string.IsNullOrEmpty(errorString))
            {
                if (ReadDataStorePacket(sampleCount, ref data, ref status)) return status;
            }
            return status;
        }

        private bool ReadDataStorePacket(uint sampleCount, ref List<double> data, ref int status)
        {

            string receivedString;
            {
                // Read header....
                string response;
                if (!ReadBinaryUntil(NewportUsbPowerMeter.END_OF_HEADER, 1024, out response, out status))
                    return true;

                // Copy remaining data (after 'End Of Header'), which is actual data, to receive buffer. 
                var markerIndex = response.IndexOf(NewportUsbPowerMeter.END_OF_HEADER, StringComparison.Ordinal) +
                                  NewportUsbPowerMeter.END_OF_HEADER.Length;

                receivedString = response.Substring(markerIndex);
            }
            // Read remaining data
            //List<double> rxData=null;
            if (!ReadDataUntil(NewportUsbPowerMeter.END_OF_DATA, sampleCount, receivedString, out status, out data))
                return true;
            return false;
        }



        private int DisplayDeviceTable()
        {
            var hashTable = _usb?.GetDeviceTable();

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
            var devices = _usb?.GetDevInfoList();
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

        public string ContinuousMeasurementSetup()
        {
            return DatastoreInitialize(true,NewportMeasurementSettings.ContiuousMeasurement(0,1));
        }

        private uint _samples = 0;
        public void ContinuousReading(uint samples)
        {
            //if (samples == _samples) return;
            //if (_samples > 0)
            //{   // already running
            //    _samples = samples;
            //}
            //else
            {   // Not currently running
                if (samples == 0) return;
                _samples = samples;
                ThreadPool.QueueUserWorkItem(RunContinuousTask, null);
            }
        }



        public virtual event Action<List<double>> ContinuousData;
        public virtual event Action<NewportState> MeasuringChanged;
        public int SamplesRead { get; private set; }

        protected virtual void RunContinuousTask(object unused)
        {
            try
            {
                SamplesRead = 0;
                Measuring = new NewportState(true);
                while (Measuring.Measuring)
                {
                    var samples = _samples;
                    if (samples <= 0) break;
                    //               Flush();
//                WaitForDataStore(samples);

                    var samplesReady = WaitForDataStore(samples);
                    if (samplesReady >= samples)
                    {
                        List<double> result;
                        ReadDataStoreValues(samples, out result);
                        SamplesRead += result.Count;
                        ContinuousData?.Invoke(result);
                    }
                }
            }
            finally
            {
                Measuring = new NewportState(false);
            }
        }

        protected virtual void RunTriggeredMeasurementTask(object settings)
        {
            try
            {
                SamplesRead = 0;
                Measuring = new NewportState(true);
                uint startIndex = 1;
                var newportSettings = (NewportMeasurementSettings) settings;
                if (newportSettings.ResetInstrument) resetMeasurement();
                Flush();
                TriggerInitialize(true, newportSettings);
                Write(NewportScpi.DataStoreEnable);
                Measuring = new NewportState(true, true);
                if (WaitForTrigger(Math.Min(_samples + 1000, _samples)))
                {
                    Measuring = new NewportState(true, false, true);
                    while (Measuring.Measuring)
                    {
                        var samples = _samples;
                        if (samples <= 0) break;
                        var samplesRequested = Math.Min(startIndex + 1000, _samples);
                        var endIndex = WaitForDataStore(samplesRequested);
                        if (true) //samplesReady >= samples)
                        {
                            List<double> result;
                            var numberRead = ReadDataStoreValues(startIndex, endIndex, out result);
                            if (numberRead < 0)
                            {
                                Debug.WriteLine($"Error reading triggered measurement [{startIndex},{endIndex}]");
                                Debug.WriteLine(ErrorString(numberRead));
                                break;
                            }
                            SamplesRead += result.Count;
                            ContinuousData?.Invoke(result);

                            if (endIndex >= _samples) break;
                            startIndex += (uint) result.Count;
                        }
                    }
                }
            }
            finally
            {
                Measuring = new NewportState(false);
            }
        }

        private bool WaitForTrigger(uint minSamples)
        {
            var start = DateTime.Now;
            var triggered = 0;
            Debug.WriteLine("Waiting for trigger...");
            // Repeat until an error occurs
            while (triggered == 0 && Measuring.Measuring)
            {// Query the trigger state
                var writeResponse = Write(NewportScpi.TriggerStateQuery);

                if (string.IsNullOrEmpty(writeResponse))
                {
                    // Read the sample count
                    string response;
                    int ioStatus;
                    if (TryRead(out response, out ioStatus))
                    {
                        triggered = int.Parse(response);
                        Debug.WriteLine($"{triggered} - {(DateTime.Now - start).TotalSeconds}");
                        if (triggered > 0)
                        {
                            Debug.WriteLine($"Triggered {(DateTime.Now - start).TotalSeconds}");
                            return true;
                        }
                        Write(NewportScpi.DataStoreCountQuery);
                        if(TryRead(out response,out ioStatus))
                        {
                            Debug.WriteLine($"{response} samples in buffer");
                            uint samplesReady;
                            if (uint.TryParse(response, out samplesReady))
                            {
                                if (samplesReady >= minSamples)
                                {
                                    Debug.WriteLine($"Triggered {(DateTime.Now - start).TotalSeconds}");
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"WaitForTrigger Error response '{writeResponse}'");
                }
                Thread.Sleep(250);
            }
            return false;
        }

        [Obsolete("not used",true)]
        public void TriggeredMeasurement(bool risingEdge, 
                                            uint channel=0,
                                            double durationSeconds=5.0, 
                                            TriggerStartEvent startEvent=TriggerStartEvent.ExternalTrigger, 
                                            TriggerStopEvent stopEvent=TriggerStopEvent.StopAfterTime, 
                                            uint holdoffSeconds=0)
        {
  //          Flush();
  //          DatastoreInitialize(true, true, CaptureMode.DC_CONTINUOUS);
            var settings = new NewportMeasurementSettings(channel, startEvent, stopEvent, risingEdge, CaptureMode.DC_CONTINUOUS, durationSeconds, 1, holdoffSeconds);
   //         DatastoreInitialize(false,settings);
   ////         TriggerInitialize(true, risingEdge,0, durationSeconds, startEvent, stopEvent, holdoffSeconds);
   //         TriggerInitialize(true,settings);
   //         _samples = (uint)(durationSeconds*10000.0);
   //         ThreadPool.QueueUserWorkItem(RunTriggeredMeasurementTask, null);
            TriggeredMeasurement(settings);
        }

        public void TriggeredMeasurement(NewportMeasurementSettings settings)
        {
 //           TriggerInitialize(true, settings);
            _samples = (uint)(settings.DurationSeconds * 10000.0);
            ThreadPool.QueueUserWorkItem(RunTriggeredMeasurementTask, settings);
        }

        public void ResetMeasurement()
        {
            var measuring = Measuring.Measuring;
            Measuring = new NewportState(false);
            if (measuring) Thread.Sleep(50);

            resetMeasurement();
        }

        private void resetMeasurement()
        {
            Flush();
            Write(NewportScpi.TriggerExternalDisable);
            Write("*RST\r");
            Thread.Sleep(1000);
            // DatastoreInitialize(false, true);
            DatastoreInitialize(false,
                new NewportMeasurementSettings(0, TriggerStartEvent.ContinuousMeasurement, TriggerStopEvent.NeverStop, true, CaptureMode.DC_CONTINUOUS, 1, 0));
            //TriggerInitialize(false,true,0,0.0,TriggerStartEvent.ContinuousMeasurement, TriggerStopEvent.NeverStop,0);
            TriggerInitialize(false, new NewportMeasurementSettings(0, TriggerStartEvent.ContinuousMeasurement, TriggerStopEvent.NeverStop, true, CaptureMode.DC_CONTINUOUS, 1, 0));
        }

        [Obsolete("Use NewportMeasurmentSettings",true)]
        public string DatastoreInitialize(bool enable, bool ringBuffer, CaptureMode mode= CaptureMode.DC_CONTINUOUS, uint datastoreSize=DATASTORE_SIZE_MAX, uint datastoreInterval = 1)
        {
            var result = Write(NewportScpi.DataStoreDisable);
            if(string.IsNullOrEmpty(result))    result = Write(NewportScpi.DataStoreBuffer(ringBuffer));
            if (string.IsNullOrEmpty(result))   result = Write(NewportScpi.DataStoreClear);
            if (string.IsNullOrEmpty(result))   result = Write(NewportScpi.DataStoreInterval(datastoreInterval));
            if (string.IsNullOrEmpty(result))   result = Write(NewportScpi.DataStoreSize(datastoreSize));
            if (string.IsNullOrEmpty(result))   result = Write(NewportScpi.Mode(mode));
            if(string.IsNullOrEmpty(result) && enable) result = Write(NewportScpi.DataStoreEnable);
            return result;
        }
		
        public string DatastoreInitialize(bool enable, NewportMeasurementSettings settings)
        {
            var result = Write(NewportScpi.DataStoreDisable);
            Flush();
            if (string.IsNullOrEmpty(result)) result = Write(NewportScpi.DataStoreBuffer(settings.Ringbuffer));
            if (string.IsNullOrEmpty(result)) result = Write(NewportScpi.DataStoreClear);
            if (string.IsNullOrEmpty(result)) result = Write(NewportScpi.DataStoreInterval(settings.DatastoreInterval));
            if (string.IsNullOrEmpty(result)) result = Write(NewportScpi.DataStoreSize(DATASTORE_SIZE_MAX));
            if (string.IsNullOrEmpty(result)) result = Write(NewportScpi.Mode(settings.Mode));
            if (string.IsNullOrEmpty(result) && enable) result = Write(NewportScpi.DataStoreEnable);
            return result;
        }
		
		
        private void TriggerInitialize(bool enable, NewportMeasurementSettings settings)
        {
            Write(NewportScpi.TriggerExternalDisable);
            Write(NewportScpi.TriggerHoldoff((uint)(settings.Holdoff * 1000.0)));
            Write(NewportScpi.TriggerEdge(settings.RisingEdge));
            //Write(settings.StartEvent == TriggerStartEvent.ExternalTrigger
            //    ? NewportScpi.TriggerExternalEnable(settings.Channel)
            //    : NewportScpi.TriggerExternalDisable);

            Write(NewportScpi.TriggerStartEvent(settings.StartEvent));
            Write(NewportScpi.TriggerStopEvent(settings.StopEvent));
            Write(NewportScpi.TriggerTimeMs((uint)(settings.DurationSeconds * 1000.0)));

            Write(NewportScpi.TriggerStateArm);
            if (enable && settings.StartEvent == TriggerStartEvent.ExternalTrigger) Write(NewportScpi.TriggerExternalEnable(settings.Channel));
        }

		[Obsolete("Use NewportMeasurmentSettings",true)]
        private void TriggerInitialize(bool enable, bool risingEdge,
                                uint channel = 0,
                                double duration = 5.0,
                                TriggerStartEvent startEvent = TriggerStartEvent.ExternalTrigger,
                                TriggerStopEvent stopEvent = TriggerStopEvent.StopAfterTime,
                                uint holdoffMs = 0)
        {
            Write(NewportScpi.TriggerExternalDisable);
            Write(NewportScpi.TriggerHoldoff(holdoffMs));
            Write(NewportScpi.TriggerEdge(risingEdge));
            Write(NewportScpi.TriggerStartEvent(startEvent));
            Write(NewportScpi.TriggerStopEvent(stopEvent));
            Write(NewportScpi.TriggerTimeMs((uint)(duration * 1000)));

            Write(NewportScpi.TriggerStateArm);
            if (enable) Write(NewportScpi.TriggerExternalEnable(channel));
        }
    }
}
