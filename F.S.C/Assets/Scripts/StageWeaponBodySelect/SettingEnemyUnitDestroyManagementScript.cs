using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
using TMPro;
using DG.Tweening;
/// <summary>
/// EUDMSのうち、UI周りの処理のみを変更したもの。撃墜時のスコア加算などの処理がオミットされている
/// </summary>
public class SettingEnemyUnitDestroyManagementScript : AbstractUnitDestroyManagementScript
{
    [SerializeField]GameObject WeaponControllConectedUIText;
    protected override void Start(){
        //データ登録をする際のタグを決めている
        childObjTagName = GSetting.ObjTagName.EnemyUnit;
        base.Start();
    }
    protected override List<GameObject> Regenerate()
    {
        List<GameObject> ParentObjectList = base.Regenerate();
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
    IEnumerator WaitUntilSuccessTextFade(float fadeTime,GameObject obj){
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(obj.transform.position);
        //ゲームシーン上の名前に依存しているので要注意である
        RectTransform uiRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
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
    public override void DestroyProcess(UnitData DeleteData)
    {
        Debug.Log("EUDMS" + DeleteData.ReturnThisUnit().GetUnitStatus());
        base.DestroyProcess(DeleteData);
    }
}
