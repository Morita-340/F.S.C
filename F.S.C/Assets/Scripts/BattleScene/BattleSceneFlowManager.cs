using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSceneFlowManager : MonoBehaviour
{
    int waveNum = 10;
    int nowWave = 1;
    bool playerIsDead = false;
    SpawnSystem SpS;
    [SerializeField]
    PlayerUnitDestroyManagementScript PUDMS;
    List<GameObject> WaveEnemyList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        SpS = this.GetComponent<SpawnSystem>();
        Debug.Log("BSFM waveNum "+waveNum);
        StartCoroutine(GameFlow());
        //while(nowWave <= waveNum){
        //    Debug.Log("BSFM "+nowWave);
        //    WaveEnemyList = SpS.Spawn(nowWave);
        //    nowWave ++;
        //    //PUDMSの撃墜判定が有効ならループを直ぐに抜ける
        //    if(playerIsDead){Debug.Log("BSFM PlayerDead");break;}
        //    //WaveEnemyListの全機体が撃墜されるまで次のループに移らない
        //    StartCoroutine(WaitUntilAllEnemyDead(WaveEnemyList));
        //}
        ////最終ウェーブまで到達またはプレイヤーが撃墜されたのでスコア計算を行いバトルを終える
        //BattleEndProcess();
    }
    IEnumerator GameFlow(){
        while(nowWave <= waveNum){
            Debug.Log("BSFM "+nowWave);
            WaveEnemyList = SpS.Spawn(nowWave);
            nowWave ++;
            //PUDMSの撃墜判定が有効ならループを直ぐに抜ける
            if(playerIsDead){Debug.Log("BSFM PlayerDead");break;}
            //WaveEnemyListの全機体が撃墜されるまで次のループに移らない
            yield return new WaitUntil(() => AllEnemyDead(WaveEnemyList));
            yield return new WaitForSeconds(2f);
        }
        //最終ウェーブまで到達またはプレイヤーが撃墜されたのでスコア計算を行いバトルを終える
        BattleEndProcess();
    }
    /// <summary>
    /// 敵機が全て撃墜されるまで処理を中断する
    /// </summary>
    /// <param name="WaveEnemyList"></param>
    /// <returns></returns>
    IEnumerator WaitUntilAllEnemyDead(List<GameObject> WaveEnemyList){
        yield return new WaitUntil(() => AllEnemyDead(WaveEnemyList));
        Debug.Log("BSFM AllEnemyDead");
    }
    /// <summary>
    /// 敵機が全て撃墜されたのか調べる
    /// </summary>
    /// <param name="WaveEnemyList"></param>
    /// <returns></returns>
    private bool AllEnemyDead(List<GameObject> WaveEnemyList){
        foreach(GameObject WaveEnemy in WaveEnemyList){
            if(WaveEnemy.GetComponent<EnemyUnitDestroyManagementScript>().GetIsDead() == false){
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        playerIsDead = PUDMS.GetIsDead();
    }
    private void BattleEndProcess(){
        Debug.Log("BSFM BEP");
    }
}
