using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractUnitMoveManagementScript : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody2D rb2d;
    [SerializeField, ReadOnly] protected AugmentorEffectController AEC;
    protected virtual void Start()
    {
        AEC = GetComponent<AugmentorEffectController>();
    }
    public void KnockBack(Vector2 hitPos)
    {
        rb2d.velocity += ((Vector2)transform.position - hitPos).normalized * 20;
    }
}
