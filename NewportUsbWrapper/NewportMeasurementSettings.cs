namespace Newport.Usb
{
    public struct NewportMeasurementSettings
    {
        public bool RisingEdge { get; }
        public uint Channel { get; }
        public double DurationSeconds { get; }
        public TriggerStartEvent StartEvent { get; }
        public TriggerStopEvent StopEvent { get; }
        public double Holdoff { get; }
        public uint DatastoreInterval { get; }
        public bool Ringbuffer { get; }
        public CaptureMode Mode { get; }
        public bool ResetInstrument { get; }
        public uint Samples { get; }
        

        public NewportMeasurementSettings(uint channel, TriggerStartEvent startEvent, TriggerStopEvent stopEvent, bool risingEdge,CaptureMode mode , double measurementDuration ,uint interval, double holdoff = 0, bool resetInstrument = false)
        {
            RisingEdge = risingEdge;
            Channel = channel;
            StartEvent = startEvent;
            StopEvent = stopEvent;
            Holdoff = holdoff;
            DurationSeconds = measurementDuration;
            DatastoreInterval = interval;
            Mode = mode;
            Ringbuffer = false;
            ResetInstrument = resetInstrument;
            Samples = (uint) (DurationSeconds*10000.0/DatastoreInterval);
        }

        public static NewportMeasurementSettings ContiuousMeasurement(uint channel, uint interval, bool resetInstrument = false)
        {
            if (interval == 0) interval = 1;
            return new NewportMeasurementSettings(channel,TriggerStartEvent.ContinuousMeasurement, TriggerStopEvent.NeverStop,true,CaptureMode.DC_CONTINUOUS,0,interval,0,resetInstrument);
        }

        public static NewportMeasurementSettings SoftkeyTriggered(uint channel, uint interval, uint samples, bool resetInstrument=false)
        {
            if (interval == 0) interval = 1;
            var duration = (samples*interval)/10000.0;
            return new NewportMeasurementSettings(channel, TriggerStartEvent.SoftKey, TriggerStopEvent.StopAfterTime, true, CaptureMode.DC_CONTINUOUS,duration, interval, 0,resetInstrument);
        }

        public static NewportMeasurementSettings CommandTriggered(uint channel, uint interval, uint samples, bool resetInstrument = false)
        {
            if (interval == 0) interval = 1;
            var duration = (samples * interval) / 10000.0;
            return new NewportMeasurementSettings(channel, TriggerStartEvent.TriggerStateCommand,
                TriggerStopEvent.StopAfterTime, true, CaptureMode.DC_CONTINUOUS, duration, interval, 0, resetInstrument);
        }

        public static NewportMeasurementSettings TtlTriggered(uint channel, uint interval, uint samples, bool risingEdge, bool resetInstrument = false)
        {
            if (interval == 0) interval = 1;
            var duration = (samples * interval) / 10000.0;
            return new NewportMeasurementSettings(channel, TriggerStartEvent.ExternalTrigger, TriggerStopEvent.StopAfterTime, risingEdge, CaptureMode.DC_CONTINUOUS, duration, interval, 0,resetInstrument);
        }

        public static NewportMeasurementSettings TtlTriggerToTrigger(uint channel, uint interval, bool risingEdge, bool resetInstrument = false)
        {
            return new NewportMeasurementSettings(channel, TriggerStartEvent.ExternalTrigger, TriggerStopEvent.StopOnExternalTrigger, risingEdge, CaptureMode.DC_CONTINUOUS, (NewportUsbPowerMeter.DATASTORE_SIZE_MAX*interval)/10000.0, interval, 0,resetInstrument);
        }
    }
}