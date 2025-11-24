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
        return offsetRotation;
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
    public AbstractPartsController GetJointedAnotherPart()
    {
        AbstractPartsController jointAnotherParts = ThisUnitData.ReturnFourWayLink()[1]?.ReturnThisUnit().GetAPC();
        List<UnitData> a = ThisUnitData.ReturnFourWayLink();
        return jointAnotherParts;
    }
}
