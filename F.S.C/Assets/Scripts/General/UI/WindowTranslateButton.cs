using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using FSCGeneral;

public class WindowTranslateButton : GeneralUIIconController
{
    float nowChargeTime = 0;
    float nowExecuteTime = 0;
    [SerializeField,Range(0.5f,2f)]float chargeThreshold = 1;
    [SerializeField]RectTransform ChargeSlider;
    [SerializeField]RectTransform ExecuteSlider;
    [SerializeField]protected GSetting.SceneName TranslateScene;
    float initImageWide;
    bool executeFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        initImageWide = ChargeSlider.sizeDelta.x;
    }

    // Update is called once per frame
    protected override void Update()
    {
        ButtonCharge();
        ChargeTimeSlider();
        if(executeFlag){
            ExecuteUISlider();
            StartCoroutine(SceneTranslate());
        }
        base.Update();
    }
    void ButtonCharge(){
        //ボタンにカーソルを合わせてチャージしないと進めない仕様
        if(CursolSelected){
            nowChargeTime += Time.deltaTime;
        }
        else{
            nowChargeTime -= Time.deltaTime;
            if(nowChargeTime < 0)nowChargeTime = 0;
        }
        if(nowChargeTime > chargeThreshold){
            //ボタンの効力発揮
            executeFlag = true;
        }
    }
    void ChargeTimeSlider(){
        //チャージ度合いに応じてボタンの見た目を変える
        float chargeRate = nowChargeTime / chargeThreshold;
        if(chargeRate >1 )chargeRate = 1;
        ChargeSlider.sizeDelta = new Vector2(chargeRate*initImageWide,ChargeSlider.sizeDelta.y);
    }
    IEnumerator SceneTranslate(){
        if(TranslateScene == null){Debug.LogAssertion("TranslateScene is null");yield break;}
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(TranslateScene.ToString());
    }
    void ExecuteUISlider(){
        //チャージ完了なら見た目を更に変える
        nowExecuteTime += Time.deltaTime;
        float executeRate = nowExecuteTime/0.5f;
        if(executeRate >1)executeRate = 1;
        ExecuteSlider.sizeDelta = new Vector2(executeRate*initImageWide,ExecuteSlider.sizeDelta.y);
    }
}
