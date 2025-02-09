using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponUpButton : GeneralUIIconController
{
    [SerializeField]
    PlayerSettingManager PSM;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        PSM.ChangeWeaponIndex(true);
    }
}
