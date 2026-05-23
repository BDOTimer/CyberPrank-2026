using UnityEngine;

public class FansControl : MonoBehaviour
{
    [SerializeField] private Fan[] fans;

    public void SetSpeed(float speedPercent)
    {
        if (fans == null) return;

        for(int i = 0; i < fans.Length; i++)
        {   fans[i].SetSpeed(speedPercent);
        }
    }
}
