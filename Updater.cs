using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Kurisu.ReferenceUpdater
{
    public class Updater
    {
        public static void UpdateAPI(UpdateConfig updateConfig)
        {
            UpdateAPI(typeof(ScriptableObject), updateConfig);
        }
        public static void UpdateAPI(Type filterAssetType, UpdateConfig updateConfig)
        {
            var assets = AssetDatabase.FindAssets($"t:{filterAssetType}")
           .Select(x => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(x)))
           .ToList();
            foreach (var asset in assets)
            {
                IterateAPI(asset, updateConfig);
            }
        }

        private static void IterateAPI(ScriptableObject asset, UpdateConfig updateConfig)
        {
            var serializeObject = new SerializedObject(asset)
            {
                forceChildVisibility = true
            };
            bool isDirty = false;
            SerializedProperty iterator = serializeObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (iterator.propertyType == SerializedPropertyType.ManagedReference)
                {
                    foreach (var pair in updateConfig.Pairs)
                    {
                        if (iterator.managedReferenceFullTypename == pair.sourceType.GetFullTypeName())
                        {
                            iterator.managedReferenceValue = JsonUtility.FromJson(JsonUtility.ToJson(iterator.managedReferenceValue), pair.targetType.Type);
                            isDirty = true;
                        }
                    }
                }
            }
            if (isDirty)
            {
                Debug.Log($"{asset.name} update");
                serializeObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(asset);
            }
            serializeObject.Dispose();
        }
    }
}
