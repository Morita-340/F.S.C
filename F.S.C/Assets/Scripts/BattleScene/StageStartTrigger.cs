using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;

public class StageStartTrigger : MonoBehaviour
{
    [SerializeField,ReadOnly] BoxCollider2D boxCollider2D;
    bool stageStartTrigger = false;
    void Start(){
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == GSetting.ObjTagName.PlayerUnit.ToString())stageStartTrigger = true;
    }
    public bool StageStart(){
        return stageStartTrigger;
    }
}
