using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCamera : MonoBehaviour
{
    private Camera _camera;

    private void OnEnable()
    {
        _camera = GetComponent<Camera>();
        // 第二引数に文字列を指定するとこのタグが一致するものパスだけを差し替える
        _camera?.SetReplacementShader(Shader.Find("Custom/OutlineMask"), "");
    }
    private void LateUpdate()
    {
        _camera.Render();
    }
    private void OnDisable()
    {
        _camera?.ResetReplacementShader();
    }
}
