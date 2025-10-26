using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
/// <summary>
/// 機能ごとにパーツを分けて管理する
/// パーツを構成するUnitはここで管理する
/// このクラス自体はReAUDMSやReAUAMSに管理してもらう
/// 攻撃や分離処理などの中間地点
/// </summary>
public class AbstractPartsController : MonoBehaviour
{
    [SerializeField,ReadOnly]
    protected RefineAbstractUnitDestroyManagementScript ReAUDMS;
    protected DestroyedUnitManagementScript DUMS;
    protected GSetting.ObjTagName childObjTagName;
    protected bool caluculateFlag = false;
    /// <summary>
    /// 子オブジェクトのユニット全て
    /// </summary>
    protected List<UnitData> ChildUnitDataList = new List<UnitData>();
    protected List<UnitData> NotResearchUnitList = new List<UnitData>();
    protected List<UnitData> RegeneUnitList = new List<UnitData>();
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ReAUDMS = gameObject.transform.parent.GetComponent<RefineAbstractUnitDestroyManagementScript>();
        DUMS = gameObject.transform.parent.GetComponent<DestroyedUnitManagementScript>();
        if (ReAUDMS != null)
        {
            childObjTagName = ReAUDMS.GetChildObjTagName();
        }
        if (DUMS != null)
        {
            childObjTagName = GSetting.ObjTagName.DestroyedUnit;
        }
        SetUnitData();
    }
    /// <summary>
    /// 子オブジェクトのUnitDataをまとめて格納する関数
    /// </summary>
    public virtual void SetUnitData()
    {
        ChildUnitDataList.Clear();
        //Debug.Log("HHH"+gameObject.transform.childCount);
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childUnitObject = gameObject.transform.GetChild(i).gameObject;
            UnitBase ChildUnit = childUnitObject.GetComponent<UnitBase>();
            //Debug.Log("GGG" + childUnitObject.tag+childObjTagName.ToString());
            if (childUnitObject.tag == childObjTagName.ToString())
            {
                ChildUnitDataList.Add(ChildUnit.GetThisUnitData());
            }
        }
        caluculateFlag = true;
        //combatPower = CaluculateCombatPower();
    }
    public virtual void DestroyProcess(UnitData DeleteData, bool inputIsDead)
    {
        //Debug.Log("GGG" +childObjTagName.ToString());
        ChildUnitDataList.Remove(DeleteData);
        ReAUDMS.DestroyProcess(DeleteData, inputIsDead);
    }
    /// <summary>
    /// パーツに含まれるデータを削除する
    /// </summary>
    /// <param name="unitData"></param>
    public void DeleteChildUnitData(UnitData unitData)
    {
        GameObject deleteObj = unitData.ReturnThisUnit().gameObject;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (deleteObj == transform.GetChild(i).gameObject)
            {
                DestroyImmediate(unitData.ReturnThisUnit().gameObject);
                ChildUnitDataList.Remove(unitData);
                NotResearchUnitList.Remove(unitData);
                return;
            }
        }
        GSetting.RefineDebugAssertinLog(transform,"データがこのオブジェクトの中に無い");
    }
    public List<UnitData> GetNotResearchUnitList()
    {
        return NotResearchUnitList;
    }
    public List<UnitData> GetRegeneUnitList()
    {
        return RegeneUnitList;
    }
    /// <summary>
    /// 子オブジェクトのUnitについて、全て探索結果がflagであるかどうか判定
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool AllUnitSearched(bool flag)
    {
        Debug.Log(ChildUnitDataList.Count);
        foreach (UnitData unitData in ChildUnitDataList)
        {
            Debug.Log(unitData.ReturnThisUnit().gameObject);
            if (unitData.AlreadySearch != flag) { return false; }
        }
        return true;
    }
    public bool AllUnitSearched(bool flag, List<UnitData> DataList)
    {
        foreach (UnitData unitData in DataList)
        {
            if (unitData.AlreadySearch != flag) { return false; }
        }
        return true;
    }
    public void SetRegeneUnitList()
    {
        RegeneUnitList.Clear();
        foreach (UnitData unitData in NotResearchUnitList)
        {
            if (unitData.AlreadySearch == true)
            {
                //コアを再生成対象にしてはならない
                if (unitData.ReturnThisUnit() is CoreBase) { continue; }
                GSetting.RefineDebugAssertinLog(unitData.ReturnThisUnit().gameObject.transform,"意図しないオブジェクトが紛れてるかも");
                RegeneUnitList.Add(unitData);
            }
        }
    }
    public void SetNotResearchUnitList()
    {
        NotResearchUnitList.Clear();
        foreach (UnitData unitData in ChildUnitDataList)
        {
            if (unitData.AlreadySearch == false)
            {
                NotResearchUnitList.Add(unitData);
            }
        }
    }
    public virtual int CaluculateCombatPower()
    {
        int combatPower = 0;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childUnitObject = gameObject.transform.GetChild(i).gameObject;
            AttackUnit AU = childUnitObject.GetComponent<AttackUnit>();
            WeaponControllUnitBase WCUB = childUnitObject.GetComponent<WeaponControllUnitBase>();
            ReactorBase reactorBase = childUnitObject.GetComponent<ReactorBase>();
            if (childUnitObject.tag == childObjTagName.ToString())
            {
                if (AU != null)
                {
                    combatPower += AU.GetUnitStatus();
                }
                else if (WCUB != null)
                {
                    combatPower += WCUB.GetUnitStatus();
                }
                else if (reactorBase != null)
                {
                    combatPower += reactorBase.CaluculateReactorEffect() + reactorBase.GetUnitStatus();
                }
            }
        }
        return combatPower;
    }
    public virtual void DeleteAllRangeMesh()
    {
        foreach (UnitData childUnitData in ChildUnitDataList)
        {
            if (childUnitData == null)
            {
                Debug.LogWarning("AAA");
                continue;
            }
            if (childUnitData.ReturnThisUnit() == null)
            {
                Debug.LogWarning("AAA");
                continue;
            }
            if (childUnitData.ReturnThisUnit() is CoreBase coreBase)
            {
                coreBase.DestroyFRMesh();
            }
            if (childUnitData.ReturnThisUnit() is WeaponUnitBase weaponUnitBase)
            {
                weaponUnitBase.DestroyFRMesh();
            }
        }
    }
    /*以上分離処理*/
    /*以下攻撃処理*/
    [SerializeField,ReadOnly]protected Vector3 TargetPosition;
    public void SetTargetPosition(Vector3 inputTargetPosion)
    {
        TargetPosition = inputTargetPosion;
        foreach (UnitData child in ChildUnitDataList)
        {
            //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
            if (child.ReturnThisUnit() is AttackUnit AU)
            {
                AU.SetTargetPosition(TargetPosition);
            }
        }
    }
    public virtual void NormalAttack(Vector3 inputTargetPosion){
            foreach (UnitData child in ChildUnitDataList){
                //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
                if(child.ReturnThisUnit() is AttackUnit AU){
                    StartCoroutine(AU.NormalAttack(TargetPosition));
                }
            }
    }
}
