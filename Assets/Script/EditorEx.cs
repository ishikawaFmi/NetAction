using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class Button : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Button"))
        {
            GameManager.Incetance.GameStart.OnNext(Unit.Default);
        }
    }
}
#endif