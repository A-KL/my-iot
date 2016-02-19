namespace Microsoft.Owin.Infrastructure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [System.CodeDom.Compiler.GeneratedCode("App_Packages", "")]
    internal struct HeaderSegment : IEquatable<HeaderSegment>
    {
        private readonly StringSegment formatting;
        private readonly StringSegment data;

        // <summary>
        // Initializes a new instance of the <see cref="T:System.Object"/> class.
        // </summary>
        public HeaderSegment(StringSegment formatting, StringSegment data)
        {
            this.formatting = formatting;
            this.data = data;
        }

        public StringSegment Formatting
        {
            get { return formatting; }
        }

        public StringSegment Data
        {
            get { return data; }
        }

        #region Equality members

        public bool Equals(HeaderSegment other)
        {
            return formatting.Equals(other.formatting) && data.Equals(other.data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is HeaderSegment && Equals((HeaderSegment)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (formatting.GetHashCode() * 397) ^ data.GetHashCode();
            }
        }

        public static bool operator ==(HeaderSegment left, HeaderSegment right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HeaderSegment left, HeaderSegment right)
        {
            return !left.Equals(right);
        }

        #endregion
    }

    [System.CodeDom.Compiler.GeneratedCode("App_Packages", "")]
    internal struct HeaderSegmentCollection : IEnumerable<HeaderSegment>, IEquatable<HeaderSegmentCollection>
    {
        private readonly string[] headers;

        public HeaderSegmentCollection(string[] headers)
        {
            this.headers = headers;
        }

        #region Equality members

        public bool Equals(HeaderSegmentCollection other)
        {
            return Equals(headers, other.headers);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is HeaderSegmentCollection && Equals((HeaderSegmentCollection)obj);
        }

        public override int GetHashCode()
        {
            return (headers != null ? headers.GetHashCode() : 0);
        }

        public static bool operator ==(HeaderSegmentCollection left, HeaderSegmentCollection right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HeaderSegmentCollection left, HeaderSegmentCollection right)
        {
            return !left.Equals(right);
        }

        #endregion

        public Enumerator GetEnumerator()
        {
            return new Enumerator(headers);
        }

        IEnumerator<HeaderSegment> IEnumerable<HeaderSegment>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal struct Enumerator : IEnumerator<HeaderSegment>
        {
            private readonly string[] headers;
            private int index;

            private string header;
            private int headerLength;
            private int offset;

            private int leadingStart;
            private int leadingEnd;
            private int valueStart;
            private int valueEnd;
            private int trailingStart;

            private Mode mode;

            private static readonly string[] NoHeaders = new string[0];

            public Enumerator(string[] headers)
            {
                this.headers = headers ?? NoHeaders;
                header = string.Empty;
                headerLength = -1;
                index = -1;
                offset = -1;
                leadingStart = -1;
                leadingEnd = -1;
                valueStart = -1;
                valueEnd = -1;
                trailingStart = -1;
                mode = Mode.Leading;
            }

            private enum Mode
            {
                Leading,
                Value,
                ValueQuoted,
                Trailing,
                Produce,
            }

            private enum Attr
            {
                Value,
                Quote,
                Delimiter,
                Whitespace
            }

            public HeaderSegment Current
            {
                get
                {
                    return new HeaderSegment(
                        new StringSegment(header, leadingStart, leadingEnd - leadingStart),
                        new StringSegment(header, valueStart, valueEnd - valueStart));
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (mode == Mode.Produce)
                    {
                        leadingStart = trailingStart;
                        leadingEnd = -1;
                        valueStart = -1;
                        valueEnd = -1;
                        trailingStart = -1;

                        if (offset == headerLength &&
                            leadingStart != -1 &&
                            leadingStart != offset)
                        {
                            // Also produce trailing whitespace
                            leadingEnd = offset;
                            return true;
                        }
                        mode = Mode.Leading;
                    }

                    // if end of a string
                    if (offset == headerLength)
                    {
                        ++index;
                        offset = -1;
                        leadingStart = 0;
                        leadingEnd = -1;
                        valueStart = -1;
                        valueEnd = -1;
                        trailingStart = -1;

                        // if that was the last string
                        if (index == headers.Length)
                        {
                            // no more move nexts
                            return false;
                        }

                        // grab the next string
                        header = headers[index] ?? string.Empty;
                        headerLength = header.Length;
                    }
                    while (true)
                    {
                        ++offset;
                        char ch = offset == headerLength ? (char)0 : header[offset];
                        // todo - array of attrs
                        Attr attr = char.IsWhiteSpace(ch) ? Attr.Whitespace : ch == '\"' ? Attr.Quote : (ch == ',' || ch == (char)0) ? Attr.Delimiter : Attr.Value;

                        switch (mode)
                        {
                            case Mode.Leading:
                                switch (attr)
                                {
                                    case Attr.Delimiter:
                                        leadingEnd = offset;
                                        mode = Mode.Produce;
                                        break;
                                    case Attr.Quote:
                                        leadingEnd = offset;
                                        valueStart = offset;
                                        mode = Mode.ValueQuoted;
                                        break;
                                    case Attr.Value:
                                        leadingEnd = offset;
                                        valueStart = offset;
                                        mode = Mode.Value;
                                        break;
                                    case Attr.Whitespace:
                                        // more
                                        break;
                                }
                                break;
                            case Mode.Value:
                                switch (attr)
                                {
                                    case Attr.Quote:
                                        mode = Mode.ValueQuoted;
                                        break;
                                    case Attr.Delimiter:
                                        valueEnd = offset;
                                        trailingStart = offset;
                                        mode = Mode.Produce;
                                        break;
                                    case Attr.Value:
                                        // more
                                        break;
                                    case Attr.Whitespace:
                                        valueEnd = offset;
                                        trailingStart = offset;
                                        mode = Mode.Trailing;
                                        break;
                                }
                                break;
                            case Mode.ValueQuoted:
                                switch (attr)
                                {
                                    case Attr.Quote:
                                        mode = Mode.Value;
                                        break;
                                    case Attr.Delimiter:
                                        if (ch == (char)0)
                                        {
                                            valueEnd = offset;
                                            trailingStart = offset;
                                            mode = Mode.Produce;
                                        }
                                        break;
                                    case Attr.Value:
                                    case Attr.Whitespace:
                                        // more
                                        break;
                                }
                                break;
                            case Mode.Trailing:
                                switch (attr)
                                {
                                    case Attr.Delimiter:
                                        mode = Mode.Produce;
                                        break;
                                    case Attr.Quote:
                                        // back into value
                                        trailingStart = -1;
                                        valueEnd = -1;
                                        mode = Mode.ValueQuoted;
                                        break;
                                    case Attr.Value:
                                        // back into value
                                        trailingStart = -1;
                                        valueEnd = -1;
                                        mode = Mode.Value;
                                        break;
                                    case Attr.Whitespace:
                                        // more
                                        break;
                                }
                                break;
                        }
                        if (mode == Mode.Produce)
                        {
                            return true;
                        }
                    }
                }
            }

            public void Reset()
            {
                index = 0;
                offset = 0;
                leadingStart = 0;
                leadingEnd = 0;
                valueStart = 0;
                valueEnd = 0;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("App_Packages", "")]
    internal struct StringSegment : IEquatable<StringSegment>
    {
        private readonly string buffer;
        private readonly int offset;
        private readonly int count;

        // <summary>
        // Initializes a new instance of the <see cref="T:System.Object"/> class.
        // </summary>
        public StringSegment(string buffer, int offset, int count)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.count = count;
        }

        public string Buffer
        {
            get { return buffer; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public int Count
        {
            get { return count; }
        }

        public string Value
        {
            get { return offset == -1 ? null : buffer.Substring(offset, count); }
        }

        public bool HasValue
        {
            get { return offset != -1 && count != 0 && buffer != null; }
        }

        #region Equality members

        public bool Equals(StringSegment other)
        {
            return string.Equals(buffer, other.buffer) && offset == other.offset && count == other.count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is StringSegment && Equals((StringSegment)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (buffer != null ? buffer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ offset;
                hashCode = (hashCode * 397) ^ count;
                return hashCode;
            }
        }

        public static bool operator ==(StringSegment left, StringSegment right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringSegment left, StringSegment right)
        {
            return !left.Equals(right);
        }

        #endregion

        public bool StartsWith(string text, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            int textLength = text.Length;
            if (!HasValue || count < textLength)
            {
                return false;
            }

            return string.Compare(buffer, offset, text, 0, textLength, comparisonType) == 0;
        }

        public bool EndsWith(string text, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            int textLength = text.Length;
            if (!HasValue || count < textLength)
            {
                return false;
            }

            return string.Compare(buffer, offset + count - textLength, text, 0, textLength, comparisonType) == 0;
        }

        public bool Equals(string text, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            int textLength = text.Length;
            if (!HasValue || count != textLength)
            {
                return false;
            }

            return string.Compare(buffer, offset, text, 0, textLength, comparisonType) == 0;
        }

        public string Substring(int offset, int length)
        {
            return buffer.Substring(this.offset + offset, length);
        }

        public StringSegment Subsegment(int offset, int length)
        {
            return new StringSegment(buffer, this.offset + offset, length);
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
        }
    }

    internal static partial class OwinHelpers
    {
        private static readonly Action<string, string, object> AddCookieCallback = (name, value, state) =>
        {
            var dictionary = (IDictionary<string, string>)state;
            if (!dictionary.ContainsKey(name))
            {
                dictionary.Add(name, value);
            }
        };

        private static readonly char[] SemicolonAndComma = new[] { ';', ',' };

        internal static IDictionary<string, string> GetCookies(IOwinRequest request)
        {
            var cookies = request.Get<IDictionary<string, string>>("Microsoft.Owin.Cookies#dictionary");
            if (cookies == null)
            {
                cookies = new Dictionary<string, string>(StringComparer.Ordinal);
                request.Set("Microsoft.Owin.Cookies#dictionary", cookies);
            }

            string text = GetHeader(request.Headers, "Cookie");
            if (request.Get<string>("Microsoft.Owin.Cookies#text") != text)
            {
                cookies.Clear();
                ParseDelimited(text, SemicolonAndComma, AddCookieCallback, cookies);
                request.Set("Microsoft.Owin.Cookies#text", text);
            }
            return cookies;
        }

        internal static void ParseDelimited(string text, char[] delimiters, Action<string, string, object> callback, object state)
        {
            int textLength = text.Length;
            int equalIndex = text.IndexOf('=');
            if (equalIndex == -1)
            {
                equalIndex = textLength;
            }
            int scanIndex = 0;
            while (scanIndex < textLength)
            {
                int delimiterIndex = text.IndexOfAny(delimiters, scanIndex);
                if (delimiterIndex == -1)
                {
                    delimiterIndex = textLength;
                }
                if (equalIndex < delimiterIndex)
                {
                    while (scanIndex != equalIndex && char.IsWhiteSpace(text[scanIndex]))
                    {
                        ++scanIndex;
                    }
                    string name = text.Substring(scanIndex, equalIndex - scanIndex);
                    string value = text.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);
                    callback(
                        Uri.UnescapeDataString(name.Replace('+', ' ')),
                        Uri.UnescapeDataString(value.Replace('+', ' ')),
                        state);
                    equalIndex = text.IndexOf('=', delimiterIndex);
                    if (equalIndex == -1)
                    {
                        equalIndex = textLength;
                    }
                }
                scanIndex = delimiterIndex + 1;
            }
        }
    }

    internal static partial class OwinHelpers
    {
        public static string GetHeader(IDictionary<string, string[]> headers, string key)
        {
            var values = GetHeaderUnmodified(headers, key);
            return values == null ? null : string.Join(",", values);
        }

        public static IEnumerable<string> GetHeaderSplit(IDictionary<string, string[]> headers, string key)
        {
            var values = GetHeaderUnmodified(headers, key);
            return values == null ? null : GetHeaderSplitImplementation(values);
        }

        private static IEnumerable<string> GetHeaderSplitImplementation(string[] values)
        {
            foreach (var segment in new HeaderSegmentCollection(values))
            {
                if (segment.Data.HasValue)
                {
                    yield return DeQuote(segment.Data.Value);
                }
            }
        }

        public static string[] GetHeaderUnmodified(IDictionary<string, string[]> headers, string key)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }
            string[] values;
            return headers.TryGetValue(key, out values) ? values : null;
        }

        public static void SetHeader(IDictionary<string, string[]> headers, string key, string value)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                headers.Remove(key);
            }
            else
            {
                headers[key] = new[] { value };
            }
        }

        public static void SetHeaderJoined(IDictionary<string, string[]> headers, string key, params string[] values)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            if (values == null || values.Length == 0)
            {
                headers.Remove(key);
            }
            else
            {
                headers[key] = new[] { string.Join(",", values.Select(value => QuoteIfNeeded(value))) };
            }
        }

        // Quote items that contain comas and are not already quoted.
        private static string QuoteIfNeeded(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                // Ignore
            }
            else if (value.Contains(','))
            {
                if (value[0] != '"' || value[value.Length - 1] != '"')
                {
                    value = '"' + value + '"';
                }
            }

            return value;
        }

        private static string DeQuote(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                // Ignore
            }
            else if (value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"')
            {
                value = value.Substring(1, value.Length - 2);
            }

            return value;
        }

        public static void SetHeaderUnmodified(IDictionary<string, string[]> headers, string key, params string[] values)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            if (values == null || values.Length == 0)
            {
                headers.Remove(key);
            }
            else
            {
                headers[key] = values;
            }
        }

        public static void SetHeaderUnmodified(IDictionary<string, string[]> headers, string key, IEnumerable<string> values)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }
            headers[key] = values.ToArray();
        }

        public static void AppendHeader(IDictionary<string, string[]> headers, string key, string values)
        {
            if (string.IsNullOrWhiteSpace(values))
            {
                return;
            }

            string existing = GetHeader(headers, key);
            if (existing == null)
            {
                SetHeader(headers, key, values);
            }
            else
            {
                headers[key] = new[] { existing + "," + values };
            }
        }

        public static void AppendHeaderJoined(IDictionary<string, string[]> headers, string key, params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                return;
            }

            string existing = GetHeader(headers, key);
            if (existing == null)
            {
                SetHeaderJoined(headers, key, values);
            }
            else
            {
                headers[key] = new[] { existing + "," + string.Join(",", values.Select(value => QuoteIfNeeded(value))) };
            }
        }

        public static void AppendHeaderUnmodified(IDictionary<string, string[]> headers, string key, params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                return;
            }

            var existing = GetHeaderUnmodified(headers, key);
            if (existing == null)
            {
                SetHeaderUnmodified(headers, key, values);
            }
            else
            {
                SetHeaderUnmodified(headers, key, existing.Concat(values));
            }
        }
    }

    internal static partial class OwinHelpers
    {
        private static readonly Action<string, string, object> AppendItemCallback = (name, value, state) =>
        {
            var dictionary = (IDictionary<string, List<String>>)state;

            List<string> existing;
            if (!dictionary.TryGetValue(name, out existing))
            {
                dictionary.Add(name, new List<string>(1) { value });
            }
            else
            {
                existing.Add(value);
            }
        };

        private static readonly char[] AmpersandAndSemicolon = { '&', ';' };

        internal static IDictionary<string, string[]> GetQuery(IOwinRequest request)
        {
            var query = request.Get<IDictionary<string, string[]>>("Microsoft.Owin.Query#dictionary");
            if (query == null)
            {
                query = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                request.Set("Microsoft.Owin.Query#dictionary", query);
            }

            var text = request.QueryString.Value;
            if (request.Get<string>("Microsoft.Owin.Query#text") != text)
            {
                query.Clear();
                var accumulator = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                ParseDelimited(text, AmpersandAndSemicolon, AppendItemCallback, accumulator);
                foreach (var kv in accumulator)
                {
                    query.Add(kv.Key, kv.Value.ToArray());
                }
                request.Set("Microsoft.Owin.Query#text", text);
            }
            return query;
        }

        internal static IFormCollection GetForm(string text)
        {
            IDictionary<string, string[]> form = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            var accumulator = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            ParseDelimited(text, new[] { '&' }, AppendItemCallback, accumulator);
            foreach (var kv in accumulator)
            {
                form.Add(kv.Key, kv.Value.ToArray());
            }
            return new FormCollection(form);
        }

        internal static string GetJoinedValue(IDictionary<string, string[]> store, string key)
        {
            var values = GetUnmodifiedValues(store, key);
            return values == null ? null : string.Join(",", values);
        }

        internal static string[] GetUnmodifiedValues(IDictionary<string, string[]> store, string key)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }
            string[] values;
            return store.TryGetValue(key, out values) ? values : null;
        }
    }

    internal static partial class OwinHelpers
    {
        internal static string GetHost(IOwinRequest request)
        {
            var headers = request.Headers;

            var host = GetHeader(headers, "Host");
            if (!string.IsNullOrWhiteSpace(host))
            {
                return host;
            }

            var localIpAddress = request.LocalIpAddress ?? "localhost";
            var localPort = request.Get<string>(OwinConstants.CommonKeys.LocalPort);
            return string.IsNullOrWhiteSpace(localPort) ? localIpAddress : localIpAddress + ":" + localPort;
        }
    }
}
