using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketBomb : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D thisRb2D;
    [SerializeField]
    float firstSpeed = 1f;
    [SerializeField,Range(2f, 5f)]
    float graceTime = 2;
    private float nowTime = 0;
    private bool rocketHit = false;
    // Start is called before the first frame update
    void Start()
    {
        thisRb2D.AddForce(new(0f,firstSpeed));
        thisRb2D.velocity = new Vector3(firstSpeed, 0f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        nowTime += Time.deltaTime;
        if(nowTime > graceTime){Destroy(this.gameObject);}
        else if(rocketHit){Destroy(this.gameObject);}
    }
    public void OnCollisionEnter2D(Collision2D other){
        if(true)rocketHit = true;//敵に当たれば爆発する。タグで判別せよ
    }
}
