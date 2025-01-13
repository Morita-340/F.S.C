using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    [SerializeField]
    PlayerUnitDestroyManagementScript PUDMS;
    [SerializeField]
    WaveDataBase WDB;
    int playerCombatPower = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerCombatPower = PUDMS.GetCombatPower();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public List<GameObject> Spawn(int nowWave){
        playerCombatPower = PUDMS.CaluculateCombatPower();
        //ウェーブライブラリからウェーブを取得する。難易度調整のパラメータをどこから取るのかは要検討
        WaveData UseWave = WDB.GetAppropriateWaveData(playerCombatPower, 10,0);
        Vector3[] InstPosRotInfo = UseWave.GetSelectedWaveShapePreset();
        List<GameObject> WaveEnemyList = UseWave.GetWaveEnemyList();
        List<GameObject> InstEnemyList = new List<GameObject>();
        //生成座標のプリセットから一つを選ぶ
        for(int i = 0;i < WaveEnemyList.Count;i++){
            Vector3 InstPos = Camera.main.ViewportToWorldPoint(new Vector3(InstPosRotInfo[i].x,InstPosRotInfo[i].y)+new Vector3(0,0,10));
            Quaternion InstRot = Quaternion.Euler(new Vector3(0,0,InstPosRotInfo[i].z));
            //取得したウェーブの敵機を生成する（生成場所の計算も済ませる）
            InstEnemyList.Add(Instantiate(WaveEnemyList[i],InstPos,InstRot));
            if(i >= InstPosRotInfo.Length){Debug.LogWarning("ウェーブの敵機数がウェーブの座標数より多いです"); break;}//生成座標の配列の外にアクセスしないようにするため
        }
        //生成した敵機の情報を渡す
        return InstEnemyList;
    }
}
