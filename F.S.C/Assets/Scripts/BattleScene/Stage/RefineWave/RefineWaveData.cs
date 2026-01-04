using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;
public abstract class RefineWaveData : ScriptableObject
{
    [SerializeField]protected string GoalText;
    [SerializeField]protected string ExplainText;
    [SerializeField]protected Vector2 ExplainTexPos;
    [SerializeField]protected GSetting.WaveShapePreset waveShapePreset;
    protected IWaveRuntime waveRuntime;
    /// <summary>
    /// 前から順に座標を利用していく。改行単位で1ウェーブの敵機数を増やす（3-6-9-12,4-8-12）
    /// </summary>
    protected Vector3[][] InstPosPreSet = new Vector3[9][]{//0-1 画面上のx座標,画面上のy座標,-180-180回転角
        //四隅に出現
        new Vector3[12]{new Vector3(0.1f,0.9f,-120),new Vector3(0.1f,0.1f,-60),new Vector3(0.9f,0.9f,120),new Vector3(0.9f,0.1f,60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0)},
        //上から塊で出現
        new Vector3[12]{new Vector3(0.5f,0.9f,180),new Vector3(0.1f,0.9f,-120),new Vector3(0.9f,0.9f,120),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //下から塊で出現
        new Vector3[12]{new Vector3(0.5f,0.1f,0),new Vector3(0.1f,0.1f,-60),new Vector3(0.9f,0.1f,60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //右から塊で出現
        new Vector3[12]{new Vector3(0.9f,0.5f,90),new Vector3(0.9f,0.9f,120),new Vector3(0.9f,0.1f,60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //左から塊で出現
        new Vector3[12]{new Vector3(0.1f,0.5f,-90),new Vector3(0.1f,0.9f,-120),new Vector3(0.1f,0.1f,-60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //上下から出現
        new Vector3[12]{new Vector3(0.3f,0.9f,180),new Vector3(0.3f,0.1f,0),new Vector3(0.7f,0.9f,180),new Vector3(0.7f,0.1f,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //左右から出現
        new Vector3[12]{new Vector3(0.9f,0.3f,90),new Vector3(0.1f,0.3f,-90),new Vector3(0.9f,0.7f,90),new Vector3(0.1f,0.7f,-90),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //右上と左下から出現
        new Vector3[12]{new Vector3(0.7f,0.9f,120),new Vector3(0.3f,0.1f,-60),new Vector3(0.9f,0.7f,120),new Vector3(0.1f,0.3f,-60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
        //左上と右下から出現
        new Vector3[12]{new Vector3(0.3f,0.9f,-120),new Vector3(0.9f,0.3f,60),new Vector3(0.1f,0.7f,-120),new Vector3(0.7f,0.1f,60),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),
                        new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,0,0),},
    };
    /// <summary>
    /// Vector3だが、（0-1 画面上のx座標,画面上のy座標,-180-180回転角）を割り当てている
    /// </summary>
    /// <returns></returns>
    protected Vector3[] GetSelectedWaveShapePreset(){
        return InstPosPreSet[(int)waveShapePreset];
    }
    public abstract IWaveRuntime InitialSetting(RefineBattleSceneFlowManager ReBSFM, RefinePlayerUnitDestroyManagementScript inputRePUDMS);
    public virtual void DestroyProcess(RefineBattleSceneFlowManager ReBSFM)
    {
        //RefineSpSはWaveに関係なく消去したいので
        waveRuntime?.DestroyProcess();
    }
    public abstract string GetGoalText();
    public abstract string GetExplainText();
    public abstract Vector2 GetExplainTexPos();
}
