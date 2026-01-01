using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BoneController : EnemyUnitMoveManagementScript
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        AEC = GetComponent<AugmentorEffectController>();
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new BoneIdle(Player, transform, rb2d, ReEUAMS, stateMachine, AEC));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        stateMachine.Update();
        //プレイヤーを取得できなければ
        if (Player == null)
        {
            stateMachine.ChangeState(new SampleIdle(Player,rb2d, this.transform,stateMachine,AEC));
        }
    }
}
/// <summary>
/// プレイヤー方向に近づく
/// </summary>
public class BoneIdle : SampleIdle
{
    RefineEnemyUnitAttackManagementScript ReEUAMS;
    public BoneIdle(GameObject Player, Transform myTransform, Rigidbody2D inputRb2d, RefineEnemyUnitAttackManagementScript inputReEUAMS, StateMachine stateMachine, AugmentorEffectController inputAEC)
        : base(Player, inputRb2d, myTransform, stateMachine,inputAEC)
    {
        ReEUAMS = inputReEUAMS;
    }
    public override void Update()
    {
        nowTime = Time.time % (idleTime + rotateTime + randomTime);
        if (nowTime < idleTime)
        {
            myRb2d.AddForce(myTransform.up * 1f);
            AEC.MoveForward();
        }
        else if (nowTime < idleTime + 0.5f)
        {
            if (myRb2d.velocity.magnitude < 0.3f)
            {
                myRb2d.AddForce(-myTransform.up * 1f);
                AEC.MoveBack();
            }
        }
        else if (nowTime < idleTime + rotateTime - 0.5f)
        {
            LookAtPlayer();
        }
        else
        {
            stateMachine.ChangeState(new BoneAttack(Player, myTransform, myRb2d, ReEUAMS, stateMachine, AEC));
        }
        
    }
}
/// <summary>
/// 兵装でPlayerに対して一斉射撃
/// </summary>
public class BoneAttack : SampleAttackState
{
    AugmentorEffectController AEC;
    public BoneAttack(GameObject inputPlayer, Transform inputTransform, Rigidbody2D inputRb2D, RefineEnemyUnitAttackManagementScript inputReEUAMS, StateMachine inputStateMachine, AugmentorEffectController inputAEC) : base(inputPlayer, inputTransform, inputRb2D, inputReEUAMS, inputStateMachine)
    {
        AEC = inputAEC;
    }
    public override void Update()
    {
        base.Update();
        if (nowTime > sideStepSpan - 0.01f)
        {
            stateMachine.ChangeState(new BoneIdle(Player,myTransform,rb2D,ReEUAMS,stateMachine,AEC));
        }
    }
}

