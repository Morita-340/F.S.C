using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using FSCGeneral;
using UnityEngine;

public class PlayerUnitDestroyManagementScript : AbstractUnitDestroyManagementScript
{
    private bool noDamageFlag = true;
    protected override void Start(){
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.PlayerUnit;
        base.Start();
    }
    public override int CaluculateCombatPower(){
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.PlayerUnit;
        return base.CaluculateCombatPower();
    }
    public override void DestroyProcess(UnitData DeleteData)
    {
        noDamageFlag = false;
        base.DestroyProcess(DeleteData);
    }
    public void noDamageFlagReset(){
        noDamageFlag = true;
    }
    public bool GetNoDamageFlag(){
        return noDamageFlag;
    }
}
