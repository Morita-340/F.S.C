using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using FSCGeneral;
using UnityEngine;

public class EnemyUnitDestroyManagementScript : AbstractUnitDestroyManagementScript
{
    private PlayerSActionFeedBackUIController PAFBUIC;
    protected override void Start(){
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.EnemyUnit;
        //ゲームシーン上の名前に依存しているので要注意である
        PAFBUIC = GameObject.Find("Canvas").GetComponent<PlayerSActionFeedBackUIController>();
        base.Start();
    }
    public override int CaluculateCombatPower(){
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.EnemyUnit;
        return base.CaluculateCombatPower();
    }
    public override void DestroyProcess(UnitData DeleteData)
    {
        Debug.Log("EUDMS" + DeleteData.ReturnThisUnit().GetUnitStatus());
        PAFBUIC.AddDefeatPoint(DeleteData.ReturnThisUnit().GetUnitStatus());
        base.DestroyProcess(DeleteData);
    }
}
