using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSCGeneral{
    public class GSetting : MonoBehaviour
    {
        public enum ShapeType{
            Null = 0,
            /// <summary>
            /// 正方形　４方向全てのリンクを取れる
            /// </summary>
            Square = 1,
            /// <summary>
            /// 普通の二等辺三角形　底辺方向のみリンクを取れる
            /// </summary>
            RegularTriangle = 2,
            /// <summary>
            /// 直角二等辺三角形　隣り合う２方向のみリンクを取れる
            /// </summary>
            IsoscelesRightTriangle = 3,
            /// <summary>
            /// 長方形　向かい合う２方向のみリンクを取れる
            /// </summary>
            Rectangle = 4,
            /// <summary>
            /// 鈍角五角形　上方向以外の３方向のリンクを取れる
            /// </summary>
            ObtusePentagon = 5,
        }
        /// <summary>
        /// Tag&LayerManager内に記述しているTagのインデックスナンバーを取得
        /// </summary>
        public enum ObjTagName{
            DestroyedUnit = 0,
            PlayerUnit = 1,
            EnemyUnit = 2,
            EnemyBossUnit = 3,
            PlayerWeapon1 = 4,
            PlayerWeapon2 = 5,
            EnemyWeapon1 = 6,
            EnemyWeapon2 = 7,
            ReactorExplosion = 8,
            SimulateUnit = 9,
            ReactorEffect = 10,
            CoreExplosion = 11,
            /// <summary>
            /// ユニットが画面外に出た際にプレイヤーを中心として点対称な位置に移動する為の画面外検知用の判定タグ
            /// </summary>
            DisplayScope = 12,
        }
        public enum WaveShapePreset{
            FourCorners = 0,
            Up = 1,
            Down = 2,
            Right = 3,
            Left = 4,
            UpAndDown = 5,
            RightAndDown = 6,
            UpperRightAndLowerLeft = 7,
            UpperLeftAndLowerRight = 8,
        }
        /// <summary>
        /// Tag&LayerManager内に記述しているUserLayerのインデックスナンバーを取得
        /// </summary>
        public enum UniqueLayerName{
            EnemyUnit = 3,
            DestroyedUnit = 6,
            PlayerUnit = 7,
            CaptureUnit = 9,
            PreviewUnit = 10,
        }
        /// <summary>
        /// ノージャンルのマジックナンバーはここで一元管理
        /// </summary>
        public enum UniqueMagicNumber{
            /// <summary>
            /// リアクターの強化レベル1辺りの攻撃倍率増加量
            /// </summary>
            AttackEfficiencyONReactorLevel = 1,
        }
        public enum ResultSituation{
            AllWaveClear = 0,
            PlayerDestroyed = 1,
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
