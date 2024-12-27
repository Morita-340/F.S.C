using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class MainCameraZoomRatioController : MonoBehaviour
{
    [SerializeField]CinemachineVirtualCamera MainCam;
    [SerializeField]PlayerUnitDestroyManagementScript PUDMS;
    int maxDistanse = 0;
    float zoomRatio = 30f;
    private List<AbstractUnitDestroyManagementScript>VisibleUnitList = new List<AbstractUnitDestroyManagementScript>();
    public void AddToVisibleUnitList(AbstractUnitDestroyManagementScript AUDMS){
        if(!VisibleUnitList.Contains(AUDMS)){
            VisibleUnitList.Add(AUDMS);
            Debug.Log("MCZRC ListAdd");}
    }
    public void DeleteFromVisibleUnitList(AbstractUnitDestroyManagementScript AUDMS){
        if(VisibleUnitList.Contains(AUDMS)){VisibleUnitList.Remove(AUDMS);}
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(AbstractUnitDestroyManagementScript AUDMS in VisibleUnitList){
            int maxDistanseFromCore = AUDMS.GetMaximumDistanseFromCore();
            if(maxDistanse < maxDistanseFromCore){
                maxDistanse = maxDistanseFromCore;
                Debug.Log("MCZRC maxAUDMS name: " +maxDistanse + AUDMS.gameObject.name);}
        }
        ZoomRatioControll(maxDistanse);
    }
    //カメラに映っている敵機や自機のmaximumDistanseFromCoreを全て取得、そこにランダムイベントのオブジェクトの距離も含め、その中での最大値を採用する
    /// <summary>
    /// 採用された最大距離に対して対応するカメラの倍率を設定、滑らかに移行させる
    /// </summary>
    /// <param name="distanse">採用された最大距離</param>
    private void ZoomRatioControll(int distanse){
        float goalZoomValue = 1f;
        float currentVelocity = 0;
        //距離に応じてズーム倍率を設定し、滑らかに変更する。下記マジックナンバーは
        if(distanse < 15){goalZoomValue = 30f;}
        else if(distanse < 40){goalZoomValue = 40;}
        else if(distanse < 70){goalZoomValue = 50;}
        else{goalZoomValue = 90;}
        zoomRatio = Mathf.SmoothDamp(zoomRatio,goalZoomValue,ref currentVelocity,0.1f);
        MainCam.m_Lens.OrthographicSize = zoomRatio;
        Debug.Log("MCZRC zoomratio" + zoomRatio + " " + goalZoomValue);
    }
}
