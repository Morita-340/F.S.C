using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using FSCGeneral;

public class PlayerSActionFeedBackUIController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI PlayerCombatPowerText;
    [SerializeField]
    TextMeshProUGUI ScoreText;
    [SerializeField]
    TextMeshProUGUI ExplosionComboText;
    [SerializeField, ReadOnly]
    RefinePlayerUnitDestroyManagementScript RePUDMS;
    [SerializeField, ReadOnly]
    PlayerUnitMoveManagementScript PUMMS;
    [SerializeField] GameObject WeaponControllConectedUIText;
    /*:::::::::::::::::*/
    [SerializeField]
    StatusUI PlayerAttackPower;
    [SerializeField]
    StatusUI PlayerHP;
    [SerializeField]
    StatusUI PlayerThrustPower;
    [SerializeField]
    StatusUI NumOfJointUnit;
    [SerializeField]
    HUD EventGoal;
    [SerializeField]
    GameObject WreckInfo;
    [SerializeField]
    StatusUI WreckAttackPower;
    [SerializeField]
    StatusUI WreckHP;
    [SerializeField]
    HUD HowToOpenPoseMenu;
    [SerializeField]
    HUD ExplainText;
    Camera MainCamera;
    List<HUD> HUDList;
    /*:::::::::::::::::*/
    bool defPChangeFlag = true;
    /// <summary>
    /// UIとして表示されるスコア計算用パラメータ
    /// </summary>
    int defeatPoint = 0;
    /// <summary>
    /// 敵撃墜時に加算されるスコア計算用パラメータ
    /// </summary>
    int goalDefeatPoint = 0;
    float defPChangeTime = 0.1f;
    bool plComPChangeFlag = true;
    int playerCombatPower = 0;
    int maximumPlayerCombatPower = 0;
    /// <summary>
    /// プレイヤーの戦闘力値変更アニメーションを実行するにあたって最終的に辿り着く値
    /// </summary>
    int goalPlayerCombatpower = 0;
    float plComPChangeTime = 0.1f;
    int explosionComboNum = 0;
    float explosionComboTime = 0;
    int noDamageClearedWaveNum = 0;
    private void Start()
    {
        MainCamera = FindObjectOfType<Camera>();
        RePUDMS = FindObjectOfType<RefinePlayerUnitDestroyManagementScript>();
        PUMMS = FindObjectOfType<PlayerUnitMoveManagementScript>();
        HUDList = new List<HUD> { PlayerAttackPower, PlayerHP, PlayerThrustPower, NumOfJointUnit, EventGoal, WreckAttackPower, WreckHP, HowToOpenPoseMenu,ExplainText };
        foreach (HUD hud in HUDList)
        {
            hud.SetCamera(MainCamera);
        }
    }
    // Update is called once per frame
    void Update()
    {
        PlayerCombatPowerUIDisplay();
        PlayerDefeatPointUIDisplay();
        ExplosionComboUIDisplay();
        PlayerInfoDisplay();
    }
    public IEnumerator UIStartUp()
    {
        transform.DOScaleY(1, 0.2f);
        yield break;
    }
    /// <summary>
    /// 数値変化したときにUIを伸び縮みさせる
    /// </summary>
    /// <param name="text">変形させる対象のUGUI</param>
    /// <param name="time">変形時間の総時間。1:2:1=伸ばす:伸ばしたまま:縮めるの割合</param>
    /// <returns></returns>
    IEnumerator UIStretch(TextMeshProUGUI text, float time)
    {
        text.transform.DOScale(new Vector3(1, 0.6f, 0.8f), time / 4);
        yield return new WaitForSeconds(time / 2);
        text.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), time / 4);
    }
    /// <summary>
    /// 外部で呼び出してUIの撃破ポイントを加算する仕組み
    /// </summary>
    /// <param name="unitCombatPower">破壊したユニットの持つ戦闘力＝撃破ポイント</param>
    public void AddDefeatPoint(int unitCombatPower)
    {
        goalDefeatPoint += unitCombatPower;
        defPChangeFlag = true;
    }
    /// <summary>
    /// 外部で呼び出すことでコアやリアクターが爆発した際に爆発コンボのUIを動かすメソッド
    /// </summary>
    /// <param name="unitData"></param>
    public void ExplosionOcurre(UnitData unitData)
    {
        if (unitData.ReturnThisUnit() is CoreBase coreBase || unitData.ReturnThisUnit() is ReactorBase reactorBase)
        {
            //爆発するたびに爆発コンボのUIテキストのアニメーション実行
            UIStretch(ExplosionComboText, 1f);
            //コンボ猶予期間の延長
            explosionComboTime = 1;
            //コンボ数の上昇
            explosionComboNum++;
        }
    }
    /// <summary>
    /// プレイヤーの戦闘力が変化（被弾による分離or合体）した際にUIの数値を変更する処理
    /// </summary>
    private void ChangePlayerCombatPower()
    {
        if (goalPlayerCombatpower != RePUDMS.GetCombatPower())
        {
            goalPlayerCombatpower = RePUDMS.GetCombatPower();
            plComPChangeFlag = true;
        }
        if (maximumPlayerCombatPower < RePUDMS.GetCombatPower()) maximumPlayerCombatPower = RePUDMS.GetCombatPower();
    }
    /// <summary>
    /// プレイヤーの戦闘力の表示管理
    /// </summary>
    private void PlayerCombatPowerUIDisplay()
    {
        ChangePlayerCombatPower();
        if (plComPChangeFlag && playerCombatPower != RePUDMS.GetCombatPower())
        {
            if (RePUDMS.GetCombatPower() != 0)
            {//ゼロ除算対策
                //パラメータの変化の度合に応じてある程度UIの変更時間を変えられるようにする
                plComPChangeTime = (playerCombatPower / RePUDMS.GetCombatPower() > 1) ? 1 : ((playerCombatPower / RePUDMS.GetCombatPower() < 0.4f) ? 0.4f : playerCombatPower / RePUDMS.GetCombatPower());
            }
            plComPChangeFlag = false;
            DOTween.To(() => playerCombatPower, (x) => playerCombatPower = x, RePUDMS.GetCombatPower(), plComPChangeTime);
            StartCoroutine(UIStretch(PlayerCombatPowerText, plComPChangeTime));
        }
        else if (playerCombatPower == RePUDMS.GetCombatPower()) { plComPChangeFlag = true; }
        PlayerCombatPowerText.text = "PlayerCombatPower\n" + playerCombatPower.ToString();
    }
    /// <summary>
    /// 撃墜スコアの表示管理
    /// </summary>
    private void PlayerDefeatPointUIDisplay()
    {
        if (defPChangeFlag && defeatPoint != goalDefeatPoint)
        {
            if (goalDefeatPoint != 0)
            {
                defPChangeTime = (defeatPoint / goalDefeatPoint > 1) ? 1 : ((defeatPoint / goalDefeatPoint < 0.4f) ? 0.4f : defeatPoint / goalDefeatPoint);
            }
            defPChangeFlag = false;
            DOTween.To(() => defeatPoint, (x) => defeatPoint = x, goalDefeatPoint, defPChangeTime);
            StartCoroutine(UIStretch(ScoreText, defPChangeTime));
            //Debug.Log("PAFBUIC point" + defeatPoint + " "+ goalDefeatPoint+defPChangeFlag);
        }
        else if (defeatPoint == goalDefeatPoint)
        {
            defPChangeFlag = true;
            //Debug.Log("PAFBUIC pointA" + defeatPoint + " "+ goalDefeatPoint+defPChangeFlag);
        }
        ScoreText.text = "Score\n" + defeatPoint.ToString();
    }
    /// <summary>
    /// 爆発コンボの表示管理
    /// </summary>
    private void ExplosionComboUIDisplay()
    {
        if (explosionComboTime > 0)
        {
            explosionComboTime -= Time.deltaTime;
            //猶予時間内なら表示
            ExplosionComboText.color = Color.white;
            //コンボ数に応じた表示をする
            if (explosionComboNum == 1)
            {
                ExplosionComboText.text = "Good!";
            }
            else if (explosionComboNum == 2)
            {
                ExplosionComboText.text = "Nice!!";
            }
            else if (explosionComboNum == 3)
            {
                ExplosionComboText.text = "Great!!!!";
            }
            else if (explosionComboNum == 4)
            {
                ExplosionComboText.text = "Amazing!!!!!!";
            }
            else if (explosionComboNum == 5)
            {
                ExplosionComboText.text = "Excellent!!!!!!!!!";
            }
            else if (explosionComboNum >= 6)
            {
                ExplosionComboText.text = "Marvelous!!!!!!!!!!!!!!!";
            }
            else
            {
                ExplosionComboText.text = "";
            }
        }
        else
        {
            //猶予時間外なら非表示
            ExplosionComboText.DOFade(0, 1f);
            explosionComboTime = 0;
            explosionComboNum = 0;
        }
    }
    /// <summary>
    /// ResuleUIControllerで使用するためのパラメータを渡す
    /// </summary>
    /// <returns>最終撃破ポイント</returns>
    /// <returns>最大戦闘力</returns>
    /// <returns>ノーダメージでクリアしたウェーブ数</returns>
    public (int, int) GetPlayerFinalStatus()
    {
        return (goalDefeatPoint, maximumPlayerCombatPower);
    }
    public IEnumerator Wait(float time, List<GameObject> ParentObjectList)
    {
        //再生成処理中の処理なので、制御ユニットが生成される前に下記処理が行われないために僅かに待つ
        yield return new WaitForSeconds(time);
        foreach (GameObject obj in ParentObjectList)
        {
        }
    }
    /// <summary>
    /// 武器を鹵獲できた場合にテキスト表示をする
    /// </summary>
    /// <param name="fadeTime"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    IEnumerator WaitUntilSuccessTextFade(float fadeTime, GameObject obj)
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(obj.transform.position);
        RectTransform uiRectTransform;
        uiRectTransform = GetComponent<RectTransform>();
        uiRectTransform = GameObject.Find(GSetting.UniqueObjectName.UICanvas.ToString()).GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiRectTransform,
            screenPosition,
            Camera.main,
            out Vector2 localPosition
        );
        GameObject SuccessText = Instantiate(WeaponControllConectedUIText, uiRectTransform.transform);
        SuccessText.GetComponent<RectTransform>().anchoredPosition = screenPosition + localPosition;
        SuccessText.GetComponent<TextMeshProUGUI>().DOFade(0f, fadeTime);
        yield return new WaitForSeconds(fadeTime);
        Destroy(SuccessText);

    }
    public void SetEventGoalText(string goalText)
    {
        EventGoal.GetComponent<TextMeshProUGUI>().text = goalText;
    }
    /// <summary>
    /// プレイヤーのステータスに関するUI表示
    /// </summary>
    private void PlayerInfoDisplay()
    {
        if (RePUDMS.GetIsDead())
        {
            PlayerAttackPower.SetValue(0);
            PlayerHP.SetValue(RePUDMS.GetPrimeUnitsHP());
            PlayerThrustPower.SetValue(0);
            NumOfJointUnit.SetValue(0);
        }
        else
        {
            PlayerAttackPower.SetValue(RePUDMS.GetAttackPower());
            PlayerHP.SetValue(RePUDMS.GetPrimeUnitsHP());
            PlayerThrustPower.SetValue((int)(PUMMS.GetMaximumSpeed() * 10));
            NumOfJointUnit.SetValue(RePUDMS.GetAllEmptyPassiveJointSPositionList().Count);
        }
    }
    public IEnumerator WreckInfoOpen(RefineDestroyedUnitManagementScript ReDUMS)
    {
        WreckInfo.transform.DOScaleY(1, 0.2f);
        WreckInfoDisplay(ReDUMS);
        yield break;
    }
    public IEnumerator WreckInfoClose()
    {
        WreckInfo.transform.DOScaleY(0, 0.2f);
        WreckInfoReset();
        yield break;
    }
    /// <summary>
    /// 残骸のステータスに関するUI表示
    /// </summary>
    private void WreckInfoDisplay(RefineDestroyedUnitManagementScript ReDUMS)
    {
        WreckAttackPower.SetValue(ReDUMS.GetAttackPower());
        WreckHP.SetValue(ReDUMS.GetHP());
    }
    private void WreckInfoReset()
    {
        WreckAttackPower.SetValue(0);
        WreckHP.SetValue(0);
    }
}
