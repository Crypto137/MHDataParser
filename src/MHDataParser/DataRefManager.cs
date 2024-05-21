using System.Collections;

namespace MHDataParser
{
    public class DataRefManager<T> : IEnumerable<KeyValuePair<T, string>> where T : Enum
    {
        private readonly Dictionary<T, string> _referenceDict = new();
        private readonly Dictionary<string, T> _reverseLookupDict;

        public int Count { get => _referenceDict.Count; }

        public DataRefManager(bool useReverseLookupDict)
        {
            // We can't use a dict for reverse lookup for all ref managers because some reference
            // types (e.g. assets) can have duplicate names
            if (useReverseLookupDict) _reverseLookupDict = new();
        }

        public void AddDataRef(T value, string name)
        {
            _referenceDict.Add(value, name);
            if (_reverseLookupDict != null) _reverseLookupDict.Add(name, value);
        }

        public T GetDataRefByName(string name)
        {
            // Try to use a lookup dict first
            if (_reverseLookupDict != null)
            {
                if (_reverseLookupDict.TryGetValue(name, out T dataRef) == false)
                    return default;

                return dataRef;
            }

            // Fall back to linear search if there's no dict
            foreach (var kvp in _referenceDict)
                if (kvp.Value == name) return kvp.Key;

            return default;
        }

        public string GetReferenceName(T dataRef)
        {
            if (_referenceDict.TryGetValue(dataRef, out string name) == false)
                return string.Empty;

            return name;
        }

        public IEnumerator<KeyValuePair<T, string>> GetEnumerator() => _referenceDict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
