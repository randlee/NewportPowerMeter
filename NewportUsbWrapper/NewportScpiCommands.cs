using System.Security.Authentication.ExtendedProtection;

namespace Newport.Usb
{
    public enum PowermeterMode
    {
        DcContinuous = 0,
        DcSingle = 1,
        Integrate = 2,
        PeakToPeakContinuous = 3,
        PeakToPeakSingle = 4,
        PulseContinuous = 5,
        PulseSingle = 6,
        RMS = 7
    }

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
    
    public static class NewportScpiCommands
    {
        public static string Identity => "*IDN?";
        public static string Reset => "*RST";

        public static string AutoRange(bool enable) => $"PM:AUTO {(enable ? 1 : 0)}";
        public static string Mode(PowermeterMode mode) => $"PM:MODE {(int) mode}\r";

#region DataStore
        public static string DataStoreEnable => "PM:DS:ENABLE 1\r";
        public static string DataStoreDisable => "PM:DS:ENABLE 0\r";

        public static string DataStoreClear => "PM:DS:CLEAR\r";
        public static string DataStoreCountQuery => "PM:DS:COUNT?\r";

        public static string DataStoreGetSample(uint index) => $"PM:DS:GET? {index}\r";
        public static string DataStoreGetLatest(uint samples) => $"PM:DS:GET? +{samples}\r";
        public static string DataStoreGetOldest(uint samples) => $"PM:DS:GET? -{samples}\r";
        public static string DataStoreGetRange(uint start, uint end) => $"PM:DS:GET? +{start-end}\r";
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

        public static string TriggerEnable(uint channel=0) {  return channel == 0 ? $"PM:TRIG:EXT 1" : $"PM:TRIG:EXT 2"; }

        public static string TriggerDisable => $"PM:TRIG:EXT 0";

        /// <summary>
        /// External trigger edge query
        /// </summary>
        public static string TriggerEdgeQuery => "PM:TRIG:EDGE?";

        /// <summary>
        /// This command defines whether the external trigger input on the back panel is falling edge or rising edge active.
        /// </summary>
        /// <param name="risingEdge">true indicates rising edge triggered, false indicates falling edge triggered</param>
        /// <returns></returns>
        public static string TriggerEdge(bool risingEdge) { return risingEdge ? "PM:TRIG:EDGE 1" : "PM:TRIG:EDGE 0"; }

        /// <summary>
        /// External Trigger Holdoff Time Query
        /// </summary>
        public static string TriggerHoldoffQuery => "PM:TRIG:HOLD?";

        /// <summary>
        /// External Trigger Holdoff Time Command.  Holdoff time is the delay in milliseconds for the trigger to take effect
        /// </summary>
        /// <param name="timeMs">The parameter @time is of type integer in the range 0 to 1000. 
        /// @time is the delay in milliseconds for the trigger to take effect. </param>
        /// <returns></returns>
        public static string TriggerHoldoff(uint timeMs) => $"PM:TRIG:HOLD {timeMs}";

        /// <summary>
        /// Returns the current start event setting
        /// </summary>
        public static string TriggerStartEventQuery => "PM:TRIG:START?";

        /// <summary>
        /// This command sets the optional start event.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string TriggerStartEvent(TriggerStartEvent option) => $"PM:TRIG:START {(int)option}";

        /// <summary>
        /// Returns the current trigger stop event setting (TriggerOption)
        /// </summary>
        public static string TriggerStopEventQuery => "PM:TRIG:STOP?";

        /// <summary>
        /// This command sets the optional stop event.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string TriggerStopEvent(TriggerStopEvent option) => $"PM:TRIG:STOP {(int)option}";


        /// <summary>
        /// This command queries the trigger state.  0=armed, 1=triggered
        /// </summary>
        /// <returns></returns>
        public static string TriggerStateQuery => "PM:TRIG:STATE?";

        /// <summary>
        /// This command sets the trigger state to 'armed'
        /// </summary>
        public static string TriggerStateArm => "PM:TRIG:STATE 0";

        public static string TriggerStateTriggered => "PM:TRIG:STATE 1";
        /// <summary>
        /// This command returns the measurement level that indicates a trigger stop condition.
        /// </summary>
        /// <returns></returns>
        public static string TriggerValueQuery => "PM:TRIG:VALUE?";

        /// <summary>
        /// This command sets the measurement level that indicates a trigger stop condition. 
        /// The power meter will stop taking further measurements if a measurement taken exceeds  the
        /// value specified by this command, and if trigger stop option is set to measurement level.
        /// </summary>
        /// <param name="value">Measurement level that indicates a trigger stop condition. </param>
        /// <returns></returns>
        public static string TriggerValue(float value) => $"PM:TRIG:VALUE {value}";

        /// <summary>
        /// This command returns the time duration that indicates a trigger stop condition.
        /// </summary>
        public static string TriggerTimeQuery => "PM:TRIG:TIME?";

        /// <summary>
        /// This command sets the time duration, in ms, that indicates a trigger stop condition. 
        /// The power meter will stop taking further measurements if the time from trigger start exceeds 
        /// time duration specified by this command, and if trigger stop option is set to time.
        /// </summary>
        /// <param name="timeMs">Sample time after trigger</param>
        /// <returns></returns>
        public static string TriggerTime(uint timeMs) => $"PM:TRIG:TIME {timeMs}";
#endregion Trigger
    }
}
