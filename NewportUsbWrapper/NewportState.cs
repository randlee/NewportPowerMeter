namespace Newport.Usb
{
    public struct NewportState
    {
        public bool Armed { get; }
        public bool Triggered { get; }
        public bool Measuring { get; }

        public NewportState(bool measuring, bool armed=false, bool triggered=false)
        {
            Armed = !triggered && armed;
            Triggered = triggered;
            Measuring = measuring;
        }

        #region Overrides of ValueType

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            if(obj is NewportState)
            {
                return ((NewportState) obj).Measuring == Measuring && ((NewportState) obj).Triggered == Triggered &&
                       ((NewportState) obj).Armed == Armed;
            }
            return false;
        }

        #region Overrides of ValueType

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return  (Measuring ? 0x01 : 0) |
                    (Armed ? 0x02 : 0) |
                    (Triggered ? 0x04 : 0);
        }

        #endregion

        #endregion
    }
}