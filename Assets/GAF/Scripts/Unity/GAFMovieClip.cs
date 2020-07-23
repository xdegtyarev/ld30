/*
 * File:           GAFMovieClip.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using GAF;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[System.Serializable]
[AddComponentMenu("GAF/GAFMovieClip")]
[ExecuteInEditMode]
public class GAFMovieClip :
	  MonoBehaviour
	, GAFMovieClipInterface
{
	#region Movie clip interface
	public void play()
	{
		setPlaying (true);
	}
	
	public void pause()
	{
		setPlaying (false);
	}

	public void stop()
	{
		updateToFrame (currentSequence.startFrame, true);
		setPlaying (false);
	}

	public void gotoAndStop(uint _FrameNumber)
	{
		_FrameNumber = (uint)Mathf.Clamp (
			  (int)_FrameNumber
			, (int)currentSequence.startFrame
			, (int)currentSequence.endFrame);

		updateToFrame (_FrameNumber, true);

		if (on_goto != null)
			on_goto (this);

		setPlaying (false);
	}

	public void gotoAndPlay(uint _FrameNumber)
	{
		_FrameNumber = (uint)Mathf.Clamp (
			  (int)_FrameNumber
			, (int)currentSequence.startFrame
			, (int)currentSequence.endFrame);
		
		updateToFrame (_FrameNumber, true);

		if (on_goto != null)
			on_goto (this);
		
		setPlaying (true);
	}

	public string sequenceIndexToName(uint _Index)
	{
		if (asset != null && asset.isLoaded)
		{
			var sequences = asset.getSequences (timelineID);
			return sequences[Mathf.Clamp((int)_Index, 0, sequences.Count - 1)].name;
		}
		else
		{
			return string.Empty;
		}
	}

	public uint sequenceNameToIndex(string _Name)
	{
		if (asset != null && asset.isLoaded)
		{
			int index = asset.getSequences(timelineID).FindIndex(__sequence => __sequence.name == _Name);
			return index < 0 ? uint.MaxValue : (uint)index;
		}
		else
		{
			return uint.MaxValue;
		}
	}

	public void setSequence(string _SequenceName, bool _PlayImmediately = false)
	{
		uint sequenceIndex = sequenceNameToIndex(_SequenceName);
		if (sequenceIndex != uint.MaxValue &&
		    m_SequenceIndex != sequenceIndex)
		{
			m_SequenceIndex = (int)sequenceIndex;

			updateToFrame (currentSequence.startFrame, true);

			if (on_sequence_change != null)
				on_sequence_change (this);

			setPlaying (_PlayImmediately);
		}
	}

	public void setDefaultSequence(bool _PlayImmediately = false)
	{
		setSequence ("Default", _PlayImmediately);
	}

	public uint getCurrentSequenceIndex()
	{
		return (uint)m_SequenceIndex;
	}

	public uint getCurrentFrameNumber()
	{
		return (uint)m_CurrentFrameNumber;
	}
	
	public uint getFramesCount()
	{
		return asset.getFramesCount(timelineID);
	}

	public GAFWrapMode getAnimationWrapMode()
	{
		return settings.wrapMode;
	}

	public void setAnimationWrapMode(GAFWrapMode _Mode)
	{
		settings.wrapMode = _Mode;
	}
	
	public bool isPlaying()
	{
		return m_IsPlaying;
	}

	public float duration()
	{
		return (currentSequence.endFrame - currentSequence.startFrame) * settings.targetSPF;
	}

	public string addTrigger(GAFMovieClipCallback _Callback, uint _FrameNumber)
	{
		if (_FrameNumber < getFramesCount())
		{
			GAFFrameEvent triggerEvent = new GAFFrameEvent (_Callback);
			if (m_FrameEvents.ContainsKey(_FrameNumber))
			{
				m_FrameEvents [_FrameNumber].Add (triggerEvent);
			}
			else
			{
				m_FrameEvents.Add(_FrameNumber, new List<GAFFrameEvent>());
				m_FrameEvents[_FrameNumber].Add(triggerEvent);
			}
			
			return triggerEvent.id;
		}
		
		return string.Empty;
	}
	
	public void removeTrigger(string _ID)
	{
		foreach(KeyValuePair<uint, List<GAFFrameEvent>> pair in m_FrameEvents)
		{
			pair.Value.RemoveAll(delegate(GAFFrameEvent _event) 
			{
				return _event.id == _ID;
			});
		}
	}
	
	public void removeAllTriggers(uint _FrameNumber)
	{
		if (_FrameNumber < getFramesCount())
		{
			if (m_FrameEvents.ContainsKey(_FrameNumber))
			{
				m_FrameEvents[_FrameNumber].Clear();
			}
		}
	}
	
	public void removeAllTriggers()
	{
		m_FrameEvents.Clear ();
	}

	public GAFAnimationObject getObject(uint _ID)
	{
		return animationObjects.ContainsKey (_ID) ? animationObjects [_ID] : null;
	}

	public GAFAnimationObject getObject(string _PartName)
	{
		return getObject (partNameToObjectID (_PartName));
	}

	public string objectIDToPartName(uint _ID)
	{
		if (asset != null &&
		    asset.isLoaded)
		{
			var data = asset.getNamedParts(timelineID).Find(part => part.objectID == _ID);
			return data != null ? data.name : string.Empty;
		}
		
		return string.Empty;
	}

	public uint partNameToObjectID(string _PartName)
	{
		if (asset != null &&
		    asset.isLoaded)
		{
			var data = asset.getNamedParts(timelineID).Find(part => part.name == _PartName);
			return data != null ? data.objectID : uint.MaxValue;
		}

		return uint.MaxValue;
	}

	#endregion // Movie clip interface

	#region Events
	
	public event GAFMovieClipCallback on_start_play;
	public event GAFMovieClipCallback on_stop_play;
	public event GAFMovieClipCallback on_goto;
	public event GAFMovieClipCallback on_sequence_change;
	public event GAFMovieClipCallback on_clear;
	
	#endregion // Events

	#region Behavior interface

	public void clear(bool destroyChildren = false)
	{
		if (on_clear != null)
			on_clear(this);

		if (destroyChildren)
		{
			List<GameObject> children = new List<GameObject>();
			foreach (Transform child in transform)
				children.Add(child.gameObject);

			children.ForEach (delegate(GameObject child) 
			{
				if (Application.isPlaying)
					Destroy (child);
				else
					DestroyImmediate(child, true);
			});
		}
		else
		{
			foreach(var obj in m_Objects)
			{
				if (Application.isPlaying)
					Destroy (obj);
				else
					DestroyImmediate(obj, true);
			}
		}

		if (m_ObjectsDict != null)
		{
			m_ObjectsDict.Clear();
			m_ObjectsDict = null;
		}

		if (m_MaskedObjectsDict != null)
		{
			m_MaskedObjectsDict.Clear();
			m_MaskedObjectsDict = null;
		}

		if (m_MasksDict != null)
		{
			m_MasksDict.Clear();
			m_MasksDict = null;
		}

		m_Objects.Clear ();
		m_MaskedObjects.Clear ();
		m_Masks.Clear ();
		m_FrameEvents.Clear ();

		m_GAFAsset	 			= null;
		m_Resource				= null;
		m_Settings 				= new GAFAnimationPlayerSettings ();
		m_SequenceIndex 		= 0;
		m_CurrentFrameNumber 	= 1;
		m_Stopwatch 			= 0.0f;

		m_IsInitialized = false;
	}

	public void reload()
	{
		if (!System.Object.Equals(asset, null) &&
		    isInitialized)
		{
			if (!asset.isLoaded)
				asset.load();

			if (asset.isLoaded)
			{
#if UNITY_EDITOR
				if (m_Version < GAFSystem.Version)
				{
					upgrade();
				}
#endif // UNITY_EDITOR

				resource = asset.getResource(settings.scale, settings.csf);

				if (resource != null &&
				    resource.isReady)
				{
					m_Objects.ForEach(obj => obj.init(this));

					if (!enabled)
						m_Objects.ForEach (obj => obj.visible = false);

					updateToFrame (getCurrentFrameNumber(), true);
				}
			}
		}
	}

	#endregion // Behavior interface

#if UNITY_EDITOR
	#region EditorInterface
	
	public void init(GAFAnimationAsset _Asset, int _TimelineID)
	{
		if (!isInitialized)
		{
			m_IsInitialized = true;
			
			m_GAFAsset 		= _Asset;
			m_TimelineID	= _TimelineID;
			m_Version 		= GAFSystem.Version;
			
			createMaskElements();
			createAnimationObjects();
		}
	}
	
	#endregion // EditorInterface
#endif // UNITY_EDITOR

	#region Properties

	public GAFAnimationAsset asset
	{
		get
		{
			return m_GAFAsset;
		}
	}

	public int timelineID
	{
		get
		{
			return m_TimelineID;
		}
	}

	public GAFTexturesResource resource
	{
		get
		{
			return m_Resource;
		}

		set
		{
			m_Resource = value;
		}
	}

	public GAFAnimationPlayerSettings settings
	{
		get
		{
			return m_Settings;
		}
	}

	public bool isInitialized
	{
		get
		{
			return m_IsInitialized;
		}
	}

	public Dictionary<uint, GAFAnimationObject> animationObjects
	{
		get
		{
			if (m_ObjectsDict == null)
			{
				m_ObjectsDict = new Dictionary<uint, GAFAnimationObject>();
				foreach(var obj in m_Objects)
					m_ObjectsDict.Add(obj.objectID, obj);
			}

			return m_ObjectsDict;
		}
	}

	public Dictionary<uint, GAFAnimationMaskedObject> animationMaskedObjects
	{
		get
		{
			if (m_MaskedObjectsDict == null)
			{
				m_MaskedObjectsDict = new Dictionary<uint, GAFAnimationMaskedObject>();
				foreach(var obj in m_MaskedObjects)
					m_MaskedObjectsDict.Add(obj.objectID, obj);
			}
			
			return m_MaskedObjectsDict;
		}
	}

	public Dictionary<uint, GAFAnimationMask> animationMasks
	{
		get
		{
			if (m_MasksDict == null)
			{
				m_MasksDict = new Dictionary<uint, GAFAnimationMask>();
				foreach(var mask in m_Masks)
					m_MasksDict.Add(mask.objectID, mask);
			}
			
			return m_MasksDict;
		}
	}

	public GAFSequenceData currentSequence
	{
		get
		{
			if (asset != null &&
			    asset.isLoaded)
			{
				return asset.getSequences(timelineID)[(int)getCurrentSequenceIndex()];
			}

			return null;
		}
	}

	#endregion // Properties

	#region MonoBehaviour

	private void FixedUpdate()
	{
		if (asset != null &&
		    asset.isLoaded &&
		    isPlaying() &&
		   !settings.ignoreTimeScale)
		{
			OnUpdate(Time.deltaTime);
		}
	}

	private void Update()
	{
		if (asset != null &&
		    asset.isLoaded &&
		    isPlaying() &&
		    settings.ignoreTimeScale)
		{
			OnUpdate(Mathf.Clamp(Time.realtimeSinceStartup - m_PreviouseUpdateTime, 0f, Time.maximumDeltaTime));
			m_PreviouseUpdateTime = Time.realtimeSinceStartup;
		}
	}

#if UNITY_EDITOR
	private void Awake()
	{
		GAFPostprocessorHelper.instance.on_resource_become_ready += delegate(GAFTexturesResource _Resource) {
			if (resource == _Resource)
				reload();
		};
	}
#endif // UNITY_EDITOR

	private void OnEnable()
	{
		reload ();
	}

	private void OnDisable()
	{
		m_Objects.ForEach (obj => obj.visible = false);
	}

	private void Start()
	{
		if (Application.isPlaying)
			setPlaying (settings.playAutomatically);
	}

	private void OnDestroy()
	{
		clear (true);
	}

	private void OnApplicationFocus(bool _FocusStatus) 
	{
		if (!settings.playInBackground)
		{
			setPlaying(_FocusStatus);
		}
	}

	private void OnApplicationPause(bool _PauseStatus) 
	{
		if (!settings.playInBackground)
		{
			setPlaying(_PauseStatus);
		}
	}

	#endregion // MonoBehaviour

	#region Implementation

	private void OnUpdate(float _TimeDelta)
	{
		m_Stopwatch += _TimeDelta;

		if (m_Stopwatch >= settings.targetSPF)
		{
			int framesCount = 1;
			if (settings.perfectTiming)
			{
				m_StoredTime += m_Stopwatch - settings.targetSPF;
				if (m_StoredTime > settings.targetSPF)
				{
					int additionalFrames = Mathf.FloorToInt(m_StoredTime / settings.targetSPF);
					m_StoredTime = m_StoredTime - (additionalFrames * settings.targetSPF);
					framesCount += additionalFrames;
				}
			}

			m_Stopwatch = 0f;

			if (getCurrentFrameNumber() + framesCount > currentSequence.endFrame)
			{
				switch(settings.wrapMode)
				{
				case GAFWrapMode.Once:
					setPlaying (false);
					return;

				case GAFWrapMode.Loop:
					updateToFrame(currentSequence.startFrame, true);
					
					if (on_stop_play != null)
						on_stop_play(this);

					if (on_start_play != null)
						on_start_play(this);

					return;

				default:
					setPlaying (false);
					return;
				}
			}
			
			updateToFrame (getCurrentFrameNumber() + (uint)framesCount, false);
		}
	}

	private void updateToFrame(uint _FrameNumber, bool _RefreshStates)
	{
		if (getCurrentFrameNumber() != _FrameNumber || _RefreshStates)
		{
			var states = getStates (_FrameNumber, _RefreshStates);
			if (states != null)
			{
				foreach ( var state in states )
				{
					if (animationObjects.ContainsKey(state.id))
						animationObjects[state.id].updateToState(state, _RefreshStates);

					if (animationMaskedObjects.Count > 0 &&
					    animationMaskedObjects.ContainsKey(state.id))
					{
						if (state.maskID > 0 &&
						    animationMasks.ContainsKey((uint)state.maskID))
						{
							animationMaskedObjects[state.id].setMask(animationMasks[(uint)state.maskID]);
						}
						else
						{
							animationMaskedObjects[state.id].setMask(null);
						}
					}
				}
			}

			m_CurrentFrameNumber = (int)_FrameNumber;

			if (m_FrameEvents.ContainsKey(_FrameNumber))
			{
				foreach(GAFFrameEvent _event in m_FrameEvents[_FrameNumber])
				{
					_event.trigger(this);
				}
			}
		}
	}

	private List<GAFObjectStateData> getStates(uint _FrameNumber, bool _RefreshStates)
	{
		if (!_RefreshStates)
		{
			_RefreshStates = _FrameNumber < getCurrentFrameNumber();
		}

		if (_RefreshStates)
		{
			var frame 	= new GAFFrameData(_FrameNumber);
			var objects = asset.getObjects(timelineID);
			var frames	= asset.getFrames(timelineID);

			foreach(var _obj in objects)
			{
				frame.addState(new GAFObjectStateData(_obj.id));
			}
			
			foreach(var _frame in frames)
			{
				if (_frame.Key > _FrameNumber)
					break;
				
				foreach(var _state in _frame.Value.states)
				{
					frame.states[_state.Key] = _state.Value;
				}
			}

			return frame.states.Values.ToList();
		}
		else
		{
			var frames = asset.getFrames(timelineID);
			if (_FrameNumber - getCurrentFrameNumber() == 1)
			{
				if (frames.ContainsKey(_FrameNumber))
				{
					return frames[_FrameNumber].states.Values.ToList();
				}
			}
			else
			{
				var frame = new GAFFrameData(_FrameNumber);
				foreach(var _frame in frames)
				{
					if (_frame.Key > _FrameNumber)
						break;
					
					if (_frame.Key < getCurrentFrameNumber())
						continue;
					
					foreach(var _state in _frame.Value.states)
					{
						frame.states[_state.Key] = _state.Value;
					}
				}

				return frame.states.Values.ToList();
			}

			return null;
		}
	}

	private void setPlaying(bool _IsPlay)
	{
		if (m_IsPlaying != _IsPlay)
		{
			m_IsPlaying = _IsPlay;

			if (m_IsPlaying)
			{
				if (on_start_play != null)
					on_start_play(this);

				m_Stopwatch = 0.0f;
				m_PreviouseUpdateTime = 0f;
			}
			else
			{
				if (on_stop_play != null)
					on_stop_play(this);

				m_Stopwatch = 0.0f;
				m_PreviouseUpdateTime = 0f;
			}
		}
	}

	#endregion // Implementation

#if UNITY_EDITOR
	#region EditorImplementation
	
	private void upgrade()
	{
		GAFAnimationAsset 			_asset 				= asset;
		int							_timelineID			= timelineID;
		GAFAnimationPlayerSettings 	_settings 			= settings;
		int 						_sequenceIndex 		= m_SequenceIndex;
		int 						_currentFrameNumber = m_CurrentFrameNumber;

		bool destroyChildren = m_Version == 0;
		clear(destroyChildren);
		
		m_Settings				= _settings;
		m_SequenceIndex			= _sequenceIndex;
		m_CurrentFrameNumber	= _currentFrameNumber;
		
		init (_asset, _timelineID);
	}

	private void createMaskElements()
	{
		var masks = asset.getMasks(timelineID);
		if (masks != null)
		{
			for (uint i = 0; i < masks.Count; ++i)
			{
				var maskData = masks[(int)i];
				var name = maskData.atlasElementID.ToString() + "_" + maskData.objectID.ToString() + "_mask";
				var objectTransform = transform.FindChild(name);
				GameObject maskObject = null;

				if (objectTransform != null)
				{
					maskObject = objectTransform.gameObject;
				}
				else
				{
					maskObject = new GameObject(name);
					maskObject.transform.parent = transform;
				}

				var redundantComponents = maskObject.GetComponents<GAFAnimationMask>();
				if (redundantComponents.Length > 1)
				{
					m_Objects.Add(redundantComponents[0]);
					m_Masks.Add(redundantComponents[0]);

					for (int k = 1; k < redundantComponents.Length; k++)
					{
						DestroyImmediate(redundantComponents[k]);
					}
				}
				else
				{
					GAFAnimationMask mask = maskObject.AddComponent<GAFAnimationMask>();
					m_Objects.Add(mask);
					m_Masks.Add(mask);
				}
			}
		}
	}

	private void createAnimationObjects()
	{
		var objects = asset.getObjects(timelineID);
		for (uint i = 0; i < objects.Count; ++i)
		{
			var _object = objects[(int)i];
			var name = _object.atlasElementID.ToString() + "_" + _object.id.ToString();
			var objectTransform = transform.FindChild(name);
			GameObject animationObject = null;

			if (objectTransform != null)
			{
				animationObject = objectTransform.gameObject;
			}
			else
			{
				animationObject = new GameObject(name);
				animationObject.transform.parent = transform;
			}

			if (asset.maskedObjects.Contains((int)_object.id))
			{
				var redundantComponents = animationObject.GetComponents<GAFAnimationMaskedObject>();
				if (redundantComponents.Length > 1)
				{
					m_MaskedObjects.Add(redundantComponents[0]);
					m_Objects.Add(redundantComponents[0]);

					for (int k = 1; k < redundantComponents.Length; k++)
					{
						DestroyImmediate(redundantComponents[k]);
					}
				}
				else
				{
					var component = animationObject.AddComponent<GAFAnimationMaskedObject>();
					m_MaskedObjects.Add(component);
					m_Objects.Add(component);
				}
			}
			else
			{
				var redundantComponents = animationObject.GetComponents<GAFAnimationObject>();
				if (redundantComponents.Length > 1)
				{
					m_Objects.Add(redundantComponents[0]);

					for (int k = 1; k < redundantComponents.Length; k++)
					{
						DestroyImmediate(redundantComponents[k]);
					}
				}
				else
				{
					m_Objects.Add(animationObject.AddComponent<GAFAnimationObject>());
				}
			}
		}
	}

	#endregion  // EditorImplementation
#endif // UNITY_EDITOR

	#region Members

#if UNITY_EDITOR
	[HideInInspector][SerializeField] private int 								m_Version				= 0;
#endif // UNITY_EDITOR

	[HideInInspector][SerializeField] private GAFAnimationAsset					m_GAFAsset				= null;
	[HideInInspector][SerializeField] private int								m_TimelineID			= 0;
	[HideInInspector][SerializeField] private GAFAnimationPlayerSettings 		m_Settings				= new GAFAnimationPlayerSettings ();
	[HideInInspector][SerializeField] private int 								m_SequenceIndex			= 0;
	[HideInInspector][SerializeField] private int 								m_CurrentFrameNumber 	= 1;
	[HideInInspector][SerializeField] private bool 								m_IsInitialized			= false;
	[HideInInspector][SerializeField] private List<GAFAnimationObject>			m_Objects				= new List<GAFAnimationObject>();
	[HideInInspector][SerializeField] private List<GAFAnimationMaskedObject>	m_MaskedObjects			= new List<GAFAnimationMaskedObject>();
	[HideInInspector][SerializeField] private List<GAFAnimationMask>			m_Masks					= new List<GAFAnimationMask>();

	private GAFTexturesResource	m_Resource = null;

	private Dictionary<uint, GAFAnimationObject>		m_ObjectsDict 		= null;
	private Dictionary<uint, GAFAnimationMaskedObject>	m_MaskedObjectsDict = null;
	private Dictionary<uint, GAFAnimationMask>			m_MasksDict 		= null;
	private Dictionary<uint, List<GAFFrameEvent>> 		m_FrameEvents 		= new Dictionary<uint, List<GAFFrameEvent>>();
	
	private bool 	m_IsPlaying 	= false;
	private float 	m_Stopwatch 	= 0f;
	private float 	m_StoredTime 	= 0f;

	private float 	m_PreviouseUpdateTime = 0f;

	#endregion // Members
}
