using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;

public class RefinePlayerUnitDestroyManagementScript : RefineAbstractUnitDestroyManagementScript
{
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
}
