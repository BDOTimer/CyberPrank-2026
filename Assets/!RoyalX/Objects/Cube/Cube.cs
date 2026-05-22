using UnityEngine;

public class Cube : MonoBehaviour, IInteractable
{
    public void OnInteract(float hitDistance)
    {
        Destroy(gameObject);
    }

    public void OnInteractCanceled()
    {
        // No action needed 
    }
}
