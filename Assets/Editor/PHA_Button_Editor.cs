using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Again god bless the internet and more specifically @pcosgrave on github.  https://github.com/Unity-Technologies/uGUI/blob/2019.1/UnityEditor.UI/UI/ButtonEditor.cs
/// </summary>
namespace UnityEditor.UI
{
    [CustomEditor(typeof(PHA_Button), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the PHA_Button
    /// </summary>
    public class PHA_ButtonEditor : ButtonEditor
    {
        SerializedProperty typeProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            typeProperty = serializedObject.FindProperty("type");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(typeProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

