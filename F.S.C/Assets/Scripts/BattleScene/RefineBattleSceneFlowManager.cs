using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
using TMPro;

public class RefineBattleSceneFlowManager : MonoBehaviour
{
    [SerializeField, ReadOnly]
    BattleStageData BSD;
    RefinePlayerUnitDestroyManagementScript RePUDMS;
    PlayerUnitMoveManagementScript PUMMS;
    [SerializeField]
    WaveStartEndUIController WSEUIC;
    [SerializeField]
    PlayerSActionFeedBackUIController PSAFBUIC;
    [SerializeField] ResultUIController RUC;
    [SerializeField, ReadOnly]
    GeneralFlagManager GFM;
    [SerializeField]
    RefineMouseInput ReMI;
    [SerializeField]
    MainCameraController MCC;
    [SerializeField] StageStartTrigger SST;
    [SerializeField]
    CameraPlayerHomingTrigger CPHT;
    [SerializeField] GameObject PlayerSpawnPoint;
    [SerializeField] GameObject PauseMenu;
    [SerializeField, ReadOnly] WindowTranslateButton BackToSettingButton;
    [SerializeField, ReadOnly] WindowTranslateButton ReStartButton;
    [SerializeField] GameObject MotherShip;
    [SerializeField] TextMeshProUGUI EventGoal;
    [SerializeField] ExplainText explainText;
    GSetting.ResultSituation situation;
    GameObject Player;
    private bool isBattleEnd = false;
    void Awake()
    {
        GFM = FindObjectOfType<GeneralFlagManager>();
        Player = Instantiate(GFM.GetSelectedPlayer().Item1, PlayerSpawnPoint.transform.position, Quaternion.Euler(Vector3.zero));
        BSD = GFM.GetStageData();
        RePUDMS = Player.GetComponent<RefinePlayerUnitDestroyManagementScript>();
        PUMMS = Player.GetComponent<PlayerUnitMoveManagementScript>();
        ReMI.SetPlayer(Player);
        ReMI.SetPSAFBUIC(PSAFBUIC);
        BackToSettingButton = PauseMenu.transform.Find("BackToSettingButton").GetChild(0).GetComponent<WindowTranslateButton>();
        ReStartButton = PauseMenu.transform.Find("ReStartButton").GetChild(0).GetComponent<WindowTranslateButton>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameFlow());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBattleEnd && Input.GetKeyDown(KeyCode.Space))
        {
            Pause();
        }
    }
    IEnumerator GameFlow()
    {
        yield return StartCoroutine(StartPerformance());
        List<RefineWaveData> waveDataList = BSD.GetWaveDataList();
        for (int i = 0; i < waveDataList.Count; i++)
        {
            RefineWaveData waveData = waveDataList[i];
            int nowWaveCount = i + 1;
            yield return WSEUIC.WaveStartUI(nowWaveCount, nowWaveCount == waveDataList.Count, GFM.GetStageName());
            //ウェーブの目的を大まかにテキスト表示
            EventGoal.text = waveData.GetGoalText();
            //説明テキストのテキスト設定＋配置
            //explainText?.SetText(waveData.GetExplainText(), waveData.GetExplainTexPos());
            Debug.LogWarning("WWWWWWWW"+waveDataList.Count);
            IWaveRuntime runtime = waveData.InitialSetting(this, RePUDMS);
            //スクロールするならカメラも設定が必要
            if (waveData is ScrollWaveIF SWIF) { Debug.LogAssertion("DDDDAA"); SWIF.SetMCC(MCC); }
            yield return new WaitUntil(() => runtime.GetSuccessFlag() || runtime.GetFailureFlag());
            //最終ウェーブでボス撃破時は爆発演出が入るため、それ以降は機体操作受付拒否+無敵+ポーズ画面開くのを拒否
            if (nowWaveCount == waveDataList.Count && runtime.GetSuccessFlag())
            {
                //無敵
                RePUDMS.SetInvincible(true);
                yield return new WaitForSeconds(3f);
            }
            if (runtime.GetFailureFlag())
            {
                situation = GSetting.ResultSituation.MissionFailed;
                break;
            }
            waveData.DestroyProcess(this);
            NoDamageClearWaveNumCountUp();
            if (nowWaveCount == waveDataList.Count) { }
            else
            {
                WSEUIC.WaveClearUI();
                yield return new WaitForSeconds(3f);
            }
            if (nowWaveCount == waveDataList.Count) { situation = GSetting.ResultSituation.AllWaveClear; }
        }
        isBattleEnd = true;
        //最終ウェーブまで到達またはプレイヤーが撃墜されたのでスコア計算を行いバトルを終える
        StartCoroutine(BattleEndProcess(situation));
    }
    IEnumerator StartPerformance()
    {
        WSEUIC.StageEntryUI(BSD.GetStageName());
        //yield return new WaitForSeconds(0.2f);
        //MCC.EngineIgniteShake();
        PUMMS.AutoPilot(0);
        //画面中央に来たら追従開始
        yield return new WaitUntil(() => CPHT.CameraPlayerHoming());
        WSEUIC.CurtainOFF();
        MCC.SetIsPlayerHomingTrue();
        StartCoroutine(PSAFBUIC.UIStartUp());
        //母艦が画面外になるよう（アナログ的に設定）移動できれば処理終了。戦闘開始
        yield return new WaitUntil(() => SST.StageStart());
        PUMMS.AutoPilot(2);
        //母艦は見た目だけなので戦闘中は非表示
        MotherShip.SetActive(false);
    }
    /// <summary>
    /// ポーズメニューのONを制御する
    /// 物理的にツマミとボタンが有効になる
    /// </summary>
    private void Pause()
    {
        //ポーズメニューの有効化
        PauseMenu.transform.localScale = new Vector3(1, 1, 1);
        //ポーズメニューのUIボタンの有効化
        BackToSettingButton.Executable(true);
        ReStartButton.Executable(true);
        //FixedUpdateが呼ばれなくなり、運動中の物体が静止する
        Time.timeScale = 0;
        //SEやBGMを一時停止
        WSEUIC.PauseUI();
    }
    public void UnPause()
    {
        //ポーズメニューの無効化
        PauseMenu.transform.localScale = new Vector3(0, 1, 1);
        //ポーズメニューのUIボタンの無効化
        BackToSettingButton.Executable(false);
        ReStartButton.Executable(false);
        Time.timeScale = 1;
        //SEやBGMの一時停止を解除
        WSEUIC.UnPauseUI();
    }
    private IEnumerator BattleEndProcess(GSetting.ResultSituation situation)
    {
        WSEUIC.BattleEnd();
        StartCoroutine(RUC.ResultUI(situation));
            Debug.LogAssertion(PUMMS.transform.position+"VVVV1" +BSD.GetAfterCleared1stMovePos());
        //一定時間経ったら強制的にステージ選択画面に戻る必要があるためタイマー計測開始
        StartCoroutine(ForceBackToStageSelect());
        //向きと中継地点を決める
        yield return StartCoroutine(PUMMS.SetAutoPilotVector(BSD.GetAfterCleared1stMovePos()));
            Debug.LogAssertion(PUMMS.transform.position+"VVVV2" +BSD.GetAfterCleared1stMovePos());
        //オートパイロットで進む
        PUMMS.AutoPilot(4);
        //中継地点に到達
        yield return new WaitUntil(() => (PUMMS.transform.position - BSD.GetAfterCleared1stMovePos()).magnitude <3);
        //さらに向きを変えて
        yield return StartCoroutine(PUMMS.SetAutoPilotVector(BSD.GetAfterClearedLastMovePos()));
        //オートパイロット
        PUMMS.AutoPilot(4);
    }
    /// <summary>
    /// ステージ終了後リザルト画面から3分以上遷移操作を行わなかった場合に強制的にステージ画面へ遷移する仕組み
    /// </summary>
    /// <returns></returns>
    private IEnumerator ForceBackToStageSelect()
    {
            Debug.LogAssertion("VVVV3");
        yield return new WaitForSeconds(180);
            Debug.LogAssertion("VVVV4");
        //遷移ボタンを押した判定
        RUC.BackToStageSelect();
    }
    private void NoDamageClearWaveNumCountUp()
    {
        if (RePUDMS.GetNoDamageFlag()) { RUC.noDamageClearWaveNumCountUp(); }
        RePUDMS.noDamageFlagReset();
    }
}
