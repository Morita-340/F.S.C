using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCamera : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] Camera MainCamera;

    private void OnEnable()
    {
        _camera = GetComponent<Camera>();
        // 第二引数に文字列を指定するとこのタグが一致するものパスだけを差し替える
        _camera?.SetReplacementShader(Shader.Find("Custom/OutlineMask"), "");
    }
    private void Update()
    {
        
    }
    private void LateUpdate()
    {
        this.transform.position = MainCamera.transform.position;
        _camera.Render();
    }
    private void OnDisable()
    {
        _camera?.ResetReplacementShader();
    }
}
