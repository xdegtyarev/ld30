/*
 * File:           GAFAnimationMaskedObject.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class GAFAnimationMaskedObject : GAFAnimationObject
{
	#region Members

	private GAFAnimationMask 	m_Mask			= null;

	#endregion Members

	#region Interface

	public void setMask(GAFAnimationMask _Mask)
	{
		m_Mask = _Mask;
	}

	#endregion // Interface

	#region Implementation

	private void applyMask()
	{
		Matrix4x4 maskTransform = Matrix4x4.identity;
		maskTransform.m00 = m_Mask.currentState.a;
		maskTransform.m01 = m_Mask.currentState.c;
		maskTransform.m10 = m_Mask.currentState.b;
		maskTransform.m11 = m_Mask.currentState.d;
		
#if GAF_USING_TK2D
		float screenHeight 		= 0;
		float screenWidth  		= 0;
		Vector2 cameraPosShift	= Vector2.zero;
		
		tk2dCamera tk2d_camera = Camera.current.GetComponent<tk2dCamera>();
		if (tk2d_camera != null)
		{
			tk2dCameraSettings cameraSettings = tk2d_camera.CameraSettings;
			if (cameraSettings.orthographicType == tk2dCameraSettings.OrthographicType.PixelsPerMeter)
				screenHeight = tk2d_camera.nativeResolutionHeight / cameraSettings.orthographicPixelsPerMeter;
			else
				screenHeight = tk2d_camera.CameraSettings.orthographicSize * 2;

			screenWidth  	= Camera.current.aspect * screenHeight;
			cameraPosShift	= Camera.current.transform.position - new Vector3(screenWidth / 2f, -screenHeight / 2f);
		}
		else
		{
			screenHeight 	= Camera.current.orthographicSize * 2;
			screenWidth  	= Camera.current.aspect * screenHeight;
			cameraPosShift	= Camera.current.transform.position - new Vector3(screenWidth / 2f, -screenHeight / 2f);
		}
#else
		float screenHeight 		= Camera.current.orthographicSize * 2;
		float screenWidth  		= Camera.current.aspect * screenHeight;
		Vector2 cameraPosShift	= Camera.current.transform.position - new Vector3(screenWidth / 2f, -screenHeight / 2f);
#endif // GAF_USING_TK2D
		
		float scaleX = Mathf.Sqrt( (maskTransform.m00 * maskTransform.m00) + (maskTransform.m01 * maskTransform.m01) );
		float scaleY = Mathf.Sqrt( (maskTransform.m11 * maskTransform.m11) + (maskTransform.m10 * maskTransform.m10) );
		
		float scale = movieClip.settings.pixelsPerUnit * m_Mask.atlasElement.scale * movieClip.settings.csf;
		float sizeXUV = (float)screenWidth  / (m_Mask.texture.width  / scale * scaleX * transform.parent.localScale.x * Camera.current.aspect);
		float sizeYUV = (float)screenHeight / (m_Mask.texture.height / scale * scaleY * transform.parent.localScale.y);
		
		float maskWidth 	= (float)m_Mask.texture.width  / movieClip.settings.csf;
		float maskHeight	= (float)m_Mask.texture.height / movieClip.settings.csf;
		
		float pivotX = m_Mask.atlasElement.pivotX 				 / maskWidth;
		float pivotY = (maskHeight - m_Mask.atlasElement.pivotY) / maskHeight;
		
		float moveX = (-m_Mask.transform.position.x + cameraPosShift.x) 		/ screenWidth;
		float moveY = -1f - (m_Mask.transform.position.y - cameraPosShift.y) 	/ screenHeight;
		
		Matrix4x4 _transform = Matrix4x4.identity;
		_transform *= Matrix4x4.TRS(new Vector3( pivotX, pivotY, 0f), Quaternion.identity, Vector3.one );
		_transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(sizeXUV, sizeYUV, 1f));
		_transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -transform.parent.localRotation.eulerAngles.z), Vector3.one);
		_transform *= maskTransform;
		_transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f / scaleX, 1f / scaleY, 1f));
		_transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Camera.current.aspect, 1f, 1f));
		_transform *= Matrix4x4.TRS(new Vector3( moveX, moveY, 0f), Quaternion.identity, Vector3.one );
		
		renderer.sharedMaterial.SetMatrix("_TransformMatrix", _transform);
		renderer.sharedMaterial.SetTexture ("_MaskMap", m_Mask.texture);
	}

	#endregion // Implementation
	
	#region MonoBehavior
	
	private void OnWillRenderObject()
	{
		if (movieClip 		!= null 		&&
		    movieClip.asset != null 		&&
		    movieClip.asset.isLoaded 		&&
		    movieClip.resource != null 		&&
		    movieClip.resource.isReady 		&&
		    transform.parent != null		&&
		    renderer != null 				&&
		    renderer.sharedMaterial != null)
		{
			if (m_Mask != null 					&&
			    m_Mask.currentState != null 	&&
			    m_Mask.currentState.alpha > 0	&&
			    m_Mask.texture != null			&&
			    m_Mask.atlasElement != null)
			{
				applyMask();
			}
			else
			{
				renderer.sharedMaterial.SetTexture ("_MaskMap", null);
			}
		}
	}

	#endregion // MonoBehaviur
}
