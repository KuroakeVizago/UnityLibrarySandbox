using UnityEngine;

public interface IInteractableAction
{

    public GameObject gameObject { get; }

    public Transform transform { get; }

    public abstract void OnInteract(in FirstPersonController messenger);
    
    public abstract void OnFocused();

    public abstract void EndFocused();

}