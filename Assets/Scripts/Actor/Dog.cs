using UnityEngine;
using System.Collections.Generic;

public class Dog : Actor {
	[SerializeField] Human boy;
	[SerializeField] Animator animator;
	[SerializeField] InputReceiver inputReceiver;
	List<EmotionController> emotionalControllers;
	bool isRunning;
	bool isSitting;
	bool isWalking;
	bool isMoving;
	[SerializeField] GAFMovieClip downClip;
	[SerializeField] GAFMovieClip upClip;
	[SerializeField] GAFMovieClip biteClip;
	[SerializeField] GAFMovieClip jumpClip;
	[SerializeField] GAFMovieClip peeClip;
	[SerializeField] GAFMovieClip pickClip;
	[SerializeField] float speed = 1f;
	bool dir;
	void Awake(){
		emotionalControllers = new List<EmotionController>(GetComponentsInChildren<EmotionController>(true));
	}

	void Update(){
		if(isMoving){
			if(dir){
				if(transform.position.x<170){
					transform.localScale = new Vector3(-1,1,1);
					if(isRunning){
						transform.position += Vector3.right*speed*1.5f;
					}else{
						transform.position += Vector3.right*speed;
					}
				}
			}else{
				if(transform.position.x>-15000){
					transform.localScale = new Vector3(1,1,1);
					if(isRunning){
						transform.position -= Vector3.right*speed*1.5f;
					}else{
						transform.position -= Vector3.right*speed;
					}
				}
			}
		}
	}

