namespace Windows.Http.Extensions
{
    public static class UriExtensions
    {
        public static bool MaybeUri(string s)
        {
            int p = s.IndexOf(':');
            if (p == -1)
                return false;

            if (p >= 10)
                return false;

            return IsPredefinedScheme(s.Substring(0, p));
        }

        private static bool IsPredefinedScheme(string scheme)
        {
            if (scheme == null || scheme.Length < 3)
                return false;

            char c = scheme[0];
            if (c == 'h')
                return (scheme == "http" || scheme == "https");
            if (c == 'f')
                return (scheme == "file" || scheme == "ftp");

            if (c == 'n')
            {
                c = scheme[1];
                if (c == 'e')
                    return (scheme == "news" || scheme == "net.pipe" || scheme == "net.tcp");
                if (scheme == "nntp")
                    return true;
                return false;
            }
            if ((c == 'g' && scheme == "gopher") || (c == 'm' && scheme == "mailto"))
                return true;

            return false;
        }
    }
}
