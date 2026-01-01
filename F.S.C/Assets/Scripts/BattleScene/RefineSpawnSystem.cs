using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefineSpawnSystem : MonoBehaviour
{
    /// <summary>
    /// プレイヤーの戦闘力と一番近い戦闘力の機体が出撃する
    /// </summary>
    /// <param name="playerCombatPower"></param>
    /// <param name="inputReEUDMSList"></param>
    /// <returns></returns>
    public List<RefineEnemyUnitDestroyManagementScript> Spawn(float playerCombatPower, List<RefineEnemyUnitDestroyManagementScript> inputReEUDMSList)
    {
        List<RefineEnemyUnitDestroyManagementScript> InstEnemyList = new List<RefineEnemyUnitDestroyManagementScript>();
        //生成対象を決める

        for (int i = 0; i < playerCombatPower / 100; i++)
        {
            //
            //InstEnemyList.Add();
        }
        return InstEnemyList;
    }
}
