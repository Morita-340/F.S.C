using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSCGeneral{
    public class GSetting : MonoBehaviour
    {
        public enum ShapeType
        {
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
        public enum PartsDamageStatus
        {
            NotSet = 0,
            //無傷。多少の被弾によるダメージ蓄積は許容。ひとつもユニットが破壊されていなければ該当
            NoDamage = 1,
            //小破。一部ユニットが破壊されているもののパーツの機能＆ジョイントユニットが生きている状態
            MinorDamage = 2,
            //大破。機能を持つユニットorジョイントユニットのどちらかが完全に破壊されるか分離しており、アクセスできない状態
            HeavilyDamage = 3,
        }
        public enum PartsConnectSituation
        {
            NotSet = 0,
            //大破パーツのみ＝小破以下のまとまりが0
            OnlyWreckParts = 1,
            //大破パーツを挟んで小破以下のパーツがくっついている＝小破以下のまとまりが複数
            MultipleScrachPartsGroup = 2,
            //複数の小破以下のパーツが直接くっついている（一つだけでもいい）＝小破以下のまとまりが一つだけ
            A_ScrachPartsGroup = 3,
        }
        /// <summary>
        /// Tag&LayerManager内に記述しているTagのインデックスナンバーを取得
        /// </summary>
        public enum ObjTagName
        {
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
            GrenadeExplosion = 13,
        }
        public enum WaveShapePreset
        {
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
        /// ロードするシーンをシリアライズで設定する用（SceneAssetは実行ファイルでは使用不可なので）
        /// </summary>
        public enum SceneName
        {
            SampleScene = 0,
            Title = 1,
            StageWeaponBodySelect = 2,
            InterceptEnemyForce = 3,
            TutorialStage = 4,
        }
        /// <summary>
        /// Tag&LayerManager内に記述しているUserLayerのインデックスナンバーを取得
        /// </summary>
        public enum UniqueLayerName
        {
            EnemyUnit = 3,
            DestroyedUnit = 6,
            PlayerUnit = 7,
            CaptureUnit = 9,
            PreviewUnit = 10,
        }
        /// <summary>
        /// ノージャンルのマジックナンバーはここで一元管理
        /// </summary>
        public enum UniqueMagicNumber
        {
            /// <summary>
            /// リアクターの強化レベル1辺りの攻撃倍率増加量
            /// </summary>
            AttackEfficiencyONReactorLevel = 1,
            /// <summary>
            /// スポーンシステムにおける一度に生成できる敵数の上限
            /// </summary>
            WaveEnemyListLength = 30,
        }
        public enum UniqueObjectName
        {
            RangeMesh = 0,
            ReactorEffectPool = 1,
            RangeMeshPool = 2,
            UICanvas = 3,
        }
        public enum ResultSituation
        {
            AllWaveClear = 0,
            PlayerDestroyed = 1,
        }
        /// <summary>
        /// 汎用的なエラーログ。エラーの原因のオブジェクトと想定されるエラー内容を出力できる
        /// </summary>
        /// <param name="debugObj">エラーの原因のオブジェクト。親オブジェクトや根オブジェクトも出力できる</param>
        /// <param name="message">エラーメッセージ</param>
        public static void RefineDebugAssertinLog(Transform debugObj, string message)
        {
            Debug.LogAssertion("アルゴリズムがおかしい。" + message + debugObj.name + "\n親：" + debugObj.parent?.name + "\n根：" + debugObj.root.name);
        }
    }
}
