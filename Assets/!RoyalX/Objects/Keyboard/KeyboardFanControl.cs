using System;
using UnityEngine;

public class KeyboardFanControl : MonoBehaviour, IInteractable
{
    [SerializeField] private DesktopPC desktopPC;
    void Start()
    {
        desktopPC.SetFanSpeed(FanSpeed.Low);
    }

    public void OnInteract(float hitDistance)
    {
        SwitchToNextSpeed();
    }

    public void OnInteractCanceled()
    {
    }

    private void SwitchToNextSpeed()
    {
        var values = (FanSpeed[])Enum.GetValues(typeof(FanSpeed));
        desktopPC.SetFanSpeed(values[((int)desktopPC.GetFanSpeed() + 1) % values.Length]);
    }
}
