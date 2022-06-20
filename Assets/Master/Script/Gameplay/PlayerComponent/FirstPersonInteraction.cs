using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Vizago.InputManager;

public sealed class FirstPersonInteraction : PlayerBaseComponent<FirstPersonController>
{

    /// =======================
    /// Serialized Member Field
    /// =======================
    
    [Header("Config:")] [SerializeField] private Camera m_playerCamera;
    [SerializeField] private float m_interactionRange = 2.5f;
    
    [Space]
    
    [InfoBox("Don't change Max Object In Detection value in runtime !", EInfoBoxType.Warning)]
    [SerializeField, Min(1), MaxValue(20)] private int m_maxObjectsDetection = 10; 
    [SerializeField] private LayerMask m_targetObjectsLayer;
    
    [Space]
    
    [Header("Events:")]
    [SerializeField] public UnityEvent m_onInteract;
    [SerializeField] public UnityEvent m_onFocused;
    [SerializeField] public UnityEvent m_endFocused;
    
    /// ===================
    /// Public Member Field
    /// ===================

    public IInteractableAction FocusedObject { get; private set; }
    
    public bool IsOwnerMoving => OwnerController.MovementComponent.IsMoving;

    /// ====================
    /// Private Member Field
    /// ====================
    
    private Collider[] _collidersInRange;
    private readonly List<IInteractableAction> _objectsInRange = new List<IInteractableAction>();
    private readonly List<IInteractableAction> _objectsInSight = new List<IInteractableAction>();
    protected override void InitComponent()
    {
        base.InitComponent();

        GameplayInputManager.Instance.CharacterInput.Interact.performed += inputContext => { Interact(); };

        if (!m_playerCamera)
            m_playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        _collidersInRange = new Collider[m_maxObjectsDetection];

    }

    protected override void Update()
    {
        base.Update();

        if (IsOwnerMoving)
        {
            if (GetObjectsInRange())
            {

                if (GetObjectsInSight())
                {
                    IInteractableAction cacheFocusedObject;

                    //If getting there are no raycast object than it will search the closest object in sight
                    if ((cacheFocusedObject = GetRaycastObject()) == null)
                        cacheFocusedObject = GetClosestObjectInSight();
                    
                    if (cacheFocusedObject != FocusedObject && cacheFocusedObject != null)
                    {
                        if (FocusedObject != null)
                        {
                            m_endFocused?.Invoke();
                            FocusedObject.EndFocused();
                        }

                        FocusedObject = cacheFocusedObject;
                        m_onFocused?.Invoke();
                        FocusedObject.OnFocused();
                      
                    }
                }
                else
                {
                    _objectsInSight.Clear();
                    ResetFocusedObject();
                }
            }
            else
            {
                _objectsInRange.Clear();
                ResetFocusedObject();
            }
        }
    }

    /// <summary>
    /// Call this to interact with a focused Object
    /// </summary>
    public void Interact()
    {
        if (FocusedObject != null)
        {
            FocusedObject.OnInteract(OwnerController);
            m_onInteract?.Invoke();
        }
    }

    private void ResetFocusedObject()
    {
        if (FocusedObject == null) return;

        m_endFocused?.Invoke();
        FocusedObject.EndFocused();
        FocusedObject = null;
    }
    
    /// <summary>
    /// Get Objects that in line of sight
    /// </summary>
    /// <returns> Whether there is objects in sight or not </returns>
    private bool GetObjectsInSight()
    {
        _objectsInSight.Clear();

        foreach (var obj in _objectsInRange)
        {
            var directionToObject = obj.transform.position - OwnerController.transform.position;
            if (Vector3.Dot(OwnerController.PlayerForwardDirection, directionToObject) > 0)
                _objectsInSight.Add(obj);
        }

        return _objectsInSight.Count > 0;

    }

    /// <summary>
    /// Get Objects in range that will be stored at ObjectsInRange variable
    /// </summary>
    /// <returns> Whether there is objects in range or not </returns>
    private bool GetObjectsInRange()
    {
        _objectsInRange.Clear();
        
        var detectedColliders = Physics.OverlapSphereNonAlloc(OwnerController.transform.position, m_interactionRange, 
            _collidersInRange, m_targetObjectsLayer);

        if (detectedColliders > 0)
        {
            foreach (var objInRange in _collidersInRange)
            {
                if (!objInRange) continue;
                if (objInRange.TryGetComponent(out IInteractableAction cacheObj))
                    _objectsInRange.Add(cacheObj);

            }
        }

        return _objectsInRange.Count > 0;
    }

