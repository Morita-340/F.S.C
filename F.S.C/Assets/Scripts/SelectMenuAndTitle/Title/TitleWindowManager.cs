using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleWindowManager : MonoBehaviour
{
    [SerializeField] GameObject BlackOutCurtain;
    [SerializeField] MenuOpClPerformManager MeOCPM;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //開始ボタンから発火（起動時はタイトル画面が見えている）
    public IEnumerator TitleToMenuOpen()
    {
        //タイトル画面の前の幕を徐々に暗くする
        yield return StartCoroutine(BlackOutGradually(0.3f));
        StartCoroutine(MeOCPM.MenuOpen());
    }
    public IEnumerator BlackOutGradually(float duration)
    {
        //タイトル画面の前の幕を徐々に暗くする
        this.transform.localScale = Vector3.one;
        BlackOutCurtain.GetComponent<Image>().DOColor(Color.black, duration);
        BlackOutCurtain.GetComponent<RectTransform>().localScale = Vector3.one;
        yield return new WaitForSeconds(duration);
    }
    public void BlackOut()
    {
        //暗くしておく
        this.transform.localScale = Vector3.one;
        BlackOutCurtain.GetComponent<RectTransform>().localScale = Vector3.one;
        BlackOutCurtain.GetComponent<Image>().color = Color.black;
    }
    public void NotBlack()
    {
        //暗くしておく
        this.transform.localScale = Vector3.zero;
    }
}
