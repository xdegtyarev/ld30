/*
 * File:           GAFConverterWindowListener.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.IO;

using GAFEditor.Converter.Window;

[InitializeOnLoad]
public static class GAFConverterWindowListener 
{
	static GAFConverterWindowListener()
	{
		GAFConverterWindowEventDispatcher.onCreateMovieClipEvent	+= onCreateMovieClip;
		GAFConverterWindowEventDispatcher.onCreatePrefabEvent		+= onCreatePrefab;
	}

	private static void onCreateMovieClip(string _AssetPath)
	{
		var assetName	= Path.GetFileNameWithoutExtension(_AssetPath).Replace(" ", "_");
		var assetDir	= "Assets" + Path.GetDirectoryName(_AssetPath).Replace(Application.dataPath, "") + "/";

		var asset = AssetDatabase.LoadAssetAtPath(assetDir + assetName + ".asset", typeof(GAFAnimationAsset)) as GAFAnimationAsset;
		if (!System.Object.Equals(asset, null))
		{
			var movieClipObject = createMovieClip(asset);

			var selected = new List<Object>(Selection.objects);
			selected.Add(movieClipObject);
			Selection.objects = selected.ToArray(); 
		}
	}

	private static void onCreatePrefab(string _AssetPath)
	{
		var assetName = Path.GetFileNameWithoutExtension(_AssetPath).Replace(" ", "_");
		var assetDir = "Assets" + Path.GetDirectoryName(_AssetPath).Replace(Application.dataPath, "") + "/";

		var asset = AssetDatabase.LoadAssetAtPath(assetDir + assetName + ".asset", typeof(GAFAnimationAsset)) as GAFAnimationAsset;
		if (!System.Object.Equals(asset, null))
		{
			var movieClipObject = createMovieClip(asset);
			var prefab = PrefabUtility.CreateEmptyPrefab(assetDir + assetName + ".prefab");
			prefab = PrefabUtility.ReplacePrefab(movieClipObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
			GameObject.DestroyImmediate(movieClipObject);

			var selected = new List<Object>(Selection.objects);
			selected.Add(movieClipObject);
			Selection.objects = selected.ToArray();
		}
	}

	private static GameObject createMovieClip(GAFAnimationAsset _Asset)
	{
		var movieClipObject = new GameObject(_Asset.name);
		var movieClip = movieClipObject.AddComponent<GAFMovieClip>();

		movieClip.settings.init(_Asset);
		movieClip.init(_Asset, 0);
		movieClip.reload();

		return movieClipObject;
	}
}
