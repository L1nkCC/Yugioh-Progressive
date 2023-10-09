namespace UnityEditor.UI
{
    [CustomEditor(typeof(INS_Arrow), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the LP_Button
    /// </summary>
    public class INS_ArrowEditor : ImageEditor
    {
        SerializedProperty typeProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            typeProperty = serializedObject.FindProperty("arrowType");
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
