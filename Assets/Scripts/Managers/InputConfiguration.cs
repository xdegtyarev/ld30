using UnityEngine;
using System.Collections.Generic;

public enum InputType{
	Keyboard,
	Joystick,
	Mouse
}

[System.Serializable]
public class InputConfiguration{
	public string name;
	public InputType inputType;
	public List<ActionButtonPair> configuration;
	public List<ActionAxisPair> axisConfiguration;
}
