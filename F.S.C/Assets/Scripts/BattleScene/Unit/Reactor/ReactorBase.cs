using System;
using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;

public class ReactorBase : UnitBase
{
    [SerializeField]ReactorEffectManager REM;
    protected GameObject ReactorEffectPool;
    [SerializeField,ReadOnly]int reactorEffectToOtherUnit = 0;
    /// <summary>
    /// リアクターが付与できる強化倍率
    /// </summary>
    [SerializeField,Range(1,100)]int reactorsEfficiencyLevel = 1;
    public int GetReactorsEfficiencyLevel(){
        return reactorsEfficiencyLevel;
    }
    protected override void Awake(){
        ReactorEffectPool = GameObject.Find("ReactorEffectPool");
        SetReactorEffectScope();
        REM.SetThisReactor(this);
        REM.transform.SetParent(ReactorEffectPool.transform);
        REM.ScopeActive(true);
        REM.ExplosionActive(false);
        base.Awake();
    }
    //継承先で使う攻撃の属性の定義をすること
    protected override void Start(){
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //判定用の二種のオブジェクトの座標と回転角の変更。
        if(tag == GSetting.ObjTagName.PlayerUnit.ToString() || tag == GSetting.ObjTagName.EnemyUnit.ToString())REM.ChangeTransform(transform);
        base.Update();
        //強化用コライダーはPlayerUnitまたはEnemyUnitの時のみ展開すること
        //リアクターのレベルに従ってリアクターの色を変えること
    }
    protected override void DestroyUnit()
    {
        //destroyする際に爆発判定のコライダーを展開する
        REM.ExplosionActive(true);
        REM.ScopeActive(false);
        base.DestroyUnit();
    }
    private int CaluculateScopeRadius(){
        int scopeRadius = 1;
        switch(reactorsEfficiencyLevel){
            case int x when x < 5:  scopeRadius = 1; break;
            case int x when x <15:  scopeRadius = 2; break;
            case int x when x <40:  scopeRadius = 3; break;
            case int x when x <70:  scopeRadius = 4; break;
            case int x when x <=100: scopeRadius = 5; break;
            default: scopeRadius = 1; break;
        }
        return scopeRadius;
    }
    /// <summary>
    /// インスペクターで定めたレベルに応じてリアクターの効果範囲のコライダー半径が決められる
    /// </summary>
    [ContextMenu("SetReactorEffectScope")]
    public void SetReactorEffectScope(){
        REM?.SetReactorEffectScope(CaluculateScopeRadius());
    }
    [ContextMenu("CaluculateReactorEffectFromInspector")]
    public int CaluculateReactorEffect(){
        Vector2[] OffsetList = {new Vector2(0,0),new Vector2(0.5f,0.5f),new Vector2(0.5f,-0.5f),new Vector2(-0.5f,0.5f),new Vector2(-0.5f,-0.5f)};
        int previsionalNum = 0;
        SetReactorEffectScope();
        AbstractUnitDestroyManagementScript AUDMS = this.transform.root.GetComponent<AbstractUnitDestroyManagementScript>();
        if(AUDMS != null){
            for(int i = 0; i < AUDMS.transform.childCount; i++){
            AttackUnit attackUnit = AUDMS.transform.GetChild(i).gameObject.GetComponent<AttackUnit>();
                if(attackUnit == null){continue;}
                foreach(Vector2 OffsetVector in OffsetList){
                    Vector2 distanseFromThisReactor = attackUnit.transform.localPosition - this.transform.localPosition + new Vector3(OffsetVector.x,OffsetVector.y,0);
                    if(distanseFromThisReactor.magnitude < CaluculateScopeRadius()){
                        if(attackUnit is WeaponUnitBase weaponUnitBase){
                            previsionalNum += weaponUnitBase.GetUnitAttackPower();
                        }
                        else{
                            previsionalNum += attackUnit.GetUnitAttackPower();
                        }
                        Debug.Log(previsionalNum);
                        break;
                    }
                }
            }
        }
        reactorEffectToOtherUnit = previsionalNum * reactorsEfficiencyLevel;
        if(reactorEffectToOtherUnit < 0)reactorEffectToOtherUnit = 0;
        Debug.Log(" "+reactorEffectToOtherUnit);
        return reactorEffectToOtherUnit;
    }
}
