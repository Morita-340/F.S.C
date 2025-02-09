using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GeneralUIIconController : MonoBehaviour , IPointerEnterHandler,IPointerExitHandler
{
    protected bool CursolSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Debug.Log(CursolSelected);
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        CursolSelected = true;
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        CursolSelected = false;
    }
}
