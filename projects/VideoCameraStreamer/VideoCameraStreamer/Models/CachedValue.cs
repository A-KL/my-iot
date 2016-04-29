namespace VideoCameraStreamer.Models
{
    using System;

    public class CachedValue<T> where T : IDisposable
    {
        public void Write(T item)
        {
            
        }

        public T Read()
        {
            return default(T);
        }
    }
}
