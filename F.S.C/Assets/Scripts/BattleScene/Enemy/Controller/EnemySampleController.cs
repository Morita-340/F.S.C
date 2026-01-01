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
        stateMachine.ChangeState(new SampleIdle(Player,rb2d, this.transform,stateMachine,AEC));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        stateMachine.Update();
        if(Player != null){
            if(Player.name != null){
                stateMachine.ChangeState(new SampleAttackState(Player,transform,rb2d,ReEUAMS,stateMachine));
                }
            else{stateMachine.ChangeState(new SampleIdle(Player,rb2d, this.transform,stateMachine,AEC));}
        }else{stateMachine.ChangeState(new SampleIdle(Player,rb2d, this.transform,stateMachine,AEC));}
    }
}
/// <summary>
/// 適当にさまよう
/// </summary>
public class SampleIdle : IdleState
{
    protected Rigidbody2D myRb2d;
    protected Transform myTransform;
    int[] rotateAngleList = new int[6] { 3, 10, 5, -3, -10, -5 };
    protected float idleTime = 4f;
    protected float rotateTime = 2f;
    float selectAngle = 0;
    protected float nowTime;
    protected float randomTime = 0;
    protected AugmentorEffectController AEC;
    public SampleIdle(GameObject inputPlayer, Rigidbody2D rb2d, Transform transform, StateMachine inputStateMachine, AugmentorEffectController inputAEC)
    {
        Player = inputPlayer;
        myRb2d = rb2d;
        myTransform = transform;
        randomTime = Random.Range(0, 1);
        stateMachine = inputStateMachine;
        AEC = inputAEC;
    }
    public override void Update()
    {
        base.Update();
        nowTime = Time.time % (idleTime + rotateTime + randomTime);
        if (nowTime < idleTime)
        {
            selectAngle = rotateAngleList[Random.Range(0, rotateAngleList.Count())];
            myRb2d.AddForce(myTransform.up * 1f);
            AEC.MoveForward();
        }
        else if (nowTime < idleTime + rotateTime + randomTime)
        {
            RandomRotate(selectAngle);
        }
    }
    private void RandomRotate(float rotateAngle)
    {
        myRb2d.velocity = myTransform.up * 10f;
        myRb2d.AddTorque(rotateAngle);
        if (rotateAngle <= 0)
        {
            AEC.RightTurn();
        }
        else
        {
            AEC.LeftTurn();
        }
        myRb2d.angularVelocity = 0;
    }
    protected void LookAtPlayer()
    {
        Vector2 direction = Player.transform.position - myTransform.position;
        if (Vector2.SignedAngle(direction, myTransform.up) <= 0)
        {
            AEC.RightTurn();
        }
        else
        {
            AEC.LeftTurn();
        }
        myTransform.up = direction;

    }
}
/// <summary>
/// プレイヤーがいるなら正面方向に向いて攻撃をする
/// 時々正面に対して法線方向に加速する
/// </summary>
public class SampleAttackState : AttackState
{
    protected Transform myTransform;
    protected Rigidbody2D rb2D;
    protected RefineEnemyUnitAttackManagementScript ReEUAMS;
    /// <summary>
    /// 加速倍率
    /// </summary>
    float sideStepVelocityEfficiency = 5f;
    /// <summary>
    /// 法線方向に加速するスパン
    /// </summary>
    protected float sideStepSpan = 10;
    protected float nowTime;
    protected float attackTime = -0.5f;
    bool playerDetected = false;
    public SampleAttackState(GameObject inputPlayer, Transform inputTransform, Rigidbody2D inputRb2D, RefineEnemyUnitAttackManagementScript inputEUAMS, StateMachine inputStateMachine)
    {
        Player = inputPlayer;
        myTransform = inputTransform;
        rb2D = inputRb2D;
        ReEUAMS = inputEUAMS;
        stateMachine = inputStateMachine;
    }
    public override void Enter()
    {
        Debug.Log("HyperAttack: Enter");
    }
    public override void Update(){
        base.Update();
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(myTransform.position, (Player.transform.position - myTransform.position).normalized*30);
        Debug.DrawLine(myTransform.position, (Player.transform.position - myTransform.position).normalized * 30,Color.cyan);
        for (int i = 0; i < raycastHit2Ds.Count(); i++)
        {
            RaycastHit2D hit2D = raycastHit2Ds[i];
            if (hit2D.transform.root.tag == GSetting.ObjTagName.PlayerUnit.ToString())
            {
                ReEUAMS.SetTargetPosition(Player.transform.position, hit2D);
                attackTime += Time.deltaTime;
                playerDetected = true;
                break;
            }
        }
        if (!raycastHit2Ds.Any(hit2D => hit2D.transform.root.tag == GSetting.ObjTagName.PlayerUnit.ToString()))
        {
            attackTime = -0.5f;
        }
        Debug.Log("GGGGGGGGG"+attackTime);
        nowTime = Time.time % sideStepSpan;
        if (nowTime == 0)
        {
            SideStep(rb2D, myTransform, sideStepVelocityEfficiency);
        }
        else
        {
            LookPlayer(myTransform);
        }
        ReEUAMS.NormalAttack(attackTime);
        Debug.Log("HyperAttack: Update");
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