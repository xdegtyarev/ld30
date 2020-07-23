/*
 * File:           GAFAnimationObject.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class GAFAnimationObject : MonoBehaviour 
{
	#region Members
	
	[SerializeField] private string CustomShaderPath 	= string.Empty;
	[SerializeField] private bool 	Visible 			= true;
	
	[HideInInspector][SerializeField] private int m_ObjectID 		= -1;
	[HideInInspector][SerializeField] private int m_AtlasElementID	= -1;
	
	private GAFMovieClip  		m_Player 		= null;
	private GAFObjectStateData 	m_CurrentState 	= null;
	private GAFAtlasData 		m_TextureAtlas	= null;
	private GAFAtlasElementData m_AtlasElement	= null;
	private GAFTexturesData 	m_TextureInfo	= null;

	private MeshFilter 			m_Filter 		= null;
	private bool 				m_IsVisible		= true;

	private List<GAFAnimationObjectComponent> m_Components = new List<GAFAnimationObjectComponent> ();
	
	#endregion // Members
	
	#region Interface
	
	public virtual void init(GAFMovieClip _Player)
	{
		m_Components.Clear ();
		m_CurrentState = null;

		visible = initialVisible;

		initBaseData (_Player);
		initMesh ();
		initRenderer ();

		addComponent(new GAFTransform(this));
		if (movieClip.asset.coloredObjects.Contains((int)objectID))
			addComponent(new GAFColorTransform(this));
		
		updateToState (currentState, true);
	}
	
	public virtual void updateToState(GAFObjectStateData _State, bool _Refresh)
	{
		gameObject.SetActive (_State.alpha > 0);
		if(renderer!=null){
			renderer.enabled = visible;
		}
		foreach(var component in m_Components)
			component.updateToState(_State, _Refresh);
	}
	
	#endregion // Interface
	
	#region Properties
	
	public GAFMovieClip movieClip
	{
		get
		{
			return m_Player;
		}
	}
	
	public uint objectID
	{
		get
		{
			return (uint)m_ObjectID;
		}
	}
	
	public uint atlasElementID
	{
		get
		{
			return (uint)m_AtlasElementID;
		}
	}
	
	public GAFAtlasData textureAtlas
	{
		get
		{
			return m_TextureAtlas;
		}
	}
	
	public GAFAtlasElementData atlasElement
	{
		get
		{
			return m_AtlasElement;
		}
	}
	
	public GAFTexturesData textureInfo
	{
		get
		{
			return m_TextureInfo;
		}
	}
	
	public GAFObjectStateData currentState
	{
		get
		{
			return m_CurrentState;
		}
		
		protected set
		{
			m_CurrentState = value;
		}
	}
	
	public MeshFilter filter
	{
		get
		{
			if (m_Filter == null)
			{
				m_Filter = GetComponent<MeshFilter>();
			}
			
			return m_Filter;
		}
		
		set
		{
			m_Filter = value;
		}
	}

	public bool visible
	{
		get
		{
			return m_IsVisible;
		}

		set
		{
			if (m_IsVisible != value)
			{
				if (renderer != null)
					renderer.enabled = value;

				m_IsVisible = value;
			}
		}
	}

	public bool initialVisible
	{
		get
		{
			return Visible;
		}
	}

	#endregion // Properties
	
	#region Implementation
	
	private GAFAnimationObjectComponent addComponent(GAFAnimationObjectComponent _Component)
	{
		if (!m_Components.Contains(_Component))
		{
			m_Components.Add(_Component);
			return _Component;
		}
		
		return null;
	}
	
	protected virtual void initBaseData(GAFMovieClip _Player)
	{
		m_Player = _Player;
		
		if (m_ObjectID 			< 0 ||
		    m_AtlasElementID	< 0)
		{
			string [] names = gameObject.name.Split ('_');
			m_AtlasElementID = int.Parse(names[0]);
			m_ObjectID 		 = int.Parse(names[1]);
		}
		
		if (m_CurrentState == null)
		{
			m_CurrentState = new GAFObjectStateData(objectID);
		}

		m_TextureAtlas	= movieClip.asset.getAtlases(movieClip.timelineID).Find(atlas => atlas.scale == movieClip.settings.scale);
		m_AtlasElement	= textureAtlas.getElement(atlasElementID);
		m_TextureInfo	= textureAtlas.getAtlas(atlasElement.atlasID);
	}
	
	protected virtual void initMesh()
	{
		GAFAtlasElementData element = atlasElement;
		GAFTexturesData		info	= textureInfo;
		
		if (filter == null)
		{
			filter = gameObject.AddComponent<MeshFilter> ();
		}
		
		float scale = element.scale * movieClip.settings.pixelsPerUnit;
		float scaledPivotX 	= element.pivotX / scale;
		float scaledPivotY 	= element.pivotY / scale;
		float scaledWidth 	= element.width  / scale;
		float scaledHeight 	= element.height / scale;
		
		Vector3 [] vertices = new Vector3[4];
		vertices[0] = new Vector3(-scaledPivotX					, scaledPivotY - scaledHeight	, 0f);
		vertices[1] = new Vector3(-scaledPivotX					, scaledPivotY					, 0f);
		vertices[2] = new Vector3(-scaledPivotX + scaledWidth	, scaledPivotY					, 0f);
		vertices[3] = new Vector3(-scaledPivotX + scaledWidth	, scaledPivotY - scaledHeight	, 0f);
		
		Texture2D atlasTexture = movieClip.resource.getTexture(System.IO.Path.GetFileNameWithoutExtension(info.getFileName(movieClip.settings.csf)));
		float scaledElementLeftX 	=  element.x * movieClip.settings.csf																	/ atlasTexture.width;
		float scaledElementRightX 	= (element.x + element.width) * movieClip.settings.csf													/ atlasTexture.width;
		float scaledElementTopY 	= (atlasTexture.height - element.y * movieClip.settings.csf - element.height * movieClip.settings.csf)	/ atlasTexture.height;
		float scaledElementBottomY	= (atlasTexture.height - element.y * movieClip.settings.csf)											/ atlasTexture.height;
		
		Vector2 [] uv = new Vector2[vertices.Length];
		uv [0] = new Vector2 (scaledElementLeftX	, scaledElementTopY);
		uv [1] = new Vector2 (scaledElementLeftX	, scaledElementBottomY);
		uv [2] = new Vector2 (scaledElementRightX	, scaledElementBottomY);
		uv [3] = new Vector2 (scaledElementRightX	, scaledElementTopY);
		
		Vector3 [] normals = new Vector3[vertices.Length];
		normals[0] = new Vector3(0f, 0f, -1f);
		normals[1] = new Vector3(0f, 0f, -1f);
		normals[2] = new Vector3(0f, 0f, -1f);
		normals[3] = new Vector3(0f, 0f, -1f);
		
		int [] triangles = new int[6];
		triangles[0] = 2;
		triangles[1] = 0;
		triangles[2] = 1;
		triangles[3] = 3;
		triangles[4] = 0;
		triangles[5] = 2;

		Mesh mesh = new Mesh ();
		mesh.name = "Element_" + atlasElementID;
		
		mesh.vertices 	= vertices;
		mesh.uv 		= uv;
		mesh.triangles 	= triangles;
		mesh.normals 	= normals;

		filter.mesh = mesh;
		filter.sharedMesh.Optimize();
	}
	
	protected virtual void initRenderer()
	{
		if (renderer == null)
		{
			gameObject.AddComponent<MeshRenderer> ();
		}
		
		Material material = null;
		if (!string.IsNullOrEmpty(CustomShaderPath))
		{
			material 				= new Material(Shader.Find(CustomShaderPath));
			material.color 			= new Color(1f, 1f, 1f, 1f);
			material.mainTexture	= movieClip.resource.getTexture(System.IO.Path.GetFileNameWithoutExtension(textureInfo.getFileName(movieClip.settings.csf)));
		}
		else
		{
			material = movieClip.resource.getMaterial(movieClip.asset, objectID, System.IO.Path.GetFileNameWithoutExtension(textureInfo.getFileName(movieClip.settings.csf)));
		}
		
		renderer.sharedMaterial 	= material;
		renderer.castShadows 		= false;
		renderer.receiveShadows 	= false;
		renderer.sortingLayerID 	= movieClip.settings.spriteLayerID;
		renderer.sortingOrder		= movieClip.settings.spriteLayerValue;
	}
	
	#endregion // Implementation
}