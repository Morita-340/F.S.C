using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using FSCGeneral;

public class CoreBase : AttackUnit
{
    /// <summary>
    /// 爆風だけのREMを実装したいので、REMをそのまま使用する
    /// </summary>
    [SerializeField]
    protected CoreEffectManager CEM;
    protected GameObject ReactorEffectPool;
    protected override void Awake()
    {
        base.Awake();
        FR.InitialSetting();
    }
    protected override void Start(){
        ReactorEffectPool = GameObject.Find(GSetting.UniqueObjectName.ReactorEffectPool.ToString());
        //CEM.SetSCer(GetComponent<SoundController>());
        CEM.transform.SetParent(ReactorEffectPool.transform);
        CEM.ExplosionActive(false);
        CEM.SetThisCore(this);
        base.Start();
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    /// <summary>
    /// 分離回数が一定値を超えた際に強制的に撃破させるpublic処理。ゲームのテンポ向上を目的とする
    /// </summary>
    public void OutCoreDestroyProcess()
    {
        if (this.gameObject.tag == GSetting.ObjTagName.EnemyUnit.ToString())
        {
            CEM.ExplosionActive(true);
            FR.DestroyRMM();
            DestroyCore();
        }
    }
    /// <summary>
    /// コアが破壊された場合はプレイヤーユニットが完全に消去される
    /// </summary>
    protected override void DestroyUnit()
    {
        if (!ReAUDMS.GetIsDead())
        {
            CEM.ExplosionActive(true);
            FR.DestroyRMM();
            DestroyCore();
        }
    }
    protected void DestroyCore(){
        if(this.gameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            this.gameObject.transform.root.gameObject.SetActive(false);
            }
        else{
        StartCoroutine(DestroyObj());
        }
        ReAUDMS.IsDead();
    }
    IEnumerator DestroyObj(){
        APC.DeleteAllRangeMesh();
        FM.DeleteData(ThisUnitData);
        yield return new WaitForSeconds(0.3f);
        DestroyImmediate(this.gameObject.transform.root.gameObject);
    }
    public bool InCoreRange(Vector3 TargetPositon){
        return FR.InRange(TargetPositon);
    }
}
