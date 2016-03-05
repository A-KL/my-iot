using System.ComponentModel;

namespace System.Collections.Generic
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class DictionaryExtensions
    {
        public static void RemoveFromDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> removeCondition)
        {
            DictionaryExtensions.RemoveFromDictionary<TKey, TValue, Func<KeyValuePair<TKey, TValue>, bool>>(dictionary, (Func<KeyValuePair<TKey, TValue>, Func<KeyValuePair<TKey, TValue>, bool>, bool>)((entry, innerCondition) => innerCondition(entry)), removeCondition);
        }

        public static void RemoveFromDictionary<TKey, TValue, TState>(this IDictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, TState, bool> removeCondition, TState state)
        {
            int index1 = 0;
            TKey[] keyArray = new TKey[dictionary.Count];
            foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>)dictionary)
            {
                if (removeCondition(keyValuePair, state))
                {
                    keyArray[index1] = keyValuePair.Key;
                    ++index1;
                }
            }
            for (int index2 = 0; index2 < index1; ++index2)
                dictionary.Remove(keyArray[index2]);
        }

        public static bool TryGetValue<T>(this IDictionary<string, object> collection, string key, out T value)
        {
            object obj;
            if (collection.TryGetValue(key, out obj) && obj is T)
            {
                value = (T)obj;
                return true;
            }
            value = default(T);
            return false;
        }

        internal static IEnumerable<KeyValuePair<string, TValue>> FindKeysWithPrefix<TValue>(this IDictionary<string, TValue> dictionary, string prefix)
        {
            TValue exactMatchValue;
            if (dictionary.TryGetValue(prefix, out exactMatchValue))
                yield return new KeyValuePair<string, TValue>(prefix, exactMatchValue);
            foreach (KeyValuePair<string, TValue> keyValuePair in (IEnumerable<KeyValuePair<string, TValue>>)dictionary)
            {
                string key = keyValuePair.Key;
                if (key.Length > prefix.Length && key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    if (prefix.Length == 0)
                    {
                        yield return keyValuePair;
                    }
                    else
                    {
                        char charAfterPrefix = key[prefix.Length];
                        switch (charAfterPrefix)
                        {
                            case '.':
                            case '[':
                                yield return keyValuePair;
                                continue;
                            default:
                                continue;
                        }
                    }
                }
            }
        }
    }
}
