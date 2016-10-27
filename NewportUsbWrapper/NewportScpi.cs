namespace Newport.Usb
{
    public enum TriggerStopEvent
    {
        /// <summary>
        /// The measurement never stops
        /// </summary>
        NeverStop = 0,
        /// <summary>
        /// Measurement stops when an external trigger occurs
        /// </summary>
        StopOnExternalTrigger = 1,
        /// <summary>
        /// Measurement stops when a designated Soft key is pressed
        /// </summary>
        StopOnSoftkey = 2,
        /// <summary>
        /// Measurement stops when PM:TRIG:STATE 0 command is issued
        /// </summary>
        StopOnCommandState0 = 3,
        /// <summary>
        /// Measurement stops when a pre-specified measurement level(PM:TRIG:VALUE) is reached
        /// </summary>
        StopWhenMeasurementLevelExceeded = 4,
        /// <summary>
        /// Measurement stops when a pre-specified time interval(PM:TRIG:TIME) from TRIGGER START has been reached
        /// </summary>
        StopAfterTime = 5,
    }

    public enum TriggerStartEvent
    {
        /// <summary>
        /// Continuous measurement
        /// </summary>
        ContinuousMeasurement = 0,
        /// <summary>
        /// Measurement starts when an external trigger occurs
        /// </summary>
        ExternalTrigger = 1,
        /// <summary>
        /// Measurement starts when a designated Soft key is pressed
        /// </summary>
        SoftKey = 2,
        /// <summary>
        /// Measurement starts when PM:TRIG:STATE 1 command is issued.
        /// </summary>
        TriggerStateCommand = 3
    }

    public enum AnalogFilter
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// 250kHz
        /// </summary>
        F250kHz = 1,
        /// <summary>
        /// 12.5kHz
        /// </summary>
        F12_5kHz = 2,
        /// <summary>
        /// 1kHz
        /// </summary>
        F1kHz = 3,
        /// <summary>
        /// 5Hz
        /// </summary>
        F5Hz = 4
    }

    public static class NewportScpi
    {
        public static string Identity => "*IDN?\r";
        public static string Reset => "*RST\r";

        public static string AutoRange(bool enable) => $"PM:AUTO {(enable ? 1 : 0)}\r";
        public static string Mode(CaptureMode mode) => $"PM:MODE {(int) mode}\r";

        #region DataStore
        public static string DataStoreEnableQuery => "PM:DS:ENABLE?\r";
        public static string DataStoreEnable => "PM:DS:ENABLE 1\r";
        public static string DataStoreDisable => "PM:DS:ENABLE 0\r";

        public static string DataStoreClear => "PM:DS:CLEAR\r";
        public static string DataStoreCountQuery => "PM:DS:COUNT?\r";

        public static string DataStoreGetSample(uint index) => $"PM:DS:GET? {index}\r";
        public static string DataStoreGetLatest(uint samples) => $"PM:DS:GET? +{samples}\r";
        public static string DataStoreGetOldest(uint samples) => $"PM:DS:GET? -{samples}\r";
        public static string DataStoreGetRange(uint start, uint end) => $"PM:DS:GET? {start}-{end}\r";
        /// <summary>
        /// Data Store buffer behavior selection. 0=Fixed size buffer, 1=ring buffer
        /// </summary>
        /// <param name="bufferBehavior"> 0=Fixed size buffer, 1=ring buffer</param>
        /// <returns></returns>
        public static string DataStoreBuffer(bool bufferBehavior) { return bufferBehavior ? "PM:DS:BUFFER 1\r" : $"PM:DS:BUFFER 0\r"; }

        /// <summary>
        /// An interval value of 1 causes the power meter to put all measurements taken in the data store buffer; 
        /// a value of 2 causes every other measurement to be put in the data buffer and so on.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string DataStoreInterval(uint index) => $"PM:DS:INTERVAL {index}\r";

        /// <summary>
        /// PM:DS:SIZE 
        /// Parameters: The parameter @bufferSize has range 1 to 250000.
        /// The parameter represents the size of the data buffer to be used for data storing.
        /// Function: This command sets the size of the buffer for the currently selected channel used for data storing.
        /// </summary>
        /// <param name="bufferSize">Size of DS buffer to allocate</param>
        /// <returns></returns>
        public static string DataStoreSize(uint bufferSize) => $"PM:DS:SIZE {bufferSize}\r";
        #endregion DataStore

        #region Trigger

        public static string TriggerExternalEnable(uint channel=0) {  return channel == 0 ? $"PM:TRIG:EXT 1\r" : $"PM:TRIG:EXT 2\r"; }

        public static string TriggerExternalDisable => $"PM:TRIG:EXT 0\r";

        /// <summary>
        /// External trigger edge query
        /// </summary>
        public static string TriggerEdgeQuery => "PM:TRIG:EDGE?\r";

        /// <summary>
        /// This command defines whether the external trigger input on the back panel is falling edge or rising edge active.
        /// </summary>
        /// <param name="risingEdge">true indicates rising edge triggered, false indicates falling edge triggered</param>
        /// <returns></returns>
        public static string TriggerEdge(bool risingEdge) { return risingEdge ? "PM:TRIG:EDGE 1\r" : "PM:TRIG:EDGE 0\r"; }

        /// <summary>
        /// External Trigger Holdoff Time Query
        /// </summary>
        public static string TriggerHoldoffQuery => "PM:TRIG:HOLD?\r";

        /// <summary>
        /// External Trigger Holdoff Time Command.  Holdoff time is the delay in milliseconds for the trigger to take effect
        /// </summary>
        /// <param name="timeMs">The parameter @time is of type integer in the range 0 to 1000. 
        /// @time is the delay in milliseconds for the trigger to take effect. </param>
        /// <returns></returns>
        public static string TriggerHoldoff(uint timeMs) => $"PM:TRIG:HOLD {timeMs}\r";

        /// <summary>
        /// Returns the current start event setting
        /// </summary>
        public static string TriggerStartEventQuery => "PM:TRIG:START?\r";

        /// <summary>
        /// This command sets the optional start event.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string TriggerStartEvent(TriggerStartEvent option) => $"PM:TRIG:START {(int)option}\r";

        /// <summary>
        /// Returns the current trigger stop event setting (TriggerOption)
        /// </summary>
        public static string TriggerStopEventQuery => "PM:TRIG:STOP?\r";

        /// <summary>
        /// This command sets the optional stop event.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string TriggerStopEvent(TriggerStopEvent option) => $"PM:TRIG:STOP {(int)option}\r";


        /// <summary>
        /// This command queries the trigger state.  0=armed, 1=triggered
        /// </summary>
        /// <returns></returns>
        public static string TriggerStateQuery => "PM:TRIG:STATE?\r";

        /// <summary>
        /// PM:TRIG:STATE 0
        /// This command sets the trigger state to 'armed'
        /// </summary>
        public static string TriggerStateArm => "PM:TRIG:STATE 0\r";

        /// <summary>
        /// PM:TRIG:STATE 1
        /// This command sets the trigger state to 'triggered'
        /// </summary>
        public static string TriggerStateTriggered => "PM:TRIG:STATE 1\r";
        /// <summary>
        /// This command returns the measurement level that indicates a trigger stop condition.
        /// </summary>
        /// <returns></returns>
        public static string TriggerValueQuery => "PM:TRIG:VALUE?\r";

        /// <summary>
        /// This command sets the measurement level that indicates a trigger stop condition. 
        /// The power meter will stop taking further measurements if a measurement taken exceeds  the
        /// value specified by this command, and if trigger stop option is set to measurement level.
        /// </summary>
        /// <param name="value">Measurement level that indicates a trigger stop condition. </param>
        /// <returns></returns>
        public static string TriggerValue(float value) => $"PM:TRIG:VALUE {value}\r";

        /// <summary>
        /// This command returns the time duration that indicates a trigger stop condition.
        /// </summary>
        public static string TriggerTimeQuery => "PM:TRIG:TIME?\r";

        /// <summary>
        /// This command sets the time duration, in ms, that indicates a trigger stop condition. 
        /// The power meter will stop taking further measurements if the time from trigger start exceeds 
        /// time duration specified by this command, and if trigger stop option is set to time.
        /// </summary>
        /// <param name="timeMs">Sample time after trigger</param>
        /// <returns></returns>
        public static string TriggerTimeMs(uint timeMs) => $"PM:TRIG:TIME {timeMs}\r";
        #endregion Trigger

        #region Filters
        public static string DigitalFilter(uint val) => $"PM:DIGITALFILTER {val}\r";

        public static string DigitalFilterQuery => "PM:DIGITALFILTER?\r";

        public static string AnalogFilter(uint val) => $"PM:ANALOGFILTER {val}\r";

        public static string AnalogFilterQuery => "PM:ANALOGLFILTER?\r";
        #endregion

        public static string Echo(bool on) => $"ECHO {(@on ? 1 : 0)}\r";
        public static string EchoQuery => $"ECHO?\r";
        #region Error Codes

        public static string ErrorString => "ERRSTR?\r";
        public static string Error => "ERR?\r";

        #endregion
    }
}