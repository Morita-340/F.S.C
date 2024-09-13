using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewUnitManagerScript : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    private bool isCovered = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TTT");
    }

    // Update is called once per frame
    void Update()
    {
        if(isCovered){spriteRenderer.color = Color.red;}
        else{spriteRenderer.color = Color.green;}

    }
    public bool GetIsCovered(){
        return isCovered;
    }

    void OnTriggerStay2D(Collider2D collider){
        if(collider.tag == "PlayerUnit"){
            //stayCovered  = true;
            isCovered = true;
            Debug.Log(collider.transform.gameObject.name + gameObject.transform.localPosition + "TTC");
        }
    }
    void OnTriggerEnter2D(Collider2D collider){
        if(collider.tag == "PlayerUnit"){
            //enterCovered= true;
            isCovered = true;
            Debug.Log(collider.transform.gameObject.name + gameObject.transform.localPosition + "TTS");
        }
    }
    void OnTriggerExit2D(Collider2D collider){
        if(collider.tag == "PlayerUnit"){
            //exitCovered = true;
            isCovered = false;
            Debug.Log(collider.transform.gameObject.name + gameObject.transform.localPosition + "TTF");
        }
    }
}
