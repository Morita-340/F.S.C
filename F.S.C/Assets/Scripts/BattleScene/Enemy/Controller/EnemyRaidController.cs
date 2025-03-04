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
                stateMachine.ChangeState(new RaidState(Player,transform,rb2d,lineRenderer,lookTime,raidVelocityEfficiency));
                }
        }
        else{stateMachine.ChangeState(new IdleState());}
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
    int raidVelocityEfficiency = 1;
    int lookTime = 1;
    float raidTime = 0.3f;
    int raidWaitTime = 1;
    float lookWaitTime = 1;
    public RaidState(GameObject inputPlayer,Transform inputMyTransform,Rigidbody2D inputRb2d,LineRenderer inputLineRenderer,int inputLookTime,int inputRaidVelocityEfficiency){
        Player = inputPlayer;
        myTransform = inputMyTransform;
        rb2D = inputRb2d;
        lineRenderer = inputLineRenderer;
        lookTime = inputLookTime;
        raidVelocityEfficiency = inputRaidVelocityEfficiency;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Update(){
        float nowTime = Time.time%(lookWaitTime + lookTime + raidWaitTime + raidTime);
        if(nowTime < lookWaitTime){
            rb2D.velocity = Vector2.zero;
        }
        if(lookWaitTime < nowTime && nowTime < lookWaitTime + lookTime){
            Look();
        }
        if(lookWaitTime + lookTime + raidWaitTime < nowTime && nowTime < lookWaitTime + lookTime + raidWaitTime + raidTime){
            RaidAttack();
        }
        base.Update();
    }
    public override void Exit(){
        base.Exit();
    }
    private void Look(){
        Vector2 direction = Player.transform.position - myTransform.position;
        float realAngle = Vector2.Angle(Vector2.up,direction);
        float angle = Mathf.Atan2(direction.x,direction.y) * Mathf.Rad2Deg;
        myTransform.up = Vector2.Lerp(myTransform.up,direction,Time.deltaTime);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0,myTransform.position);
        lineRenderer.SetPosition(1,(Vector3)direction*10 + myTransform.position);
    }
    private void RaidAttack(){
        rb2D.velocity =myTransform.up *10* raidVelocityEfficiency;
        lineRenderer.enabled = false;
    }
}