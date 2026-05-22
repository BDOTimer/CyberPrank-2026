using UnityEngine;

public class DesktopPC : MonoBehaviour
{
    [SerializeField] private Fan[] fans;
    void Start()
    {
        SetFanSpeed(5f);
    }
    public void SetFanSpeed(float percent)
    {
        foreach (var fan in fans)
            fan.SetSpeed(percent);
    }
}
