/*
 * File:           GAFColorTransform.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

public class GAFColorTransform : GAFAnimationObjectComponent
{
	#region Members
	
	private static readonly Color m_sDisabledOffset = new Color( 0, 0, 0, 0);
	
	#endregion // Members
	
	#region Interface
	
	public GAFColorTransform(GAFAnimationObject _Object) : base(_Object)
	{
	}
	
	public override void updateToState(GAFObjectStateData _State, bool _Refresh)
	{
		if (animationObject.currentState.alpha != _State.alpha)
		{
			if (animationObject.renderer.sharedMaterial != null)
				animationObject.renderer.sharedMaterial.SetFloat("_Alpha", _State.alpha);
		}
		
		if (animationObject.currentState.colorMatrix != _State.colorMatrix)
		{
			if (animationObject.renderer.sharedMaterial != null)
			{
				animationObject.renderer.sharedMaterial.SetColor("_ColorMult",  _State.colorMatrix != null ? _State.colorMatrix.multipliers 	: Color.white );
				animationObject.renderer.sharedMaterial.SetColor("_ColorShift", _State.colorMatrix != null ? _State.colorMatrix.offsets 		: m_sDisabledOffset);
			}
		}
		
		updateObjectState(_State);
	}
	
	#region Implementation
	
	private void updateObjectState(GAFObjectStateData _State)
	{
		currentState.alpha 	 		= _State.alpha;
		currentState.colorMatrix	= _State.colorMatrix;
	}
	
	#endregion // Implementation
	
	#endregion // Interface
}
