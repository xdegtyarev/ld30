using UnityEngine;
using System.Collections.Generic;

public enum Motion{
	Switch,
	GetAngry,
	Bark,
	Bite,
	DropStick,
	Happy,
	Caress,
	Jump,
	GoLeft,
	SayNo,
	Pee,
	GoRight,
	RunLeft,
	RunRight,
	Sit,
	Speak,
	Stick,
	SayYes
}
[System.Serializable]
public class ActorMotionPair{
	public Actor actor;
	public Motion motion;
	public ActorMotionPair(Actor a,Motion m){
		actor = a;
		motion = m;
	}
} 
public class MotionTracker : MonoBehaviour {
	public static MotionTracker instance;
	int sequenceProgress;
	List<ActorMotionPair> sequence;
	Artefact currentArtefact;

	public void Awake(){
		instance = this;
	}
		
	public void SetArtefact(Artefact artefact){
		currentArtefact = artefact;
		sequence = artefact.GetSequence();
		sequenceProgress = 0;
	}



	public void TrackMotion(Actor actor,Motion motion){
		if(sequence!=null){
			if(sequence[sequenceProgress].actor == actor && sequence[sequenceProgress].motion == motion)
			{
				currentArtefact.Highlight();
				sequenceProgress++;
			}
			if(sequenceProgress==sequence.Count){
				currentArtefact.Solved();
				currentArtefact = null;
				sequence = null;
			}
		}
	}
}
