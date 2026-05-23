using UnityEngine;

public class CyberCup : MonoBehaviour, IInteractable
{
    [SerializeField] private Light _light;

    public void OnInteract(float hitDistance)
    {
        _light.enabled = !_light.enabled;
    }

    public void OnInteractCanceled()
    {
    }
}
