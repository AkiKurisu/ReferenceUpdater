# Reference Updater

`SerializeReference` is a power serialization feature in Unity, however when changing class name, namespace or assembly, data will loss.
This repo give you a tool to update your class more easily.

## How to use
1. Do not change your source class directly
2. Duplicate and modify mirrored one
3. Create ReferenceUpdater/UpdateConfig
4. Select original class type and select new class type
5. Click `Update API`

## Limit
Currently updater only find ``ScriptableObject`` in your project.