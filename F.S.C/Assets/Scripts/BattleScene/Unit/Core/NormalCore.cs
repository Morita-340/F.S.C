using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCore : CoreBase
{
    [SerializeField]
    GameObject RocketBomb;
    [SerializeField]
    GameObject FirePositionObj;
    protected override void AttackAction()
    {
        Vector3 FirePosition = FirePositionObj.transform.position;
        Quaternion FireRotation = FirePositionObj.transform.rotation;
        base.AttackAction();
        //ロケット弾を前方に射出
        Instantiate(RocketBomb,FirePosition,FireRotation).GetComponent<Rigidbody2D>().velocity = transform.right * 10;
    }
}
