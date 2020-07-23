using UnityEngine;
using System.Collections.Generic;

public class Human : Actor {
	[SerializeField] Animator animator;
	[SerializeField] InputReceiver inputReceiver;
	List<EmotionController> emotionalControllers;

	[SerializeField] GAFMovieClip YesClip;
	[SerializeField] GAFMovieClip NoClip;
	[SerializeField] GAFMovieClip dropClip;
	[SerializeField] GAFMovieClip pickClip;
	bool isCarresing;
	bool isWalking;
	bool isMoving;
	[SerializeField] float speed = 1f;
	bool dir;
	int currentEmotionId;

	void Awake(){
		emotionalControllers = new List<EmotionController>(GetComponentsInChildren<EmotionController>(true));
	}

	void Start(){
		inputReceiver.KeyPressActionMap.Add(KeyAction.Down,StartCaress);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Left,StartLeft);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Right,StartRight);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Up,StartSpeak);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action1,Pick);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action2,Throw);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action5,GetAngry);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action6,GetHappy);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action3,SayYes);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action4,SayNo);


		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action1,StopPick);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action2,StopThrow);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action3,StopSayingYes);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action4,StopSayingNo);

		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Up,GetNormal);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Down,EndCaress);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Left,EndLeft);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Right,EndRight);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action5,GetNormal);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action6,GetNormal);
	}

	void Update(){
		if(isMoving){
			if(dir){
				if(transform.position.x<170){
					transform.localScale = new Vector3(-1,1,1);
					transform.position += Vector3.right*speed;
				}
			}else{
				if(transform.position.x>-15000){
					transform.localScale = new Vector3(1,1,1);
					transform.position -= Vector3.right*speed;
				}
			}
		}
	}

	void StartCaress(){
		if(!isCarresing){
			AudioController.Play("boy_happiness");
			isMoving = false;
			animator.SetBool("isCaressing",true);
			isCarresing = true;
//			GetHappy();
			MotionTracker.instance.TrackMotion(this,Motion.Caress);
		}
	}

	void EndCaress(){
		if(isCarresing){
			animator.SetBool("isCaressing",false);
			isCarresing = false;
			GetNormal();
		}
	}

	float NoLastTime;
	bool isSayingNo;
	void SayNo(){
		if(!isSayingNo){
			if(Time.timeSinceLevelLoad > NoLastTime){
				AudioController.Play("boy_no");
				isMoving = false;
				isSayingNo = true;
				NoLastTime=NoLastTime+NoClip.duration();
				animator.SetTrigger("No");
				NoClip.gotoAndPlay(0);
				MotionTracker.instance.TrackMotion(this,Motion.SayNo);
			}
		}
	}



	void StopSayingNo(){
		isSayingNo = false;
	}

	float YesLastTime;
	bool isSayingYes;
	void SayYes(){
		if(!isSayingYes){
			if(Time.timeSinceLevelLoad > YesLastTime){
				AudioController.Play("boy_yes");
				isMoving = false;
				isSayingYes = true;
				YesLastTime=YesLastTime+YesClip.duration();
				animator.SetTrigger("Yes");
				YesClip.gotoAndPlay(0);
				MotionTracker.instance.TrackMotion(this,Motion.SayYes);
			}
		}
	}

	void StopSayingYes(){
		isSayingYes = false;
	}
		
	void StartLeft(){
		if(isSayingNo || isSayingYes || isPicking || isThrowing){
			return;
		}
		if(!isWalking){
			AudioController.Play("pug_steps");
			dir = false;
			animator.SetBool("isWalking",true);
			isWalking = true;
			isMoving = true;
			MotionTracker.instance.TrackMotion(this,Motion.GoLeft);
		}
	}


	void EndLeft(){
		if(isWalking){
			AudioController.Stop("pug_steps2");
			isMoving = false;
			animator.SetBool("isWalking",false);
			isWalking = false;
		}
	}

	void StartRight(){
		if(isSayingNo || isSayingYes || isPicking || isThrowing){
			return;
		}
		if(!isWalking){
			AudioController.Play("pug_steps2");
			dir = true;
			isMoving = true;
			animator.SetBool("isWalking",true);
			MotionTracker.instance.TrackMotion(this,Motion.GoRight);
			isWalking = true;
		}
	}


	void EndRight(){
		if(isWalking){
			AudioController.Stop("pug_steps2");
			isMoving = false;
			animator.SetBool("isWalking",false);
			isWalking = false;
		}
	}

	void LateUpdate(){
		animator.ResetTrigger("Yes");
		animator.ResetTrigger("No");
		animator.ResetTrigger("Drop");
		animator.ResetTrigger("Pickup");
	}

	//UP
	void StartSpeak(){
		if(currentEmotionId!=4){
			AudioController.Play("boy_blah");
			MotionTracker.instance.TrackMotion(this,Motion.Speak);
			currentEmotionId = 4;
			foreach(var o in emotionalControllers){
				o.OnEmotionChange(4);
			}
		}
	}


	void GetAngry(){
		if(currentEmotionId!=2){
			AudioController.Play("boy_anger");
			MotionTracker.instance.TrackMotion(this,Motion.GetAngry);
			currentEmotionId = 2;
			foreach(var o in emotionalControllers){
				o.OnEmotionChange(2);
			}
		}
	}

	public void GetPain(){
		if(currentEmotionId!=3){
			AudioController.Play("boy_pain");
			currentEmotionId = 3;
			foreach(var o in emotionalControllers){
				o.OnEmotionChange(3);
			}
		}
	}


	void GetHappy(){
		if(currentEmotionId!=1){
			AudioController.Play("boy_happiness2");
			MotionTracker.instance.TrackMotion(this,Motion.Happy);
			currentEmotionId = 1;
			foreach(var o in emotionalControllers){
				o.OnEmotionChange(1);
			}
		}
	}
		
	void GetNormal(){
		if(currentEmotionId!=0){
			currentEmotionId = 0;
			foreach(var o in emotionalControllers){
				o.OnEmotionChange(0);
			}
		}
	}

	float throwLastTime;
	bool isThrowing;
	void Throw(){
		if(!isThrowing){
			if(Time.timeSinceLevelLoad > throwLastTime){
//				AudioController.Play("throw");
				isThrowing = true;
				isMoving = false;
				MotionTracker.instance.TrackMotion(this,Motion.DropStick);
				throwLastTime=throwLastTime+ dropClip.duration();
				animator.SetTrigger("Drop");
				dropClip.gotoAndPlay(0);
			}
		}
	}
	void StopThrow(){
		isThrowing = false;
	}
		
	float pickLastTime;
	bool isPicking;
	void Pick(){
		if(!isPicking){
			if(Time.timeSinceLevelLoad > pickLastTime){
				AudioController.Play("boy_take");
				isMoving = false;
				isPicking = true;
				MotionTracker.instance.TrackMotion(this,Motion.Stick);
				pickLastTime=pickLastTime+pickClip.duration();
				animator.SetTrigger("Pickup");
				pickClip.gotoAndPlay(0);
			}
		}
	}

	void StopPick(){
		isPicking = false;
	}
}
