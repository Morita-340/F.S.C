using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// DestroyedUnitの時の移動制御を行う
/// </summary>
public class DestroyedUnitManagementScript : MonoBehaviour
{
    [SerializeField]
    GameObject EnemyUnitManager;
    [SerializeField,Range(1,100)]
    //回転係数
    protected float gyrationFactor = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// カーソルに沿って動くRayCastで外部から呼び出される
    /// </summary>
    public void MovePosition(Vector3 position){
        EnemyUnitManager.transform.position = position;
    }
    public void Spin(float wheelInput,Quaternion rotation){
        float currentRotation = 0;
        currentRotation += wheelInput;
        //EnemyUnitManager.transform.rotation = EnemyUnitManager.transform.rotation * Quaternion.AngleAxis(wheelInput*gyrationFactor,Vector3.forward);
        EnemyUnitManager.transform.rotation = Quaternion.Euler(0,0,rotation.z + currentRotation*gyrationFactor);
    }
}