using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;

public class RefinePlayerUnitDestroyManagementScript : RefineAbstractUnitDestroyManagementScript
{
    private bool noDamageFlag = true;
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
            //Debug.LogAssertion(passivejointList.Count);
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
    public List<float> GetSelectedJointSOffsetRotation()
    {
        List<float> JointSOffsetRotationList = new List<float>();
        //Debug.LogAssertion(PartsList.Count);
        foreach (AbstractPartsController APC in PartsList)
        {
            List<PassiveJointUnit> passivejointList = APC.GetPassiveJointLinkList();
            //Debug.LogAssertion(passivejointList.Count);
            foreach (JointUnit jointUnit in passivejointList)
            {
                if (jointUnit.isJointLinkEmpty())
                {
                    float OffsetRotation = jointUnit.GetOffsetRotation();
                    JointSOffsetRotationList.Add(OffsetRotation);
                }
            }
        }
        return JointSOffsetRotationList;
    }
    public override int CaluculateCombatPower(){
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.PlayerUnit;
        return base.CaluculateCombatPower();
    }
    public override void DestroyProcess(UnitData DeleteData,bool inputIsDead)
    {
        noDamageFlag = false;
        base.DestroyProcess(DeleteData,inputIsDead);
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
    public bool GetNoDamageFlag(){
        return noDamageFlag;
    }
}
