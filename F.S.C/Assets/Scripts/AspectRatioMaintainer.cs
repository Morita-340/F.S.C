using UnityEngine;

public class AspectRatioMaintainer : MonoBehaviour
{
    [SerializeField,ReadOnly] float targetAspect = 16.0f / 9.0f;

    void Start()
    {
        // アスペクト比を固定
        int width = Screen.width;
        int height = (int)(width / targetAspect);
        Screen.SetResolution(width, height, false);
    }

    void Update()
    {
        // ウィンドウサイズが変更された場合、アスペクト比を維持する
        int width = Screen.width;
        int height = Screen.height;
        float currentAspect = (float)width / height;

        if (Mathf.Abs(currentAspect - targetAspect) > 0.01f)
        {
            height = (int)(width / targetAspect);
            Screen.SetResolution(width, height, false);
        }
    }
}