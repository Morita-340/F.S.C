using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//タイトル画面とシーンを共有している前提
//ブリーフィング用ウィンドウを開いているような演出
public class MenuOpClPerformManager : MonoBehaviour
{
    [SerializeField] TitleWindowManager TWM;
    [SerializeField] StagePlayerSelectMenuManager SPSMM;
    //ステージクリア後に遷移する際に発火。タイトル画面を映さず直接セレクト画面を見せる演出
    public IEnumerator MenuOpen()
    {
        TWM.BlackOut();
        //ちょっとだけ縦にウィンドウ展開しつつ横に展開
        SPSMM.transform.DOScale(new Vector3(1, 0.1f, 1), 0.2f);
        yield return new WaitForSeconds(0.2f);
        //縦に展開
        SPSMM.transform.DOScale(Vector3.one, 0.2f);
    }
    /// <summary>
    /// 黒背景を一旦消す
    /// </summary>
    public void NotBlack()
    {
        TWM.NotBlack();
    }
    public void BlackOut()
    {
        TWM.BlackOut();
    }
    //TakeOffボタンを押した後に発火
    public IEnumerator MenuClose()
    {
        yield return TWM.BlackOutGradually(0.2f);
        //縦に縮小（ちょい残し）
        SPSMM.transform.DOScale(new Vector3(1, 0.1f, 1), 0.2f);
        yield return new WaitForSeconds(0.2f);
        //横に縮小
        //残りの縦ウィンドウ縮小
        SPSMM.transform.DOScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }
}
