using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeEffectManager : AbstractExplosionEffectManager
{
    [SerializeField,ReadOnly]private Grenade Grenade;
    [SerializeField,Range(2,10)]int explosionScopeRadius = 2;
    [SerializeField,ReadOnly]private int explosionDamage = 0;
    public Grenade GetThisGrenade(){
        return Grenade;
    }
    public void SetThisGrenade(Grenade inputGrenade){
        Grenade = inputGrenade;
    }
    public void SetExplosionDamage(int inputExplosionDamage){
        explosionDamage = inputExplosionDamage;
    }
    public int GetExplosionDamage(){
        return explosionDamage;
    }
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        SetGrenadeExplosionScope(explosionScopeRadius);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void SetGrenadeExplosionScope(int scopeRadius){
        //コライダーではなくスケールなのは、エフェクト（仮でスプライト、後でアニメーションに変更する予定）のサイズも変えないといけないから
        EffectRadius = scopeRadius * 2;
        ExplosionObj.transform.localPosition = new Vector3(0,0,10);
    }
}
