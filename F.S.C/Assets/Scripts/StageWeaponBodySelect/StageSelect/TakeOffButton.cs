using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;

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
    public void SetTranslateScene(SceneAsset sceneAsset){
        TranslateScene = sceneAsset;
        sceneRegistered = true;
    }
}
