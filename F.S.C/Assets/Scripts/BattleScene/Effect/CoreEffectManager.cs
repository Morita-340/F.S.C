using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class CoreEffectManager : AbstractExplosionEffectManager
{
    [SerializeField,ReadOnly]
    CoreBase ThisCore;
    [SerializeField,Range(2,10)]int explosionScopeRadius = 2;

    public CoreBase GetThisCore(){
        return ThisCore;
    }
    public void SetThisCore(CoreBase coreBase){
        ThisCore = coreBase;
    }
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SetCoreExplosionScope(explosionScopeRadius);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(ThisCore != null){this.transform.position = ThisCore.transform.position+new Vector3(0,0,5);}
    }
    private void SetCoreExplosionScope(int scopeRadius){
        //コライダーではなくスケールなのは、エフェクト（仮でスプライト、後でアニメーションに変更する予定）のサイズも変えないといけないから
        ExplosionObj.transform.localScale = new Vector3(scopeRadius*2,scopeRadius*2,scopeRadius*2);
        ExplosionObj.transform.localPosition = new Vector3(0,0,10);
    }
}
