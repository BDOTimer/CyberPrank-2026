using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] private float maxRPM = 2000f;
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private Transform rotor;

    private float currentSpeed = 0f;
    private float targetSpeed = 0f;

    // Update is called once per frame
    void Update()
    {
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            Time.deltaTime / accelerationTime
        );
        // 1 RPM = 360 degrees per minute = 6 degrees per second
        rotor.Rotate(Vector3.up, 6f * currentSpeed * maxRPM * Time.deltaTime, Space.Self);
    }

    public void SetSpeed(float percent)
    { 
        targetSpeed = Mathf.Clamp01(percent / 100f);
    }
}
