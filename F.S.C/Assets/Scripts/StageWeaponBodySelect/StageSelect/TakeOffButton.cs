using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening;
using FSCGeneral;

public class TakeOffButton : WindowTranslateButton
{
    bool sceneRegistered = false;
    bool sceneRegisteredFirstTime = true;
    protected override void Update()
    {
        if(sceneRegisteredFirstTime){
            if(sceneRegistered){
                GetComponent<RectTransform>().DOScaleX(1,0.5f);
                sceneRegisteredFirstTime = false;
            }
        }
        base.Update();
    }
    public void SetTranslateScene(GSetting.SceneName sceneName){
        TranslateScene = sceneName;
        sceneRegistered = true;
    }
}
