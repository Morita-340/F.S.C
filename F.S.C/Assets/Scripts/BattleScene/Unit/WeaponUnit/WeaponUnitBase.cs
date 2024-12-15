using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;

public class WeaponUnitBase : UnitBase
{
    [SerializeField]protected GameObject DividableIcon;
    private GameObject icon;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //分離の度にiconが再生成されてしまうため、その前に以前生成したiconを消去しておく
        //毎回Instantiateするのは処理が重くなりそうだが、WeaponUnit全てに対して予めヒエラルキー上でiconを設定しSetActiveを管理するのは面倒くさすぎるのでこちらを採用した
        if(this.transform.childCount > 0){
            foreach(Transform child in this.transform){
                Destroy(child.gameObject);
            }
        }
        icon = Instantiate(DividableIcon,this.gameObject.transform,false);
        if(GetThisUnitData().dividable){
            icon.SetActive(true);
        }else{icon.SetActive(false);}
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(this.tag == GSetting.ObjTagName.PlayerUnit.ToString()){
            if(GetThisUnitData().dividable){
                icon.SetActive(true);
            }else{icon.SetActive(false);}
        }
    }
}
