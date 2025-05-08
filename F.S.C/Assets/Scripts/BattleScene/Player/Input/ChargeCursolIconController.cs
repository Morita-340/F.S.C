using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeCursolIconController : MonoBehaviour
{
    [SerializeField]
    private LineRenderer chargeCircle;
    [SerializeField]
    private LineRenderer fullChargeCircle;
    private int chargeCircleRange = 0;
    private int fullChargeCircleRange = 0;
    private float nowDischargeTime = 0;
    private float nowChargeTime = 0;
    private bool fullCharge = false;
    Vector3 CursolPosition;

    private void Start()
    {
    }

    private void Update()
    {
        DrawLine(chargeCircle,1,chargeCircleRange);
        DrawLine(fullChargeCircle,1.1f,fullChargeCircleRange);
    }
    private void DrawLine(LineRenderer lineRenderer,float inputRadiusEfficiency,int range){
        this.transform.localScale = new Vector2(this.transform.localScale.x, this.transform.localScale.x);
        CursolPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,0));
        this.transform.position = CursolPosition;

        float radius = this.transform.localScale.x * 1.5f * inputRadiusEfficiency;
        float lineWidth = radius * 0.1f;
        float posX = this.transform.position.x;
        float posY = this.transform.position.y;

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = range;

        var points = new Vector3[range];

        for (int i = 0; i < range; i++)
        {
            var rad = Mathf.Deg2Rad * (i * range / range);
            var x = posX + Mathf.Sin(rad) * radius;
            var y = posY + Mathf.Cos(rad) * radius;
            points[i] = new Vector3(x, y, 0);
        }

        lineRenderer.SetPositions(points);
    }
    /// <summary>
    /// チャージ時間をGUIで表示
    /// チャージできていればロックオン中は任意のタイミングでチャージ攻撃が出来る
    /// </summary>
    /// <param name="chargeTime">チャージ時間（敵にかざし続けている時間）</param>
    /// <returns>チャージ攻撃をするタイミングであるかどうか</returns>
    public void ChargeCommand(float inputTime,PlayerUnitAttackManagementScript PUAMS,bool lockOn)
    {
        int chargeAttackSpan = 2;
        float attackableTime = 2f;
        float chargeTime = 0.4f;
        int dischargeTime = 1;
        float time = inputTime;
        if(fullCharge){
            ChargeAttackCommand(chargeAttackSpan,attackableTime,chargeTime,time,PUAMS);
        }
        else{
            if(lockOn){
                //チャージ中　増える
                if(time <= chargeTime){
                    if(nowChargeTime <= chargeTime){
                    nowChargeTime += Time.deltaTime;
                    IconCharge(Color.cyan,Color.clear,nowChargeTime,chargeTime);
                    }
                //チャージ完了　満タン
                }else{
                    //チャージ攻撃　減っていく
                    //ChargeAttackCommand(chargeAttackSpan,attackableTime,chargeTime,time,PUAMS,TargetPosition);
                }
            }else{
                //かざしていない　減っていく
                if(nowChargeTime > 0){
                    nowChargeTime -= Time.deltaTime;
                }else{nowChargeTime = 0;} 
                IconDischarge(Color.cyan,dischargeTime - nowChargeTime * (dischargeTime / chargeTime),dischargeTime);
            }
        }
    }
    private void ChargeAttackCommand(int chargeAttackSpan,float attackableTime,float chargeTime,float time,PlayerUnitAttackManagementScript PUAMS)
    {
        if(Input.GetMouseButtonDown(1)){
            time = Mathf.Floor(time);
            PUAMS.ChargeAttack();
            //押し始めたタイミングで最速で一発撃てるようにしたいので、timeが如何なる値であろうと即時PUAMS.ChargeAttack内で弾丸を発射するようにする
            }
        if(Input.GetMouseButton(1)){
            if(nowChargeTime > 0){
            nowChargeTime -= Time.deltaTime;
            //PUAMS.ChargeAttack(time,chargeAttackSpan,attackableTime,TargetPosition);
            float dischargeInputTime = attackableTime - nowChargeTime *(attackableTime/chargeTime);
            IconDischarge(Color.cyan,dischargeInputTime,attackableTime);
            }else{nowChargeTime = 0;}
        }  
    }
    /// <summary>
    /// チャージアイコンの上昇
    /// </summary>
    /// <param name="inputColor">UIの色</param>
    /// <param name="inputTime">入力される時間情報</param>
    /// <param name="chargeTime">チャージに要する時間</param>
    public void IconCharge(Color inputColor,Color postColor,float inputTime, float chargeTime){
        float chargeTimeForAttackRatio = inputTime /chargeTime;//Debug.LogWarning(inputTime +" "+chargeTime);
        if(chargeTimeForAttackRatio < 0){chargeTimeForAttackRatio = 0;Debug.LogWarning("AAAA");}
        chargeCircle.startColor = inputColor;
        chargeCircle.endColor = inputColor;
        chargeCircleRange = (int)(chargeTimeForAttackRatio * 360 * 1 +1);
        fullChargeCircleRange = 361;
        if(chargeCircleRange >= 360){fullCharge = true;}
        fullChargeCircle.startColor = postColor;
        fullChargeCircle.endColor = postColor;
    }
    /// <summary>
    /// チャージアイコンの下降
    /// </summary>
    /// <param name="inputColor">UIの色</param>
    /// <param name="inputTime"></param>
    /// <param name="dischargeTime">アイコンの減衰時間＝攻撃可能時間</param>
    public void IconDischarge(Color inputColor,float inputTime,float dischargeTime){
        float timeForAttackRatio =0;
        if(inputTime <= dischargeTime)timeForAttackRatio = inputTime / dischargeTime;
        else{timeForAttackRatio = 1;}
        //else timeForAttackRatio = 1;Debug.LogWarning("AAAA");
        if(timeForAttackRatio < 0){timeForAttackRatio = 0;Debug.LogWarning("AAAA");}
        chargeCircle.startColor = inputColor;
        chargeCircle.endColor = inputColor;
        chargeCircleRange = (int)((1 - timeForAttackRatio) * 360 * 1);
        if(chargeCircleRange <= 0){fullCharge = false;}
        fullChargeCircle.startColor = Color.clear;
        fullChargeCircle.endColor = Color.clear;
    }
}
