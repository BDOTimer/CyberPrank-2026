using UnityEngine;

public enum FanSpeed { Off, Low, Medium, High }

public class Fan : MonoBehaviour
{
    
    [SerializeField] private Transform rotor;
    [SerializeField] private Material motionEffectMaterial;


    private float maxSpeed = 1200f;
    private float accelerationTime = 2f;
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
        rotor.Rotate(Vector3.up, currentSpeed * maxSpeed * Time.deltaTime, Space.Self);
    }

    void OnDestroy()
    {
        SetAlpha(0f);
    }

    public void SetSpeed(FanSpeed mode)
    {
        switch (mode)
        {
            case FanSpeed.Off:
                targetSpeed = 0f;
                SetAlpha(0f);
                break;
            case FanSpeed.Low:
                targetSpeed = 0.5f;
                SetAlpha(0f);
                break;
            case FanSpeed.Medium:
                targetSpeed = 1f;
                SetAlpha(0.3f);
                break;
            case FanSpeed.High:
                targetSpeed = 1f;
                SetAlpha(0.6f);
                break;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = motionEffectMaterial.GetColor("_BaseColor");
        c.a = alpha;
        motionEffectMaterial.SetColor("_BaseColor", c);
    }
}
