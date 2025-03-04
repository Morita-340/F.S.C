using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GeneralUIIconController : MonoBehaviour , IPointerEnterHandler,IPointerExitHandler
{
    protected bool CursolSelected = false;
    [SerializeField,ReadOnly]protected SoundController SCer;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        SCer = GetComponent<SoundController>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        CursolSelected = true;
        SCer.PlaySE(0);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        CursolSelected = false;
    }
}
