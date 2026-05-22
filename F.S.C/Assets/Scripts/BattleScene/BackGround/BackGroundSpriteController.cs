using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundSpriteController : MonoBehaviour
{
    [SerializeField,Range(0,10)]
    float spriteDepth = 0;
    [SerializeField]
    Camera MainCamera;
    RectTransform uiRectTransform;
    Vector2 screenPosition;
    Vector3 InstPosition;
    // Start is called before the first frame update
    void Start()
    {
        uiRectTransform = GetComponent<RectTransform>();
        InstPosition = uiRectTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        screenPosition = Camera.main.WorldToScreenPoint(MainCamera.transform.position);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            uiRectTransform, 
            screenPosition, 
            Camera.main, 
            out Vector3 worldaPoint
        );
        uiRectTransform.anchoredPosition = InstPosition -worldaPoint *(1 + spriteDepth);
    }
}
