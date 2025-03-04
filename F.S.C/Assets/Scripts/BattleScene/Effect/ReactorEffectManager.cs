using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
using DG.Tweening;

public class ReactorEffectManager : AbstractEffectManager
{
    [SerializeField,ReadOnly]
    ReactorBase ThisReactor;
    //ｚ座標は10にすること。あまりにz座標が小さいとリンクのデバッグツールの邪魔になってしまう
    [SerializeField]GameObject EffectScope;
    public ReactorBase GetThisReactor(){
        return ThisReactor;
    }
    public void SetThisReactor(ReactorBase reactorBase){
        ThisReactor = reactorBase;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ThisReactor != null){this.transform.position = this.ThisReactor.transform.position+new Vector3(0,0,5);}
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
}
