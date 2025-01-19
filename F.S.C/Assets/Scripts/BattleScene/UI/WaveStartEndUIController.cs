using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class WaveStartEndUIController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI WaveNumberText;
    [SerializeField]
    TextMeshProUGUI WaveText;
    [SerializeField]
    TextMeshProUGUI WaveClearText;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WaveStartUI(int waveNum,bool finalWaveFlag){
        string waveNumText;
        if(waveNum == 1){waveNumText = waveNum.ToString() + "st";}
        else if(waveNum == 2){waveNumText = waveNum.ToString() + "nd";}
        else if(waveNum == 3){waveNumText = waveNum.ToString() + "rd";}
        else{waveNumText = waveNum.ToString() + "th";}
        if(finalWaveFlag){waveNumText = "Final";}
        //初期値設定
        WaveNumberText.text = waveNumText;
        WaveNumberText.color = new Color(1,1,1,1);
        WaveNumberText.rectTransform.anchoredPosition = new Vector3(-500,0,0);
        WaveText.color = new Color(1,1,1,1);
        WaveText.rectTransform.anchoredPosition = new Vector3(500,0,0);
        //アニメーション
        StartCoroutine(UIFadeGradually(WaveNumberText));
        StartCoroutine(UIFadeGradually(WaveText));
    }
    public void WaveClearUI(){
        WaveClearText.color = new Color(1,1,1,1);
        StartCoroutine(UIFadeGradually(WaveClearText));
    }
    public IEnumerator UIFadeGradually(TextMeshProUGUI text){
        float moveTime = 1f;
        text.rectTransform.DOLocalMove(Vector2.zero, moveTime);
        yield return new WaitForSeconds(moveTime+0.2f);
        text.DOFade(0,0.5f);
    }
}
