using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
/// <summary>
/// ジョイントユニットが無い、自機に付くバルカン砲などのデフォルトのパーツ。これまでAPCにあったジョイントユニット関係の処理だけRCPCに移すことでAPCでデフォルトの武装にも武装UIを適用可能な仕組みにした
/// PlayerUnitのみ！！従って損傷状況の記録や合体と分離の処理で用いるメソッドを簡略化
/// </summary>
public class IntegratedPartsController : AbstractPartsController
{
    protected override void Start()
    {
        base.Start();
    }
    /// <summary>
    /// PUDMSしか受け付けないようオーバーライド
    /// </summary>
    protected override void SetPartSetting()
    {
        ReAUDMS = gameObject.transform.parent.GetComponent<RefinePlayerUnitDestroyManagementScript>();
        base.SetPartSetting();
    }
}
