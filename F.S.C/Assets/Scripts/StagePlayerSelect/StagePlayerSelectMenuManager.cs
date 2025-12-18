using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StagePlayerSelectMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject StageSelectManagerObject;
    [SerializeField]
    GameObject PlayerSelectManagerObject;
    // Start is called before the first frame update
    void Start()
    {
        StageSelectManagerObject.transform.localScale = new Vector3(1, 1, 1);
    }
    public void ChangeToPlayerSelectMenu()
    {
        PlayerSelectManagerObject.transform.DOScaleX(1,0.3f);
        StageSelectManagerObject.transform.DOScaleX(0,0.3f);
    }
    public void ChangeToStageSelectMenu()
    {
        PlayerSelectManagerObject.transform.DOScaleX(0,0.3f);
        StageSelectManagerObject.transform.DOScaleX(1,0.3f);
    }
}
