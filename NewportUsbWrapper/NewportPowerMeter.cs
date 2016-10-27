using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Newport.Usb
{
    public abstract class NewportPowerMeter
    {
        /// <summary>
        /// The USB communication object.
        /// </summary>
        protected readonly INewportInstrument _newport;

        public virtual event Action<List<double>> ContinuousData;
        public virtual event Action<NewportState> MeasuringChanged;
        public virtual event Action<string> Profile;

        private const uint MAX_DATA_PACKET = 240;
        public const uint DATASTORE_SIZE_MAX = 250000;
        public const uint INTERVAL_MAX = 100000;
        public const string END_OF_HEADER = "End of Header\r\n";
        public const string END_OF_DATA = "End of Data";

        public int m_knIOError => _newport.m_knIOError;

        public int m_knUSBAddrNotFound => _newport.m_knUSBAddrNotFound;

        public int m_knWDStatusInvalidWDHandle => _newport.m_knWDStatusInvalidWDHandle;

        public int m_knWDDeviceNotFound => _newport.m_knWDDeviceNotFound;

        public uint m_knWDDeviceNotResponding => _newport.m_knWDDeviceNotResponding;

        public uint m_knWDStatusEndpointHalted => _newport.m_knWDStatusEndpointHalted;

        public int SamplesRead { get; private set; }
        private NewportState _measuring = new NewportState(false, false, false);

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

        protected NewportPowerMeter(INewportInstrument newport)
        {
            _newport = newport;
        }

        public virtual int Write(string command) => _newport.Write(command);

        public void ResetMeasurement()
        {
            var measuring = Measuring.Measuring;
            Measuring = new NewportState(false);
            if (measuring) Thread.Sleep(50);

            resetMeasurement();
        }

        protected void resetMeasurement()
        {
            Flush();
            Write(NewportScpi.TriggerExternalDisable);
            Write("*RST\r");
            Thread.Sleep(1000);


            // DatastoreInitialize(false, true);
            var __continuousSettings = NewportMeasurementSettings.ContiuousMeasurement(0, 1);
            DatastoreInitialize(false, __continuousSettings);


            // TriggerInitialize(false,true,0,0.0,TriggerStartEvent.ContinuousMeasurement, TriggerStopEvent.NeverStop,0);
            TriggerInitialize(false, __continuousSettings);
        }

        public string DatastoreInitialize(bool enable, NewportMeasurementSettings settings)
        {
            var result = Write(NewportScpi.DataStoreDisable);
            //            Flush();
            if (result == 0) result = Write(NewportScpi.DataStoreBuffer(settings.Ringbuffer));
            if (result == 0) result = Write(NewportScpi.DataStoreClear);

            if (result == 0) result = Write(NewportScpi.DataStoreInterval(settings.DatastoreInterval));
            if (result == 0) result = Write(NewportScpi.DigitalFilter(settings.DatastoreInterval));
            if (result == 0) result = Write(NewportScpi.AnalogFilter(1));
            if (result == 0) result = Write(NewportScpi.DataStoreSize(settings.Samples));
            if (result == 0) result = Write(NewportScpi.Mode(settings.Mode));
            if (result == 0 && enable) result = Write(NewportScpi.DataStoreEnable);

            return ErrorString(result);
        }


        private void TriggerInitialize(bool enable, NewportMeasurementSettings settings)
        {
            Write(NewportScpi.TriggerExternalDisable);
            Write(NewportScpi.TriggerHoldoff((uint)(settings.Holdoff * 1000.0)));
            Write(NewportScpi.TriggerEdge(settings.RisingEdge));


            // Write(settings.StartEvent == TriggerStartEvent.ExternalTrigger
            // ? NewportScpi.TriggerExternalEnable(settings.Channel)
            // : NewportScpi.TriggerExternalDisable);
            Write(NewportScpi.TriggerStartEvent(settings.StartEvent));
            Write(NewportScpi.TriggerStopEvent(settings.StopEvent));
            Write(NewportScpi.TriggerTimeMs((uint)(settings.DurationSeconds * 1000.0)));

            Write(NewportScpi.TriggerStateArm);
            if (enable && settings.StartEvent == TriggerStartEvent.ExternalTrigger)
                Write(NewportScpi.TriggerExternalEnable(settings.Channel));
        }

        public string ContinuousMeasurementSetup(uint channel, uint interval)
        {
            return DatastoreInitialize(true, NewportMeasurementSettings.ContiuousMeasurement(channel, interval));
        }

        private uint _samples = 0;
        public void ContinuousReading(uint samples)
        {
            {
                if (samples == 0) return;
                _samples = samples;
                ThreadPool.QueueUserWorkItem(RunContinuousTask, null);
            }
        }





        protected virtual void RunContinuousTask(object newportInstrument)
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            try
            {
                Thread.CurrentThread.Name = "Newport Continuous Measurement Task";
                SamplesRead = 0;
                Measuring = new NewportState(true);
                while (Measuring.Measuring)
                {
                    var samples = _samples;
                    if (samples <= 0) break;

                    var samplesReady = WaitForDataStore(samples);
                    if (samplesReady >= samples)
                    {
                        List<double> result;
                        ReadDataStoreValues(samples, out result);
                        SamplesRead += result.Count;
                        ContinuousData?.Invoke(result);
                    }
                }
                end = DateTime.Now;
            }
            finally
            {
                Write(NewportScpi.TriggerStartEvent(0));
                Write(NewportScpi.TriggerStopEvent(0));
                Measuring = new NewportState(false);
                Profile?.Invoke($"Continuous Task {(end - start).TotalSeconds}");
            }
        }

        protected virtual void RunTriggeredMeasurementTask(object settings)
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            try
            {
                Thread.CurrentThread.Name = "Newport Triggered Measurement Task";
                SamplesRead = 0;
                Measuring = new NewportState(true);
                uint startIndex = 1;
                var newportSettings = (NewportMeasurementSettings)settings;
                if (newportSettings.ResetInstrument) resetMeasurement();
                else Flush();
                DatastoreInitialize(true, newportSettings);
                TriggerInitialize(true, newportSettings);
                Write(NewportScpi.DataStoreEnable);
                Measuring = new NewportState(true, true);
                if (WaitForTrigger(Math.Min(MAX_DATA_PACKET, _samples)))
                {
                    start = DateTime.Now;
                    Measuring = new NewportState(true, false, true);
                    uint lastIndex = 0;
                    while (Measuring.Measuring)
                    {
                        var samples = _samples;
                        if (samples <= 0) break;
                        var samplesRequested = Math.Min(MAX_DATA_PACKET, _samples- startIndex);
                        var endIndex = startIndex + samplesRequested;
                        if (lastIndex < samplesRequested)
                        {
                            lastIndex = WaitForDataStore(samplesRequested);
                        }
                        if (lastIndex < endIndex) continue;
                        // samplesReady >= samples)
                        List<double> result;
                        var status = ReadDataStoreValues(startIndex, endIndex, out result);
                        if (status < 0)
                        {
                            Debug.WriteLine($"Error reading triggered measurement [{startIndex},{endIndex}]");
                            Debug.WriteLine(ErrorString(status));
                            break;
                        }

                        SamplesRead += result.Count;
                        ContinuousData?.Invoke(result);

                        if (endIndex >= _samples) break;
                        startIndex += (uint)result.Count;
                    }
                    end = DateTime.Now;
                }
            }
            catch (Exception exc)
            {
                Profile?.Invoke($"RunTriggeredMeasurementTask Aborted: {exc.Message}");
            }
            finally
            {
                Write(NewportScpi.TriggerStartEvent(0));
                Write(NewportScpi.TriggerStopEvent(0));
                Measuring = new NewportState(false);
                Profile?.Invoke($"Triggered Task {(end - start).TotalSeconds}");
            }
        }

        private bool WaitForTrigger(uint minSamples)
        {
            var start = DateTime.Now;
            var triggered = 0;
            Debug.WriteLine("Waiting for trigger...");

            // Repeat until an error occurs
            while (triggered == 0 && Measuring.Measuring)
            {
                // Query the trigger state
                var writeResponse = Write(NewportScpi.TriggerStateQuery);

                if (writeResponse == 0)
                {
                    // Read the sample count
                    string response;
                    int ioStatus;
                    if (TryRead(out response, out ioStatus))
                    {
                        { 
                            if (string.IsNullOrEmpty(response)) continue;
                            var split = response.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                            if (split.Length != 1) continue;
                            response = split[0];
                        }
                        if (int.TryParse(response, out triggered))
                        {
                            Debug.WriteLine($"{triggered} - {(DateTime.Now - start).TotalSeconds}");
                            if (triggered > 0)
                            {
                                Debug.WriteLine($"Triggered {(DateTime.Now - start).TotalSeconds}");
                                return true;
                            }
                        }
                        else
                            continue;
                        //Write(NewportScpi.DataStoreCountQuery);
                        //if (TryRead(out response, out ioStatus))
                        //{
                        //    Debug.WriteLine($"{response} samples in buffer");
                        //    uint samplesReady;
                        //    if (uint.TryParse(response, out samplesReady))
                        //    {
                        //        if (samplesReady >= minSamples)
                        //        {
                        //            Debug.WriteLine($"Triggered {(DateTime.Now - start).TotalSeconds}");
                        //            return true;
                        //        }
                        //    }
                        //}
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

        public void TriggeredMeasurement(NewportMeasurementSettings settings)
        {
            // TriggerInitialize(true, settings);
            _samples = (settings.Samples);
            ThreadPool.QueueUserWorkItem(RunTriggeredMeasurementTask, settings);
        }

        public int ReadDataStoreValues(uint sampleCount, out List<double> data)
        {
            var status = -1;
            var start = DateTime.Now;
            int errorCode = 0;
            data = new List<double>((int)sampleCount);
            if (sampleCount > MAX_DATA_PACKET)
            {
                uint startIndex = 0;
                uint endIndex = MAX_DATA_PACKET;
                while(startIndex < sampleCount)
                {
                    errorCode = Write(NewportScpi.DataStoreGetRange(startIndex+1,endIndex+1));
                    if (errorCode == 0)
                    {
                        var result = ReadDataStorePacket(endIndex - startIndex, ref data, ref status);
                        if (result) break;
                        //if (!result)
                        //{
                            startIndex = endIndex + 1;
                            endIndex = Math.Min(endIndex + MAX_DATA_PACKET, sampleCount - 1);
                        //}
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                errorCode = Write(NewportScpi.DataStoreGetLatest(sampleCount));
                if (errorCode == 0)
                {
                    if (ReadDataStorePacket(sampleCount, ref data, ref status)) return status;
                }
            }
            if(errorCode != 0)
            {
                Profile?.Invoke($"{(DateTime.Now - start).TotalSeconds} s\tReadDataStoreValues Error-{errorCode} Bytes-{data.Count}");
            }
            return status;
        }

        public int ReadDataStoreValues(uint start, uint end, out List<double> data)
        {
            Debug.Assert(end > start);
            var startTime = DateTime.Now;
            var sampleCount = end - start;

            var status = -1;

            data = new List<double>((int)sampleCount);

            var errorCode = Write(NewportScpi.DataStoreGetRange(start, end));
            if (errorCode == 0)
            {
                if (ReadDataStorePacket(sampleCount, ref data, ref status)) return status;
            }
            else
            {
                Profile?.Invoke($"{(DateTime.Now - startTime).TotalSeconds} s\tReadDataStoreValues Error-{errorCode} Bytes-{data.Count}");
            }
            return status;
        }

        private bool ReadDataStorePacket(uint sampleCount, ref List<double> data, ref int status)
        {
            string receivedString;
            {
                // Read header....
                string response;
                if (!ReadBinaryUntil(END_OF_HEADER, 1024, out response, out status))
                    return true;

                // Copy remaining data (after 'End Of Header'), which is actual data, to receive buffer. 
                var markerIndex = response.IndexOf(END_OF_HEADER, StringComparison.Ordinal) + END_OF_HEADER.Length;

                receivedString = response.Substring(markerIndex);
            }

            // Read remaining data
            // List<double> rxData=null;
            if (!ReadDataUntil(END_OF_DATA, sampleCount, receivedString, out status, ref data))
                return true;
            return false;
        }


 

        public void Flush()
        {
            var start = DateTime.Now;
            string response;
            int status;
            var readCount = 1;
            var bytes = 0;
            while (TryRead(out response, out status))
            {
                readCount++;
                bytes += response.Length;
            }

            Profile?.Invoke($"{DateTime.Now - start}Flush ReadCount-{readCount} Bytes-{bytes}");
        }


#if true // .net 4.5.2 or greater
        public static string ErrorString(int nIOStatus, [CallerMemberName] string callingMemberName = null)
#else
        public static string ErrorString(int nIOStatus, string callingMemberName)
#endif
        {
            if (nIOStatus == 0) return string.Empty;
            var response = $"{callingMemberName} Error Code = {nIOStatus}, 0x{nIOStatus.ToString("X")}";
            Debug.WriteLine(response);
            return response;
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
                var writeStatus = Write(NewportScpi.DataStoreCountQuery);

                if (writeStatus == 0)
                {
                    Thread.Sleep(200);
                    // Read the sample count
                    string response;
                    if (TryRead(out response, out nStatus))
                    {
                        if (!string.IsNullOrEmpty(response))
                        {
                            var split = response.Split(new char[] {'\r', '\n'},StringSplitOptions.RemoveEmptyEntries);
                            if (split.Length == 1)
                            {
                                if (UInt32.TryParse(split[0], out samplesReady))
                                {
                                    //samplesReady = Convert.ToUInt32(split.Last(), 10);
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
                                Debug.WriteLine($"WaitForDataStore returned {split.Length} strings!");
                                Thread.Sleep(50);
                                while (TryRead(out response, out nStatus))
                                {
                                    Thread.Sleep(50);
                                    Flush();
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"{NewportScpi.DataStoreCountQuery} Error response '{writeStatus}'");
                    Write(NewportScpi.DataStoreEnableQuery);
                    string status;
                    if (TryRead(out status, out nStatus))
                    {
                        Debug.WriteLine($"{NewportScpi.DataStoreEnableQuery} {status}");
                    }
                    else
                    {
                        Debug.WriteLine($"{NewportScpi.DataStoreEnableQuery} Error response '{status}'");
                    }
                }

                Thread.Sleep(10);
            }

            Debug.WriteLine($"{samplesReady} Samples ready.  {(DateTime.Now - startTime).TotalSeconds}");

            return samplesReady;
        }

        public bool TryRead(out string response, out int ioStatus)
        {
            ioStatus = _newport.m_knUSBAddrNotFound;
            if (_newport != null)
            {
                try
                {
                    var sbResponse = new StringBuilder(64);
                    ioStatus = _newport.Read(sbResponse);
                    if (ioStatus == 0)
                    {
                        response = sbResponse.ToString();
                        //                        Profile?.Invoke($"{DateTime.Now} TryRead-{response}");
                        return !string.IsNullOrEmpty(response);
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
                response = ErrorString(_newport.m_knUSBAddrNotFound);
            }

            return false;
        }

        public bool TryReadBinary(out string response, out int ioStatus)
        {
            ioStatus = _newport.m_knUSBAddrNotFound;
            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                var sbResponse = new StringBuilder(64);
                if (_newport != null)
                {
                    ioStatus = _newport.ReadBinary(sbResponse);

                    if (ioStatus == 0)
                    {
                        response = sbResponse.ToString();
                        //                        Profile?.Invoke($"{DateTime.Now} TryReadBinary-{response.Length}");
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

        //public abstract bool ReadBinaryUntil(string match, uint expectedLength, out string response, out int ioStatus)
        public bool ReadBinaryUntil(string match, uint expectedLength, out string response, out int ioStatus)
        {
            ioStatus = _newport.m_knUSBAddrNotFound;
            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                var readCount = 0;
                var start = DateTime.Now;
                var sbResponse = new StringBuilder((int)expectedLength);
                if (_newport != null)
                {
                    ioStatus = 0;
                    while (ioStatus == 0 && !sbResponse.ToString().Contains(match))
                    {
                        var sbTemp = new StringBuilder((int)expectedLength);
                        ioStatus = _newport.ReadBinary(sbTemp);
                        //if (_deviceID > 0) ioStatus = newport.ReadBinary(_deviceID, sbTemp);
                        //else if (!string.IsNullOrEmpty(_deviceKey)) ioStatus = newport.ReadBinary(_deviceKey, sbTemp);
                        sbResponse.Append(sbTemp);
                        if (ioStatus >= 0) ioStatus = 0;
                        readCount++;
                    }
                }

                if (ioStatus == 0)
                {
                    Debug.WriteLine($"{readCount} reads in {(DateTime.Now - start).TotalSeconds} sec");
                    Debug.WriteLine($"Response Length={sbResponse.Length}");
                    response = sbResponse.ToString();
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

        public  bool ReadDataUntil(string match, uint expectedLength, string preamble, out int ioStatus, ref List<double> data)
        {
            ioStatus = _newport.m_knUSBAddrNotFound;
            //data = new List<double>((int)expectedLength);
            var readSize = new List<int>(128);

            try
            {
                // The firmware limits the transfer size to the maximum packet size of 64 bytes
                var readCount = 0;
                var start = DateTime.Now;
                // var sbResponse = new StringBuilder((int)expectedLength);
                if (_newport != null)
                {
                    ioStatus = 0;
                    var sbTemp = new StringBuilder((int)expectedLength * 16);
                    while (ioStatus == 0)
                    {
                        sbTemp.Length = 0;
                        ioStatus = _newport.ReadBinary( sbTemp);
                        readCount++;
                        if (ioStatus < 0)   break;
                        ioStatus = 0;

                        preamble += sbTemp.ToString();
                        var split = preamble.Split(new[] { '\r','\n' }, StringSplitOptions.RemoveEmptyEntries);
                        preamble = string.Empty;
                        var length = split.Length;
                        if (length <= 0) continue;
                        readSize.Add(length);
                        if (length > 1)
                        {
                            for (var i = 0; i < length - 1; i++)
                            {
                                double result;
                                if (split[i].Length == 13)
                                {
                                    if (double.TryParse(split[i], out result))
                                    {
                                        data.Add(result);
                                    }
                                }
                                else
                                {
                                    if (double.TryParse(split[i], out result))
                                    {
                                        data.Add(result);
                                    }
                                }
                            }
                        }

                        if (split[length - 1].Contains(match))
                        {
                            var seconds = (DateTime.Now - start).TotalSeconds;
                            var min = int.MaxValue;
                            var max = 0;
                            foreach (var size in readSize)
                            {
                                if (size < min) min = size;
                                if (size > max) max = size;
                            }

                            Debug.WriteLine($"{readCount} reads in {seconds} sec. Min={min} Max={max}");
                            Debug.WriteLine($"Response Length={data.Count} {seconds / data.Count} samples/sec");
                            return true;
                        }


                        if (data.Count >= expectedLength)
                        {
                            Debug.WriteLine($"Count:{data.Count} >= {expectedLength}");
                        }

                        preamble = split[length - 1];
                    }
                }
                Debug.WriteLine($"Error Code = {ioStatus}, 0x{ioStatus.ToString("X")}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not read the command response.\r\n{ex.Message}");
            }

            return false;
        }

        public List<double> GetMeasurements(uint sampleSize, uint interval, out int ioStatus)
        {
            List<double> data = null;
            ioStatus = 0;
            // device not connected...
            try
            {
                _newport.Flush();
                // If the sample size is not valid
                if (sampleSize <= 0) return null;

//                rtbResponse.Text = string.Empty;
                var status = DatastoreInitialize(true,
                    new NewportMeasurementSettings(0, TriggerStartEvent.ContinuousMeasurement,
                        TriggerStopEvent.NeverStop, true, CaptureMode.DC_CONTINUOUS, sampleSize / (10000.0 / interval),
                        interval, 0));

                uint nSamples = 0;
                if (string.IsNullOrEmpty(status))
                {
                    nSamples = WaitForDataStore(sampleSize);
                    var writeStatus = _newport.Write(NewportScpi.DataStoreDisable);
                    if (writeStatus != 0) status = ErrorString(writeStatus);
                }
                if (nSamples > 0)
                {
                    if (string.IsNullOrEmpty(status))
                    {
                        ioStatus = ReadDataStoreValues(nSamples, out data);
                        if (ioStatus == 0) return data;
                    }
                }
                else
                {
                    throw new Exception("Nothing to read!");
                }

                if (!string.IsNullOrEmpty(status))
                {
                    throw new Exception($"GetMeasurements({sampleSize},{interval}) failed: {status}");
                }
            }
            finally
            {
                Write(NewportScpi.TriggerStartEvent(0));
                Write(NewportScpi.TriggerStopEvent(0));
                Measuring = new NewportState(false);
                ResetMeasurement();
                //Profile?.Invoke($"Continuous Task {(end - start).TotalSeconds}");
            }
            return data;
        }
    }
}