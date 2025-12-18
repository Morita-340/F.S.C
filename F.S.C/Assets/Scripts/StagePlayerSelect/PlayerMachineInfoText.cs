using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMachineInfoText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI infoText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetMachine(BodyFlag inputMachine)
    {
        //inputMachine.Body.GetComponent<RefineAbstractUnitDestroyManagementScript>().;
        RefinePlayerUnitDestroyManagementScript RePUDMS = inputMachine.Body.GetComponent<RefinePlayerUnitDestroyManagementScript>();
        RePUDMS.CaluculateCombatPower();
        PlayerUnitMoveManagementScript PUMMS = inputMachine.Body.GetComponent<PlayerUnitMoveManagementScript>();
        infoText.text = "機体名\n・"+inputMachine.Name+"\n攻撃力\n・"+RePUDMS.GetAttackPower()+"\n耐久力\n・"+RePUDMS.GetPrimeUnitsHP()+"\n移動力\n・"+PUMMS.GetMaximumSpeed()*10+"\n特徴\n・"+inputMachine.disctiption;
    }
}
