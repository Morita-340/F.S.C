using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
/// <summary>
/// PlayerのMouseCursolControllと処理プロセスは同じであるが、Playerがカーソルで狙うのに対して
/// こちらはFanRangeを用いてソナーを出しソナー内部に潜り込んだプレイヤーに対してNormalAttackを仕掛けていく
/// </summary>
public class EnemySearchManagementScript : MonoBehaviour
{
    [SerializeField]
    FanRange SearchRader;
    [SerializeField]
    EnemyUnitAttackManagementScript EUAMS;
    CircleCollider2D circleCollider2D;
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        circleCollider2D = this.gameObject.GetComponent<CircleCollider2D>();
        circleCollider2D.radius = SearchRader.GetRangeRadius();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D other){
        //timer = 0;
    }
    public void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            EUAMS.NormalAttack(timer,other.transform.position);
            timer += Time.deltaTime;
        }
    }
}
