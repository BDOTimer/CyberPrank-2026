using UnityEngine;
public class RockingChair : MonoBehaviour
{
    private AudioSource audioSource;

    private float speed = 2f;
    private float maxForward = 15f;
    private float maxBack = -4f;
    private float _time;
    private bool isRocking = false;
    private bool isStopping = false;
    private float previousAngle = 0f;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        if (isRocking || isStopping)
        {
            _time += Time.deltaTime * speed;
            float t = (Mathf.Sin(_time) + 1f) / 2f;
            float angle = Mathf.Lerp(maxBack, maxForward, t);
            ApplyAngle(angle);

            if (isStopping)
            {
                bool crossedZero = (previousAngle < 0f && angle >= 0f) ||
                                   (previousAngle > 0f && angle <= 0f);
                if (crossedZero)
                {
                    ApplyAngle(0f);
                    isRocking = false;
                    isStopping = false;
                    audioSource.Stop();
                }
            }

            previousAngle = angle;
        }
    }

    private void ApplyAngle(float angle)
    {
        Vector3 angles = transform.localEulerAngles;
        angles.x = angle;
        transform.localEulerAngles = angles;
    }

    public void StartRocking()
    {
        if (!isRocking && !isStopping)
        {
            isRocking = true;
            audioSource.Play();
        }
    }

    public void StopRocking()
    {
        if (isRocking)
        {
            isRocking = false;
            isStopping = true;
        }
    }

    public bool IsRocking()
    {
        return isRocking || isStopping;
    }
}