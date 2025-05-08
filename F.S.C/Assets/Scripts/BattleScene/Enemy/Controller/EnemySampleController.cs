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
        stateMachine.ChangeState(new SampleIdle(rb2d,this.transform));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        stateMachine.Update();
        if(Player != null){
            if(Player.name != null){
                stateMachine.ChangeState(new SampleAttackState(Player,transform,rb2d,EUAMS));
                }
            else{stateMachine.ChangeState(new SampleIdle(rb2d,this.transform));}
        }else{stateMachine.ChangeState(new SampleIdle(rb2d,this.transform));}
    }
}
/// <summary>
/// 適当にさまよう
/// </summary>
public class SampleIdle : IdleState
{
    Rigidbody2D myRb2d;
    Transform myTransform;
    int[] rotateAngleList = new int[6]{3,10,5,-3,-10,-5};
    float idleTime = 4f;
    float rotateTime = 1f;
    float selectAngle = 0;
    public SampleIdle(Rigidbody2D rb2d,Transform transform){
        myRb2d = rb2d;
        myTransform = transform;
    }
    public override void Update()
    {
        base .Update();
        float nowTime = Time.time % (idleTime + rotateTime);
        if(nowTime < idleTime){
            selectAngle = rotateAngleList[Random.Range(0, rotateAngleList.Count())];
            myRb2d.AddForce(myTransform.up * 1f);
        }else if(nowTime < idleTime + rotateTime){
            RandomRotate(selectAngle);
        }
    }
    private void RandomRotate(float rotateAngle){
        myRb2d.velocity = myTransform.up * 10f;
        myRb2d.AddTorque(rotateAngle);
        myRb2d.angularVelocity = 0;
    }
}
/// <summary>
/// プレイヤーがいるなら正面方向に向いて攻撃をする
/// 時々正面に対して法線方向に加速する
/// </summary>
public class SampleAttackState : AttackState
{
    GameObject Player;
    Transform myTransform;
    Rigidbody2D rb2D;
    EnemyUnitAttackManagementScript EUAMS;
    /// <summary>
    /// 加速倍率
    /// </summary>
    float sideStepVelocityEfficiency = 5f;
    /// <summary>
    /// 法線方向に加速するスパン
    /// </summary>
    float sideStepSpan = 10;
    public SampleAttackState(GameObject inputPlayer,Transform inputTransform,Rigidbody2D inputRb2D,EnemyUnitAttackManagementScript inputEUAMS){
        Player = inputPlayer;
        myTransform = inputTransform;
        rb2D = inputRb2D;
        EUAMS = inputEUAMS;
    }
    public override void Enter()
    {
        Debug.Log("HyperAttack: Enter");
    }
    public override void Update(){
        Debug.Log("HyperAttack: Update");
        base.Update();
        float nowTime = Time.time % sideStepSpan;
        if(nowTime == 0){
            SideStep(rb2D,myTransform,sideStepVelocityEfficiency);
        }else{
            EUAMS.NormalAttack(nowTime);
            LookPlayer(myTransform);
        }
    }
    /// <summary>
    /// 法線方向への加速処理
    /// </summary>
    /// <param name="inputRb2D"></param>
    /// <param name="inputTransform"></param>
    /// <param name="inputsideStepVelocityEfficiency"></param>
    private void SideStep(Rigidbody2D inputRb2D,Transform inputTransform,float inputsideStepVelocityEfficiency){
        int leftOrRight = Random.Range(0,2);
        if(leftOrRight == 0){
            inputRb2D.AddForce(myTransform.right * sideStepVelocityEfficiency);
        }else{
            inputRb2D.AddForce(-myTransform.right * sideStepVelocityEfficiency);
        }
    }
    /// <summary>
    /// プレイヤー方向へ向く処理
    /// </summary>
    /// <param name="inputPlayer"></param>
    /// <param name="inputTransform"></param>
    private void LookPlayer(Transform inputTransform){
        Vector2 direction = Player.transform.position - inputTransform.position;
        inputTransform.up = Vector2.Lerp(inputTransform.up,direction,Time.deltaTime);
    }
}