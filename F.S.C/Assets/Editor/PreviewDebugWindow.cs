#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using UnityEngine;

public class PreviewDebugWindow : EditorWindow 
{
    [MenuItem("Window/PreviewDebugWindow")]
    private static void ShowDebugWindow(){
         EditorWindow.GetWindow<PreviewDebugWindow>();
    }
}
