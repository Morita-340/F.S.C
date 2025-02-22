using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSCGeneral;
using UnityEngine;

/// <summary>
/// ステートマシンを用いた挙動制御の原理検証用のサンプルスクリプト
/// </summary>
public class EnemySampleController : EnemyUnitMoveManagementScript
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new IdleState());
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
        if(Player != null){
            if(Player.name != null){
                stateMachine.ChangeState(new SampleChaseMove(rb2d,Player,transform));
                }
            else{stateMachine.ChangeState(new SampleIdle(rb2d,this.transform));}
        }
    }
}
public class SampleIdle : IdleState
{
    Rigidbody2D myRb2d;
    Transform myTransform;
    float rotateTime = 0;
    int[] rotateAngleList = new int[6]{30,10,5,-30,-10,-5};
    int selectAngle = 0;
    public SampleIdle(Rigidbody2D rb2d,Transform transform){
        myRb2d = rb2d;
        myTransform = transform;
    }
    public override void Update()
    {
        rotateTime += Time.deltaTime;
        if(rotateTime > 5){
            selectAngle = rotateAngleList[Random.Range(0, rotateAngleList.Count())];
            rotateTime = 0;
        }
        if(rotateTime > 4){
            myRb2d.AddTorque(selectAngle);
            myRb2d.angularVelocity = 0;
        }else{
        myRb2d.velocity = myTransform.up * 10f;
        }
        base.Update();

    }
}
public class SampleChaseMove : MoveState
{
    GameObject ChasedPlayer;
    Transform myTransform;
    Rigidbody2D myRb2d;
    public SampleChaseMove(Rigidbody2D rb2d,GameObject player,Transform transform){
        ChasedPlayer = player;
        myTransform = transform;
        myRb2d = rb2d;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Update(){
        base.Update();
        if(ChasedPlayer == null){return;}
        myRb2d.velocity = myTransform.up * 10f;
        float targetAngle = Vector2.SignedAngle(myTransform.position,ChasedPlayer.transform.position);
        if(targetAngle >= 0){myRb2d.AddTorque(-(targetAngle*2));myRb2d.angularVelocity = 0;}
        else{myRb2d.AddTorque(targetAngle*2);myRb2d.angularVelocity = 0;}
    }
}
public class HyperAttackState : AttackState
{
    public override void Enter()
    {
        Debug.Log("HyperAttack: Enter");
    }
    public override void Update(){
        Debug.Log("HyperAttack: Update");
        base.Update();
    }
    public override void Exit(){}
}