using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

/// <summary>
/// グレネード
/// 生成後、着弾or一定時間経過で爆発を引き起こす
/// 爆風は敵味方関係なく巻き込まれるため、使用場所には要注意
/// </summary>
public class Grenade : RocketBomb
{
    [SerializeField]GrenadeEffectManager GEM;
    [SerializeField,Range(5,10)]int explosionDamage = 5;
    private GameObject ReactorEffectPool;
    protected override void Awake()
    {
        base.Awake();
        ReactorEffectPool = GameObject.Find(GSetting.UniqueObjectName.ReactorEffectPool.ToString());
    }
    protected override void Start()
    {
        base.Start();
        GEM.transform.SetParent(ReactorEffectPool.transform);
        GEM.ExplosionActive(false);
        GEM.SetThisGrenade(this);
        GEM.SetExplosionDamage(explosionDamage);
    }
    protected override void Update()
    {
        GEM.ChangeTransform(this.transform);
        base.Update();
    }
    protected override void NotHit()
    {
        GrenadeExplode();
        base.NotHit();
    }
    protected override void Hit()
    {
        GrenadeExplode();
        base.Hit();
    }
    /// <summary>
    /// グレネードが爆発する処理
    /// </summary>
    private void GrenadeExplode(){
        GEM.ExplosionActive(true);
    }
}
