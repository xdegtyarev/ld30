using UnityEngine;
using System.Collections.Generic;

public class Inputron : MonoBehaviour{
	public static Inputron instance;
	void Awake(){
		instance = this;
	}
	[SerializeField] List<InputConfiguration> inputConfigurations;
	[SerializeField] List<InputReceiver> inputReceivers;
	int switchControllOffset;
	InputType targetConfiguration;
	string info;
	void Update(){
		if(Input.GetJoystickNames().Length > 0){
			targetConfiguration = InputType.Joystick;
		} else{ 
			targetConfiguration = InputType.Keyboard;
		}

		int inputReciverIndex = 0 + switchControllOffset;
		for(int i = 0; i < inputConfigurations.Count; i++){
			if(inputConfigurations[i].inputType == targetConfiguration){
				foreach(var key in inputConfigurations[i].configuration){
					if(Input.GetKey(key.keyCode)){
						inputReceivers[inputReciverIndex % inputReceivers.Count].ActionStart(key.keyAction);
					}
					if(Input.GetKeyUp(key.keyCode)){
//						if(key.keyAction == KeyAction.Switch){ //Handling switch internally
//							if(Input.GetJoystickNames().Length<2){
//								SwitchConfigurations();
//								return;
//							}
//						} else{

							inputReceivers[inputReciverIndex % inputReceivers.Count].ActionEnd(key.keyAction);
//						}
					}
				}

//				foreach(var key in inputConfigurations[i].axisConfiguration){
//					if(Input.GetAxis(key.axis)>0f == key.value){
//						info += "key axis" + key.axis + " key val" + key.value + " action:" + key.action + "Key val " + Input.GetAxis(key.axis) + "\n";
//						inputReceivers[inputReciverIndex % inputReceivers.Count].ActionStart(key.action);
//					}
//				}
				inputReciverIndex++;
			}
		}



	}
	
	public void SwitchConfigurations(){
		for(int i = 0; i < inputConfigurations.Count; i++){
			foreach(var key in inputConfigurations[i].configuration){
				inputReceivers[switchControllOffset % inputReceivers.Count].ActionEnd(key.keyAction);
			}
		}
		switchControllOffset++;
		switchControllOffset %= inputReceivers.Count;

		FollowX.instance.target = inputReceivers[switchControllOffset].transform;
		MotionTracker.instance.TrackMotion(inputReceivers[switchControllOffset].GetComponent<Actor>(),Motion.Switch);
		Debug.Log("Switching: " + switchControllOffset);
	}
}
