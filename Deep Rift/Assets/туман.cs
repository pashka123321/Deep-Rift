using UnityEngine;

public class FogOnHeight : MonoBehaviour
{
    // Параметры для настройки тумана
    public float fogDensity = 0.05f;  // Плотность тумана
    public Color fogColor = Color.gray;  // Цвет тумана
    public float heightThreshold = 491f;  // Порог высоты для активации тумана

    private float defaultFogDensity;  // Стандартная плотность тумана до включения
    private Color defaultFogColor;  // Стандартный цвет тумана
    private bool fogEnabled = false;  // Флаг включения тумана

    void Start()
    {
        // Сохраняем начальные настройки тумана
        defaultFogDensity = RenderSettings.fogDensity;
        defaultFogColor = RenderSettings.fogColor;
        RenderSettings.fog = false;  // Туман выключен по умолчанию
    }

    void Update()
    {
        // Проверяем текущую высоту объекта
        if (transform.position.y < heightThreshold)
        {
            // Если высота ниже порога, включаем туман
            if (!fogEnabled)
            {
                EnableFog();
            }
        }
        else
        {
            // Если высота выше порога, выключаем туман
            if (fogEnabled)
            {
                DisableFog();
            }
        }
    }

    // Включение тумана
    void EnableFog()
    {
        RenderSettings.fog = true;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogColor = fogColor;
        fogEnabled = true;
    }

    // Выключение тумана
    void DisableFog()
    {
        RenderSettings.fog = false;
        RenderSettings.fogDensity = defaultFogDensity;
        RenderSettings.fogColor = defaultFogColor;
        fogEnabled = false;
    }
}
