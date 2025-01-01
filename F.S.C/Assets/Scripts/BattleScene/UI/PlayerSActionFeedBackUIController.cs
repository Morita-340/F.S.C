using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;

public class PlayerSActionFeedBackUIController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI PlayerCombatPowerText;
    [SerializeField]
    TextMeshProUGUI DefeatPointText;
    [SerializeField]
    PlayerUnitDestroyManagementScript PUDMS;
    bool defPChangeFlag = true;
    //UIとして表示されるスコア計算用パラメータ
    int defeatPoint = 0;
    //敵撃墜時に加算されるスコア計算用パラメータ
    int goalDefeatPoint = 0;
    float defPChangeTime = 0.1f;
    bool plComPChangeFlag = true;
    int playerCombatPower = 0;
    float plComPChangeTime = 0.1f;
    // Update is called once per frame
    void LateUpdate()
    {
        //プレイヤーの戦闘力の表示管理
        if(plComPChangeFlag && playerCombatPower != PUDMS.GetCombatPower()){
            if(PUDMS.GetCombatPower() != 0){//ゼロ除算対策
                //パラメータの変化の度合に応じてある程度UIの変更時間を変えられるようにする
                plComPChangeTime = (playerCombatPower / PUDMS.GetCombatPower() > 1) ? 1 : ((playerCombatPower / PUDMS.GetCombatPower() < 0.4f) ? 0.4f : playerCombatPower / PUDMS.GetCombatPower());
            }
            plComPChangeFlag = false;
            DOTween.To(() => playerCombatPower, (x) => playerCombatPower = x, PUDMS.GetCombatPower() , plComPChangeTime);
            StartCoroutine(UIStretch(PlayerCombatPowerText,plComPChangeTime));
        }else if(playerCombatPower == PUDMS.GetCombatPower()){plComPChangeFlag = true;}
        PlayerCombatPowerText.text = "PlayerCombatPower\n" +playerCombatPower.ToString();
        //撃墜スコアの表示管理
        if(defPChangeFlag && defeatPoint != goalDefeatPoint){
            if(goalDefeatPoint != 0){
                defPChangeTime = (defeatPoint / goalDefeatPoint > 1) ? 1 : ((defeatPoint / goalDefeatPoint < 0.4f) ? 0.4f : defeatPoint / goalDefeatPoint);
            }
            defPChangeFlag = false;
            DOTween.To(() => defeatPoint, (x) => defeatPoint = x, goalDefeatPoint , defPChangeTime);
            StartCoroutine(UIStretch(DefeatPointText,defPChangeTime));
        }else if(defeatPoint == goalDefeatPoint){defPChangeFlag = true;}
        DefeatPointText.text = "DefeatPoint\n" + defeatPoint.ToString();
    }
    IEnumerator UIStretch(TextMeshProUGUI text,float time){
        text.transform.DOScale(new Vector3(1, 0.6f,0.8f),time/4);
        yield return new WaitForSeconds(time/2);
        text.transform.DOScale(new Vector3(0.8f, 0.8f,0.8f),time/4);
    }
    public void AddDefeatPoint(int unitCombatPower){
        goalDefeatPoint += unitCombatPower;
        Debug.Log("PAFBUIC defeatpoint" + unitCombatPower + " "+ goalDefeatPoint);
    }
}
