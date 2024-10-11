using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using FSCGeneral;
using UnityEngine;

public class PlayerUnitDestroyManagementScript : AbstractUnitDestroyManagementScript
{
    protected override GSetting.ObjTagName childObjTagName {get; set;} = GSetting.ObjTagName.PlayerUnit;
}
