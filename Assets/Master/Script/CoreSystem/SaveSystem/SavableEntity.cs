using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Vizago.SaveSystem
{
    public class SavableEntity : MonoBehaviour
    {
        [SerializeField] protected string m_entityId;

        public string EntityId => m_entityId;
        
        [Button("Generate Id")]
        public void GenerateId() { m_entityId = System.Guid.NewGuid().ToString(); }
        
        public SerializableDictionary<string, string> SaveEntity()
        {
            var entityData = new SerializableDictionary<string, string>();
            
            foreach (var savable in GetComponents<ISavableComponent>())
            {
                entityData[savable.GetType().ToString()] = JsonUtility.ToJson(savable.Save());
            }
            
            return entityData;
        }

        //bndvjonsouidvn
        public void LoadEntity(SerializableDictionary<string, string> entityData)
        {
            foreach (var savable in GetComponents<ISavableComponent>())
            {
                if (entityData.TryGetValue(savable.GetType().ToString(), out var data))
                {
                    savable.Load(data);
                }
            }
        }
        
    }
}
