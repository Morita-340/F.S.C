using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    Camera MainCam;
    GameObject Player;
    [SerializeField]bool Setting = false;
    int maxDistanse = 0;
    float zoomRatio = 60f;
    private List<AbstractUnitDestroyManagementScript>VisibleUnitList = new List<AbstractUnitDestroyManagementScript>();
    public void AddToVisibleUnitList(AbstractUnitDestroyManagementScript AUDMS){
        if(!VisibleUnitList.Contains(AUDMS)){
            VisibleUnitList.Add(AUDMS);
            Debug.Log("MCC ListAdd");}
    }
    public void DeleteFromVisibleUnitList(AbstractUnitDestroyManagementScript AUDMS){
        if(VisibleUnitList.Contains(AUDMS)){VisibleUnitList.Remove(AUDMS);}
    }
    // Start is called before the first frame update
    void Start()
    {
        MainCam = this.GetComponent<Camera>();
        if(!Setting)Player = FindObjectOfType<PlayerUnitDestroyManagementScript>().gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if(!Setting)this.transform.position = Player.transform.position + new Vector3(0,0, -5);
        foreach(AbstractUnitDestroyManagementScript AUDMS in VisibleUnitList){
            int maxDistanseFromCore = AUDMS.GetMaximumDistanseFromCore();
            if(maxDistanse < maxDistanseFromCore){
                maxDistanse = maxDistanseFromCore;
                Debug.Log("MCC maxAUDMS name: " +maxDistanse + AUDMS.gameObject.name);}
        }
        ZoomRatioControll(maxDistanse);
    }
    public void ExplosionShake( float duration, float magnitude )
    {
        StartCoroutine( Shake(duration,magnitude));
    }
    private IEnumerator Shake( float duration, float magnitude )
    {
        Vector3 pos = transform.localPosition;
        float elapsed = 0f;
        while ( elapsed < duration )
        {
            var x = pos.x + Random.Range( -1f, 1f ) * magnitude;
            var y = pos.y + Random.Range( -1f, 1f ) * magnitude;
            transform.localPosition =new Vector3(x,y,pos.z);
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
    private void ZoomRatioControll(int distanse){
        if(Setting){MainCam.orthographicSize = 30; return;}
        float goalZoomValue = 1f;
        float currentVelocity = 0;
        //距離に応じてズーム倍率を設定し、滑らかに変更する。下記マジックナンバーは
        if(distanse < 15){goalZoomValue = 60f;}
        else if(distanse < 40){goalZoomValue = 80;}
        else if(distanse < 70){goalZoomValue = 100;}
        else{goalZoomValue = 90;}
        zoomRatio = Mathf.SmoothDamp(zoomRatio,goalZoomValue,ref currentVelocity,0.1f);
        MainCam.orthographicSize = zoomRatio;
        Debug.Log("MCC zoomratio" + zoomRatio + " " + goalZoomValue);
    }
    /// <summary>
    /// プレイヤーの座標を取得する際はGameObject.Find("Player")とかを使わずにここから取得する
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayer(){
        if(!Setting)return Player;
        else {Debug.LogWarning("UnexpectedSituation!");return null;}
    }
}
