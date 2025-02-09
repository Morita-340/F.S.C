using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyUpButton : GeneralUIIconController
{
    [SerializeField]
    PlayerSettingManager PSM;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        PSM.ChangeBodyIndex(true);
    }
}
