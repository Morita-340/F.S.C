using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;
/// <summary>
/// DownerUnitのみを他パーツとのジョイント部分とする。
/// </summary>
public class JointUnit : UnitBase
{
    public float GetOffsetRotation()
    {
        return offsetRotation + transform.localRotation.eulerAngles.z;
    }
    protected override UnitBase GetUpLink(string SelectedObjTag)
    {
        UnitBase upperUnit;
        GameObject UpperObj = GetUpperGameObject(SelectedObjTag);
        if (UpperObj == null) { return null; }
        upperUnit = UpperObj.GetComponent<UnitBase>();
        if (upperUnit == null){ Debug.LogAssertion("UpperUnit is null;");}
        return upperUnit;
    }
    protected override UnitBase GetDownLink(string SelectedObjTag)
    {
        UnitBase downerUnit;
        GameObject DownerObj = GetDownerGameObject(SelectedObjTag);
        if (DownerObj == null) { return null; }
        downerUnit = DownerObj.GetComponent<UnitBase>();
        if (downerUnit == null) {Debug.LogAssertion("DownerUnit is null;");}
        return downerUnit;
    }
    protected override UnitBase GetRightLink(string SelectedObjTag)
    {
        UnitBase rightUnit;
        GameObject RightObj = GetRightGameObject(SelectedObjTag);
        if (RightObj == null) { return null; }
        rightUnit = RightObj.GetComponent<UnitBase>();
        if (rightUnit == null){ Debug.LogAssertion("RightUnit is null;");}
        return rightUnit;
    }
    protected override UnitBase GetLeftLink(string SelectedObjTag)
    {
        UnitBase leftUnit;
        GameObject LeftObj = GetLeftGameObject(SelectedObjTag);
        if (LeftObj == null) { return null; }
        leftUnit = LeftObj.GetComponent<UnitBase>();
        if (leftUnit == null) {Debug.LogAssertion("LeftUnit is null;");}
        return leftUnit;
    }
    protected override void GetAdjacentObjLink(string SelectedObjTag)
    {
        base.GetAdjacentObjLink(SelectedObjTag);
        //ReturnFourWayLink()[1]がDownerUnitに当たる
        AbstractPartsController jointAnotherParts = ThisUnitData.ReturnFourWayLink()[1]?.ReturnThisUnit().GetAPC();
        //DownerUnitが同じパーツだった場合にエラーメッセージ
        if (transform.parent.GetComponent<AbstractPartsController>() == jointAnotherParts)
        {
            GSetting.RefineDebugAssertinLog(ThisUnitData.ReturnFourWayLink()[1].ReturnThisUnit().transform, "ジョイント箇所なのに同じパーツと合体している");
        }
    }
    /// <summary>
    /// 大破ユニットを消去した後にリンクを取り直して更新する
    /// </summary>
    public void ReloadLink()
    {
        GetAdjacentObjLink(tag);
    }
    public bool isJointLinkEmpty()
    {
        AbstractPartsController jointAnotherParts = ThisUnitData.ReturnFourWayLink()[1]?.ReturnThisUnit().GetAPC();
        if (jointAnotherParts == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// ActiveJointUnitが合体する場所を起点とするため起点座標を計算して渡す
    /// </summary>
    /// <returns></returns>
    public Vector3 GetEmptyLinkPosition()
    {
        return transform.position + (Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, offsetRotation)) * Vector3.down);
    }
    //ジョイントユニットは合体場所が必ずユニットの下側（ユニットスプライトのコネクタ部分が露出している部分）なので[1]
    public AbstractPartsController GetJointedAnotherPart()
    {
        AbstractPartsController jointAnotherParts = ThisUnitData.ReturnFourWayLink()[1]?.ReturnThisUnit().GetAPC();
        List<UnitData> a = ThisUnitData.ReturnFourWayLink();
        return jointAnotherParts;
    }
}
