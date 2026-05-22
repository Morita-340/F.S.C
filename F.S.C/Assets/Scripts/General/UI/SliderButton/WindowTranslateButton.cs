using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using FSCGeneral;

public class WindowTranslateButton : SliderButton
{
    [SerializeField] protected GSetting.SceneName TranslateScene;
    protected bool soundPlaying = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (executeFlag)
        {
            StartCoroutine(SceneTranslate());
        }
    }
    protected virtual IEnumerator SceneTranslate()
    {
        if (TranslateScene == null) { Debug.LogAssertion("TranslateScene is null"); yield break; }
        if (soundPlaying == false)
        {
            SCer.PlaySE(1);
            yield return SCer.FadeBGM();
            soundPlaying = true;
        }
        if (Time.timeScale == 0)
        {
            Time.timeScale = 0.1f;
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        Time.timeScale = 1;
        Debug.Log("QQQ_" + gameObject.name);
        SceneManager.LoadScene(TranslateScene.ToString());
    }
    /// <summary>
    /// リザルト画面にて一定時間放置時に強制的に戻す処置
    /// </summary>
    public void Execute()
    {
        StartCoroutine(SceneTranslate());
    }
}
