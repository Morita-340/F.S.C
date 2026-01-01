using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExplainText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI TextObj;
    [SerializeField]
    RectTransform rectTransform;
    public void SetText(string text, Vector2 pos)
    {
        TextObj.text = text;
        rectTransform.anchoredPosition = pos;
    }
}
