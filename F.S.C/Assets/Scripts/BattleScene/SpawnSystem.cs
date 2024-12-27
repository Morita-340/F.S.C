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
        List<GameObject> WaveEnemyList = WDB.GetAppropriateWaveData(playerCombatPower, 10,0).GetWaveEnemyList();
        List<GameObject> InstEnemyList = new List<GameObject>();
        //生成座標のプリセットから一つを選ぶ
        Vector3 InstPos = Camera.main.ViewportToWorldPoint(new Vector3(0.3f,0.8f)+new Vector3(0,0,10));
        Quaternion InstRot = Quaternion.Euler(Vector3.zero);
        Debug.Log("SpS"+WaveEnemyList.Count);
        foreach (GameObject Enemy in WaveEnemyList){
            //取得したウェーブの敵機を生成する（生成場所の計算も済ませる）
            InstEnemyList.Add(Instantiate(Enemy,InstPos,InstRot));
        }
        //生成した敵機の情報を渡す
        return InstEnemyList;
    }
}
