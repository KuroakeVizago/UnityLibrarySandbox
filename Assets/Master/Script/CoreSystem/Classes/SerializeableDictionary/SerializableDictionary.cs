using UnityEngine;
using UnityEngine.Serialization;

namespace System.Collections.Generic
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {

        [SerializeField] public List<TKey> m_keys = new List<TKey>();
        [SerializeField] public List<TValue> m_values = new List<TValue>();

        public void OnBeforeSerialize()
        {
            m_keys.Clear();
            m_values.Clear();
            foreach (var keyValuePair in this)
            {
                m_keys.Add(keyValuePair.Key);
                m_values.Add(keyValuePair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            if (m_keys.Count != m_values.Count)
                Debug.LogError("Attempt to deserialize dictionary, but keys length doesn't match value length" + 
                               "\n Keys Length: " + m_keys.Count + 
                               "\n Values Length: " + m_values.Count);

            for (int i = 0; i < m_keys.Count; i++)
            {
                Add(m_keys[i], m_values[i]);
            }
        }
    }
}
