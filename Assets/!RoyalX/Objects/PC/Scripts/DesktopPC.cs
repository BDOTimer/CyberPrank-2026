using UnityEngine;

public class DesktopPC : MonoBehaviour
{
    [SerializeField] private Fan[] fans;
    [SerializeField] private AudioSource fanAudioSource;

    private FanSpeed _currentSpeed;
 
    public void SetFanSpeed(FanSpeed mode)
    {
        _currentSpeed = mode;
        foreach (var fan in fans)
            fan.SetSpeed(mode);
        fanAudioSource.volume = mode switch
        {
            FanSpeed.Off => 0f,
            FanSpeed.Low => 0.05f,
            FanSpeed.Medium => 0.2f,
            FanSpeed.High => 1f,
            _ => fanAudioSource.volume
        };
    }

    public FanSpeed GetFanSpeed() => _currentSpeed;
}
