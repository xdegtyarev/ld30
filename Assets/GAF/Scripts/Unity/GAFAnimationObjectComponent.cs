/*
 * File:           GAFAnimationObjectComponent.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public abstract class GAFAnimationObjectComponent 
{
	#region Members

	private GAFAnimationObject m_Object	= null;

	#endregion // Members

	#region Interface

	public GAFAnimationObjectComponent(GAFAnimationObject _Object)
	{
		m_Object = _Object;
	}

	public abstract void updateToState (GAFObjectStateData _State, bool _Refresh);

	#endregion // Interface

	#region Properties

	public GAFAnimationObject animationObject
	{
		get
		{
			return m_Object;
		}
	}

	public GAFMovieClip movieClip
	{
		get
		{
			return animationObject.movieClip;
		}
	}

	public GAFObjectStateData currentState
	{
		get
		{
			return animationObject.currentState;
		}
	}

	#endregion // Properties
}
