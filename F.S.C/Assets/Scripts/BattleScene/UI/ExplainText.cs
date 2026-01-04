using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExplainText : MonoBehaviour
{
    TextMeshProUGUI TextObj;
    RectTransform rectTransform;
    void Start()
    {
        TextObj = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetText(string text, Vector2 pos)
    {
        TextObj.text = text;
        rectTransform.anchoredPosition = pos;
    }
}
