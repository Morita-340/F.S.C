using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 設定画面において試射用の敵を用意する
/// </summary>
public class SettingEnemySpawnSystem : MonoBehaviour
{
    /// <summary>
    /// スポーンさせる敵のリスト。この中から抽選する
    /// </summary>
    [SerializeField] List<GameObject> SpawnEnemyList = new List<GameObject>();
    [SerializeField] GameObject EnemySpawnPoint;
    private int nowSpawnTimes = 0;
    /// <summary>
    /// スポーン回数の上限
    /// </summary>
    private int spawnTimesLimit = 100;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemySpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 撃破するたびに敵を1機ずつスポーンさせる。無限ではなく計100機にとどめる（100機撃墜すると隠し要素を解除するようにしたいから）
    /// </summary>
    /// <returns></returns>
    IEnumerator EnemySpawn()
    {
        while(nowSpawnTimes < spawnTimesLimit)
        {
            if(SpawnEnemyList.Count <= 0){Debug.LogWarning("SpawnEnemyList is Empty!");break;}
            int randNum = Random.Range(0,SpawnEnemyList.Count);
            GameObject SelectedEnemy = SpawnEnemyList[randNum];
            GameObject InstEnemy = Instantiate(SelectedEnemy,EnemySpawnPoint.transform.position/* - new Vector3(0,5,0)*/,EnemySpawnPoint.transform.rotation);
            yield return new WaitUntil(() => EnemyDead(InstEnemy));
            yield return new WaitForSeconds(2f);
        }
        yield return null;
    }
    private bool EnemyDead(GameObject Enemy){
        if(Enemy.GetComponent<EnemyUnitDestroyManagementScript>().GetIsDead() == false){
            return false;
        }else{return true;}
    }
}
