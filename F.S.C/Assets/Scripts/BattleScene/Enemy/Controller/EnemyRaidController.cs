using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class EnemyRaidController : EnemyUnitMoveManagementScript
{
    [SerializeField,Range(1,5)]int lookTime = 3;
    [SerializeField,Range(1,20)]int raidVelocityEfficiency = 1;
    [SerializeField]LineRenderer lineRenderer;
    [SerializeField,ReadOnly]private AugmentorEffectController AEC;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        AEC = GetComponent<AugmentorEffectController>();
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new SampleIdle(rb2d,this.transform));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        stateMachine.Update();
        //プレイヤーを発見したら
        if(Player != null){
            if(Player?.name != null){
                stateMachine.ChangeState(new RaidState(Player,transform,rb2d,lineRenderer,lookTime,raidVelocityEfficiency,AEC));
                }
            else{stateMachine.ChangeState(new SampleIdle(rb2d,this.transform));}
        }else{stateMachine.ChangeState(new SampleIdle(rb2d,this.transform));}
    }
}
/// <summary>
/// プレイヤーの方向を向き続ける,一定時間たったら少し硬直してから突進する
/// </summary>
public class RaidState : AttackState
{
    GameObject Player;
    Transform myTransform;
    Rigidbody2D rb2D;
    LineRenderer lineRenderer;
    AugmentorEffectController AEC;
    int raidVelocityEfficiency = 1;
    int lookTime = 1;
    float raidTime = 0.3f;
    int raidWaitTime = 1;
    float lookWaitTime = 1;
    float randWaitTime = 0;
    float afterRaidWaitTime = 1f;
    public RaidState(GameObject inputPlayer,Transform inputMyTransform,Rigidbody2D inputRb2d,LineRenderer inputLineRenderer,int inputLookTime,int inputRaidVelocityEfficiency,AugmentorEffectController inputAEC){
        Player = inputPlayer;
        myTransform = inputMyTransform;
        rb2D = inputRb2d;
        lineRenderer = inputLineRenderer;
        lookTime = inputLookTime;
        raidVelocityEfficiency = inputRaidVelocityEfficiency;
        AEC = inputAEC;
    }
    public override void Enter()
    {
        base.Enter();
        randWaitTime = Random.Range(0,9) * 0.1f;
    }
    public override void Update(){
        float nowTime = Time.time%(lookWaitTime + lookTime + raidWaitTime + randWaitTime + raidTime + afterRaidWaitTime);
        if(nowTime < lookWaitTime){
            rb2D.velocity = Vector2.zero;
            AEC.Idle();
        }
        if(lookWaitTime < nowTime && 
                    nowTime < lookWaitTime + lookTime)
        {
            Look();
            LineDraw();
        }
        if(lookWaitTime + lookTime < nowTime && 
                    nowTime < lookWaitTime + lookTime + raidWaitTime + randWaitTime)
        {
            LineDraw();
        }
        if(lookWaitTime + lookTime + raidWaitTime + randWaitTime < nowTime && 
                    nowTime < lookWaitTime + lookTime + raidWaitTime + randWaitTime + raidTime)
        {
            RaidAttack();
        }
        if(lookWaitTime + lookTime + raidWaitTime + randWaitTime + raidTime < nowTime && 
                    nowTime < lookWaitTime + lookTime + raidWaitTime + randWaitTime + raidTime + afterRaidWaitTime){
            rb2D.velocity =myTransform.up *raidVelocityEfficiency;
            AEC.MoveForward(0.7f);
        }
        base.Update();
    }
    public override void Exit(){
        lineRenderer.enabled = false;
        base.Exit();
    }
    private void Look(){
        Vector2 direction = Player.transform.position - myTransform.position;
        float realAngle = Vector2.Angle(Vector2.up,direction);
        float angle = Mathf.Atan2(direction.x,direction.y) * Mathf.Rad2Deg;
        myTransform.up = Vector2.Lerp(myTransform.up,direction,Time.deltaTime);
        AEC.MoveForward(0.5f);
    }
    private void LineDraw(){
        Vector2 direction = myTransform.up;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0,myTransform.position);
        lineRenderer.SetPosition(1,(Vector3)direction*50 + myTransform.position);
    }
    private void RaidAttack(){
        rb2D.velocity =myTransform.up *5* raidVelocityEfficiency;
        AEC.MoveForward(1.5f);
        lineRenderer.enabled = false;
    }
}