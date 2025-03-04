using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class KazagurumaController : EnemyUnitMoveManagementScript
{
    protected override void Start()
    {
        base.Start();
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new SampleIdle(rb2d,this.transform));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        stateMachine.Update();
        foreach(GameObject gameObject in DiscoveredObjectList){
            if(gameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
                Player = gameObject;
                break;
            }
        }
        //プレイヤーを発見したら
        if(Player != null){
            if(Player?.name != null){
                stateMachine.ChangeState(new RollingAttackState(rb2d,transform,Player));
                }
        }
        else{stateMachine.ChangeState(new IdleState());}
    }
}
/// <summary>
/// プレイヤーを発見したらプレイヤーに向けて突進しながら回転してくる
/// </summary>
public class RollingAttackState : AttackState
{
    Rigidbody2D myRb2d;
    Transform myTransform;
    GameObject Player;
    float force = 10;
    float torque = 5;
    //以下時間管理用変数
    float lookTime = 1;
    float accelateTime = 1.5f;
    float addTorqueTime = 1f;
    float raidTime = 3f;
    float decelateTime = 1.5f;
    float subtractTorqueTime = 1f;
    float idleTime = 5f;
    public RollingAttackState(Rigidbody2D inputRb2d, Transform inputTransform ,GameObject inputPlayer){
        myRb2d = inputRb2d;
        myTransform = inputTransform;
        Player = inputPlayer;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        float nowTime = Time.time % (lookTime + accelateTime + addTorqueTime + raidTime + decelateTime + subtractTorqueTime + idleTime);
        if(nowTime < lookTime){
        //プレイヤー方向を向いて
            Look();
        }else if(nowTime < lookTime + accelateTime){
        //加速
            myRb2d.AddForce(myTransform.forward * force);
        }
        else if(nowTime < lookTime + accelateTime + addTorqueTime){
        //回転する
            myRb2d.AddTorque(torque);
        }
        else if(nowTime < lookTime + accelateTime + addTorqueTime + raidTime){
        //何もせず慣性に従って突進する
        }
        else if(nowTime < lookTime + accelateTime + addTorqueTime + raidTime + decelateTime){
        //減速する
            myRb2d.AddTorque(-torque);
        }
        else if(nowTime < lookTime + accelateTime + addTorqueTime + raidTime + decelateTime + subtractTorqueTime){
        //一定時間経つと回転がじわじわと遅くなり静止する
            myRb2d.AddForce(-myTransform.forward * force);
        }
        else if(nowTime < lookTime + accelateTime + addTorqueTime + raidTime + decelateTime + subtractTorqueTime + idleTime){
        //一定時間完全静止する
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
