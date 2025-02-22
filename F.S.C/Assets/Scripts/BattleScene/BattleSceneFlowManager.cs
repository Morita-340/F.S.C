using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;


public class BattleSceneFlowManager : MonoBehaviour
{
    int waveNum = 3;
    int nowWave = 1;
    bool playerIsDead = false;
    SpawnSystem SpS;
    PlayerUnitDestroyManagementScript PUDMS;
    [SerializeField]
    WaveStartEndUIController WCEUIC;
    [SerializeField] ResultUIController RUC;
    List<GameObject> WaveEnemyList = new List<GameObject>();
    [SerializeField,ReadOnly]
    GeneralFlagManager GFM;
    [SerializeField]
    MouseInput MI;
    string situation;
    void Awake(){
        SpS = this.GetComponent<SpawnSystem>();
        GFM = FindObjectOfType<GeneralFlagManager>();
        GameObject Player = Instantiate(GFM.GetSelectedPlayer().Item1,Vector3.zero,Quaternion.Euler(Vector3.zero));
        Player.GetComponent<PlayerUnitAttackManagementScript>().SetChargeWeapon(GFM.GetSelectedPlayer().Item2.GetComponent<WeaponBase>());
        PUDMS = Player.GetComponent<PlayerUnitDestroyManagementScript>();
        SpS.SetPUDMS(PUDMS);
        MI.SetPlayer(Player);
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("BSFM waveNum "+waveNum);
        StartCoroutine(GameFlow());
        //while(nowWave <= waveNum){
        //    Debug.Log("BSFM "+nowWave);
        //    WaveEnemyList = SpS.Spawn(nowWave);
        //    nowWave ++;
        //    //PUDMSの撃墜判定が有効ならループを直ぐに抜ける
        //    if(playerIsDead){Debug.Log("BSFM PlayerDead");break;}
        //    //WaveEnemyListの全機体が撃墜されるまで次のループに移らない
        //    StartCoroutine(WaitUntilAllEnemyDead(WaveEnemyList));
        //}
        ////最終ウェーブまで到達またはプレイヤーが撃墜されたのでスコア計算を行いバトルを終える
        //BattleEndProcess();
    }
    IEnumerator GameFlow(){
        while(nowWave <= waveNum){
            Debug.Log("BSFM "+nowWave);
            //最終ウェーブであるかを判定する
            bool finalwaveFlag = nowWave == waveNum;
            //ウェーブ開始演出
            yield return WCEUIC.WaveStartUI(nowWave,finalwaveFlag,GFM.GetStageName());
            //yield return new WaitForSeconds(0.1f);
            //敵スポーン
            WaveEnemyList = SpS.Spawn(finalwaveFlag);
            nowWave ++;
            //WaveEnemyListの全機体が撃墜されるまで次のループに移らない
            //PUDMSの撃墜判定が有効ならループを直ぐに抜ける
            yield return new WaitUntil(() => AllEnemyDead(WaveEnemyList) || playerIsDead);
            if(playerIsDead){
                situation = GSetting.ResultSituation.PlayerDestroyed.ToString();
                Debug.Log("BSFM PlayerDead");
                yield return new WaitForSeconds(3f);
                break;}
            NoDamageClearWaveNumCountUp();
            WCEUIC.WaveClearUI();
            yield return new WaitForSeconds(3f);
            if(nowWave>=waveNum){situation = GSetting.ResultSituation.AllWaveClear.ToString();}
        }

        //最終ウェーブまで到達またはプレイヤーが撃墜されたのでスコア計算を行いバトルを終える
        BattleEndProcess(situation);
    }
    /// <summary>
    /// 敵機が全て撃墜されるまで処理を中断する
    /// </summary>
    /// <param name="WaveEnemyList"></param>
    /// <returns></returns>
    IEnumerator WaitUntilAllEnemyDead(List<GameObject> WaveEnemyList){
        yield return new WaitUntil(() => AllEnemyDead(WaveEnemyList));
        Debug.Log("BSFM AllEnemyDead");
    }
    /// <summary>
    /// 敵機が全て撃墜されたのか調べる
    /// </summary>
    /// <param name="WaveEnemyList"></param>
    /// <returns></returns>
    private bool AllEnemyDead(List<GameObject> WaveEnemyList){
        foreach(GameObject WaveEnemy in WaveEnemyList){
            if(WaveEnemy?.GetComponent<EnemyUnitDestroyManagementScript>().GetIsDead() == false){
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        playerIsDead = PUDMS.GetIsDead();
        Debug.LogWarning("FFF" + PUDMS.GetIsDead());
    }
    private void BattleEndProcess(string situation){
        StartCoroutine(RUC.ResultUI(situation));
    }
    private void NoDamageClearWaveNumCountUp(){
        if(PUDMS.GetNoDamageFlag()){RUC.noDamageClearWaveNumCountUp();}
        PUDMS.noDamageFlagReset();
    }
}
