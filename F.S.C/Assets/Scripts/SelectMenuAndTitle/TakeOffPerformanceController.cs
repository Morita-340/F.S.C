using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TakeOffPerformanceController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    PlayerUnitMoveManagementScript PUMMS;
    [SerializeField, ReadOnly]
    PlayerSelectManager PSM;
    [SerializeField]
    SpriteRenderer Alert;
    [SerializeField]
    GameObject InstPosObj;
    bool alertRedBlinkingFlag = true;
    [SerializeField,ReadOnly]
    BodyFlag nowSelectedBody;
    float redvalue, greenvalue, bluevalue, alphavalue = 0f;
    [SerializeField,ReadOnly]
    SoundController SCer;
    // Start is called before the first frame update
    void Start()
    {
        SCer = GetComponent<SoundController>();
        //遷移演出発火まで点滅を続ける
        StartCoroutine(AlertRedBlink());
    }
    public void SetPSM(PlayerSelectManager PSM)
    {
        this.PSM = PSM;
    }

    // Update is called once per frame
    void Update()
    {
        Alert.color = new Color(redvalue,greenvalue,bluevalue,alphavalue);
    }
    public void SetMachine(BodyFlag inputBodyFlag)
    {
        if (nowSelectedBody != inputBodyFlag)
        {
            nowSelectedBody = inputBodyFlag;
            if (PUMMS != null)
            {
                Destroy(PUMMS.gameObject);
            }
            PUMMS = Instantiate(inputBodyFlag.Body, InstPosObj.transform.position, InstPosObj.transform.rotation).GetComponent<PlayerUnitMoveManagementScript>();
            PUMMS.SetNotDrive();
        }
    }
    public IEnumerator Perform()
    {
        //アフターバーナーちょび点火
        StartCoroutine(PUMMS.Ignition());
        alertRedBlinkingFlag = false;
        yield return new WaitForSeconds(1.0f);
        //アラート赤点滅処理実行
        //SCer.PlaySE(0);
        //yield return StartCoroutine(AlertRedBlink());
        //アラート緑点灯
        SCer.PlaySE(1);
        redvalue = 0; greenvalue = 1; bluevalue = 0;
        //機体発進！
        yield return StartCoroutine(PUMMS.TakeOFF());
        PSM.MenuClose();
        yield return null;
    }
    private IEnumerator AlertRedBlink()
    {
        //遷移発火するまで点滅を続ける
        float changeSpan = 0.7f;
        Debug.LogWarning("DDDDD");
        redvalue = 1;greenvalue = 0;bluevalue = 0;alphavalue = 1f;
        while (alertRedBlinkingFlag) {
            DOTween.To(() => alphavalue,(x) => alphavalue = x, 1f,changeSpan);
            yield return new WaitForSeconds(changeSpan);
            if (!alertRedBlinkingFlag) break;
            DOTween.To(() => alphavalue,(x) => alphavalue = x, 0f,changeSpan);
            yield return new WaitForSeconds(changeSpan);  
        }
        alphavalue = 1;
        yield break;

        //float changeSpan = 0.7f;
        //Debug.LogWarning("DDDDD");
        //redvalue = 1;greenvalue = 0;bluevalue = 0;alphavalue = 1f;
        //DOTween.To(() => alphavalue,(x) => alphavalue = x, 1f,changeSpan);
        //yield return new WaitForSeconds(changeSpan);
        //DOTween.To(() => alphavalue,(x) => alphavalue = x, 0f,changeSpan);
        //yield return new WaitForSeconds(changeSpan);
        //DOTween.To(() => alphavalue,(x) => alphavalue = x, 1f,changeSpan);
        //yield return new WaitForSeconds(changeSpan);
        //DOTween.To(() => alphavalue,(x) => alphavalue = x, 0f,changeSpan);
        //yield return new WaitForSeconds(changeSpan);
        //DOTween.To(() => alphavalue,(x) => alphavalue = x, 1f,changeSpan);
        //yield return new WaitForSeconds(changeSpan);
        //DOTween.To(() => alphavalue,(x) => alphavalue = x, 0f,changeSpan);
        //yield return new WaitForSeconds(changeSpan);
        //DOTween.To(() => alphavalue,(x) => alphavalue = x, 1f,changeSpan);
        //yield return new WaitForSeconds(1f);
        //yield break;
    }
}