    private IInteractableAction GetClosestObjectInSight()
    {

        IInteractableAction cacheClosestObject = null;

        var ownerPosition = OwnerController.transform.position;

        foreach (var obj in _objectsInSight)
        {

            if (cacheClosestObject == null)
                cacheClosestObject = obj;
            else
            {
                var distance1 = (obj.transform.position - ownerPosition).magnitude;
                var distance2 = (cacheClosestObject.transform.position - ownerPosition).magnitude;

                if (distance1 < distance2)
                    cacheClosestObject = obj;

            }
        }

        return cacheClosestObject;
    }

    private IInteractableAction GetRaycastObject()
    {

        var cameraForward = m_playerCamera.transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(OwnerController.transform.position, cameraForward, out var hitInfo, m_interactionRange,
                m_targetObjectsLayer))
        {
            if (hitInfo.collider.TryGetComponent(out IInteractableAction interactableObject))
                return interactableObject;

        }

        return null;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_isDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_interactionRange);
        }   
    }
#endif
}

//      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠟⠛⢹⡿⠟⠛⠛⠉⢉⡽⣿⣿⣿⣿⣿⣿⡿⠿⠿⠿⣿⣿⣿⣿⣿⣿⡿⠿⠿⠿⠛⠛⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠉⠁⠁
//      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⡿⠛⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠉⠁⠀⠀⠀⠀⠀⠀⠛⠉⠉⠀⠀⠀⠀⠀⠀⠀⠀⠐⠛⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠀⠀⠀⠀
//      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠋⠈⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⢉⣿⣿⡿⣿⣿⣿⣿⣿⣿⣿⠏⠀⠀⠀⠀⠀
//      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⢿⡿⣿⣿⣿⣿⣿⣿⣿⣿⡏⠀⠀⠀⠀⠀⠀
//      ⣿⣿⣿⣿⣿⣿⣿⣿⣿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⣿⣿⣿⣯⣻⢿⣻⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀
//      ⣿⣿⣿⣿⣿⣿⣿⣿⡟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣶⣫⣿⣿⣿⣿⣿⣻⣾⢼⣿⢿⣿⢷⡇⠀⠀⠀⠀⠀⠀⠀
//      ⣿⣿⣿⣿⣿⣿⣿⣿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⢉⣿⢽⡯⣿⣿⣿⣿⣿⣿⡿⠋⠀⢸⡅⠀⠀⠀⠀⠀⠀⠀
//      ⣿⣿⣿⣿⣿⣿⣿⣿⠀⢠⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⣠⣿⣿⣿⣯⣻⣽⡯⣟⡿⣿⡆⠀⠀⠘⡇⠀⠀⠀⠀⠀⠀⠀
//      ⣿⣿⣿⣿⣿⣿⣿⣿⠀⠀⠙⠢⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣴⢿⣿⣿⣿⣿⡿⠿⣾⣿⣯⣗⡯⣿⣦⣶⣶⣾⣹⡀⠀⠀⠀⠀⠀⠀
//      ⣿⣿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠈⠓⢦⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠢⡀⠀⠀⠀⠀⠀⠀⠘⣹⣿⣿⣿⣿⣿⠏⠀⠀⠈⢻⢿⣷⣿⣿⣿⣻⣿⣿⣿⣿⣄⠀⠀⠀⠀⠀
//      ⣿⣿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠈⠛⠿⣷⣦⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⡀⠀⠰⡄⠀⠀⡴⣟⣿⠾⢿⣿⠿⠇⠀⠀⣠⠿⢿⠾⠛⠛⢿⡾⣿⣻⣿⡿⣿⣦⡀⠀⠀⠀
//      ⣿⣿⣿⣿⡿⠋⠉⢻⡄⠀⠀⠀⢀⣀⡀⠀⠀⠠⢄⣉⠛⠻⢿⣶⣦⣄⡀⠀⠀⠀⢀⡔⠂⠔⠀⠀⠀⢤⡀⠈⠳⣼⣏⣀⣀⣀⣋⣁⣀⣀⣀⣈⣁⣀⣀⣀⣀⡤⠾⠛⢛⣿⣿⠯⠿⢿⣿⣦⠀⠀
//      ⣿⣿⣿⣿⠁⣀⠀⠈⣇⠀⠐⠉⠁⣀⣀⣀⣀⣀⣀⣀⣀⠀⠀⠈⠙⣻⣿⣷⣶⣶⠋⠀⠀⠀⠀⠀⠀⠀⠈⠑⠀⠈⠻⠿⠿⠿⠿⠿⠿⠿⠿⠟⠛⠛⠉⠉⠀⠀⠀⠀⠘⣁⣠⣤⣶⣾⣭⣄⡀⠀
//      ⣿⣿⣿⡷⠊⡹⠳⡀⣿⠀⠀⣨⣿⣿⠿⠿⢟⣿⣿⣿⣿⣿⣿⣶⣦⣌⡑⠪⡉⡁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠤⠤⠀⠀⠤⠤⠤⠤⠤⠤⢄⡀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣧⡀
//      ⣿⣿⣿⡇⢀⣇⠀⢹⣿⡇⠀⠊⠁⠀⠀⠀⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣄⠙⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠔⠔⢋⣀⣤⣴⣶⣶⣶⣶⣶⣦⣤⣄⣈⠉⠑⠀⠀⠀⠀⠀⠀⣾⣿⣿⣟
//      ⣿⣿⣿⡇⣎⠈⠦⣿⣿⣿⠀⠀⠀⠀⠀⠀⢷⢈⣿⣿⡏⠀⠙⣿⣿⣿⣿⠛⢿⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣾⣿⠿⣿⣿⣿⣿⡿⣿⣿⣿⣿⣿⡿⠿⣦⡀⠀⠀⠀⠀⢠⣿⣿⣿⡏
//      ⣿⣿⣿⢿⠘⣖⢲⣿⣿⣿⡇⠀⠀⠀⠀⠀⠈⢯⡹⢿⣿⣶⡿⠟⠉⢹⡏⠀⠀⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠞⠉⠀⠐⣿⠻⣿⣿⡀⠀⣉⣿⡿⠿⡿⠀⠈⠁⠀⠀⠀⠀⣼⣿⣿⣿⡇
//      ⣿⣿⣿⣟⡆⠈⢺⣿⣿⣿⣿⡄⠀⠈⠳⢤⣀⠀⠙⠦⢤⣁⣀⡤⠔⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣾⠟⠻⣿⡿⡿⠉⢁⡴⠃⠀⠀⠀⠀⠀⠀⢰⣿⣿⣿⣿⡇
//      ⣿⣿⣿⣿⣿⣄⣼⣿⣿⡿⠁⢳⠀⠀⠀⠀⠈⠙⠲⠶⠤⠤⠤⠤⠤⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠓⢦⣥⠥⠤⠒⠈⠀⢀⣠⠖⠀⠀⠀⢠⣿⠏⢿⣿⣿⣇
//      ⣿⣿⣿⣿⣿⣿⣿⡿⠋⠳⣄⡘⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠲⠦⣤⣤⣤⣤⣤⠄⠚⠉⠁⠀⠀⠀⢀⣾⣿⠞⢧⢻⣿⣿
//      ⣿⣿⣿⣯⣿⣿⣽⣧⡀⠀⠀⠉⢹⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣶⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⣯⠧⠤⠚⠉⠙⠻
//      ⣯⣭⣿⣿⣿⣿⣿⣗⣿⣦⡰⣿⠂⢷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⠏⣀⠀⠀⠀⠀⣠⣾
//      ⣻⢝⡻⡻⢻⣟⢟⣿⣿⣿⢟⣿⠦⠬⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⠏⠘⡟⢀⣠⣶⣿⣿⣿
//      ⠁⠀⠀⣤⡤⠶⠿⠛⠋⠁⠀⣿⢀⣤⠘⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⢌⢣⠀⠀⠐⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⢛⡛⢒⡟⠋⠹⣿⣿⣿⣿
//      ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣴⣾⣿⢻⠀⢘⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⡇⢸⢿⣿⣇⠀⠀⠙⣿⣿⣿
//      ⠀⠀⠀⠀⠀⠀⠀⠀⠈⡿⣽⣿⣿⢾⠀⣾⣿⢳⡀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⣤⣤⣤⣄⣀⣀⠀⡀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣼⣿⡇⣾⣾⣿⣿⡗⠀⠀⠈⠻⣿
//      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⡷⢺⣿⣟⢻⢸⣿⣿⡀⢷⣄⠀⠀⠀⠀⠀⠀⠀⠸⣯⢿⣦⣤⣀⣀⠀⠀⠈⠉⠉⠉⠙⣻⣷⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣾⠃⢻⣇⣿⣾⣿⣏⡇⠀⠀⠀⠀⠈
//      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⣷⢻⢻⡏⢿⣿⣿⣿⡇⠘⣿⣷⣄⠀⠀⠀⠀⠀⠀⠈⠛⠺⠭⣙⣛⠛⠛⠛⠛⠛⠛⢛⣋⣼⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠞⣻⡏⢧⠀⢹⡷⣿⣿⢹⠃⠀⠀⠀⠀⠀
//      ⠆⠀⠀⣀⣠⠤⠖⠒⠒⡇⡎⢸⠸⢸⣿⣿⣿⣿⣄⠘⡽⣿⣷⣄⠀⠀⠀⠀⠀⠀⠀⠀⠠⠬⠅⠈⡉⠉⠉⠉⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⠞⠁⢠⣿⠁⢸⡀⢸⣱⣻⢻⣿⡀⠀⠀⠀⠀⠀
//      ⣶⠖⠋⠁⠀⠀⠀⠀⠀⣷⠁⢸⠀⢿⣿⣿⣿⣿⣿⣆⠘⢞⠻⡿⠳⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⠶⠋⠀⠀⢀⡾⢋⠄⣾⡇⢸⠃⡇⢸⣿⠀⠀⠀⠀⠀⠀
//      ⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠴⠛⠛⢻⣿⣿⣿⣿⣿⣿⣷⡄⢐⠮⡲⣼⣟⠲⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠴⠞⠋⠀⠀⠀⢀⡴⢟⡵⠁⣴⣿⣇⡟⠶⣇⠸⡇⠀⠀⠀⠀⠀⠀
//      ⠀⠀⠀⠀⠀⠀⠀⠀⢀⡏⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣦⡑⢬⡛⢿⣷⡀⠉⠓⠦⣄⣀⠀⠀⠀⠀⠀⣀⣠⠴⠞⠋⠀⠀⠀⠀⠀⠀⠔⢋⡠⠍⣠⣾⣿⣿⠀⡇⠀⠈⢷⣇⠀⠀⠀⠀⠀⠀
//      ⠀⠀⠀⠀⠀⠀⢀⣴⣿⡇⠀⠀⢀⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣍⠲⠭⡛⠶⢤⣄⣀⡉⠙⠛⠛⠛⠉⠁⠀⠀⢀⣀⣠⠤⠖⢚⣉⠤⠂⣉⣤⣾⣿⣿⣿⣿⣄⠃⠀⠀⢸⠉⠙⠒⠶⠤⣤⣀
//      ⠀⠀⠀⠀⣀⣴⣿⣿⣿⣷⣶⣾⡟⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣤⣈⡑⠓⠠⡉⢹⡖⠒⢛⣙⣋⣉⣉⡩⠅⠀⠀⣉⣡⣤⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣄⠀⢸⠀⠀⠀⠀⠀⠀⠈
//      ⠀⠀⣠⣶⣿⣿⣿⣿⣿⣽⣷⣿⣿⡼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡆⢹⢸⡇⠀⣤⣤⣤⣤⣤⣴⣶⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣯⠻⣦⡾⠀⠀⠀⠀⠀⠀⠀⠀
//
//      ██╗  ██╗ ██████╗ ██╗  ██╗ ██████╗ ██████╗  ██████╗     ██╗    ██╗ ██████╗     ███╗   ███╗ ██████╗ ██╗   ██╗ █████╗ ███████╗███████╗    ██╗██╗██╗
//      ██║ ██╔╝██╔═══██╗██║ ██╔╝██╔═══██╗██╔══██╗██╔═══██╗    ██║    ██║██╔═══██╗    ████╗ ████║██╔═══██╗╚██╗ ██╔╝██╔══██╗██╔════╝██╔════╝    ██║██║██║
//      █████╔╝ ██║   ██║█████╔╝ ██║   ██║██████╔╝██║   ██║    ██║ █╗ ██║██║   ██║    ██╔████╔██║██║   ██║ ╚████╔╝ ███████║███████╗█████╗      ██║██║██║
//      ██╔═██╗ ██║   ██║██╔═██╗ ██║   ██║██╔══██╗██║   ██║    ██║███╗██║██║   ██║    ██║╚██╔╝██║██║   ██║  ╚██╔╝  ██╔══██║╚════██║██╔══╝      ╚═╝╚═╝╚═╝
//      ██║  ██╗╚██████╔╝██║  ██╗╚██████╔╝██║  ██║╚██████╔╝    ╚███╔███╔╝╚██████╔╝    ██║ ╚═╝ ██║╚██████╔╝   ██║   ██║  ██║███████║███████╗    ██╗██╗██╗
//      ╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═╝ ╚═════╝      ╚══╝╚══╝  ╚═════╝     ╚═╝     ╚═╝ ╚═════╝    ╚═╝   ╚═╝  ╚═╝╚══════╝╚══════╝    ╚═╝╚═╝╚═╝