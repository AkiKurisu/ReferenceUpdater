using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.ReferenceUpdater
{
    [CreateAssetMenu(fileName = "UpdateConfig", menuName = "ReferenceUpdater/UpdateConfig")]
    public class UpdateConfig : ScriptableObject
    {
        [Serializable]
        public class SerializeType
        {
            public Type Type => Type.GetType(assemblyQualifiedName);
            public string assemblyQualifiedName;
            public string GetFullTypeName()
            {
                return $"{Type.Assembly.GetName().Name} {Type.FullName}";
            }
        }
        [Serializable]
        public class Pair
        {
            public SerializeType sourceType;
            public SerializeType targetType;
        }
        [field: SerializeField]
        public Pair[] Pairs { get; set; }
    }
    [CustomPropertyDrawer(typeof(UpdateConfig.SerializeType))]
    public class SerializeTypeDrawer : PropertyDrawer
    {
        private const string NullType = "Null";
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var reference = property.FindPropertyRelative("assemblyQualifiedName");
            var type = Type.GetType(reference.stringValue);
            string id = type != null ? $"{type.Assembly.GetName().Name} {type.Namespace} {type.Name}" : NullType;
            if (EditorGUI.DropdownButton(position, new GUIContent(id), FocusType.Keyboard))
            {
                var provider = ScriptableObject.CreateInstance<TypeSearchWindow>();
                provider.Initialize((type) =>
                {
                    reference.stringValue = type?.AssemblyQualifiedName ?? NullType;
                    property.serializedObject.ApplyModifiedProperties();
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
            }
            EditorGUI.EndProperty();
        }
    }
    [CustomEditor(typeof(UpdateConfig))]
    public class UpdateConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Update API"))
            {
                Updater.UpdateAPI(target as UpdateConfig);
            }
        }
    }
}
