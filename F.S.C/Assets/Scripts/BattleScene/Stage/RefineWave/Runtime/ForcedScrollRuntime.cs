using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ForcedScrollRuntime : IWaveRuntime
{
    [SerializeField, ReadOnly]
    float nowTime = 0;
    [SerializeField, ReadOnly]
    MainCameraController MCC;
    float scrollTime = 0;
    [SerializeField, ReadOnly]
    Vector2 scrollStartPos = Vector2.zero;
    [SerializeField, ReadOnly]
    Vector2 scrollGoalPos = Vector2.zero;
    List<RefineEnemyUnitDestroyManagementScript> EnemyList;
    Vector3[] InstPosList;
    public void InitialSetting(RefinePlayerUnitDestroyManagementScript inputRePUDMS, List<RefineEnemyUnitDestroyManagementScript> EUDMSList, Vector3[] inputInstPosList, float inputScrollTime)
    {
        RePUDMS = inputRePUDMS;
        ReSpS = this.gameObject.AddComponent<RefineSpawnSystem>();
        EnemyList = EUDMSList;
        InstPosList = inputInstPosList;
        initialSettingFlag = true;
        scrollTime = inputScrollTime;
    }
    public void ScrollFlowCall(MainCameraController inputMCC, Vector2 StartPos, Vector2 GoalPos)
    {
        StartCoroutine(ScrollFlow(inputMCC, StartPos, GoalPos));
    }
    private IEnumerator ScrollFlow(MainCameraController inputMCC, Vector2 StartPos, Vector2 GoalPos)
    {
        Debug.LogAssertion("DDDD");
        //自機のスクロール設定(挙動設定)
        MCC = inputMCC;
        scrollStartPos = StartPos;
        scrollGoalPos = GoalPos;
        float flowTime = scrollTime;
        float spawnSpan = (30 < scrollTime / 4) ? scrollTime / 4 : 30;
        float nowTime = 0;
        //カメラのプレイヤー追従を解除
        MCC.SetPlayerHoming(false);
        //MCC側のスクロール初期設定　StartPosまで移動してもらう
        yield return StartCoroutine(MCC.MoveToScrollStartPos(StartPos, x => flowTime += x));
        //StartPosまで移動してもらったらGoalPosまでの移動開始
        StartCoroutine(MCC.MoveToScrollGoalPos(GoalPos, scrollTime));
        RePUDMS.gameObject.GetComponent<PlayerUnitMoveManagementScript>().ScrollEnable((GoalPos - StartPos)/scrollTime);
        do
        {
            //最短30秒ごと、最長scrollTime/4で生成を繰り返す
            foreach (RefineEnemyUnitDestroyManagementScript ReEUDMS in ReSpS.Spawn(RePUDMS.GetCombatPower(), EnemyList, InstPosList))
            {
                //敵機のスクロール設定(挙動設定)
                ReEUDMS.gameObject.GetComponent<EnemyUnitMoveManagementScript>().ScrollForce((GoalPos - StartPos).normalized);
                SpawnedEnemyList.Add(ReEUDMS);
            }
            nowTime += spawnSpan;
            yield return new WaitForSeconds(spawnSpan > scrollTime?scrollTime:spawnSpan);
        } while (nowTime <= scrollTime);
        //自機のスクロール設定(挙動設定)解除
        RePUDMS.gameObject.GetComponent<PlayerUnitMoveManagementScript>().ScrollDisable();
        yield return new WaitForSeconds(scrollTime - nowTime <0?0f:scrollTime - nowTime);
        //カメラのプレイヤー追従を開始（２連続のスクロールの場合ならその時に解除すればいい※全てのウェーブでMCC参照を取ってくるのが面倒くさいのでこうした）
        MCC.SetPlayerHoming(true);
        Debug.LogAssertion("JJHHHHH"+RePUDMS.GetIsDead());
        if (RePUDMS.GetIsDead())
        {
            failureFlag = true;
        }
        else
        {
            successFlag = true;
        }
        yield return null;
    }
    protected override void DetermineSuccessORFailure()
    {
        base.DetermineSuccessORFailure();
    }
    protected override void Update()
    {
        base.Update();

    }
}
