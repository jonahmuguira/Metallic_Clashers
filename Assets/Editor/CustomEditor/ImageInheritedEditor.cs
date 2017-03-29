namespace CustomEditor
{
    using UnityEditor;
    using UnityEditor.UI;

    public class ImageInheritedEditor : ImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();

            var iter = serializedObject.FindProperty("m_FillOrigin");
            while (iter.NextVisible(false))
                EditorGUILayout.PropertyField(iter, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
