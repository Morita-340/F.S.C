using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class CameraPlayerHomingTrigger : MonoBehaviour
{
    [SerializeField,ReadOnly] BoxCollider2D boxCollider2D;
    bool cameraPlayerHomingTrigger = false;
    void Start(){
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == GSetting.ObjTagName.PlayerUnit.ToString())cameraPlayerHomingTrigger = true;
    }
    public bool CameraPlayerHoming(){
        return cameraPlayerHomingTrigger;
    }
}
