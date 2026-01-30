using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class RefineEnemyUnitDestroyManagementScript : RefineAbstractUnitDestroyManagementScript
{
    [SerializeField,ReadOnly]private PlayerSActionFeedBackUIController PAFBUIC;
    [SerializeField]GameObject WeaponControllConectedUIText;
    /// <summary>
    /// 再生成処理呼び出し回数上限
    /// コアを破壊するまで撃破扱いにならないのはテンポが悪いので、テンポアップのために実装
    /// 呼び出し回数が上限を超えた際は分離処理をしたのちに即刻コアを爆破
    /// </summary>
    [SerializeField,Range(1,5)] int regenePoint = 1;
    /// <summary>
    /// 現在の再生成処理呼び出し回数
    /// </summary>
    private int currentRegenePoint = 0;
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
        currentRegenePoint++;
        if (currentRegenePoint >= regenePoint)
        {
            primeUnitsHP = 0;
            //コアを爆破して強制撃破
            ThisUnitSCore?.OutCoreDestroyProcess();
        }
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
