using System.Text;

namespace Newport.Usb
{
    public interface INewportInstrument
    {
        string Name { get; }
        int m_knIOError { get; }
        int m_knUSBAddrNotFound { get; }
        int m_knWDStatusInvalidWDHandle { get; }
        int m_knWDDeviceNotFound { get; }
        uint m_knWDDeviceNotResponding { get; }
        uint m_knWDStatusEndpointHalted { get; }

        bool Logging { get; set; }
        string LogFilePath { get; }
        bool OpenDevices();
        bool OpenDevices(int port);
        bool OpenDevices(int port, bool usingDeviceKey);
        void CloseDevices();
        int Write(string deviceKey, string cmd);
        int Write(int deviceID, string cmd);
        int Read(string deviceKey, StringBuilder buffer);
        int Read(int deviceID, StringBuilder buffer);
        int ReadBinary(string deviceKey, StringBuilder buffer);
        int ReadBinary(int deviceID, StringBuilder buffer);
        int ReadBinary(StringBuilder buffer);
        int Query(string deviceKey, string cmd, StringBuilder buffer);
        int Query(int deviceID, string cmd, StringBuilder buffer);
        string Read();
        int Write(string text);
        int Read(StringBuilder sbResponse);
        void Flush();
    }
}