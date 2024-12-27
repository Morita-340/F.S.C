using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
 
 /// <summary>
 /// カスタム属性でインスペクターでは編集不可能な属性を付ける
 /// </summary>
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(_position, _property, _label);
        EditorGUI.EndDisabledGroup();
    }
}
#endif