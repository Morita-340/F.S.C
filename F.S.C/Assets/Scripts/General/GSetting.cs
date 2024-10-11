using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSCGeneral{
    public class GSetting : MonoBehaviour
    {
        public enum ShapeType{
            Null = 0,
            Square = 1,
            RegularTriangle = 2,
            IsoscelesRightTriangle = 3,
        }
        public enum ObjTagName{
            DestroyedUnit = 0,
            PlayerUnit = 1,
            EnemyUnit = 2,
            EnemyBossUnit = 3,
            PlayerWeapon1 = 4,
            PlayerWeapon2 = 5,
            EnemyWeapon1 = 6,
            EnemyWeapon2 = 7,
            Weapon5 = 8,

        }
        public string TagEnumToString(int num)
        {
            switch (num){
                case 0: return "DestroyedUnit";
                case 1: return "PlayerUnit";
                case 2: return "EnemyUnit";
                case 3: return "EnemyBossUnit";
                case 4: return "PlayerWeapon1  ";
                case 5: return "PlayerWeapon2";
                case 6: return "EnemyWeapon1";
                case 7: return "EnemyWeapon2";
                case 8: return "Weapon5";
                default: Debug.LogAssertion("TagName Enum IsNot Registared!"); return null;
            }   
        }
        public int TagStringToEnum(string name){
            switch (name){
                case "DestroyedUnit": return 0;
                case "PlayerUnit": return 1;
                case "EnemyBossUnit" : return 2;
                case "PlayerWeapon1  ":return 3;
                case "PlayerWeapon2": return 4;
                case "EnemyWeapon1": return 5;
                case "EnemyWeapon2": return 6;
                case "Weapon5": return 7;
                default: Debug.LogAssertion("TagName Enum IsNot Registared!"); return -1;
            }
        }
    }
}
