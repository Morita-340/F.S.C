using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 機能確認のため、全てのAttackUnitにRocketBombが付いているパーツ
/// </summary>
public class NormalAttackParts : ReversibleConnectionPartsController, PartConnectToUI_IF
{
    public void SetPartIcon(Sprite sprite)
    {
        PartIconSprite = sprite;
    }
    protected override void CountNOFU()
    {
        if (initialFlag)
        {
            numOfInitialFunctionUnit = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<WeaponUnitBase>())
                {
                    numOfInitialFunctionUnit++;
                }
            }
            numOfFunctionUnit = numOfInitialFunctionUnit;
        }
        else
        {
            numOfFunctionUnit = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<WeaponUnitBase>())
                {
                    numOfFunctionUnit++;
                }
            }
        }
    }
    public Sprite GetThisPartIcon()
    {
        return PartIconSprite;
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
        if (ChildAttackUnitList?.Count == 0) { return false; }
        foreach (var AU in ChildAttackUnitList)
        {
            if (AU == null) { continue; }
            if (AU.InRange()) { return true; }
        }
        return false;
    }
    public float GetNowChargeTime()
    {
        if (ChildAttackUnitList?.Count == 0) { return 0; }
        foreach (var AU in ChildAttackUnitList)
        {
            if (AU == null) { continue; }
            return AU.GetAttackTimeOffset();
        }
        return 0;
    }
    public float GetMaxChargeTime()
    {
        if (ChildAttackUnitList?.Count == 0) { return 1; }
        foreach (var AU in ChildAttackUnitList)
        {
            if (AU == null) { continue; }
            return AU.GetAttackTimeOffset();
        }
        return 1;
    }
    //public override GameObject DuplicateParentOnly(Vector3 basePosition, Transform parentTransform)
    //{
    //    GameObject newParent = base.DuplicateParentOnly(basePosition, parentTransform);
    //    NormalAttackParts NAP = newParent.AddComponent(GetType());
    //    NAP = gameObject.GetComponent(GetType());
    //    return newParent;
    //}
}
