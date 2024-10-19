using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeCursolIconController : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    private int circleRange = 72;
    Vector3 CursolPosition;

    private void Start()
    {
    }

    private void Update()
    {
        this.transform.localScale = new Vector2(this.transform.localScale.x, this.transform.localScale.x);
        CursolPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,0));
        this.transform.position = CursolPosition;

        float radius = this.transform.localScale.x * 1.5f;
        float lineWidth = radius * 0.1f;
        float posX = this.transform.position.x;
        float posY = this.transform.position.y;

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = circleRange;

        var points = new Vector3[circleRange];

        for (int i = 0; i < circleRange; i++)
        {
            var rad = Mathf.Deg2Rad * (i * circleRange / circleRange);
            var x = posX + Mathf.Sin(rad) * radius;
            var y = posY + Mathf.Cos(rad) * radius;
            points[i] = new Vector3(x, y, 0);
        }

        lineRenderer.SetPositions(points);
    }
    /// <summary>
    /// チャージ時間をGUIで表示
    /// </summary>
    /// <param name="chargeTime">チャージ時間</param>
    /// <returns>チャージ攻撃をするタイミングであるかどうか</returns>
    public void ChangeCircleRange(float time,PlayerUnitAttackManagementScript PUAMS,Vector3 TargetPosition){
        float chargeTime = time % 5;
        PUAMS.ChargeAttack(time,5,3,TargetPosition);
        if(chargeTime < 3){
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            circleRange = (int)(chargeTime * 121) * 1;
        }else{
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
            circleRange = (int)((5 - chargeTime) * 180) * 1;
            }
    }
}
