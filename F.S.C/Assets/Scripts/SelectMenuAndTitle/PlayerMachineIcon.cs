using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMachineIcon : PushableIcon
{
    private BodyFlag bodyFlag;
    [SerializeField,ReadOnly]
    private PlayerMachineDisplayManager PMDM;
    public PlayerMachineIcon Initialize(BodyFlag inputBodyFlag, PlayerMachineDisplayManager inputPMDM)
    {
        bodyFlag = inputBodyFlag;
        PMDM = inputPMDM;
        return this;
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void IconPushedAccepted()
    {
        base.IconPushedAccepted();
        if (PMDM.SetSelectedBodyFlag(bodyFlag, GetComponent<RectTransform>().anchoredPosition + (Vector2)transform.parent.localPosition))
        {
            SCer.PlaySE(1);
        }
        else
        {
            SCer.PlaySE(2);
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        PMDM.SetPointedBodyFlag(bodyFlag);
    }
}
