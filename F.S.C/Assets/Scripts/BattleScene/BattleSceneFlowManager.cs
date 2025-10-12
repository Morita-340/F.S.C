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
    WaveStartEndUIController WSEUIC;
    [SerializeField]
    PlayerSActionFeedBackUIController PSAFBUIC;
    [SerializeField] ResultUIController RUC;
    GameObject[] WaveEnemyList = new GameObject[(int)GSetting.UniqueMagicNumber.WaveEnemyListLength];
    [SerializeField,ReadOnly]
    GeneralFlagManager GFM;
    [SerializeField]
    MouseInput MI;
    [SerializeField]
    MainCameraController MCC;
    [SerializeField]StageStartTrigger SST;
    [SerializeField]
    CameraPlayerHomingTrigger CPHT;
    [SerializeField]GameObject PlayerSpawnPoint;
    [SerializeField]GameObject PauseMenu;
    [SerializeField]GameObject MotherShip;
    
    GSetting.ResultSituation situation;
    GameObject Player;
    private bool firstUpdate = true;
    private bool isBattleEnd = false;
    void Awake(){
        SpS = this.GetComponent<SpawnSystem>();
        GFM = FindObjectOfType<GeneralFlagManager>();
        Player = Instantiate(GFM.GetSelectedPlayer().Item1,PlayerSpawnPoint.transform.position,Quaternion.Euler(Vector3.zero));
        PUDMS = Player.GetComponent<PlayerUnitDestroyManagementScript>();
        SpS.SetPUDMS(PUDMS);
        MI.SetPlayer(Player);
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("BSFM waveNum "+waveNum);
        StartCoroutine(GameFlow());
    }
    // Update is called once per frame
    void Update()
    {
        if(firstUpdate){
            firstUpdate = false;
            Player.GetComponent<PlayerUnitAttackManagementScript>().SetChargeWeapon(GFM.GetSelectedPlayer().Item2.GetComponent<WeaponBase>());
        }
        playerIsDead = PUDMS.GetIsDead();
        //スペースボタンでポーズ画面
        if(!isBattleEnd && Input.GetKeyDown(KeyCode.Space)){
            Pause();
        }
    }
    IEnumerator GameFlow(){
        yield return StartCoroutine(StartPerformance());
        while(nowWave <= waveNum){
            Debug.Log("BSFM "+nowWave);
            //最終ウェーブであるかを判定する
            bool finalwaveFlag = nowWave == waveNum;
            //ウェーブ開始演出
            yield return WSEUIC.WaveStartUI(nowWave,finalwaveFlag,GFM.GetStageName());
            //敵リストをリセット
            for(int i = 0; i < WaveEnemyList.Length;i ++)
            {
                WaveEnemyList[i] = null;
            }
            //敵スポーン
            WaveEnemyList = SpS.Spawn(finalwaveFlag);
            //WaveEnemyListの全機体が撃墜されるまで次のループに移らない
            //PUDMSの撃墜判定が有効ならループを直ぐに抜ける
            yield return new WaitUntil(() => AllEnemyDead(WaveEnemyList) || playerIsDead);
            if(playerIsDead){
                situation = GSetting.ResultSituation.PlayerDestroyed;
                Debug.Log("BSFM PlayerDead");
                yield return new WaitForSeconds(3f);
                break;}
            NoDamageClearWaveNumCountUp();
            WSEUIC.WaveClearUI();
            yield return new WaitForSeconds(3f);
            if(nowWave>=waveNum){situation = GSetting.ResultSituation.AllWaveClear;}
            nowWave ++;
        }
        isBattleEnd = true;

        //最終ウェーブまで到達またはプレイヤーが撃墜されたのでスコア計算を行いバトルを終える
        BattleEndProcess(situation);
    }
    IEnumerator StartPerformance(){
        WSEUIC.StageEntryUI(GFM.GetStageName());
        yield return new WaitForSeconds(0.2f);
        MCC.EngineIgniteShake();
        Player.GetComponent<PlayerUnitMoveManagementScript>().AutoPilot(0);
        //画面中央に来たら追従開始
        yield return new WaitUntil(() => CPHT.CameraPlayerHoming());
        MCC.SetIsPlayerHomingTrue();
        StartCoroutine(PSAFBUIC.UIStartUp());
        //母艦が画面外になるよう（アナログ的に設定）移動できれば処理終了。戦闘開始
        yield return new WaitUntil(() => SST.StageStart());
        Player.GetComponent<PlayerUnitMoveManagementScript>().AutoPilot(2);
        //母艦は見た目だけなので戦闘中は非表示
        MotherShip.SetActive(false);
        WSEUIC.StageEntryUIFade();
    }
    /// <summary>
    /// 敵機が全て撃墜されるまで処理を中断する
    /// </summary>
    /// <param name="WaveEnemyList"></param>
    /// <returns></returns>
    IEnumerator WaitUntilAllEnemyDead(GameObject[] WaveEnemyList){
        yield return new WaitUntil(() => AllEnemyDead(WaveEnemyList));
        Debug.Log("BSFM AllEnemyDead");
    }
    /// <summary>
    /// 敵機が全て撃墜されたのか調べる
    /// </summary>
    /// <param name="WaveEnemyList"></param>
    /// <returns></returns>
    private bool AllEnemyDead(GameObject[] WaveEnemyList){
        for(int i = 0; i < WaveEnemyList.Length; i++){
            if(WaveEnemyList[i] == null){continue;}
            if(WaveEnemyList[i]?.GetComponent<EnemyUnitDestroyManagementScript>().GetIsDead() == false){
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// ポーズメニューのONを制御する
    /// 物理的にツマミとボタンが有効になる
    /// </summary>
    private void Pause(){
        //ポーズメニューの有効化
        PauseMenu.transform.localScale = new Vector3(1,1,1);
        //FixedUpdateが呼ばれなくなり、運動中の物体が静止する
        Time.timeScale = 0;
        //SEやBGMを一時停止
        WSEUIC.PauseUI();
    }
    public void UnPause(){
        PauseMenu.transform.localScale=new Vector3(0,1,1);
        Time.timeScale = 1;
        //SEやBGMの一時停止を解除
        WSEUIC.UnPauseUI();
    }
    private void BattleEndProcess(GSetting.ResultSituation situation){
        WSEUIC.BattleEnd();
        StartCoroutine(RUC.ResultUI(situation));
    }
    private void NoDamageClearWaveNumCountUp(){
        if(PUDMS.GetNoDamageFlag()){RUC.noDamageClearWaveNumCountUp();}
        PUDMS.noDamageFlagReset();
    }
}
