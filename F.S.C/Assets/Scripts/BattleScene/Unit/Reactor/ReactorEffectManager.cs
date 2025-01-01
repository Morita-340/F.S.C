using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        ExplosionObj.SetActive(flag);
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
}
