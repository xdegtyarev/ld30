/*
 * File:           GAFColorTransformationMatrix.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

public enum GAFColorTransformIndex
{
	  GAFCTI_R
	, GAFCTI_G
	, GAFCTI_B
	, GAFCTI_A
	, COUNT
};

public enum GAFFilterType
{
	  GFT_DropShadow = 0
	, GFT_Blur = 1
	, GFT_Glow = 2
	, GFT_ColorMatrix = 6
};

[System.Serializable]
public class GAFColorTransformationMatrix 
{	
	#region Members

	private float m_MultiplierR;
	private float m_MultiplierG;
	private float m_MultiplierB;
	private float m_MultiplierA;
	private float m_OffsetR;
	private float m_OffsetG;
	private float m_OffsetB;
	private float m_OffsetA;	

	#endregion // Members

	#region Interface 
	
	public GAFColorTransformationMatrix()
	{
		multipliers 	= Color.white;
		offsets 		= Color.black;
		m_OffsetA 		= 0f;
	}
	
	public GAFColorTransformationMatrix(Color _Multipliers, Color _Offsets)
	{
		multipliers 	= _Multipliers;
		offsets 		= _Offsets;
	}
	
	#endregion // Interface
	
	#region Properties

	public Color multipliers
	{
		get
		{			
			return new Color( m_MultiplierR, m_MultiplierG, m_MultiplierB, m_MultiplierA );
		}
		set
		{
			m_MultiplierR = value.r;
			m_MultiplierG = value.g;
			m_MultiplierB = value.b;
			m_MultiplierA = value.a;
		}
	}

	public Color offsets
	{
		get
		{
			return new Color( m_OffsetR, m_OffsetG, m_OffsetB, m_OffsetA );
		}
		set
		{
			m_OffsetR = value.r;
			m_OffsetG = value.g;
			m_OffsetB = value.b;
			m_OffsetA = value.a;
		}
	}

	#endregion // Properties
}
