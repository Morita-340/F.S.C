using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 音量調整つまみ
/// </summary>
public class SoundVolumeController : GeneralUIIconController
{
    /// <summary>
    /// つまみの画像を回転させるときの上限のラジアン角
    /// </summary>
    [SerializeField,Range(0,135)]float imageUpperRotLimit = 100;
    /// <summary>
    /// つまみの画像を回転させるときの下限のラジアン角
    /// </summary>
    [SerializeField,Range(0,-135)]float imageLowerRotLimit = -100;
    [SerializeField,ReadOnly]float nowAngle = 0;
    [SerializeField,ReadOnly]GeneralFlagManager GFM;
    // Start is called before the first frame update
    protected override void Start()
    {
        nowAngle = imageLowerRotLimit;
        base.Start();
        GFM = FindObjectOfType<GeneralFlagManager>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,-nowAngle));
        //カーソルが乗っている状態ならマウスホイールによる操作が可能になる
        if(CursolSelected){
            nowAngle += Input.GetAxis("Mouse ScrollWheel") *100;
            if(Input.GetAxis("Mouse ScrollWheel") != 0){
                SCer.PlaySE(0);
            }
            //上限と下限がある
            if(nowAngle > imageUpperRotLimit){
                nowAngle = imageUpperRotLimit;
            }
            if(nowAngle < imageLowerRotLimit){
                nowAngle = imageLowerRotLimit;
            }
            GFM.SetSoundVolume(nowAngle - imageLowerRotLimit,imageUpperRotLimit - imageLowerRotLimit);
            Debug.LogWarning(nowAngle - imageLowerRotLimit + " " + (imageUpperRotLimit - imageLowerRotLimit));
        }
    }
}
