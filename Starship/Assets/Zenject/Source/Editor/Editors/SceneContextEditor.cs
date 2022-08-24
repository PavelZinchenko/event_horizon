using UnityEditor;

namespace Zenject
{
    [CustomEditor(typeof(SceneContext))]
    public class SceneContextEditor : ContextEditor
    {
        SerializedProperty _parentNewObjectsUnderRootProperty;

        public override void OnEnable()
        {
            base.OnEnable();

            _parentNewObjectsUnderRootProperty = serializedObject.FindProperty("_parentNewObjectsUnderRoot");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_parentNewObjectsUnderRootProperty);
        }
    }
}



