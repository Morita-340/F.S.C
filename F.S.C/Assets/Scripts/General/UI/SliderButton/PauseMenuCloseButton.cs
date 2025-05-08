using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuCloseButton : SliderButton
{
    BattleSceneFlowManager BSFM;
    protected override void Start()
    {
        base.Start();
        BSFM = FindObjectOfType<BattleSceneFlowManager>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(executeFlag){
            SCer.PlaySE(1);
            BSFM.UnPause();
            executeFlag = false;
            ExecuteSlider.sizeDelta = new Vector2(0,ExecuteSlider.sizeDelta.y);
        }
    }
}
