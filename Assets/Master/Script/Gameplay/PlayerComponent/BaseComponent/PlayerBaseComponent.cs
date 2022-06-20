using UnityEngine;

public abstract class PlayerBaseComponent<TControllerClass> : MonoBehaviour 
    where TControllerClass : MonoBehaviour
{
    [Header("Base Config:")]
    [SerializeField] private float m_updateInterval;
    [SerializeField] protected bool m_isDebug;
    [SerializeField] private TControllerClass m_ownerController;

    public TControllerClass OwnerController { get; private set; }

    private float _updateTimer;

    private void Awake()
    {
        if (!m_ownerController)
        { 
            OwnerController = GetComponent<TControllerClass>();
            
            if (!OwnerController)
                OwnerController = GetComponentInParent<TControllerClass>();

            m_ownerController = OwnerController;
        }
        else
            OwnerController = m_ownerController;

        _updateTimer = m_updateInterval;
        
        InitComponent();
    }

    protected virtual void InitComponent() { }
    
    protected virtual void Update()
    {
        if (_updateTimer > 0)
            _updateTimer -= Time.deltaTime;
        else
        {
            IntervalUpdate(Time.deltaTime);
            _updateTimer = m_updateInterval;
        }    
    }

    protected virtual void IntervalUpdate(float deltaTime) { }
}
