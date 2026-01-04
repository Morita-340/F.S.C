using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSCGeneral;

public class RefineSpawnSystem : MonoBehaviour
{
    /// <summary>
    /// プレイヤーの戦闘力と一番近い戦闘力の機体が出撃する（ステータスだけ弄った機体ではなく、特性の違う敵を出現させる形。強い敵ほど鹵獲パーツも凄い）
    /// </summary>
    /// <param name="playerCombatPower"></param>
    /// <param name="inputReEUDMSList"></param>
    /// <returns></returns>
    public List<RefineEnemyUnitDestroyManagementScript> Spawn(float playerCombatPower, List<RefineEnemyUnitDestroyManagementScript> inputReEUDMSList,Vector3[] InstPosList)
    {
        List<RefineEnemyUnitDestroyManagementScript> InstEnemyList = new List<RefineEnemyUnitDestroyManagementScript>();
        //案その１生成対象を戦闘力に応じて決める（戦闘力に合わせて生成数を増減とか、リストNo.10のところに強いユニットを格納しておくことで10体以上生成するなら強いユニットも生成するようにするとか）
        //
        //案その２とりあえずリストに格納してあるユニットは全生成
        for (int i = 0; i < inputReEUDMSList.Count; i++)
        {
            Vector3 InstPos = Camera.main.ViewportToWorldPoint(new Vector3(InstPosList[i].x, InstPosList[i].y) + new Vector3(0, 0, 10));
            Quaternion InstRot = Quaternion.Euler(new Vector3(0, 0, InstPosList[i].z));
            //取得したウェーブの敵機を生成する（生成場所の計算も済ませる）
            if (i < (int)GSetting.UniqueMagicNumber.WaveEnemyListLength)
            {
                InstEnemyList.Add(Instantiate(inputReEUDMSList[i], InstPos * 1.2f, InstRot));
            }
            if (i >= InstPosList.Length) { Debug.LogWarning("ウェーブの敵機数がウェーブの座標数より多いです"); break; }//生成座標の配列の外にアクセスしないようにするため
        }
        return InstEnemyList;
    }
}
