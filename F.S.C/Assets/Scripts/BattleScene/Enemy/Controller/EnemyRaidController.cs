using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class EnemyRaidController : EnemyUnitMoveManagementScript
{
    [SerializeField,Range(1,5)]int lookTime = 3;
    [SerializeField,Range(1,20)]int raidVelocityEfficiency = 1;
    [SerializeField]LineRenderer lineRenderer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        AEC = GetComponent<AugmentorEffectController>();
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new RaidState(Player,transform,rb2d,lineRenderer,lookTime,raidVelocityEfficiency,AEC,stateMachine));
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
/// プレイヤーの方向を向き続ける,一定時間たったら少し硬直してから突進する
/// </summary>
public class RaidState : AttackState
{
    Transform myTransform;
    Rigidbody2D rb2D;
    LineRenderer lineRenderer;
    AugmentorEffectController AEC;
    int raidVelocityEfficiency = 1;
    int lookTime = 1;
    float raidTime = 0.3f;
    float raidWaitTime = 1.5f;
    float lookWaitTime = 1;
    float randWaitTime = 0;
    float afterRaidWaitTime = 1f;
    public RaidState(GameObject inputPlayer, Transform inputMyTransform, Rigidbody2D inputRb2d, LineRenderer inputLineRenderer, int inputLookTime, int inputRaidVelocityEfficiency, AugmentorEffectController inputAEC, StateMachine inputStateMachine)
    {
        Player = inputPlayer;
        myTransform = inputMyTransform;
        rb2D = inputRb2d;
        lineRenderer = inputLineRenderer;
        lookTime = inputLookTime;
        raidVelocityEfficiency = inputRaidVelocityEfficiency;
        AEC = inputAEC;
        stateMachine = inputStateMachine;
        lineRenderer.numCapVertices = 10;
    }
    public override void Enter()
    {
        base.Enter();
        randWaitTime = Random.Range(5, 9) * 0.1f;
    }
    public override void Update()
    {
        float nowTime = Time.time % (lookWaitTime + lookTime + raidWaitTime + randWaitTime + raidTime + afterRaidWaitTime+1);
        if (nowTime < lookWaitTime)
        {
            rb2D.velocity = Vector2.zero;
            AEC.Idle();
        }
        if (lookWaitTime < nowTime &&
                    nowTime < lookWaitTime + lookTime)
        {
            Look();
            LineDraw(new Color(1, 0.5f, 0, 0.7f));
        }
        if (lookWaitTime + lookTime < nowTime &&
                    nowTime < lookWaitTime + lookTime + raidWaitTime + randWaitTime)
        {
            LineDraw(new Color(1, 0, 0, 0.7f));
        }
        if (lookWaitTime + lookTime + raidWaitTime + randWaitTime < nowTime &&
                    nowTime < lookWaitTime + lookTime + raidWaitTime + randWaitTime + raidTime)
        {
            RaidAttack();
        }
        if (lookWaitTime + lookTime + raidWaitTime + randWaitTime + raidTime < nowTime &&
                    nowTime < lookWaitTime + lookTime + raidWaitTime + randWaitTime + raidTime + afterRaidWaitTime)
        {
            rb2D.velocity = myTransform.up * raidVelocityEfficiency;
            AEC.MoveForward(0.7f);
        }
        if (nowTime > lookWaitTime + lookTime + raidWaitTime + randWaitTime + raidTime + afterRaidWaitTime)
        {
            stateMachine.ChangeState(new RaidIdle(Player,myTransform,rb2D,lineRenderer,lookTime,raidVelocityEfficiency,AEC,stateMachine));
        }
        base.Update();
    }
    public override void Exit()
    {
        lineRenderer.enabled = false;
        base.Exit();
    }
    private void Look()
    {
        Vector2 direction = Player.transform.position - myTransform.position;
        float realAngle = Vector2.SignedAngle(Vector2.up, direction);
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        myTransform.up = Vector2.Lerp(myTransform.up, direction, Time.deltaTime);
        if (realAngle <= 0)
        {
            AEC.RightTurn();
        }
        else
        {
            AEC.LeftTurn();
        }
    }
    private void LineDraw(Color color)
    {
        Vector2 direction = myTransform.up;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, myTransform.position);
        lineRenderer.SetPosition(1, (Vector3)direction * 50 + myTransform.position);
        lineRenderer.startColor = new Color(0, 0, 0, 0);lineRenderer.endColor = color;
    }
    private void RaidAttack()
    {
        rb2D.velocity = myTransform.up * 5 * raidVelocityEfficiency;
        AEC.MoveForward(3.5f);
        lineRenderer.enabled = false;
    }
}
public class RaidIdle : SampleIdle
{
    Rigidbody2D rb2D;
    LineRenderer lineRenderer;
    int raidVelocityEfficiency = 1;
    int lookTime = 1;
    float stateChangeTime ;
    public RaidIdle(GameObject inputPlayer, Transform inputMyTransform, Rigidbody2D inputRb2d, LineRenderer inputLineRenderer, int inputLookTime, int inputRaidVelocityEfficiency, AugmentorEffectController inputAEC, StateMachine inputStateMachine)
        : base(inputPlayer, inputRb2d, inputMyTransform, inputStateMachine, inputAEC)
    {
        Player = inputPlayer;
        myTransform = inputMyTransform;
        rb2D = inputRb2d;
        lineRenderer = inputLineRenderer;
        lookTime = inputLookTime;
        raidVelocityEfficiency = inputRaidVelocityEfficiency;
        stateMachine = inputStateMachine;
        stateChangeTime = idleTime + rotateTime + randomTime -1;
    }
    public override void Update()
    {
        base.Update();
        if (nowTime > stateChangeTime)
        {
            stateMachine.ChangeState(new RaidState(Player,myTransform,rb2D,lineRenderer,lookTime,raidVelocityEfficiency,AEC,stateMachine));
        }
    }
}