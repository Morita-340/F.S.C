using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;

public class RefinePlayerUnitDestroyManagementScript : RefineAbstractUnitDestroyManagementScript
{
    //ダメージを受けていない状態であるかどうか
    private bool noDamageFlag = true;
    //ダメージを受け付けない（無敵）であるかどうか
    private bool invincible = false;
    protected override void Awake()
    {
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.PlayerUnit;
        base.Awake();
    }
    public override void SetUnitData()
    {
        childObjTagName = GSetting.ObjTagName.PlayerUnit;
        base.SetUnitData();
    }
    public List<Vector3> GetAllEmptyPassiveJointSPositionList()
    {
        List<Vector3> jointUnitSPositionList = new List<Vector3>();
        //Debug.LogAssertion(PartsList.Count);
        foreach (AbstractPartsController APC in PartsList)
        {
            List<PassiveJointUnit> passivejointList = APC.GetPassiveJointLinkList();
            foreach (JointUnit jointUnit in passivejointList)
            {
                if (jointUnit.isJointLinkEmpty())
                {
                    Vector3 EmptyPassiveJointSPosition = jointUnit.GetEmptyLinkPosition();
                    jointUnitSPositionList.Add(EmptyPassiveJointSPosition);
                }
            }
        }
        //Debug.LogAssertion("jointUnitSPositionList.Count" + jointUnitSPositionList.Count);
        return jointUnitSPositionList;
    }
    public List<PassiveJointUnit> GetSelectedPassiveJointList()
    {
        List<PassiveJointUnit> EmptyPassiveJointList = new List<PassiveJointUnit>();
        //Debug.LogAssertion(PartsList.Count);
        foreach (AbstractPartsController APC in PartsList)
        {
            List<PassiveJointUnit> passivejointList = APC.GetPassiveJointLinkList();
            //Debug.LogAssertion(passivejointList.Count);
            foreach (PassiveJointUnit jointUnit in passivejointList)
            {
                if (jointUnit.isJointLinkEmpty())
                {
                    EmptyPassiveJointList.Add(jointUnit);
                }
            }
        }
        return EmptyPassiveJointList;
    }
    /// <summary>
    /// 無敵時間なら被弾しない
    /// </summary>
    /// <param name="decreaseValue">ダメージによるHP減少量</param>
    public override void DecreasePrimeUnitsHP(int decreaseValue,bool ratio)
    {
        if (invincible)
        {

        }
        else
        {
            base.DecreasePrimeUnitsHP(decreaseValue,ratio);
            //一定時間無敵になる
            StartCoroutine(TemporaryInvincibility(0.1f));
        }
    }
    /// <summary>
    /// 一定時間無敵になる
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator TemporaryInvincibility(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }
    public override int CaluculateCombatPower()
    {
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.PlayerUnit;
        return base.CaluculateCombatPower();
    }
    public override void DestroyProcess(UnitData DeleteData, bool inputIsDead)
    {
        noDamageFlag = false;
        base.DestroyProcess(DeleteData, inputIsDead);
    }
    public void RepairMachine(int RepairHP)
    {
        primeUnitsHP += RepairHP;
    }
    public int GetAttackPower()
    {
        return attackPower;
    }
    public void noDamageFlagReset()
    {
        noDamageFlag = true;
    }
    public bool GetNoDamageFlag()
    {
        return noDamageFlag;
    }
    public void SetInvincible(bool flag)
    {
        invincible = flag;
        foreach (AbstractPartsController APC in PartsList)
        {
            APC.SetInvincible(flag);
        }
    }
}
