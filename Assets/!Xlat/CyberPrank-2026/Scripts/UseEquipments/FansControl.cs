using UnityEngine;

public class FansControl : MonoBehaviour
{
    [SerializeField] private Fan[] fans;

    public void SetSpeed(FanSpeed mode)
    {
        if (fans == null) return;

        for(int i = 0; i < fans.Length; i++)
        {   fans[i].SetSpeed(mode);
        }
    }
}
