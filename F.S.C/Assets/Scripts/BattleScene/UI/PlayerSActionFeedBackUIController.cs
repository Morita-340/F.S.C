using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class PlayerSActionFeedBackUIController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI PlayerCombatPowerText;
    [SerializeField]
    TextMeshProUGUI ScoreText;
    [SerializeField]
    TextMeshProUGUI ExplosionComboText;
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
    int explosionComboNum = 0;
    float explosionComboTime = 0;
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
            StartCoroutine(UIStretch(ScoreText,defPChangeTime));
            Debug.Log("PAFBUIC point" + defeatPoint + " "+ goalDefeatPoint+defPChangeFlag);
        }else if(defeatPoint == goalDefeatPoint){defPChangeFlag = true;
        Debug.Log("PAFBUIC pointA" + defeatPoint + " "+ goalDefeatPoint+defPChangeFlag);}
        ScoreText.text = "Score\n" + defeatPoint.ToString();

        //爆発コンボの表示管理
        if(explosionComboTime > 0){
            explosionComboTime -= Time.deltaTime;
            //猶予時間内なら表示
            ExplosionComboText.color = Color.white;
            //コンボ数に応じた表示をする
            if(explosionComboNum == 1){
                ExplosionComboText.text = "Good!";
            }else if(explosionComboNum == 2){
                ExplosionComboText.text = "Nice!!";
            }else if(explosionComboNum == 3){
                ExplosionComboText.text = "Great!!!!";
            }else if(explosionComboNum == 4){
                ExplosionComboText.text = "Amazing!!!!!!";
            }else if(explosionComboNum == 5){
                ExplosionComboText.text = "Excellent!!!!!!!!!";
            }else if(explosionComboNum >= 6){
                ExplosionComboText.text = "Marvelous!!!!!!!!!!!!!!!";
            }else{
                ExplosionComboText.text = "";
            }
        }else{
            //猶予時間外なら非表示
            ExplosionComboText.DOFade(0,1f);
            explosionComboTime = 0;
            explosionComboNum = 0;
        }
    }
    //IEnumerator GuradualNumChange(float before,float after,bool flag,float time){
    //    yield return new WaitUntil(() => DOTween.To(() => before, (x) => before = x,after, time));
    //}
    IEnumerator UIStretch(TextMeshProUGUI text,float time){
        text.transform.DOScale(new Vector3(1, 0.6f,0.8f),time/4);
        yield return new WaitForSeconds(time/2);
        text.transform.DOScale(new Vector3(0.8f, 0.8f,0.8f),time/4);
    }
    public void AddDefeatPoint(int unitCombatPower){
        goalDefeatPoint += unitCombatPower;
        defPChangeFlag = true;
        Debug.Log("PAFBUIC defeatpoint" + unitCombatPower + " "+ goalDefeatPoint);
    }
    public void ExplosionOcurre(UnitData unitData){
        if(unitData.ReturnThisUnit() is CoreBase coreBase ||unitData.ReturnThisUnit() is ReactorBase reactorBase){
            //爆発するたびに爆発コンボのUIテキストのアニメーション実行
            UIStretch(ExplosionComboText,1f);
            //コンボ猶予期間の延長
            explosionComboTime =1;
            //コンボ数の上昇
            explosionComboNum ++;
        }
    }
}
