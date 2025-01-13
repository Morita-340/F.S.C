using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
using DG.Tweening;

public class ReactorEffectManager : MonoBehaviour
{
    [SerializeField,ReadOnly]
    ReactorBase ThisReactor;
    /// <summary>
    /// 判定用オブジェクト。リアクターの子オブジェクトだと他のユニットとの衝突を検知できないので要注意
    /// </summary>
    [SerializeField]GameObject ExplosionObj;
    //ｚ座標は10にすること。あまりにz座標が小さいとリンクのデバッグツールの邪魔になってしまう
    [SerializeField]GameObject EffectScope;
    public ReactorBase GetThisReactor(){
        return ThisReactor;
    }
    public void SetThisReactor(ReactorBase reactorBase){
        ThisReactor = reactorBase;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ExplosionActive(bool flag){
        //爆発エフェクトが徐々にフェードアウトする仕組み
        if(flag == true){
            ExplosionObj.SetActive(flag);
            Vector3 ExplosionScale = ExplosionObj.transform.localScale;
            ExplosionObj.transform.localScale = new Vector3(1,1,1);
            StartCoroutine(DestroyObjectGradually(ExplosionObj.GetComponent<SpriteRenderer>(),ExplosionScale));
        }else{ExplosionObj.SetActive(flag);}
    }
    public void ScopeActive(bool flag){
        EffectScope.SetActive(flag);
    }
    public void SetReactorEffectScope(int scopeRadius){
        //コライダーではなくスケールなのは、エフェクト（仮でスプライト、後でアニメーションに変更する予定）のサイズも変えないといけないから
        EffectScope.transform.localScale = new Vector3(scopeRadius*2,scopeRadius*2,scopeRadius*2);
        ExplosionObj.transform.localScale = new Vector3(scopeRadius*2,scopeRadius*2,scopeRadius*2);
        EffectScope.transform.localPosition = new Vector3(0,0,10);
        ExplosionObj.transform.localPosition = new Vector3(0,0,10);
    }
    public void ChangeTransform(Transform transform){
        this.transform.position = transform.position;
        this.transform.rotation = transform.rotation;
    }
    public IEnumerator DestroyObjectGradually(SpriteRenderer SR,Vector3 ExplosionScale){
        ExplosionObj.transform.DOScale(ExplosionScale,0.1f);
        yield return new WaitUntil(() => ExplosionObj.transform.localScale == ExplosionScale);
        SR.DOFade(0,0.5f);
        yield return new WaitUntil(() => SR.color.a == 0);
        Destroy(SR.gameObject);
    }
}
