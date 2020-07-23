/*
 * File:           GAFAnimationMask.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class GAFAnimationMask : GAFAnimationObject
{
	#region Members
	
	private Texture2D m_MaskTexture = null;
	
	#endregion // Members
	
	#region Interface
	
	public override void init(GAFMovieClip _Player)
	{
		initBaseData (_Player);
		initTexture ();
		updateToState (currentState, true);
	}

	public override void updateToState (GAFObjectStateData _State, bool _Refresh)
	{
		currentState = _State;

		if (movieClip != null)
		{
			float scale = movieClip.settings.pixelsPerUnit / movieClip.settings.scale;
			transform.localPosition = new Vector3(_State.tX / scale, -_State.tY / scale, -_State.zOrder / scale);
		}
	}

	#endregion // Interface
	
	#region Properties
	
	public Texture2D texture
	{
		get
		{
			return m_MaskTexture;
		}
	}

	#endregion // Properties
	
	#region Implementation
	
	protected override void initMesh ()
	{
		// Empty
	}
	
	protected override void initRenderer()
	{
		// Empty
	}
	
	private void initTexture()
	{
		GAFAtlasElementData element = atlasElement;
		GAFTexturesData		info	= textureInfo;
		
		int csf = (int)movieClip.settings.csf;
		
		m_MaskTexture = new Texture2D(
			  (int)(element.width  * csf)
			, (int)(element.height * csf)
			, TextureFormat.ARGB32
			, false);
		
		Color [] textureColor = texture.GetPixels ();
		for (uint i = 0; i < textureColor.Length; ++i)
			textureColor [i] = Color.black;
		
		m_MaskTexture.SetPixels( textureColor );
		m_MaskTexture.Apply();
		
		Texture2D atlasTexture = movieClip.resource.getTexture(System.IO.Path.GetFileNameWithoutExtension(info.getFileName(csf)));
		Color [] maskTexturePixels = atlasTexture.GetPixels (
			  (int)(element.x * csf)
			, (int)(atlasTexture.height - element.y * csf - element.height * csf)
			, (int)(element.width  * csf)
			, (int)(element.height * csf));
		
		m_MaskTexture.SetPixels(
			  0
			, 0
			, (int)(element.width  * csf)
			, (int)(element.height * csf)
			, maskTexturePixels);
		
		m_MaskTexture.Apply(true);
		
		m_MaskTexture.filterMode 	= FilterMode.Bilinear;
		m_MaskTexture.wrapMode 		= TextureWrapMode.Clamp;
		
		m_MaskTexture.Apply();
	}
	
	#endregion // Implementation
}
