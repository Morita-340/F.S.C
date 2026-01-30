using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuCloseButton : SliderButton
{
    RefineBattleSceneFlowManager ReBSFM;
    protected override void Start()
    {
        base.Start();
        ReBSFM = FindObjectOfType<RefineBattleSceneFlowManager>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(executeFlag){
            SCer.PlaySE(1);
            ReBSFM.UnPause();
            executeFlag = false;
            ExecuteSlider.sizeDelta = new Vector2(0,ExecuteSlider.sizeDelta.y);
        }
    }
}
