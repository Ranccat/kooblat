using UnityEngine;
using UnityEditor;
using System.Linq;

public class RemoveMissingScripts : EditorWindow
{
	private GameObject prefab;

	[MenuItem("Tools/Remove Missing Scripts from Prefab")]
	public static void ShowWindow()
	{
		GetWindow(typeof(RemoveMissingScripts));
	}

	void OnGUI()
	{
		prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

		if (GUILayout.Button("Remove Missing Scripts"))
		{
			if (prefab != null)
			{
				RemoveMissingScriptsFromPrefab(prefab);
			}
			else
			{
				Debug.LogWarning("Please select a prefab first.");
			}
		}

		if (GUILayout.Button("Remove Missing Scripts from All Prefabs"))
		{
			RemoveMissingScriptsFromAllPrefabs();
		}
	}

	void RemoveMissingScriptsFromPrefab(GameObject prefab)
	{
		string assetPath = AssetDatabase.GetAssetPath(prefab);
		GameObject prefabAsset = PrefabUtility.LoadPrefabContents(assetPath);

		int removedCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabAsset);

		if (removedCount > 0)
		{
			PrefabUtility.SaveAsPrefabAsset(prefabAsset, assetPath);
			Debug.Log($"Removed {removedCount} missing scripts from {prefab.name}");
		}
		else
		{
			Debug.Log($"No missing scripts found on {prefab.name}");
		}

		PrefabUtility.UnloadPrefabContents(prefabAsset);
	}

	void RemoveMissingScriptsFromAllPrefabs()
	{
		string[] allPrefabPaths = AssetDatabase.GetAllAssetPaths()
			.Where(path => path.EndsWith(".prefab"))
			.ToArray();

		int totalRemovedCount = 0;

		foreach (string prefabPath in allPrefabPaths)
		{
			GameObject prefabAsset = PrefabUtility.LoadPrefabContents(prefabPath);
			int removedCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabAsset);

			if (removedCount > 0)
			{
				PrefabUtility.SaveAsPrefabAsset(prefabAsset, prefabPath);
				totalRemovedCount += removedCount;
				Debug.Log($"Removed {removedCount} missing scripts from {prefabPath}");
			}

			PrefabUtility.UnloadPrefabContents(prefabAsset);
		}

		Debug.Log($"Removed a total of {totalRemovedCount} missing scripts from all prefabs");
	}
}