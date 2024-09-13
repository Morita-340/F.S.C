using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCore : CoreBase
{
    [SerializeField]
    GameObject RocketBomb;
    [SerializeField]
    GameObject FirePositionObj;
    // Start is called before the first frame update
    protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override void AttackAction()
    {
        Vector3 FirePosition = FirePositionObj.transform.position;
        Quaternion FireRotation = FirePositionObj.transform.rotation;
        base.AttackAction();
        //ロケット弾を前方に射出
        Instantiate(RocketBomb,FirePosition,FireRotation);
    }
}