	public void RegisterInput(){
		inputReceiver.KeyPressActionMap.Add(KeyAction.Down,Sit);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Left,StartLeft);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Right,StartRight);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action1,Bark);

		inputReceiver.KeyPressActionMap.Add(KeyAction.Action2,Pick);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action3,Bite);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Up,Jump);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action4,Pee);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action5,GetAngry);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action6,GetHappy);
		inputReceiver.KeyPressActionMap.Add(KeyAction.Action8,StartRun);

		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action2,StopPick);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action3,StopBite);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Up,StopJump);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action4,StopPee);

		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Down,Stand);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Left,EndLeft);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Right,EndRight);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action1,GetNormal);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action5,GetNormal);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action6,GetNormal);
		inputReceiver.KeyReleaseActionMap.Add(KeyAction.Action8,EndRun);
	}
		
	//Down hold
	void Sit(){
		if(!isSitting){
			AudioController.Play("pug_hard_breath");
			isMoving = false;
			EndLeft();
			EndRight();
			MotionTracker.instance.TrackMotion(this,Motion.Sit);
			animator.SetBool("isSitting",true);
			isSitting = true;
			downClip.gotoAndPlay(0);
		}
	}
	//Down release
	void Stand(){
		if(isSitting){
			animator.SetBool("isSitting",false);
			isSitting = false;
			upClip.gotoAndPlay(0);
		}
	}
		
	//R2
	void StartRun(){
		if(!isRunning){
			animator.SetBool("isRunning",true);
			isRunning = true;
		}
	}
	//R2 release
	void EndRun(){
		if(isRunning){
			animator.SetBool("isRunning",false);
			isRunning = false;

		}
	}

	void StartLeft(){
		if(isPeeing || isSitting || isPicking || isBiting || isJumping){
			return;
		}
		if(!isWalking){
			AudioController.Play("pug_steps");
			isMoving = true;
			dir = false;
			if(isRunning){
				MotionTracker.instance.TrackMotion(this,Motion.RunLeft);
			}else{
				MotionTracker.instance.TrackMotion(this,Motion.GoLeft);
			}
			animator.SetBool("isWalking",true);
			isWalking = true;
		}
	}

	void EndLeft(){
		if(isWalking){
			AudioController.Stop("pug_steps");
			isMoving = false;
			animator.SetBool("isWalking",false);
			isWalking = false;
		}
	}

	void StartRight(){
		if(isPeeing || isSitting || isPicking || isBiting || isJumping){
			return;
		}
		if(!isWalking){
			AudioController.Play("pug_steps");
			isMoving = true;
			dir = true;
			if(isRunning){
				MotionTracker.instance.TrackMotion(this,Motion.RunRight);
			}else{
				MotionTracker.instance.TrackMotion(this,Motion.GoRight);
			}
			animator.SetBool("isWalking",true);
			isWalking = true;
		}
	}

	void EndRight(){
		if(isWalking){
			AudioController.Stop("pug_steps");
			isMoving = false;
			animator.SetBool("isWalking",false);
			isWalking = false;
		}
	}

	float jumpT;
	bool isJumping;
	void Jump(){
		if(!isJumping){
			if(Time.timeSinceLevelLoad > jumpT){
				AudioController.Play("pug_jump2");
				isMoving = false;
				isJumping = true;
				EndLeft();
				EndRight();
				jumpClip.stop();
				MotionTracker.instance.TrackMotion(this,Motion.Jump);
				jumpT+=jumpClip.duration();
				animator.SetTrigger("Jump");
				jumpClip.gotoAndPlay(0);
			}
		}
	}

	void StopJump(){
		isJumping = false;
	}

	int currentEmotionId;
	//L1
	void GetAngry(){
		if(currentEmotionId!=2){
			AudioController.Play("pug_rrrrrrr_short");
			MotionTracker.instance.TrackMotion(this,Motion.GetAngry);
			currentEmotionId = 2;
			foreach(var o in emotionalControllers){
				o.OnEmotionChange(2);
			}
		}
	}
	//R1
	public void GetHappy(){
		if(currentEmotionId!=1){
			AudioController.Play("pug_happy");
			MotionTracker.instance.TrackMotion(this,Motion.Happy);
			currentEmotionId = 1;
			foreach(var o in emotionalControllers){
				o.OnEmotionChange(1);
			}
		}
	}

	void GetNormal(){
		if(currentEmotionId!=0){
			AudioController.Stop("pug_bark1");
			currentEmotionId = 0;
			foreach(var o in emotionalControllers){
				o.OnEmotionChange(0);
			}
		}
	}

	void LateUpdate(){
		animator.ResetTrigger("Pee");
		animator.ResetTrigger("Bite");
		animator.ResetTrigger("Jump");
		animator.ResetTrigger("PickUp");
	}

	//A
	void Bark(){
		if(currentEmotionId!=3){
			AudioController.Play("pug_bark1");
			MotionTracker.instance.TrackMotion(this,Motion.Bark);
			currentEmotionId = 3;
			foreach(var o in emotionalControllers){
				o.OnEmotionChange(3);
			}
		}
	}

	float pickT;
	bool isPicking;
	void Pick(){
		if(!isPicking){
			if(Time.timeSinceLevelLoad > pickT){
				AudioController.Play("pug_bite");
				isMoving = false;
				isPicking = true;
				EndLeft();
				EndRight();
				MotionTracker.instance.TrackMotion(this,Motion.Stick);
				pickT+=pickClip.duration();
				animator.SetTrigger("PickUp");
				pickClip.gotoAndPlay(0);
			}
		}
	}

	void StopPick(){
		isPicking = false; 
	}

	float peeT;
	bool isPeeing;
	void Pee(){
		if(!isPeeing){
			if(Time.timeSinceLevelLoad > peeT){
				AudioController.Play("pug_piss");
				isPeeing = true;
				isMoving = false;
				EndLeft();
				EndRight();
				MotionTracker.instance.TrackMotion(this,Motion.Pee);
				peeT+=peeClip.duration();
				animator.SetTrigger("Pee");
				peeClip.gotoAndPlay(0);
			}
		}
	}
	void StopPee(){
		isPeeing = false;
	}
	float biteT;
	bool isBiting;
	void Bite(){
		if(!isBiting){
		if(Time.timeSinceLevelLoad > biteT){
			AudioController.Play("pug_bite");
			isMoving = false;
			isBiting = true;
			EndLeft();
			EndRight();
			MotionTracker.instance.TrackMotion(this,Motion.Bite);
			biteT+=biteClip.duration();
			animator.SetTrigger("Bite");
			biteClip.gotoAndPlay(0);
			if(Mathf.Abs(boy.transform.position.x-transform.position.x)<20f){
				Debug.Log("get Pain");
				boy.GetPain();
			}
		}
		}
	}
	void StopBite(){
		isBiting = false;
	}

}
