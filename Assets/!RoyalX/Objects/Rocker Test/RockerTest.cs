using UnityEngine;

public class RockerTest : MonoBehaviour, IInteractable
{
    [SerializeField] private RockingChair rockingChair;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rockingChair.StartRocking();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract(float hitDistance)
    {
        if (rockingChair.IsRocking())
        {
            rockingChair.StopRocking();
        }
        else
        {
            rockingChair.StartRocking();
        }
    }

    public void OnInteractCanceled()
    {
    }
}
