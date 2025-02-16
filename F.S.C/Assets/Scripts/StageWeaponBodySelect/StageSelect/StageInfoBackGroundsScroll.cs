using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoBackGroundsScroll : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage = null;
    [SerializeField,Range(0f,3f)]
    private float xSpeed = 0;
    [SerializeField,Range(0f,3f)]
    private float ySpeed = 0;
    [SerializeField]
    private bool roop = false;
    private int rectRandValue1 = 0;
    private int rectRandValue2 = 0;
    private void Reset(){
        rawImage = GetComponent<RawImage>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rectRandValue1 = Random.Range(0, 9);
        rectRandValue2 = Random.Range(0,9);
    }

    // Update is called once per frame
    void Update()
    {
        var uv_rect     = rawImage.uvRect;
        if(roop){
            if(Time.deltaTime % 2f < 1){
                uv_rect.x       = Mathf.Repeat( Time.time * xSpeed,rectRandValue1*0.1f );
                uv_rect.y       = Mathf.Repeat( Time.time * ySpeed,rectRandValue2*0.1f );
            }else{
                uv_rect.x       = Mathf.Repeat( -Time.time * xSpeed,2f - rectRandValue1*0.1f );
                uv_rect.y       = Mathf.Repeat( -Time.time * ySpeed,2f - rectRandValue2*0.1f );
            }
        }
        else{
            uv_rect.x       = Mathf.Repeat( Time.time * xSpeed,1.0f );
            uv_rect.y       = Mathf.Repeat( Time.time * ySpeed,1.0f );
        }
        rawImage.uvRect  = uv_rect;
    }
}
