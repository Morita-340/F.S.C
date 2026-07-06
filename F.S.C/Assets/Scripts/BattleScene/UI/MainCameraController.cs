using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MainCameraController : MonoBehaviour
{
    Camera MainCam;
    GameObject Player;
    [SerializeField] bool Setting = false;
    int maxDistanse = 0;
    float zoomRatio = 60f;
    [SerializeField,ReadOnly]
    bool isPlayerHoming = false;
    bool isScrolling = false;
    /// <summary>
    /// プレイヤーに対するカーソルの位置次第でカーソル側にカメラを寄らせるためのオフセット
    /// </summary>
    private Vector3 cursolToPlayerOffset = Vector3.zero;
    BoolEdgeTrigger isPlayerHomingTrigger;
    private List<RefineAbstractUnitDestroyManagementScript> VisibleUnitList = new List<RefineAbstractUnitDestroyManagementScript>();
    public void AddToVisibleUnitList(RefineAbstractUnitDestroyManagementScript ReAUDMS)
    {
        if (!VisibleUnitList.Contains(ReAUDMS))
        {
            VisibleUnitList.Add(ReAUDMS);
            Debug.Log("MCC ListAdd");
        }
    }
    public void DeleteFromVisibleUnitList(RefineAbstractUnitDestroyManagementScript ReAUDMS)
    {
        if (VisibleUnitList.Contains(ReAUDMS)) { VisibleUnitList.Remove(ReAUDMS); }
    }
    // Start is called before the first frame update
    void Start()
    {
        isPlayerHomingTrigger = new BoolEdgeTrigger(() => isPlayerHoming);
        MainCam = this.GetComponent<Camera>();
        if (!Setting) Player = FindObjectOfType<RefinePlayerUnitDestroyManagementScript>().gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if (!Setting && isPlayerHoming) {
            SetCursolToPlayerOffset();
            this.transform.position = Player.transform.position + cursolToPlayerOffset + new Vector3(0, 0, -5);
            Debug.LogAssertion("WWWHHHWWWW"); }
        //ZoomRatioControll(maxDistanse);
    }
    private void SetCursolToPlayerOffset()
    {
        //カーソルの画面上の座標を取得
        Vector3 CursolPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        //カメラの中心座標を取得
        Vector3 CameraPos = transform.position;
        //差分ベクトルを取得
        Vector3 diffVec = CursolPos - CameraPos;
        float offsetX = 0;
        float offsetY = 0;
        //大きさが一定以上なら定数、それ未満なら割合で設定
        if (diffVec.sqrMagnitude < 3000)
        {
            offsetX = 0;
        }
        else if (diffVec.normalized.x > 0.7f)
        {
            offsetX = diffVec.normalized.x*22;
        }
        else if (diffVec.normalized.x < -0.7f)
        {
            offsetX = diffVec.normalized.x*22;
        }
        else
        {
            offsetX = 0;
        }
        if (diffVec.sqrMagnitude < 900)
        {
            offsetY = 0;
        }
        else if (diffVec.normalized.y > 0.4f)
        {
            offsetY = diffVec.normalized.y*13;
        }
        else if (diffVec.normalized.y < -0.4f)
        {
            offsetY = diffVec.normalized.y*13;
        }
        else
        {
            offsetY = 0;
        }
        //if (diffVec.sqrMagnitude >= 200)
        //{
        //    cursolToPlayerOffset = diffVec.normalized * 50;
        //    cursolToPlayerOffset = DOVirtual.Vector3()
        //}
        //else if(diffVec.sqrMagnitude < 200)
        //{
        //    float ratio = diffVec.sqrMagnitude / 200; 
        //    cursolToPlayerOffset = diffVec.normalized*ratio*50;
        //}
        //cursolToPlayerOffset = new Vector3(offsetX, offsetY, 0);
        DOTween.To(() => cursolToPlayerOffset,(x) => cursolToPlayerOffset = x,new Vector3(offsetX, offsetY, 0),0.2f);
        //DOVirtual.Vector3(cursolToPlayerOffset,new Vector3(offsetX, offsetY, 0),0.1f,cursolToPlayerOffset)
    }
    public void SetIsPlayerHomingTrue()
    {
        isPlayerHoming = true;
    }
    public void EngineIgniteShake()
    {
        StartCoroutine(Shake(0.2f, 0.2f));
    }
    public void ExplosionShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 pos = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            var x = pos.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            var y = pos.y + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, y, pos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = pos;
    }
    //カメラに映っている敵機や自機のmaximumDistanseFromCoreを全て取得、そこにランダムイベントのオブジェクトの距離も含め、その中での最大値を採用する
    /// <summary>
    /// 採用された最大距離に対して対応するカメラの倍率を設定、滑らかに移行させる
    /// </summary>
    /// <param name="distanse">採用された最大距離</param>
    private void ZoomRatioControll(int distanse)
    {
        if (Setting) { MainCam.orthographicSize = 30; return; }
        float goalZoomValue = 1f;
        float currentVelocity = 0;
        //距離に応じてズーム倍率を設定し、滑らかに変更する。下記マジックナンバーは
        if (distanse < 15) { goalZoomValue = 40f; }
        else if (distanse < 40) { goalZoomValue = 80; }
        else if (distanse < 70) { goalZoomValue = 100; }
        else { goalZoomValue = 90; }
        zoomRatio = Mathf.SmoothDamp(zoomRatio, goalZoomValue, ref currentVelocity, 0.1f);
        MainCam.orthographicSize = zoomRatio;
        //Debug.Log("MCC zoomratio" + zoomRatio + " " + goalZoomValue);
    }
    /// <summary>
    /// プレイヤーの座標を取得する際はGameObject.Find("Player")とかを使わずにここから取得する
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayer()
    {
        if (!Setting) return Player;
        else { Debug.LogWarning("UnexpectedSituation!"); return null; }
    }
    public void SetPlayerHoming(bool input)
    {
        StartCoroutine(SetIsPlayerHoming(input));
    }
    private IEnumerator SetIsPlayerHoming(bool input)
    {
        if (input)
        {
            Debug.LogAssertion("JJJJJJ");
            transform.DOMove(Player.transform.position + new Vector3(0, 0, -5), 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
        isPlayerHoming = input;
        yield return null;
    }
    /// <summary>
    /// MCC側のスクロール初期設定　StartPosまで移動してもらう
    /// </summary>
    /// <param name="input"></param>
    public IEnumerator MoveToScrollStartPos(Vector2 inputPos ,Action<int> callBack)
    {
        int moveSec = (int)(transform.position - new Vector3(inputPos.x, inputPos.y, transform.position.z)).magnitude/10 +1;
        callBack.Invoke(moveSec);
        transform.DOMove(new Vector3(inputPos.x,inputPos.y,transform.position.z),moveSec).SetEase(Ease.Linear);
        yield return null;
    }
    /// <summary>
    /// StartPosまで移動してもらったらGoalPosまでの移動開始
    /// </summary>
    /// <param name="inputPos"></param>
    /// <returns></returns>
    public IEnumerator MoveToScrollGoalPos(Vector2 inputPos,float ScrollTime)
    {
        transform.DOMove(new Vector3(inputPos.x,inputPos.y,transform.position.z),ScrollTime).SetEase(Ease.Linear);
        yield return null;
    }
}
