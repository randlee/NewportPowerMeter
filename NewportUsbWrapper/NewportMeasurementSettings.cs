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

        public uint Samples() => (uint)(DurationSeconds*10000.0/DatastoreInterval);
        

        public NewportMeasurementSettings(uint channel, TriggerStartEvent startEvent, TriggerStopEvent stopEvent, bool risingEdge,CaptureMode mode , double measurementDuration ,uint interval, double holdoff = 0)
        {
            RisingEdge = risingEdge;
            Channel = channel;
            StartEvent = startEvent;
            StopEvent = stopEvent;
            Holdoff = holdoff;
            DurationSeconds = measurementDuration;
            DatastoreInterval = interval;
            Ringbuffer = true;
            Mode = mode;
        }

        public static NewportMeasurementSettings ContiuousMeasurement(uint channel, uint interval)
        {
            if (interval == 0) interval = 1;
            return new NewportMeasurementSettings(channel,TriggerStartEvent.ContinuousMeasurement, TriggerStopEvent.NeverStop,true,CaptureMode.DC_CONTINUOUS,interval,0);
        }
    }
}