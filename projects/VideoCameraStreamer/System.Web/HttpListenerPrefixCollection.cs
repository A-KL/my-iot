namespace System.Net
{
    using System.Collections;
    using System.Collections.Generic;

#if EMBEDDED_IN_1_0
	public class HttpListenerPrefixCollection : IEnumerable, ICollection {
		ArrayList prefixes;
		
#else
    public class HttpListenerPrefixCollection : ICollection<string>
    {
        private readonly List<string> prefixes = new List<string>();
#endif
        private readonly HttpListener listener;

        internal HttpListenerPrefixCollection(HttpListener listener)
        {
            this.listener = listener;
        }

        public int Count
        {
            get { return this.prefixes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public void Add(string uriPrefix)
        {
            this.listener.CheckDisposed();
            ListenerPrefix.CheckUri(uriPrefix);

            if (this.prefixes.Contains(uriPrefix))
            {
                return;
            }

            this.prefixes.Add(uriPrefix);

            if (this.listener.IsListening)
            {
                EndPointManager.AddPrefix(uriPrefix, this.listener);
            }
        }

        public void Clear()
        {
            this.listener.CheckDisposed();
            this.prefixes.Clear();

            if (this.listener.IsListening)
            {
                EndPointManager.RemoveListener(this.listener);
            }
        }

        public bool Contains(string uriPrefix)
        {
            this.listener.CheckDisposed();
            return this.prefixes.Contains(uriPrefix);
        }

        public void CopyTo(string[] array, int offset)
        {
            this.listener.CheckDisposed();
            this.prefixes.CopyTo(array, offset);
        }

        public void CopyTo(Array array, int offset)
        {
            this.listener.CheckDisposed();

            ((ICollection)this.prefixes).CopyTo(array, offset);
        }

#if !EMBEDDED_IN_1_0
        public IEnumerator<string> GetEnumerator()
        {
            return this.prefixes.GetEnumerator();
        }
#endif

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.prefixes.GetEnumerator();
        }

        public bool Remove(string uriPrefix)
        {
            this.listener.CheckDisposed();

            if (uriPrefix == null)
            {
                throw new ArgumentNullException("uriPrefix");
            }

            var result = this.prefixes.Remove(uriPrefix);

            if (result && this.listener.IsListening)
            {
                EndPointManager.RemovePrefix(uriPrefix, this.listener);
            }

            return result;
        }
    }
}