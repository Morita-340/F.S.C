using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using FSCGeneral;
using UnityEngine;

public class EnemyUnitDestroyManagementScript : AbstractUnitDestroyManagementScript
{
    protected override void Start(){
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.EnemyUnit;
        base.Start();
    }
}
