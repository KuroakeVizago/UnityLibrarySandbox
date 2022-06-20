using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using Vizago.IO;
using Vizago.Security;

namespace Vizago.SaveSystem
{
    public sealed class SaveSystemManager : MonoBehaviour
    {

        #region Serialize Property

        //[SerializeField] private bool m_useEncryption = true;
        [SerializeField] private string m_encryptionPassword = "76a54dcf14ae41d49bc1a5c00bf24b34";

        #endregion
        
        #region Singleton

        public static SaveSystemManager Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region SaveDataPath

#if !UNITY_EDITOR
        public string SavePath => $"{Application.persistentDataPath}/{Application.productName.Replace(" ", string.Empty)}SaveData.txt";
#else
        public string SavePath => $"{Application.dataPath}/{Application.productName.Replace(" ", string.Empty)}SaveData.txt";
#endif

        #endregion

        [Button("Generate Password", EButtonEnableMode.Editor)]
        public void GeneratePassword()
        {
            if (m_encryptionPassword.Length < 1)
                m_encryptionPassword = Guid.NewGuid().ToString("N");
        }

        [Button("Test Save Game", EButtonEnableMode.Playmode)]
        public void SaveGame()
        {
            // TODO: take prev data, update data, store data in file
            var currentSaveData = LoadObjectFromFile();
            SaveEntitiesData(ref currentSaveData);
            SaveObjectToFile(currentSaveData);
        }

        [Button("Test Load Game", EButtonEnableMode.Playmode)]
        public void LoadGame()
        {
            var savedData = LoadObjectFromFile();
            LoadEntitiesData(in savedData);
        }

        public void SaveObjectToFile(in SerializableDictionary<string, string> objectData)
        {
            try
            {
                // Parse the data into Json
                string dataInJson = JsonUtility.ToJson(objectData);
                
                //Encrypt the written data
                var encryptedData = AesEncryptor.EncryptStringToBytes(dataInJson, m_encryptionPassword);

                // Write the encrypted data into file
                using (Stream stream = File.Open(SavePath, FileMode.Create, FileAccess.Write))
                    using (var writer = new BinaryWriter(stream))
                        writer.Write(encryptedData);
                
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Console.WriteLine(e);
            }
        }

        public SerializableDictionary<string, string> LoadObjectFromFile()
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogError("Previous save file at: " + SavePath + " Not found !");
                // Return a new slot of Save Dictionary
                return new SerializableDictionary<string, string>();
            }

            try
            {
                using (var stream = File.Open(SavePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var jsonData = AesEncryptor.DecryptBytesToString(reader.ReadAllBytes(), m_encryptionPassword);
                        
                        return JsonUtility.FromJson<SerializableDictionary<string, string>>(jsonData);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Console.WriteLine(e);
                throw;
            } 
        }

        private static void SaveEntitiesData(ref SerializableDictionary<string, string> previousData)
        {
            foreach (var entity in FindObjectsOfType<SavableEntity>())
            {
                previousData[entity.EntityId] = JsonUtility.ToJson(entity.SaveEntity());
            } 
        }

        private static void LoadEntitiesData(in SerializableDictionary<string, string> savedData)
        {
            foreach (var entity in FindObjectsOfType<SavableEntity>())
            {
                if (savedData.TryGetValue(entity.EntityId, out var entityData))
                {
                    entity.LoadEntity(
                        JsonUtility.FromJson<SerializableDictionary<string, string>>(entityData)
                        );
                }
            }
        }
        
    }
}
