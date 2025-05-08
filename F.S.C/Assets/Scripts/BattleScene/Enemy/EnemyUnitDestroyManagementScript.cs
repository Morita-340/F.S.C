using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using FSCGeneral;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class EnemyUnitDestroyManagementScript : AbstractUnitDestroyManagementScript
{
    [SerializeField,ReadOnly]private PlayerSActionFeedBackUIController PAFBUIC;
    [SerializeField]GameObject WeaponControllConectedUIText;
    protected override void Awake()
    {
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.EnemyUnit;
        SCer = GetComponent<SoundController>();
        base.Awake();
    }
    protected override void Start(){
        //ゲームシーン上の名前に依存しているので要注意である
        PAFBUIC = GameObject.Find(GSetting.UniqueObjectName.UICanvas.ToString()).transform.Find("PlayerSActionFeedBackUI").GetComponent<PlayerSActionFeedBackUIController>();
        base.Start();
    }
    protected override List<GameObject> Regenerate(bool inputIsDead)
    {
        List<GameObject> ParentObjectList = base.Regenerate(inputIsDead);
        StartCoroutine(PAFBUIC?.Wait(0.1f,ParentObjectList));
        return ParentObjectList;
    }
    public override int CaluculateCombatPower(){
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.EnemyUnit;
        return base.CaluculateCombatPower();
    }
    public override void DestroyProcess(UnitData DeleteData,bool inputIsDead)
    {
        if(!Setting){
            PAFBUIC.AddDefeatPoint(DeleteData.ReturnThisUnit().GetUnitStatus());
            PAFBUIC.ExplosionOcurre(DeleteData);
        }
        base.DestroyProcess(DeleteData,inputIsDead);
    }
    public override void DeleteAllRangeMesh()
    {
        transform.Find("SearchRader").GetComponent<EnemySearchManagementScript>().DestroyRMM();
        base.DeleteAllRangeMesh();
    }
}
