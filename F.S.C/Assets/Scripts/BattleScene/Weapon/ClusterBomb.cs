using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterBomb : RocketBomb
{
    [SerializeField]WeaponBase Bomb;
    [SerializeField,Range(1f,5f)]float BurstWaitTime;
    [SerializeField,Range(2,10)]int generateNum = 2;
    float fromGenerateTime = 0;
    List<int> angleList = new List<int>{0,15,-15,40,-40,70,-70,100,-100};
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if(Bomb is ClusterBomb){Debug.LogAssertion("無限生成が始まるのでダメ！！");return;}
        StartCoroutine(Crush());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    IEnumerator Crush(){
        yield return new WaitForSeconds(BurstWaitTime);
        for(int i = 0;i < generateNum;i++){
            //クラスター弾生成
            int randNum = Random.Range(0,angleList.Count);
            WeaponBase GeneBomb = Instantiate(Bomb,transform.position,transform.rotation);
            //向きを調整
            GeneBomb.transform.rotation = Quaternion.Euler(new Vector3(0,0,this.transform.rotation.eulerAngles.z + angleList[randNum]));
            //速度付与
            GeneBomb.SetVelocity(GeneBomb.transform.up * 35f);
        }
        Destroy(gameObject);
    }
    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
