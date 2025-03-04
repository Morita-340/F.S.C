using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class AbstractEffectManager : MonoBehaviour
{
    /// <summary>
    /// 判定用オブジェクト。リアクターの子オブジェクトだと他のユニットとの衝突を検知できないので要注意
    /// </summary>
    [SerializeField]protected GameObject ExplosionObj;
    [SerializeField,ReadOnly]protected SoundController SCer;
    protected virtual void Start(){
        SCer = GetComponent<SoundController>();
    }
    public virtual void ExplosionActive(bool flag){
        //爆発エフェクトが徐々にフェードアウトする仕組み
        if(flag == true){
            ExplosionObj.SetActive(flag);
            Vector3 ExplosionScale = ExplosionObj.transform.localScale;
            ExplosionObj.transform.localScale = new Vector3(1,1,1);
            StartCoroutine(DestroyObjectGradually(ExplosionObj.GetComponent<SpriteRenderer>(),ExplosionScale));
        }else{ExplosionObj.SetActive(flag);}
    }
    public void ChangeTransform(Transform transform){
        this.transform.position = transform.position;
        this.transform.rotation = transform.rotation;
    }
    public IEnumerator DestroyObjectGradually(SpriteRenderer SR,Vector3 ExplosionScale){
        ExplosionObj.transform.DOScale(ExplosionScale,0.1f);
        //爆発音を鳴らす
        SCer.PlaySE(0);
        yield return new WaitUntil(() => ExplosionObj.transform.localScale == ExplosionScale);
        SR.DOFade(0,0.5f);
        yield return new WaitUntil(() => SR.color.a == 0);
        ExplosionObj.SetActive(false);
        yield return new WaitUntil(() => !SCer.IsPlayingSE());
        Destroy(this.gameObject);
    }
}
