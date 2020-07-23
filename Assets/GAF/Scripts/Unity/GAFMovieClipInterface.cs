﻿/*
 * File:           GAFMovieClipInterface.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

public delegate void GAFMovieClipCallback(GAFMovieClip _Clip);

public interface GAFMovieClipInterface 
{
	void play ();
	bool isPlaying ();
	void pause ();
	void stop ();

	void gotoAndStop (uint _FrameNumber);
	void gotoAndPlay (uint _FrameNumber);

	void setSequence (string _SequenceName, bool _PlayImmediately = false);
	void setDefaultSequence (bool _PlayImmediately = false);
	string sequenceIndexToName(uint _Index);
	uint sequenceNameToIndex(string _Name);
	uint getCurrentSequenceIndex ();

	uint getCurrentFrameNumber ();
	uint getFramesCount ();

	GAFWrapMode getAnimationWrapMode ();
	void setAnimationWrapMode (GAFWrapMode _Mode);

	float duration ();

	string addTrigger (GAFMovieClipCallback _Callback, uint _FrameNumber);
	void removeTrigger(string _ID);
	void removeAllTriggers(uint _FrameNumber);
	void removeAllTriggers();

	GAFAnimationObject getObject(uint _ID);
	GAFAnimationObject getObject(string _PartName);
	string objectIDToPartName(uint _ID);
	uint partNameToObjectID(string _PartName);
}
