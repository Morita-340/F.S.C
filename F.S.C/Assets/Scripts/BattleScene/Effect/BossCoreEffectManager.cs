using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossCoreEffectManager : CoreEffectManager
{
    [SerializeField] protected List<GameObject> subExplosionObjList = new List<GameObject>();
    protected override void Start()
    {
        base.Start();
        SetCoreExplosionScope(explosionScopeRadius); ;
    }
    public override void ExplosionActive(bool flag)
    {
        //メインの爆発はそのまま
        //見かけだけのサブの爆発は新たに処理追加
        if (flag == true)
        {
            StartCoroutine(TrueExplosionProcess());
        }
        else
        {
            foreach (GameObject obj in subExplosionObjList)
            {
                obj.SetActive(false);
            }
            base.ExplosionActive(flag);
        }
    }
    private IEnumerator TrueExplosionProcess()
    {

        //リストに格納しているエフェクトを順に再生していく
        foreach (GameObject obj in subExplosionObjList)
        {
            obj.transform.localScale = new Vector3(2f,2f,2f);
            obj.SetActive(true);
            Vector3 ExplosionScale = new Vector3(explosionScopeRadius*2f, explosionScopeRadius*2f, explosionScopeRadius*2f);
            SoundController SCer = obj.GetComponent<SoundController>();
            SCer.PlaySE(0);
            StartCoroutine(DestroyObjectGradually(obj.GetComponent<SpriteRenderer>(), ExplosionScale));
            yield return new WaitForSeconds(0.2f);
            Debug.LogAssertion("AAAAA");
        }
        base.ExplosionActive(true);
    }
    public override IEnumerator DestroyObjectGradually(SpriteRenderer SR, Vector3 ExplosionScale)
    {
        ExplosionObj.transform.DOScale(ExplosionScale, 0.1f);
        //爆発音を鳴らす
        SCer.PlaySE(0);
        yield return new WaitUntil(() => ExplosionObj.transform.localScale == ExplosionScale);
        SR.DOFade(0, 0.5f);
        yield return new WaitUntil(() => SR.color.a == 0);
        ExplosionObj.SetActive(false);
        yield return new WaitUntil(() => !SCer.IsPlayingSE());
        //サブ爆発エフェクトでは不要なのでDestroy処理だけなくした
    }
    private void SetCoreExplosionScope(int scopeRadius)
    {
        //コライダーではなくスケールなのは、エフェクト（仮でスプライト、後でアニメーションに変更する予定）のサイズも変えないといけないから
        EffectRadius = scopeRadius * 2;
        ExplosionObj.transform.localPosition = new Vector3(0, 0, 10);
    }
}
