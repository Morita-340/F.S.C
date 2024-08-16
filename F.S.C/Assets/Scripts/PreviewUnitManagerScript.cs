using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewUnitManagerScript : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    private bool isCovered = false;
    private bool enterCovered = false,exitCovered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //enterCovered = false;
        //exitCovered = false;
    }
    public bool GetIsCovered(){
        isCovered = enterCovered||exitCovered;
        return isCovered;
    }
    void OnTriggerEnter2D(Collider2D collider){
        if(collider.tag == "PlayerUnit"){
            enterCovered= true;
            spriteRenderer.color = Color.red;
            Debug.Log(collider.transform.gameObject.name + "TTS");
        }else{
            spriteRenderer.color = Color.green;
            enterCovered = false;
        }
    }
    void OnTriggerExit2D(Collider2D collider){
        if(collider.tag == "PlayerUnit"){
            exitCovered = true;
            spriteRenderer.color = Color.red;
            Debug.Log(collider.transform.gameObject.name + "TTF");
        }else{
            spriteRenderer.color = Color.green;
            exitCovered = false;
        }
    }
}
