#region Copyright (c) 2015-2016 Visyn
//The MIT License(MIT)
//
//Copyright(c) 2015-2016 Visyn
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using Visyn.Newport.Collections;

namespace Visyn.Newport.Log
{
    public class ComLogItem : IDelimitedCollection
    {
        public DateTime Time { get; set; } = DateTime.Now;
        public ComEventType EventType { get; set; }
        public object DataWritten { get; set; }
        public object DataRead { get; set; }
        public string DataReadString => DelimitedString(DataRead.ToString());

        private string shortString(object data, int length=20)
        {
            if (data == null) return string.Empty;
            var str = data.ToString();
            return str.Length <= length ? str : str.Substring(0, length) + "...";
        }

        [Obsolete("For serialization only")]
        public ComLogItem() : this(ComEventType.Unknown) { }

        public ComLogItem(ComEventType eventType=ComEventType.Unknown)
        {
            EventType = eventType;
        }

        #region Overrides of Object

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            switch(EventType)
            {
                case ComEventType.Connect:      return "Connect";
                case ComEventType.Disconnect:   return "Disconnect";
                case ComEventType.Read:         return $"Read: {shortString(DataRead)}";
                case ComEventType.Write:        return $"Write: {shortString(DataWritten)}";
                case ComEventType.Query:        return $"Query: {shortString(DataWritten)} : {shortString(DataRead)}";
                default:                        throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        public static ComLogItem ReadEvent(object data)
        {
            return new ComLogItem(ComEventType.Read) {DataRead = data};
        }

        public static ComLogItem WriteEvent(object data)
        {
            return new ComLogItem(ComEventType.Write) {DataWritten = data};
        }

        public static ComLogItem QueryEvent(object cmd, object resoponse)
        {
            return new ComLogItem(ComEventType.Query) {DataWritten = cmd, DataRead = resoponse};
        }

        #region Implementation of IDelimitedCollection
        private static string DelimitedString(object obj)
        {
            if (obj == null) return "";
            var read = obj.ToString();
            if (read.Contains(",") || read.Contains("\r") || read.Contains("\n")) return $"'{read}'";
            return read;
        }

        public ICollection<string> DelimitedCollection => new[] {Time.ToString(),EventType.ToString(), DelimitedString(DataWritten), DelimitedString(DataRead)};//);
        public string ToDelimitedString(string delimiter) => $"{Time}{delimiter}{EventType}{delimiter}{DelimitedString(DataWritten)}{delimiter}{DelimitedString(DataRead)}";

        #endregion
    }
}