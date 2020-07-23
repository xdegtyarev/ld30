/*
 * File:           GAFAnimationAsset.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using GAF;
using GAF.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class GAFAnimationAsset : ScriptableObject
{
	#region Members

	[HideInInspector][SerializeField] private int m_Version	= GAFSystem.Version;

	[HideInInspector][SerializeField] private string			m_GUID					= string.Empty;
	[HideInInspector][SerializeField] private byte [] 			m_AssetData 			= null;

	[HideInInspector][SerializeField] private bool 				m_IsObjectsCollected 	= false;
	[HideInInspector][SerializeField] private List<int> 		m_BatchedObjects 		= new List<int>();
	[HideInInspector][SerializeField] private List<int> 		m_MaskedObjects 		= new List<int>();
	[HideInInspector][SerializeField] private List<int> 		m_ColoredObjects 		= new List<int>();

	[HideInInspector][SerializeField] private List<string>		m_ResourcePaths			= new List<string>();
	
	private GAFAnimationData m_SharedData = null;

	private Object m_Locker = new Object();

	#endregion // Members
	
	#region Interface

	public void load()
	{
		lock(m_Locker)
		{
#if UNITY_EDITOR
			m_GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));

			m_Version = GAFSystem.Version;
#endif // UNITY_EDITOR

			if (!isLoaded)
			{
				if (m_AssetData != null)
				{
					GAFReader reader = new GAFReader();
					try
					{
						reader.Load(m_AssetData, ref m_SharedData);
					}
					catch (GAFException _Exception)
					{
						GAFUtils.Error(_Exception.Message);
						
						m_SharedData = null;
					}
				}
			}

			if (isLoaded)
			{
#if UNITY_EDITOR
				if (!m_IsObjectsCollected)
				{
					initObjectsLists();
				}
#endif // UNITY_EDITOR
			}
		}
	}

	public void init(byte [] _GAFData)
	{
		m_AssetData 			= _GAFData;
		m_SharedData 			= null;
		m_IsObjectsCollected	= false;

		load();

#if UNITY_EDITOR
		initResources();
#endif // UNITY_EDITOR
	}

	public void reload()
	{
		m_SharedData 			= null;
		m_IsObjectsCollected	= false;

		load ();

#if UNITY_EDITOR
		initResources();
#endif // UNITY_EDITOR
	}

	public void cleanUp()
	{
#if UNITY_EDITOR
		deleteResources();
#endif // UNITY_EDITOR
	}

	public GAFTexturesResource getResource(float _Scale, float _CSF)
	{
#if UNITY_EDITOR
		bool recreateResources = false;
		foreach (var path in m_ResourcePaths)
		{
			GAFTexturesResource resource = AssetDatabase.LoadAssetAtPath(path, typeof(GAFTexturesResource)) as GAFTexturesResource;
			if (resource == null)
			{
				recreateResources = true;
				break;
			}
		}

		if (recreateResources)
			initResources();
#endif // UNITY_EDITOR

		string resourcePath = "Cache/" + m_GUID + "_" + _Scale.ToString() + "_" + _CSF.ToString();
		return Resources.Load<GAFTexturesResource>(resourcePath);
	}

	public List<GAFAtlasData> getAtlases(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].atlases;
		}
		else
		{
			return null;
		}
	}

	public List<GAFObjectData> getObjects(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].objects;
		}
		else
		{
			return null;
		}
	}

	public List<GAFMaskData> getMasks(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].masks;
		}
		else
		{
			return null;
		}
	}

	public Dictionary<uint, GAFFrameData> getFrames(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].frames;
		}
		else
		{
			return null;
		}
	}

	public List<GAFSequenceData> getSequences(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].sequences;
		}
		else
		{
			return null;
		}
	}

	public List<string> getSequenceIDs(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].sequences.ConvertAll(sequence => sequence.name);
		}
		else
		{
			return null;
		}
	}

	public List<GAFNamedPartData> getNamedParts(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].namedParts;
		}
		else
		{
			return null;
		}
	}

	public uint getFramesCount(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].framesCount;
		}
		else
		{
			return (uint)0;
		}
	}

	public Rect getFrameSize(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].frameSize;
		}
		else
		{
			return new Rect(0, 0, 0, 0);
		}
	}
	
	public Vector2 getPivot(int _TimelineID)
	{
		if (isLoaded &&
		    m_SharedData.timelines.ContainsKey(_TimelineID))
		{
			return m_SharedData.timelines[_TimelineID].pivot;
		}
		else
		{
			return Vector2.zero;
		}
	}

	#endregion // Interface

	#region Properties

	public bool isLoaded
	{
		get
		{
			return m_SharedData != null;
		}
	}

	public bool isResourcesAvailable
	{
		get
		{
			if (m_ResourcePaths.Any())
			{
				string assetsPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
				foreach (var path in m_ResourcePaths)
				{
					string fullPath = assetsPath + path;
					if (!System.IO.File.Exists(fullPath))
					{
						return false;
					}
				}

				return true;
			}
			else
				return false;
		}
	}

	public int assetVersion
	{
		get
		{
			return m_Version;
		}
	}

	public ushort majorDataVersion
	{
		get
		{
			return isLoaded ? m_SharedData.majorVersion : (ushort)0;
		}
	}

	public ushort minorDataVersion
	{
		get
		{
			return isLoaded ? m_SharedData.minorVersion : (ushort)0;
		}
	}

	public List<string> resourcesPaths
	{
		get
		{
			return m_ResourcePaths;
		}
	}

	public List<float> scales
	{
		get
		{
			return isLoaded ? m_SharedData.scales : null;
		}
	}

	public List<float> csfs
	{
		get
		{
			return isLoaded ? m_SharedData.csfs : null;
		}
	}

	public bool hasMasks
	{
		get
		{
			return maskedObjects.Count > 0;
		}
	}

	public List<int> batchableObjects
	{
		get
		{
			return m_BatchedObjects;
		}
	}

	public List<int> coloredObjects
	{
		get
		{
			return m_ColoredObjects;
		}
	}

	public List<int> maskedObjects
	{
		get
		{
			return m_MaskedObjects;
		}
	}

	#endregion // Properties

	#region ScriptableObject
	
	private void OnEnable()
	{
		load();
	}

	#endregion // ScriptableObject

	#region Implementation

	private void initObjectsLists()
	{
		if (isLoaded)
		{
			m_IsObjectsCollected = true;
			
			m_BatchedObjects 	= new List<int>();
			m_MaskedObjects		= new List<int>();
			m_ColoredObjects	= new List<int>();
			
			foreach(var timeline in m_SharedData.timelines.Values)
			{
				foreach(var obj in timeline.objects)
				{
					int type = 0;
					foreach(var frame in timeline.frames.Values)
					{
						if (frame.states.ContainsKey(obj.id))
						{
							var state = frame.states[obj.id];
							
							if ((state.alpha != 1f && state.alpha != 0f) ||
							    state.colorMatrix != null)
							{
								type |= 1;
							}
							
							if (state.maskID > 0)
							{
								type |= 2;
							}
						}
					}
					
					int id = (int)obj.id;
					switch(type)
					{
					case 0: m_BatchedObjects.Add(id); break;
					case 1: m_ColoredObjects.Add(id); break;
					case 2: m_MaskedObjects.Add(id); break;
					case 3: m_MaskedObjects.Add(id); m_ColoredObjects.Add(id); break;
					}
				}
			}
			
			if (m_BatchedObjects.Count > 0)
			{
				GAFUtils.Log("GAF! Your animation asset has batchable objects. Make sure that dynamic batching is enabled for you project.");
			}
			
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif // UNITY_EDITOR
		}
	}

	#endregion  // Implementation

#if UNITY_EDITOR

	#region EditorProperties
	
	public List<GAFTimelineData> timelines
	{
		get
		{
			return m_SharedData.timelines.Values.ToList();
		}
	}

	#endregion // EditorProperties

	#region EditorImplementation

	private void initResources()
	{
		Dictionary<KeyValuePair<float, float>, List<string>> resourceTexturesNames = new Dictionary<KeyValuePair<float, float>, List<string>>();

		m_ResourcePaths.Clear ();

		GAFSystem.getCachePath ();

		foreach(var timeline in m_SharedData.timelines.Values)
		{
			foreach(var atlas in timeline.atlases)
			{
				foreach (var data in atlas.texturesData.Values)
				{
					foreach (var textureInfo in data.files)
					{
						string textureName = Path.GetFileNameWithoutExtension(textureInfo.Value);
						var key = new KeyValuePair<float, float>(atlas.scale, textureInfo.Key);

						if (!resourceTexturesNames.ContainsKey(key))
							resourceTexturesNames[key] = new List<string>();

						resourceTexturesNames[key].Add(textureName);
					}
				}
			}
		}

		foreach(var pair in resourceTexturesNames)
		{
			string name = m_GUID + "_" + pair.Key.Key.ToString() + "_" + pair.Key.Value.ToString();
			string path = GAFSystem.getCachePath () + name + ".asset";

			var resource = ScriptableObject.CreateInstance<GAFTexturesResource>();
			resource = GAFAssetUtils.saveAsset(resource, path);
			resource.init(this, pair.Value, pair.Key.Key, pair.Key.Value);
			m_ResourcePaths.Add(path);
		}

		EditorUtility.SetDirty(this);
	}

	private void deleteResources()
	{
		foreach(var path in m_ResourcePaths)
		{
			AssetDatabase.DeleteAsset(path);
		}

		AssetDatabase.Refresh(ImportAssetOptions.Default);

		m_ResourcePaths.Clear ();

		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
	}

	private string getCachePath()
	{
		string path = Application.dataPath + "/GAF";
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);
		
		path = path + "/Resources";
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);

		path = path + "/Cache";
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);

		int assetsIndex = path.IndexOf("Assets");
		path = path.Substring(assetsIndex, path.Length - assetsIndex);

		return path;
	}

	#endregion // EditorImplementation

#endif // UNITY_EDITOR

}
