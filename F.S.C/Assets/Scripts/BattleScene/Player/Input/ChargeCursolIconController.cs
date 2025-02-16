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
        Debug.LogWarning(range);
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
    public void ChargeCommand(float inputTime,PlayerUnitAttackManagementScript PUAMS,Vector3 TargetPosition,bool lockOn)
    {
        int chargeAttackSpan = 3;
        int attackableTime = 2;
        float time = inputTime;
        if(lockOn){
            //チャージ中
            if(time <= chargeAttackSpan - attackableTime){
                nowChargeTime += Time.deltaTime;
                //else{nowChargeTime -= Time.deltaTime;}
                IconCharge(Color.cyan,Color.clear,nowChargeTime,chargeAttackSpan - attackableTime);
                nowDischargeTime = 0;
                //chargeCircle.startColor = Color.red;
                //chargeCircle.endColor = Color.red;
                //chargeCircleRange = (int)(chargeTime * 121) * 1;
            //チャージ完了
            }else{
                nowChargeTime = 0;
                //IconCharge(Color.clear,Color.cyan,0,1);
                //チャージ攻撃
                if(Input.GetMouseButtonDown(1)){time = (time % chargeAttackSpan > attackableTime) ? time:(time % chargeAttackSpan > attackableTime -1)? time + 1:time + 2;}//押し始めたタイミングで最速で一発撃てるようにしたい
                if(Input.GetMouseButton(1)){
                    if(nowDischargeTime < attackableTime){
                    nowDischargeTime += Time.deltaTime;
                    PUAMS.ChargeAttack(time,chargeAttackSpan,attackableTime,TargetPosition);
                    IconDischarge(Color.cyan,nowDischargeTime,attackableTime);
                    }
                }else{nowDischargeTime = 0;}
                //chargeCircle.startColor = Color.green;
                //chargeCircle.endColor = Color.green;
                //chargeCircleRange = (int)((5 - chargeTime) * 180) * 1;
                }
        }else{
            nowDischargeTime = 0;
            nowChargeTime = 0;
            chargeCircle.startColor = Color.clear;
            chargeCircle.endColor = Color.clear;
        }
    }
    /// <summary>
    /// チャージアイコンの上昇
    /// </summary>
    /// <param name="inputColor">UIの色</param>
    /// <param name="inputTime">入力される時間情報</param>
    /// <param name="chargeTime">チャージに要する時間</param>
    public void IconCharge(Color inputColor,Color postColor,float inputTime, int chargeTime){
        float chargeTimeForAttackRatio = inputTime /chargeTime;
        if(chargeTimeForAttackRatio < 0){chargeTimeForAttackRatio = 0;}
        chargeCircle.startColor = inputColor;
        chargeCircle.endColor = inputColor;
        chargeCircleRange = (int)(chargeTimeForAttackRatio * 360 * 1 +1);
        Debug.LogWarning("aaa"+chargeTimeForAttackRatio +" "+chargeCircleRange + " "+chargeTime);
        fullChargeCircleRange = 361;
        fullChargeCircle.startColor = postColor;
        fullChargeCircle.endColor = postColor;
    }
    /// <summary>
    /// チャージアイコンの下降
    /// </summary>
    /// <param name="inputColor">UIの色</param>
    /// <param name="inputTime"></param>
    /// <param name="dischargeTime">アイコンの減衰時間＝攻撃可能時間</param>
    public void IconDischarge(Color inputColor,float inputTime,int dischargeTime){
        float timeForAttackRatio = inputTime / dischargeTime;
        if(timeForAttackRatio < 0){timeForAttackRatio = 0;}
        chargeCircle.startColor = inputColor;
        chargeCircle.endColor = inputColor;
        chargeCircleRange = (int)((1 - timeForAttackRatio) * 360 * 1);
        Debug.LogWarning("ccc"+timeForAttackRatio +" "+chargeCircleRange + " "+ dischargeTime +" "+inputTime );
        fullChargeCircle.startColor = Color.clear;
        fullChargeCircle.endColor = Color.clear;
    }
}
