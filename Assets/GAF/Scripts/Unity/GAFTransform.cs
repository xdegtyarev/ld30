/*
 * File:           GAFTransform.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GAFTransform : GAFAnimationObjectComponent
{
	#region Members
	
	private Vector3	[] m_Vertices = null;
	
	#endregion // Members
	
	#region Interface
	
	public GAFTransform(GAFAnimationObject _Object) : base(_Object)
	{
		m_Vertices = _Object.filter.sharedMesh.vertices;
	}
	
	public override void updateToState(GAFObjectStateData _State, bool _Refresh)
	{
		animationObject.transform.localRotation = Quaternion.identity;
		animationObject.transform.localScale 	= Vector3.one;
		
		if (_Refresh ||
			currentState.tX 	!= _State.tX ||
		    currentState.tY	 	!= _State.tY ||
		    currentState.zOrder != _State.zOrder)
		{
			float scale = movieClip.settings.pixelsPerUnit / movieClip.settings.scale;
			animationObject.transform.localPosition = new Vector3(_State.tX / scale, -_State.tY / scale, -_State.zOrder / scale);
		}
		
		if (_Refresh ||
			currentState.a != _State.a  ||
		    currentState.b != _State.b  ||
		    currentState.c != _State.c  ||
		    currentState.d != _State.d)
		{
			Matrix4x4 _transform = Matrix4x4.identity;
			_transform[0, 0] =  _State.a;
			_transform[0, 1] = -_State.c;
			_transform[1, 0] = -_State.b;
			_transform[1, 1] =  _State.d;
			
			Vector3 [] vertices = new Vector3[m_Vertices.Length];
			for(int i = 0; i< vertices.Length; i++)
				vertices[i] = _transform * m_Vertices[i];
			
			if (animationObject.filter.sharedMesh != null)
			{
				animationObject.filter.sharedMesh.vertices = vertices;
				animationObject.filter.sharedMesh.RecalculateBounds();
			}
		}
		
		updateObjectState (_State);
	}
	
	#endregion // Interface
	
	#region Implementation
	
	private void updateObjectState(GAFObjectStateData _State)
	{
		currentState.tX 	= _State.tY;
		currentState.tY	 	= _State.tY;
		currentState.zOrder = _State.zOrder;
		currentState.a 	 	= _State.a;
		currentState.b 	 	= _State.b;
		currentState.c 	 	= _State.c;
		currentState.d		= _State.d;
	}
	
	#endregion // Implementation
}
