using UnityEngine;

public class WorldX : MonoBehaviour
{
    [Header("Скорость вращения скайбокса (градусов в секунду)")]
    public float   rotSpeedSkybox = 2.0f;

    void Update()
    {
        RotateSkybox();
    }

    void RotateSkybox()
    {
        // Получаем текущий угол поворота из материала скайбокса
        float currentRotation = RenderSettings.skybox.GetFloat("_Rotation");
        // Вычисляем новый угол, увеличивая его на скорость, умноженную на время кадра
        float d = rotSpeedSkybox * Time.deltaTime;
        float newRotation = currentRotation +  d;
        // Применяем новый угол обратно к материалу
        RenderSettings.skybox.SetFloat("_Rotation", newRotation);
    }
}
