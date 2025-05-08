using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndButton : WindowTranslateButton
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override IEnumerator SceneTranslate()
    {
        if(soundPlaying == false){
            SCer.PlaySE(1);
            yield return SCer.FadeBGM();
            soundPlaying = true;
        }
        yield return new WaitForSeconds(2f);
        //ゲームアプリを終了する
        Application.Quit();
    }
}
