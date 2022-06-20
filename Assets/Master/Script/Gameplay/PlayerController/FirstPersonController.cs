using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vizago;
using Vizago.SaveSystem;

[RequireComponent(typeof(PlayerMovementComponent))]
public class FirstPersonController : MonoBehaviour, ISavableComponent
{

    [Header("Component Config:")]
    [SerializeField] private Camera m_playerCamera;

    public PlayerMovementComponent MovementComponent { get; private set; }

    public Vector3 PlayerForwardDirection => transform.TransformDirection(Vector3.forward);
    
    private void Awake()
    {
        MovementComponent = GetComponent<PlayerMovementComponent>();
    }

    public object Save()
    {
        TestData testData = new TestData(transform.position);
        
        return testData;
    }

    public void Load(string savedJsonData)
    {
        var data = JsonUtility.FromJson<TestData>(savedJsonData); 
        transform.position = new Vector3(data.Position.x, data.Position.y, data.Position.z);
    }

    [System.Serializable]
    public struct TestData
    {
        public Vector3 Position;

        public TestData(Vector3 positions)
        {
            Position = positions;
        }
        
    }

}

enum ControllerType
{
    FirstPerson, ThirdPerson
}
    
