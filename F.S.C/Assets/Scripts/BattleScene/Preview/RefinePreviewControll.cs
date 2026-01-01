using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;
/// <summary>
/// プレビュー表示の制御
/// 表示対象のパーツ情報及び表示の起点座標（activeJointUnitの座標）および回転角を指定すると、指定箇所にプレビューを生成して表示する
/// 機体と被るかどうかの判定も行ってもらう
/// </summary>
public class RefinePreviewControll : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private List<AbstractPartsController> PreviewPartsGroup;
    [SerializeField, ReadOnly]
    private List<AbstractPartsController> nowPreviewPartsGroup = new List<AbstractPartsController>();
    [SerializeField, ReadOnly]
    private RefinePlayerUnitDestroyManagementScript RePUDMS;
    [SerializeField, ReadOnly]
    private RefineDestroyedUnitManagementScript ReDUMS;
    [SerializeField, ReadOnly]
    private Vector3 PreviewPosition;
    [SerializeField, ReadOnly]
    private Vector3 PreviewRotation;
    private float wheelInput;
    List<GameObject> PreviewPartsObjList = new List<GameObject>();
    List<AbstractPartsController> PartsGroup;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PreviewDisplay();
    }
    private void PreviewDisplay()
    {
        if (ReDUMS == null || RePUDMS == null)
        {
            foreach (var part in PreviewPartsObjList)
            {
                Destroy(part);
            }
            PreviewPartsObjList.Clear();
            return;
        }
        PartsGroup = new List<AbstractPartsController>(ReDUMS.GetChildPartsList());
        AbstractPartsController CenterParts = ReDUMS.GetEmptyJoint();
        //空きのある機体側のパッシブジョイントユニットの座標をリストにまとめる
        List<Vector3> passiveJointList = RePUDMS.GetAllEmptyPassiveJointSPositionList();
        //プレビューを表示するために選択中の座標や回転角を渡しておく
        //マウスホイールで選べるようにする
        if(passiveJointList.Count == 0){return;}
        int selectedNum = Mathf.Abs((int)wheelInput % passiveJointList.Count);
        Vector3 selectedPos = passiveJointList[selectedNum];
        Quaternion newRotation = Quaternion.Euler(RePUDMS.transform.rotation.eulerAngles
                                                + new Vector3(0, 0, CenterParts.GetActiveJointLink().GetOffsetRotation()+ RePUDMS.GetSelectedPassiveJointList()[selectedNum].GetOffsetRotation())
                                                - ReDUMS.transform.rotation.eulerAngles);
        //Passiveのワールド角+-180が求めたい角度
        //をパーツのローカル角度（親：RePUDMS）に直す必要がある
        Quaternion PrevRotation =Quaternion.Euler(RePUDMS.transform.rotation.eulerAngles
                                                + RePUDMS.GetSelectedPassiveJointList()[selectedNum].GetAPC().transform.localRotation.eulerAngles
                                                + new Vector3(0, 0, RePUDMS.GetSelectedPassiveJointList()[selectedNum].GetOffsetRotation() + 180
                                                - CenterParts.GetActiveJointLink().GetOffsetRotation()));
        //Quaternion.Euler(RePUDMS.transform.rotation.eulerAngles + new Vector3(0, 0, CenterParts.GetActiveJointLink().GetOffsetRotation() + RePUDMS.GetSelectedJointSOffsetRotation()[selectedNum]));
        Debug.Log("FFFFF"+PrevRotation.eulerAngles+""+RePUDMS.transform.rotation.eulerAngles+""+ RePUDMS.GetSelectedPassiveJointList()[selectedNum].GetAPC().transform.localRotation.eulerAngles+" "+RePUDMS.GetSelectedPassiveJointList()[selectedNum].GetOffsetRotation()+" "+ 180);
        //PrevRotation = RePUDMS.transform.rotation * Quaternion.Euler(0,0,CenterParts.GetActiveJointLink().GetOffsetRotation()) * Quaternion.Euler(0,0,RePUDMS.GetSelectedJointSOffsetRotation()[selectedNum]);
        //Debug.LogAssertion(newRotation.eulerAngles.z);
        //生成対象が今の表示と違うなら
        if (nowPreviewPartsGroup != PartsGroup)
        {
            nowPreviewPartsGroup = PartsGroup;
            //Previewをクリア
            for (int i = 0; i < PreviewPartsObjList.Count; i++)
            {
                Destroy(PreviewPartsObjList[i].gameObject);
            }
            PreviewPartsObjList.Clear();
            //Previewを作り直し
            foreach (AbstractPartsController part in PartsGroup)
            {
                //AjointUnitがあるパーツからの相対位置
                Vector3 RefPos = newRotation * (part.transform.position - CenterParts.transform.position);
                PreviewPartsObjList.Add(Instantiate(part.gameObject, selectedPos + RefPos, PrevRotation, transform).GetComponent<AbstractPartsController>().SetPreview());
            }
        }
        //回転と移動を行う
        //for (int i = 0; i > PreviewPartsObjList.Count; i++)
        //{
        //    Vector3 RefPos = newRotation * (PartsGroup[i].transform.position - CenterParts.transform.position);
        //    PreviewPartsObjList[i].transform.position = selectedPos + RefPos;
        //    PreviewPartsObjList[i].transform.rotation = PrevRotation;
        //    //PreviewPartsObjList[i].GetComponent<AbstractPartsController>().ChildrenSColliderEnabled(false);
        //    //PreviewPartsObjList[i].GetComponent<AbstractPartsController>().ChildrenSpriteTranslucent(true);
        //}
    }
    /// <summary>
    /// プレビュー表示をするための情報を登録する
    /// </summary>
    /// <param name="inputRePUDMS"></param>
    /// <param name="inputWheelInput"></param>
    /// <param name="inputReDUMS"></param>
    public void SetPreviewInfo(RefinePlayerUnitDestroyManagementScript inputRePUDMS, float inputWheelInput, RefineDestroyedUnitManagementScript inputReDUMS)
    {
        RePUDMS = inputRePUDMS;
        wheelInput = inputWheelInput;
        ReDUMS = inputReDUMS;
    }
    public void DeletePreviewInfo()
    {
        ReDUMS = null;
    }
    /// <summary>
    /// 機体と被るかどうかの判定を行う
    /// </summary>
    /// <returns></returns>
    public bool CapturePartsCoveredPlayer()
    {
        foreach (GameObject PreviewObj in PreviewPartsObjList)
        {
            foreach (RaycastHit2D hit2D in Physics2D.RaycastAll(PreviewObj.transform.position, new Vector3(0, 0, 1)))
            {
                if (hit2D.collider.tag == GSetting.ObjTagName.PlayerUnit.ToString())
                {
                    return true;
                }
            }
        }
        return false;
    }
    public List<CaptureObjInfo> GetCaptureObjInfos()
    {
        List<AbstractPartsController> PartsGroup = ReDUMS.GetChildPartsList();
        AbstractPartsController CenterParts = ReDUMS.GetEmptyJoint();
        //空きのある機体側のパッシブジョイントユニットの座標をリストにまとめる
        List<Vector3> passiveJointList = RePUDMS.GetAllEmptyPassiveJointSPositionList();
        //プレビューを表示するために選択中の座標や回転角を渡しておく
        //マウスホイールで選べるようにする
        int selectedNum = Mathf.Abs((int)wheelInput % passiveJointList.Count);
        Vector3 selectedPos = passiveJointList[selectedNum];
        Quaternion newRotation = Quaternion.Euler(RePUDMS.transform.rotation.eulerAngles
                                                + new Vector3(0, 0, CenterParts.GetActiveJointLink().GetOffsetRotation()+ RePUDMS.GetSelectedPassiveJointList()[selectedNum].GetOffsetRotation())
                                                - ReDUMS.transform.rotation.eulerAngles);
        //Passiveのワールド角+-180が求めたい角度
        //をパーツのローカル角度（親：RePUDMS）に直す必要がある
        Quaternion PrevRotation =Quaternion.Euler(RePUDMS.transform.rotation.eulerAngles
                                                + RePUDMS.GetSelectedPassiveJointList()[selectedNum].GetAPC().transform.localRotation.eulerAngles
                                                + new Vector3(0, 0, RePUDMS.GetSelectedPassiveJointList()[selectedNum].GetOffsetRotation() + 180
                                                - CenterParts.GetActiveJointLink().GetOffsetRotation()));
        List<CaptureObjInfo> captureObjInfoList = new List<CaptureObjInfo>();
        Debug.LogAssertion("RRR" +PartsGroup.Count +" "+ PreviewPartsObjList.Count);
        for (int i = 0; i < PartsGroup.Count; i++)
        {
            Vector3 RefPos = newRotation * (PartsGroup[i].transform.position - CenterParts.transform.position);
            CaptureObjInfo captureObjInfo = new CaptureObjInfo();
            captureObjInfo.position = selectedPos + RefPos;
            captureObjInfo.rotation = PrevRotation;
            captureObjInfo.CaptureParts = PartsGroup[i];
            captureObjInfoList.Add(captureObjInfo);
            Debug.LogAssertion("QQQ" + captureObjInfo.position + captureObjInfo.rotation.eulerAngles + captureObjInfo.CaptureParts.gameObject.name);
        }
        return captureObjInfoList;
    }
}
