using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegratedNormalAttackParts : IntegratedPartsController, PartConnectToUI_IF
{
    public Sprite GetThisPartIcon()
    {
        return PartIconSprite;
    }
    public void SetPartIcon(Sprite sprite)
    {
        PartIconSprite = sprite;
    }
    /// <summary>
    /// 武装UIにパラメータを渡す用のメソッド
    /// </summary>
    public (bool, float) GetActionProperty()
    {
        return (GetInRange(),GetNowChargeTime());
    }
    public bool GetInRange()
    {
        if(ChildAttackUnitList?.Count == 0){return false;}
        foreach (var AU in ChildAttackUnitList)
        {
            if(AU==null){continue;}
            if (AU.InRange()) { return true; }
        }
        return false;
    }
    public float GetNowChargeTime()
    {
        if(ChildAttackUnitList?.Count == 0){return 0;}
        foreach (var AU in ChildAttackUnitList)
        {
            if (AU == null) { continue; }
            return AU.GetAttackTimeOffset();
        }
        return 0;
    }
    public float GetMaxChargeTime()
    {
        if(ChildAttackUnitList?.Count == 0){return 1;}
        foreach (var AU in ChildAttackUnitList)
        {
            if (AU == null) { continue; }
            return AU.GetAttackTimeOffset();
        }
        return 1;
    }
}
