using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// それぞれのオブジェクトが鳴らす音は全てこのコンポーネントで管理する
/// </summary>
public class SoundController : MonoBehaviour
{
    /// <summary>
    /// このコンポーネントのオブジェクトが鳴らすBGM一覧
    /// </summary>
    [SerializeField]List<BGMData> BGMList = new List<BGMData>();
    /// <summary>
    /// このコンポーネントのオブジェクトが鳴らすSE一覧
    /// </summary>
    [SerializeField]List<AudioClip> SEList = new List<AudioClip>();
    AudioSource BGMPlayer;
    AudioSource SEPlayer;
    [SerializeField]bool playOnAwake = false;
    void Awake()
    {
        BGMPlayer = gameObject.AddComponent<AudioSource>();
        SEPlayer = gameObject.AddComponent<AudioSource>(); 
    }
    // Start is called before the first frame update
    void Start()
    {
        if(SEPlayer == BGMPlayer){Debug.LogAssertion("SEPlayer and BGMPlayer are Same !");}
        if(playOnAwake){StartCoroutine(PlayBGM(0));}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// BGMを再生する
    /// </summary>
    /// <param name="ListIndex"></param>
    public IEnumerator PlayBGM(int ListIndex){
        //再生する際にはBGMの名前を見れるようにする
        //指定されたインデックスのイントロBGMを再生する（イントロが無いなら即ループBGMを再生する）
        if(ListIndex >= BGMList.Count){Debug.LogAssertion("Invalid BGM Index");yield break;}
        //再生中に重ねて処理が呼び出された時には再生しないようにする処理
        //if(BGMPlayer.isPlaying){yield break;}
        BGMPlayer.volume = 1;
        BGMData SelectBGM = BGMList[ListIndex];
        AudioClip isPlayingBGM = SelectBGM.GetIntroBGM();
        if(isPlayingBGM != null){
            BGMPlayer.loop = false;
            BGMPlayer.clip = isPlayingBGM;
            BGMPlayer.Play();
            Debug.Log(isPlayingBGM.name);
            yield return new WaitWhile(() => BGMPlayer.isPlaying);
        }
        //イントロBGMの再生が終了したらロープBGMを再生し続ける
        isPlayingBGM = SelectBGM.GetRoopBGM();
        if(isPlayingBGM != null){
            BGMPlayer.loop = true;
            BGMPlayer.clip = isPlayingBGM;
            BGMPlayer.Play();
            Debug.Log(isPlayingBGM.name);
        }else{
            Debug.LogAssertion("LoopBGM isNot Register");
        }
    }
    public void StopBGM(){
        BGMPlayer.Stop();
    }
    /// <summary>
    /// 呼び出されたら、現在再生中のBGMのAudioSourceの音量を少しずつ下げ、0になるまで待つ
    /// </summary>
    public IEnumerator FadeBGM(){
        BGMPlayer.volume = Mathf.Lerp(BGMPlayer.volume,0,0.5f);
        yield return new WaitWhile(() =>BGMPlayer.volume == 0);
    }
    /// <summary>
    /// SEを再生する
    /// </summary>
    /// <param name="ListIndex"></param>
    public void PlaySE(int ListIndex){
        if(ListIndex >= SEList.Count){Debug.LogAssertion("Invalid SE Index");return;}
        //再生中に重ねて処理が呼び出された時には再生しないようにする処理
        if(SEPlayer.isPlaying){SEPlayer.Stop();}
        SEPlayer.volume = 1;
        AudioClip isPlayingSE = SEList[ListIndex];
        if(isPlayingSE != null){
            SEPlayer.PlayOneShot(isPlayingSE);
            Debug.Log(isPlayingSE.name);
        }else{
            Debug.LogAssertion("ThisSE isNot Register");
        }
    }
    /// <summary>
    /// フェードアウトするのではなく、カチッと止めたいときに使う
    /// </summary>
    public void StopSE(){
        SEPlayer.Stop();
    }
    /// <summary>
    /// 呼び出されたら、現在再生中のSEのAudioSourceの音量を少しずつ下げ、0になるまで待つ
    /// </summary>
    public IEnumerator FadeSE(){
        SEPlayer.volume = Mathf.Lerp(SEPlayer.volume,0,0.5f);
        yield return new WaitWhile(() => SEPlayer.volume == 0);
    }
    public bool IsPlayingSE(){
        return SEPlayer.isPlaying;
    }
}
/// <summary>
/// フリー音源から持ってきたBGMはイントロとループで分かれている場合があるので、これで管理する
/// </summary>
[Serializable]
public class BGMData
{
    [SerializeField]AudioClip IntroBGM;
    [SerializeField]AudioClip RoopBGM;
    public AudioClip GetIntroBGM(){
        return IntroBGM;
    }
    public AudioClip GetRoopBGM(){
        return RoopBGM;
    }
}
