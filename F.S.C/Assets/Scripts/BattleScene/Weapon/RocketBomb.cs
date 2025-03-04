using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FSCGeneral;

public class RocketBomb : WeaponBase
{
    //[SerializeField]
    //float firstSpeed = 1f;
    //生成後何にも当たらずに漂える時間
    [SerializeField,Range(2f, 5f)]
    float graceTime = 2;
    private float nowTime = 0;
    private bool rocketHit = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        nowTime += Time.deltaTime;
        if(nowTime > graceTime){Destroy(this.gameObject);}
        else if(rocketHit){Destroy(this.gameObject);}
    }
    public void OnTriggerEnter2D(Collider2D other){
        //if(true)rocketHit = true;//敵に当たれば爆発する。タグで判別する
        switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
            case GSetting.ObjTagName.PlayerWeapon1: 
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), other.transform.tag,true)){
                    case GSetting.ObjTagName.EnemyUnit: rocketHit = true; break;
                    case GSetting.ObjTagName.EnemyWeapon1: rocketHit = true; break;
                    default: rocketHit = false;break;
                }
                break;
            }
            case GSetting.ObjTagName.PlayerWeapon2:
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), other.transform.tag,true)){
                    case GSetting.ObjTagName.EnemyUnit: rocketHit = true; break;
                    case GSetting.ObjTagName.EnemyWeapon1: rocketHit = true; break;
                    default: rocketHit = false;break;
                }
                break;
            }
            case GSetting.ObjTagName.EnemyWeapon1:
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), other.transform.tag,true)){
                    case GSetting.ObjTagName.PlayerUnit:{rocketHit = true;break;}
                    case GSetting.ObjTagName.PlayerWeapon1:{rocketHit = true;break;}
                    default:{rocketHit = false;break;}
                }
                break;
            }
            case GSetting.ObjTagName.EnemyWeapon2:
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), other.transform.tag,true)){
                    case GSetting.ObjTagName.PlayerUnit:{rocketHit = true;break;}
                    case GSetting.ObjTagName.PlayerWeapon1:{rocketHit = true;break;}
                    default:{rocketHit = false;break;}
                }
                break;
            }
            default: break;
        }
    }
}
