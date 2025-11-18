using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using UnityEngine;
/// <summary>
/// プレビュー表示の制御
/// 表示対象のパーツ情報及び表示の起点座標（activeJointUnitの座標）および回転角を指定すると、指定箇所にプレビュー表示を行う
/// 機体と被るかどうかの判定も行ってもらう
/// </summary>
public class RefinePreviewControll : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private List<AbstractPartsController> PreviewPartsGroup;
    private AbstractPartsController CenterParts;
    [SerializeField, ReadOnly]
    private Vector3 PreviewPosition;
    [SerializeField, ReadOnly]
    private Vector3 PreviewRotation;
    List<GameObject> PreviewObjList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PreviewDisplay(PreviewPartsGroup,CenterParts, PreviewPosition, PreviewRotation);
    }
    /// <summary>
    /// プレビュー表示
    /// 座標と回転角で補正する
    /// spriteRendererでα値も調整してホログラム感を醸し出す
    /// </summary>
    /// <param name="PartsGroup">Previewとして複製する対象</param>
    /// <param name="CenterParts">パーツの塊の中心（これから合体させるActiveJointUnitがあるパーツ）</param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    private void PreviewDisplay(List<AbstractPartsController> PartsGroup, AbstractPartsController CenterParts, Vector3 position, Vector3 rotation)
    {
        if (PartsGroup == null)
        {
            return;
        }
        //生成対象が今の表示と違うなら
        if (PartsGroup != PreviewPartsGroup)
        {
            //Previewをクリア
            for (int i = 0; i < PreviewObjList.Count; i++)
            {
                Destroy(PreviewObjList[i].gameObject);
            }
            PreviewObjList.Clear();
            //Previewを作り直し
            foreach (AbstractPartsController part in PartsGroup)
            {
                //AjointUnitがあるパーツからの相対位置
                Vector3 RefPos = part.transform.position - CenterParts.transform.position;
                PreviewObjList.Add(Instantiate(part.gameObject, position + RefPos, Quaternion.Euler(rotation), transform).GetComponent<AbstractPartsController>().SetPreview());

            }
        }
        //回転と移動を行う
        foreach (AbstractPartsController part in PartsGroup)
        {
            //公転
            //PUDMSの回転中心からの相対座標を取得し、Quaternionで回転
            //自転
            //PUDMSのrotationだけ回転させる
        }
    }
    /// <summary>
    /// プレビュー表示をするための情報を登録する
    /// </summary>
    /// <param name="Copy"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void SetPreviewInfo(List<AbstractPartsController> CapturePartsGroup,AbstractPartsController centerParts, Vector3 position, Vector3 rotation)
    {
        PreviewPartsGroup = CapturePartsGroup;
        CenterParts = centerParts;
        PreviewPosition = position;
        PreviewRotation = rotation;
    }
    /// <summary>
    /// 機体と被るかどうかの判定を行う
    /// </summary>
    /// <returns></returns>
    public bool CapturePartsCoveredPlayer()
    {
        foreach (AbstractPartsController part in PreviewPartsGroup)
        {
            /*
            if (part.CheckPartsCoveredPlayer() == true)
            {
                return true;
            }
            */
        }
        return false;
    }
}
