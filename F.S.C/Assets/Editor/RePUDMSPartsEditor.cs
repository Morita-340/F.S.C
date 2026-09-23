using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RefinePlayerUnitDestroyManagementScript))]
public class RePUDMSPartsEditor : Editor
{
    // 8組分の情報をまとめて管理
    private class FieldSet
    {
        public readonly string Label;
        public readonly string FlagAName;
        public readonly string FlagBName;
        public readonly string VariableNameX;
        public readonly string VariableNameY;
        public bool Foldout; // 開閉状態をここで保持

        public FieldSet(string label, string flagAName, string flagBName, string variableNameX,string variableNameY)
        {
            Label = label;
            FlagAName = flagAName;
            FlagBName = flagBName;
            VariableNameX = variableNameX;
            VariableNameY = variableNameY;
            Foldout = true; // 初期状態は開いた状態にしておく
        }
    }

    private FieldSet[] fieldSets;
    private string[] excludedPropertyNames; // 手動描画する分を除外するためのリスト

    private void OnEnable()
    {
        fieldSets = new FieldSet[8];
        fieldSets[0] = new FieldSet("Front",
                                    "slotFlag_Front",
                                    "Integrated_Front",
                                    "PassiveJoint_Front",
                                    "Part_Front");
        fieldSets[1] = new FieldSet("FrontRight",
                                    "slotFlag_FrontRight",
                                    "Integrated_FrontRight",
                                    "PassiveJoint_FrontRight",
                                    "Part_FrontRight");
        fieldSets[2] = new FieldSet("FrontLeft",
                                    "slotFlag_FrontLeft",
                                    "Integrated_FrontLeft",
                                    "PassiveJoint_FrontLeft",
                                    "Part_FrontLeft");
        fieldSets[3] = new FieldSet("Right",
                                    "slotFlag_Right",
                                    "Integrated_Right",
                                    "PassiveJoint_Right",
                                    "Part_Right");
        fieldSets[4] = new FieldSet("Left",
                                    "slotFlag_Left",
                                    "Integrated_Left",
                                    "PassiveJoint_Left",
                                    "Part_Left");
        fieldSets[5] = new FieldSet("Back",
                                    "slotFlag_Back",
                                    "Integrated_Back",
                                    "PassiveJoint_Back",
                                    "Part_Back");
        fieldSets[6] = new FieldSet("BackRight",
                                    "slotFlag_BackRight",
                                    "Integrated_BackRight",
                                    "PassiveJoint_BackRight",
                                    "Part_BackRight");
        fieldSets[7] = new FieldSet("BackLeft",
                                    "slotFlag_BackLeft",
                                    "Integrated_BackLeft",
                                    "PassiveJoint_BackLeft",
                                    "Part_BackLeft");
        // 開閉状態をGameObjectごとに保持したい場合はSessionStateキーに使う
        for (int i = 0; i < fieldSets.Length; i++)
        {
            string key = GetFoldoutKey(fieldSets[i].Label);
            fieldSets[i].Foldout = SessionState.GetBool(key, true);
        }
        // 手動描画する36個のプロパティ名を集めておく（m_Scriptも除外対象に含める）
        var excluded = new List<string> { "m_Script" };
        for (int i = 0; i < fieldSets.Length; i++)
        {
            excluded.Add(fieldSets[i].FlagAName);
            excluded.Add(fieldSets[i].FlagBName);
            excluded.Add(fieldSets[i].VariableNameX);
            excluded.Add(fieldSets[i].VariableNameY);
        }
        excludedPropertyNames = excluded.ToArray();
    }

    // インスペクタ再描画のたびにOnEnableが呼ばれるとは限らないため、
    // ターゲットのインスタンスIDとラベルを組み合わせて一意なキーにする
    private string GetFoldoutKey(string label)
    {
        return $"RePUDMSPartsEditor_Foldout_{target.GetInstanceID()}_{label}";
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        // 手動描画対象以外（継承元のSerializeFieldなど）をここでまとめて自動描画
        DrawPropertiesExcluding(serializedObject, excludedPropertyNames);

        EditorGUILayout.Space(8);
        for (int i = 0; i < fieldSets.Length; i++)
        {
            DrawFieldSet(fieldSets[i]);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFieldSet(FieldSet set)
    {
        SerializedProperty flagA = serializedObject.FindProperty(set.FlagAName);
        SerializedProperty flagB = serializedObject.FindProperty(set.FlagBName);
        SerializedProperty variableX = serializedObject.FindProperty(set.VariableNameX);
        SerializedProperty variableY = serializedObject.FindProperty(set.VariableNameY);

        if (flagA == null || flagB == null || variableX == null || variableY == null)
        {
            EditorGUILayout.HelpBox($"{set.Label}: プロパティが見つかりません", MessageType.Error);
            return;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // ラベルクリックで開閉するFoldout
        bool newFoldout = EditorGUILayout.Foldout(set.Foldout, set.Label, true, EditorStyles.foldoutHeader);
        if (newFoldout != set.Foldout)
        {
            set.Foldout = newFoldout;
            SessionState.SetBool(GetFoldoutKey(set.Label), set.Foldout);
        }

        if (set.Foldout)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(flagA);

            if (flagA.boolValue)
            {
                EditorGUILayout.PropertyField(flagB);
                EditorGUI.BeginDisabledGroup(flagB.boolValue);
                EditorGUILayout.PropertyField(variableX);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(!flagB.boolValue);
                EditorGUILayout.PropertyField(variableY);
                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(4);
    }
}