using System.Collections.Generic;

namespace JdWool.Service
{
    public class DictionaryCache
    {
        private readonly Dictionary<string, string> keyValues = new();

        public string GetValue(string key)
        {
            if (key.Contains(key))
                return keyValues[key];
            return string.Empty;
        }

        public void SetValue(string key, string value)
        {
            if (key.Contains(key))
                keyValues[key] = value;
            else
                keyValues.Add(key, value);
        }

        public bool RemoveValue(string key)
        {
            return keyValues.Remove(key);
        }

    }
}
