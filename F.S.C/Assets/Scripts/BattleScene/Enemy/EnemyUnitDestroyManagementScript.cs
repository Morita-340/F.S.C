using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using FSCGeneral;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class EnemyUnitDestroyManagementScript : AbstractUnitDestroyManagementScript
{
    private PlayerSActionFeedBackUIController PAFBUIC;
    [SerializeField]GameObject WeaponControllConectedUIText;
    protected override void Awake()
    {
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.EnemyUnit;
        SCer = GetComponent<SoundController>();
        base.Awake();
    }
    protected override void Start(){
        //ゲームシーン上の名前に依存しているので要注意である
        if(!Setting)PAFBUIC = GameObject.Find("UICanvas").transform.Find("PlayerSActionFeedBackUI").GetComponent<PlayerSActionFeedBackUIController>();
        base.Start();
    }
    protected override List<GameObject> Regenerate(bool inputIsDead)
    {
        List<GameObject> ParentObjectList = base.Regenerate(inputIsDead);
        StartCoroutine(wait(0.1f,ParentObjectList));
        return ParentObjectList;
    }
    IEnumerator wait(float time,List<GameObject> ParentObjectList){
        //再生成処理中の処理なので、制御ユニットが生成される前に下記処理が行われないために僅かに待つ
        yield return new WaitForSeconds(time);
        foreach(GameObject obj in ParentObjectList){
            if(obj.GetComponent<DestroyedUnitManagementScript>().WeaponControllConected()){
                StartCoroutine(WaitUntilSuccessTextFade(0.5f,obj));
            }
        }
    }
    /// <summary>
    /// 武器を鹵獲できた場合にテキスト表示をする
    /// </summary>
    /// <param name="fadeTime"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    IEnumerator WaitUntilSuccessTextFade(float fadeTime,GameObject obj){
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(obj.transform.position);
        RectTransform uiRectTransform;
        if(!Setting){uiRectTransform = PAFBUIC.GetComponent<RectTransform>();}
        else{uiRectTransform = GameObject.Find(GSetting.UniqueObjectName.UICanvas.ToString()).GetComponent<RectTransform>();}
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiRectTransform, 
            screenPosition, 
            Camera.main, 
            out Vector2 localPosition
        );
        GameObject SuccessText = Instantiate(WeaponControllConectedUIText,uiRectTransform.transform);
        SuccessText.GetComponent<RectTransform>().anchoredPosition = screenPosition + localPosition;
        SuccessText.GetComponent<TextMeshProUGUI>().DOFade(0f,fadeTime);
        yield return new WaitForSeconds(fadeTime);
        Destroy(SuccessText);

    }
    public override int CaluculateCombatPower(){
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.EnemyUnit;
        return base.CaluculateCombatPower();
    }
    public override void DestroyProcess(UnitData DeleteData,bool inputIsDead)
    {
        if(!Setting){
            PAFBUIC.AddDefeatPoint(DeleteData.ReturnThisUnit().GetUnitStatus());
            PAFBUIC.ExplosionOcurre(DeleteData);
        }
        base.DestroyProcess(DeleteData,inputIsDead);
    }
    public override void DeleteAllRangeMesh()
    {
        transform.Find("SearchRader").GetComponent<EnemySearchManagementScript>().DestroyRMM();
        base.DeleteAllRangeMesh();
    }
}
