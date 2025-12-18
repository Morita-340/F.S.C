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
    [SerializeField,ReadOnly]AudioSource BGMPlayer;
    [SerializeField,ReadOnly]AudioSource SEPlayer;
    [SerializeField]bool playOnAwake = false;
    /// <summary>
    /// ポーズ画面のUIであるかどうか
    /// ポーズ画面のUIのSEは別管理が必要なので
    /// </summary>
    [SerializeField]bool isPoseMenuUI = false;
    [SerializeField,ReadOnly]bool SELoopFlag = false;
    [SerializeField, ReadOnly] float soundVolume = 1;
    [SerializeField,ReadOnly]GeneralFlagManager GFM;
    void Awake()
    {
        //TryGetComponent<AudioSource>(out BGMPlayer);
        //TryGetComponent<AudioSource>(out SEPlayer);
        if(BGMPlayer == null)BGMPlayer = gameObject.AddComponent<AudioSource>();
        if(SEPlayer == null) SEPlayer = gameObject.AddComponent<AudioSource>(); 
    }
    // Start is called before the first frame update
    void Start()
    {
        if(SEPlayer == BGMPlayer){Debug.LogAssertion("SEPlayer and BGMPlayer are Same !");}
        if(playOnAwake){StartCoroutine(PlayBGM(0));}
        GFM = FindObjectOfType<GeneralFlagManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //soundVolume = GFM.GetSoundVolume();
        if(BGMPlayer.isPlaying){BGMPlayer.volume = soundVolume;}
        if (SEPlayer.isPlaying)
        {
            SEPlayer.volume = soundVolume;
            if (SELoopFlag && (SEPlayer.timeSamples > SEPlayer.clip.length - 1))
            {
                SEPlayer.time = 1;
            }
        }
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
        BGMPlayer.volume = soundVolume;
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
    /// <summary>
    /// ポーズ画面へ遷移する際に使用する
    /// </summary>
    public void PauseBGM(){
        if(!isPoseMenuUI){BGMPlayer.Pause();}
    }
    public void UnPauseBGM(){
        if(!isPoseMenuUI){
            BGMPlayer.UnPause();
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
        SELoopFlag = false;
        PlaySEProcess(ListIndex);
    }
    /// <summary>
    /// SEを再生する
    /// </summary>
    /// <param name="ListIndex"></param>
    /// <param name="continuosFlag">有効ならSEの終了1秒前と開始1秒後をつなげてループさせる</param>
    public void PlaySE(int ListIndex, bool loopFlag)
    {
        SELoopFlag = loopFlag;
        PlaySEProcess(ListIndex);
    }
    private void PlaySEProcess(int ListIndex)
    {
        if(ListIndex >= SEList.Count){Debug.LogAssertion("Invalid SE Index");return;}
        //再生中に重ねて処理が呼び出された時には再生しないようにする処理
        if(SEPlayer.isPlaying){SEPlayer.Stop();}
        SEPlayer.volume = soundVolume;
        AudioClip isPlayingSE = SEList[ListIndex];
        if(isPlayingSE != null){
            SEPlayer.PlayOneShot(isPlayingSE);
            SEPlayer.clip = isPlayingSE;
            Debug.Log(isPlayingSE.name);
        }else{
            Debug.LogAssertion("ThisSE isNot Register");
        }
    }
    /// <summary>
    /// ポーズ画面へ遷移する際に使用する
    /// </summary>
    public void PauseSE()
    {
        if (!isPoseMenuUI)
        {
            SEPlayer.Pause();
        }
    }
    public void UnPauseSE(){
        if(!isPoseMenuUI){
            SEPlayer.UnPause();
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
    public void ChangeSEPitch(float input) {
        float inputValue = input;
        if (input < 0){ inputValue = 1; }
            SEPlayer.pitch = inputValue;
    }
    public bool IsPlayingSE()
    {
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
