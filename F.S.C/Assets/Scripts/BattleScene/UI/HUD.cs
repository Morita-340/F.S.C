using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSCGeneral;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 戦闘画面の固定UI
/// 画面奥にオブジェクトが存在するなら透過処理を行う
/// </summary>
public class HUD : MonoBehaviour
{
    protected RectTransform thisRectTransform;
    Camera mainCamera;
    CanvasGroup canvasGroup;
    [SerializeField, ReadOnly]
    Vector3 worldRaypos = Vector3.zero;
    protected Vector2 rayPos = Vector2.zero;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        thisRectTransform = GetComponent<RectTransform>();
        rayPos = (Vector2)thisRectTransform.position;
        canvasGroup = GetComponent<CanvasGroup>();
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        canvasGroup.alpha = CheckBehind() ? 0.3f : 1f;
        rayPos = (Vector2)thisRectTransform.position;
    }
    public void SetCamera(Camera inputCamera)
    {
        mainCamera = inputCamera;
    }
    /// <summary>
    /// UIの後ろにオブジェクトがあるかを判定する
    /// </summary>
    protected bool CheckBehind()
    {
        bool flag = false;
        Vector3[] corners = new Vector3[5];
        thisRectTransform.GetWorldCorners(corners);
        corners[4] = thisRectTransform.position;
        foreach (Vector3 corners3 in corners)
        {
            // UIのスクリーン座標
            Vector2 screenPos =
                RectTransformUtility.WorldToScreenPoint(
                    mainCamera,
                    corners3
                );

            // スクリーン → ワールド（CanvasのPlane上）
            Vector3 worldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                thisRectTransform,
                screenPos,
                mainCamera,
                out worldPos
            );

            worldRaypos = worldPos; // デバッグ確認用

            // 2D判定（点）
            Collider2D[] hits = new Collider2D[10];
            Physics2D.OverlapPointNonAlloc(worldPos, hits);
            foreach (var hit in hits)
            {
                if (hit == null) { continue; }
                switch ((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), hit.gameObject.tag, true))
                {
                    case GSetting.ObjTagName.SimulateUnit:
                    case GSetting.ObjTagName.ReactorEffect:
                    case GSetting.ObjTagName.DisplayScope:
                        flag = false; break;
                    default: flag = true; break;
                }
                //Debug.LogWarning(hit.gameObject.name);
                if (flag) Debug.LogWarning(hit.gameObject.name); break;
            }
        }
        return flag;
    }
}
