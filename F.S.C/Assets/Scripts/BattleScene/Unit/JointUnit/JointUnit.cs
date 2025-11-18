using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;
/// <summary>
/// DownerUnitのみを他パーツとのジョイント部分とする。
/// </summary>
public class JointUnit : UnitBase
{
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
        return transform.position + Quaternion.Euler(0, 0, offsetRotation) * Vector3.down;
    }
    public AbstractPartsController GetJointedAnotherPart()
    {
        AbstractPartsController jointAnotherParts = ThisUnitData.ReturnFourWayLink()[1]?.ReturnThisUnit().GetAPC();
        List<UnitData> a = ThisUnitData.ReturnFourWayLink();
        GSetting.RefineDebugAssertinLog(transform,a[0]?.ReturnThisUnit().gameObject+" "+(a[1] != null?a[1].ReturnThisUnit().gameObject:"リンク無し")+" "+a[2]?.ReturnThisUnit().gameObject+" "+a[3]?.ReturnThisUnit().gameObject+"\n");
        return jointAnotherParts;
    }
}
