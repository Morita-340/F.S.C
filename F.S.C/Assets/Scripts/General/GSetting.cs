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
        public string weaponTag = "";
    }
}
