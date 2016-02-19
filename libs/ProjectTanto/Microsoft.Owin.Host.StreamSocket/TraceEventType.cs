namespace Microsoft.Owin.Host.StreamSocket
{
    using System.ComponentModel;

    /// <summary>
    /// Identifies the type of event that has caused the trace.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public enum TraceEventType
    {
        Critical = 1,
        Error = 2,
        Warning = 4,
        Information = 8,
        Verbose = 16,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Start = 256,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Stop = 512,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Suspend = 1024,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Resume = 2048,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Transfer = 4096,
    }
}
