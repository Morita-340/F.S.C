using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
/// <summary>
/// アフターバーナーの見た目を表示する。カッコよくなる
/// </summary>
public class AugmentorEffectController : MonoBehaviour
{
    [SerializeField] List<GameObject> FrontAugmentors;
    [SerializeField] List<GameObject> FrontLeftAugmentors;
    [SerializeField] List<GameObject> FrontRightAugmentors;
    [SerializeField] List<GameObject> BackAugmentors;
    [SerializeField] List<GameObject> BackLeftAugmentors;
    [SerializeField] List<GameObject> BackRightAugmentors;
    /// <summary>
    /// 呼び出されている間、閾値までアフターバーナーが伸びる
    /// サイズが閾値に到達すると少し縮み再び伸びるので炎が靡く演出代わりに使える
    /// 閾値より大きい場合は縮む
    /// </summary>
    /// <param name="Augmentor">対象のアフターバーナー</param>
    /// <param name="maxAugmentorScale">伸びの上限</param>
    private void EffectIncrease(GameObject Augmentor,float maxAugmentorScale){
        if(maxAugmentorScale <= 0){Debug.LogAssertion("Invalid inputValue");return;}
        float yLocalScale = Augmentor.transform.localScale.y;
        float xLocalScale = 1f;
        if (yLocalScale < maxAugmentorScale)
        {
            yLocalScale += 0.1f;
            Augmentor.transform.localScale = new Vector3(xLocalScale,
                                                        yLocalScale,
                                                        Augmentor.transform.localScale.z);
        }
        else
        {
            if (yLocalScale > 0.2f)
            {
                Augmentor.transform.localScale = new Vector3(xLocalScale,
                                                            maxAugmentorScale - 0.2f,
                                                            Augmentor.transform.localScale.z);
            }
            else
            {
                Augmentor.transform.localScale = new Vector3(xLocalScale,
                                                            0,
                                                            Augmentor.transform.localScale.z);
            }
        }
    }
    /// <summary>
    /// 細長くアフターバーナーを伸ばしたいときに使う
    /// </summary>
    /// <param name="Augmentor"></param>
    /// <param name="maxAugmentorScale"></param>
    private void EffectTinyIncrease(GameObject Augmentor, float maxAugmentorScale,float minWidthScale)
    {
        if(maxAugmentorScale <= 0 || minWidthScale <=0){Debug.LogAssertion("Invalid inputValue");return;}
        float yLocalScale = Augmentor.transform.localScale.y;
        float xLocalScale = Augmentor.transform.localScale.x;
        if (xLocalScale > minWidthScale)
        {
            xLocalScale -= 0.05f;
            Augmentor.transform.localScale = new Vector3(xLocalScale,
                                                        yLocalScale,
                                                        Augmentor.transform.localScale.z);
        }
        if (yLocalScale < maxAugmentorScale)
        {
            yLocalScale += 0.1f;
            Augmentor.transform.localScale = new Vector3(xLocalScale,
                                                        yLocalScale,
                                                        Augmentor.transform.localScale.z);
        }
        else
        {
            if (yLocalScale > 0.2f)
            {
                Augmentor.transform.localScale = new Vector3(xLocalScale,
                                                            maxAugmentorScale - 0.2f,
                                                            Augmentor.transform.localScale.z);
            }
            else
            {
                Augmentor.transform.localScale = new Vector3(xLocalScale,
                                                            0,
                                                            Augmentor.transform.localScale.z);
            }
        }
    }
    /// <summary>
    /// 呼び出されると即座に引数のサイズまでアフターバーナーが伸びる
    /// </summary>
    /// <param name="Augmentor"></param>
    /// <param name="augmentorScale"></param>
    private void EffectIncreaseRapidly(GameObject Augmentor, float augmentorScale)
    {
        Augmentor.transform.localScale = new Vector3(Augmentor.transform.localScale.x,
                                                    augmentorScale,
                                                    Augmentor.transform.localScale.z);
    }
    /// <summary>
    /// 呼び出されている間、アフターバーナーが縮む
    /// サイズが0になるとそれ以上縮まない
    /// </summary>
    /// <param name="Augmentor">対称のアフターバーナー</param>
    //private void EfefctReduction(GameObject Augmentor){
    //    float yLocalScale = Augmentor.transform.localScale.y;
    //    if(yLocalScale > 0){
    //        yLocalScale -= 0.05f;
    //        Augmentor.transform.localScale = new Vector3(Augmentor.transform.localScale.x,
    //                                                    yLocalScale,
    //                                                    Augmentor.transform.localScale.z);
    //    }else{
    //        Augmentor.transform.localScale = new Vector3(Augmentor.transform.localScale.x,
    //                                                    0,
    //                                                    Augmentor.transform.localScale.z);
    //    }
    //}
    private IEnumerator EfefctReduction(GameObject Augmentor){
        Augmentor.transform.DOScaleY(0,0.2f);
        yield return new WaitUntil(() => Augmentor.transform.localScale.y == 0);
    }
    public void IgniteBoost()
    {
        Debug.LogWarning("DDDDDD");
        if (FrontAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in FrontAugmentors)
            {
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if (FrontLeftAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in FrontLeftAugmentors)
            {
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if (FrontRightAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in FrontRightAugmentors)
            {
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if (BackAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in BackAugmentors)
            {
                EffectTinyIncrease(Augmentor, 1f,0.3f);
            }
        }
        if (BackLeftAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in BackLeftAugmentors)
            {
                EffectTinyIncrease(Augmentor, 1f,0.3f);
            }
        }
        if (BackRightAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in BackRightAugmentors)
            {
                EffectTinyIncrease(Augmentor, 1f,0.3f);
            }
        }
    }
    public void MoveForward()
    {
        Debug.LogWarning("AAAAAAA");
        if (FrontAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in FrontAugmentors)
            {
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if (FrontLeftAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in FrontLeftAugmentors)
            {
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if (FrontRightAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in FrontRightAugmentors)
            {
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if (BackAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in BackAugmentors)
            {
                EffectIncrease(Augmentor, 1f);
            }
        }
        if (BackLeftAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in BackLeftAugmentors)
            {
                EffectIncrease(Augmentor, 1f);
            }
        }
        if (BackRightAugmentors.Count != 0)
        {
            foreach (GameObject Augmentor in BackRightAugmentors)
            {
                EffectIncrease(Augmentor, 1f);
            }
        }
    }
    /// <summary>
    /// 引数のサイズまで即座に伸びる
    /// </summary>
    /// <param name="inputBoostEfficiency"></param>
    public void MoveForward(float inputBoostEfficiency){
        float boostEfficiency = inputBoostEfficiency;
        if(boostEfficiency <= 0)boostEfficiency = 0.1f;
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                EffectIncreaseRapidly(Augmentor,boostEfficiency);
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                EffectIncreaseRapidly(Augmentor,boostEfficiency);
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                EffectIncreaseRapidly(Augmentor,boostEfficiency);
            }
        }
    }
    public void MoveBack(){
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
    }
    /// <summary>
    /// 引数のサイズまで即座に伸びる
    /// </summary>
    /// <param name="inputBoostEfficiency"></param>
    public void MoveBack(float inputBoostEfficiency){
        float boostEfficiency = inputBoostEfficiency;
        if(boostEfficiency <= 0)boostEfficiency = 0.1f;
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                EffectIncreaseRapidly(Augmentor,boostEfficiency);
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                EffectIncreaseRapidly(Augmentor,boostEfficiency);
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                EffectIncreaseRapidly(Augmentor,boostEfficiency);
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
    }
    public void MoveForwardRight(){
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                EffectIncrease(Augmentor,0.5f);
            }
        }
    }
    public void MoveForwardLeft(){
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                EffectIncrease(Augmentor,0.5f);
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
    }
    public void MoveBackRight(){
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                EffectIncrease(Augmentor,0.5f);
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
    }
    public void MoveBackLeft(){
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                EffectIncrease(Augmentor,0.5f);
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
    }
    public void RightTurn(){
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
    }
    public void LeftTurn(){
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
    }
    public void PositionFix(){
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                EffectIncrease(Augmentor,1f);
            }
        }
    }
    public void Idle(){
        if(FrontAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(FrontRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in FrontRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackLeftAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackLeftAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
        if(BackRightAugmentors.Count != 0){
            foreach(GameObject Augmentor in BackRightAugmentors){
                StartCoroutine(EfefctReduction(Augmentor));
            }
        }
    }
}
