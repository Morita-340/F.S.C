using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class KazagurumaController : EnemyUnitMoveManagementScript
{
    [SerializeField,ReadOnly] private SoundController SCer;
    protected override void Start()
    {
        SCer = GetComponent<SoundController>();
        base.Start();
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new RollingAttackState(Player,rb2d, this.transform,SCer));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        stateMachine.Update();
        //プレイヤーを取得できなければ
        if(Player == null){
        stateMachine.ChangeState(new SampleIdle(Player,rb2d,this.transform,stateMachine,AEC));}
    }
}
/// <summary>
/// プレイヤーを発見したらプレイヤーに向けて突進しながら回転してくる
/// </summary>
public class RollingAttackState : AttackState
{
    Rigidbody2D myRb2d;
    Transform myTransform;
    SoundController SCer;
    float force = 10;
    float torque = 500;
    //以下時間管理用変数
    float lookTime = 1;
    float accelateTime = 1.5f;
    float addTorqueTime = 1f;
    float raidTime = 3f;
    float decelateTime = 1.5f;
    float subtractTorqueTime = 1f;
    float idleTime = 5f;
    public RollingAttackState(GameObject inputPlayer,Rigidbody2D inputRb2d, Transform inputTransform ,SoundController inputSCer){
        Player = inputPlayer;
        myRb2d = inputRb2d;
        myTransform = inputTransform;
        SCer = inputSCer;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        float nowTime = Time.time % (lookTime + accelateTime + addTorqueTime + raidTime + decelateTime + subtractTorqueTime + idleTime);
        if (nowTime < lookTime)
        {
        }
        else if (nowTime < lookTime + accelateTime)
        {
            //加速
            Vector2 toPlayerVec = Player.transform.position -myTransform.position;
            myRb2d.velocity = toPlayerVec.normalized * force;
        }
        else if (nowTime < lookTime + accelateTime + addTorqueTime)
        {
            //回転する
            myRb2d.AddTorque(torque);
            SCer.PlaySE(0);
        }
        else if (nowTime < lookTime + accelateTime + addTorqueTime + raidTime)
        {
            //何もせず慣性に従って突進する
        }
        else if (nowTime < lookTime + accelateTime + addTorqueTime + raidTime + decelateTime)
        {
            //減速する
            myRb2d.AddTorque(-torque);
        }
        else if (nowTime < lookTime + accelateTime + addTorqueTime + raidTime + decelateTime + subtractTorqueTime)
        {
            SCer.FadeSE();
            //一定時間経つと回転がじわじわと遅くなり静止する
            myRb2d.AddForce(-myTransform.forward * force);
        }
        else if (nowTime < lookTime + accelateTime + addTorqueTime + raidTime + decelateTime + subtractTorqueTime + idleTime)
        {
            //一定時間完全静止する
            //myRb2d.angularVelocity = 0;
        }
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
    }
    private void Look(){
        Vector2 direction = Player.transform.position - myTransform.position;
        myTransform.up = Vector2.Lerp(myTransform.up,direction,Time.deltaTime);
    }
}
