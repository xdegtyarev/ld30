using UnityEngine;
using System;
using System.Collections.Generic;

public class InputReceiver : MonoBehaviour {
	public Dictionary<KeyAction,Action> KeyPressActionMap;
	public Dictionary<KeyAction,Action> KeyReleaseActionMap;

	void Awake(){
		KeyPressActionMap = new Dictionary<KeyAction, Action>();
		KeyReleaseActionMap = new Dictionary<KeyAction, Action>();
	}

	public void ActionStart(KeyAction keyAction){
		if(KeyPressActionMap.ContainsKey(keyAction)){
			KeyPressActionMap[keyAction]();
		}
	}

	public void ActionEnd(KeyAction keyAction){
		if(KeyReleaseActionMap.ContainsKey(keyAction)){
			KeyReleaseActionMap[keyAction]();
		}
	}
}
