using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCoreEffectManager : CoreEffectManager
{
    [SerializeField] protected List<GameObject> ExplosionObjList = new List<GameObject>();
    public override void ExplosionActive(bool flag)
    {
        if (flag == true)
        {
            StartCoroutine(TrueExplosionProcess());
        }
        else
        {
            foreach (GameObject obj in ExplosionObjList)
            {
                obj.SetActive(false);
            }
        }
    }
    private IEnumerator TrueExplosionProcess()
    {
        if(!ExplosionObjList.Contains(ExplosionObj)){
            ExplosionObjList.Add(ExplosionObj);}
        foreach (GameObject obj in ExplosionObjList)
        {
            obj.SetActive(true);
            Vector3 ExplosionScale = new Vector3(EffectRadius, EffectRadius, EffectRadius);
            ExplosionObj.transform.localScale = new Vector3(1, 1, 1);
            StartCoroutine(DestroyObjectGradually(ExplosionObj.GetComponent<SpriteRenderer>(), ExplosionScale));
            yield return new WaitForSeconds(0.2f);
        }
    }
}
