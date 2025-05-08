using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// GeneralFlagManagerの有無を監視し、無い場合のみ生成する
/// 全シーンに配置する
/// </summary>
public class FlagManagerObserver : MonoBehaviour
{
    [SerializeField]GameObject GFM;
    void Awake()
    {
        if(FindObjectOfType<GeneralFlagManager>() == null){
            Instantiate(GFM);
        }
    }
}
