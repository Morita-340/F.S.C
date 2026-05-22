using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractUnitMoveManagementScript : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody2D rb2d;
    protected bool knockBackInterval = false;
    [SerializeField, ReadOnly] protected AugmentorEffectController AEC;
    protected virtual void Start()
    {
        AEC = GetComponent<AugmentorEffectController>();
    }
    /// <summary>
    /// ノックバック
    /// </summary>
    /// <param name="hitPos"></param>
    public void KnockBack(Vector2 hitPos)
    {
        if (knockBackInterval)
        {

        }
        else
        {
            rb2d.velocity += ((Vector2)transform.position - hitPos).normalized * 20;
            //めり込んで連続ヒットしたときに呼ばれないようにするために僅かにインターバルを設定
            StartCoroutine(Temporary_knockBackInterval(0.5f));
        }
    }
    /// <summary>
    /// 一定時間無敵になる
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator Temporary_knockBackInterval(float time)
    {
        knockBackInterval = true;
        yield return new WaitForSeconds(time);
        knockBackInterval = false;
    }
}
