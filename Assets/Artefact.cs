using UnityEngine;
using System.Collections.Generic;

public class Artefact : MonoBehaviour {
	public List<ActorMotionPair> puzzle;
	public List<tk2dSprite> puzzleView;
	public Color highlightColor;
	public GameObject target;
	public GameObject puzzleViewsDown;
	List<tk2dSprite> secret = new List<tk2dSprite>();
	int progress = 0;

	public void Awake(){
		secret = new List<tk2dSprite>(puzzleViewsDown.GetComponentsInChildren<tk2dSprite>());
	}

	public void Unlock(){
		MotionTracker.instance.SetArtefact(this);
		fadein = true;
	}

	public List<ActorMotionPair> GetSequence(){
		return puzzle;
	}

	public void Highlight(){
		puzzleView[progress].color = highlightColor;
		AudioController.Play("symbol_unlock");
		progress++;
	}

	public void Solved(){
		AudioController.Play("symbols_disappear");
		fadeout = true;
		target.SetActive(true);

	}

	bool fadeout,fadein;
	float alpha;
	float speed = 0.7f;

	void Update(){
		if(fadeout){
			if(alpha<0f){
				alpha = 0f;
				fadeout = false;
			}
			alpha-=Time.deltaTime*speed;
			foreach(var o in secret){
				o.color = new Color(o.color.r,o.color.g,o.color.b,alpha);
			}
		}

		if(fadein){
			if(alpha>1f){
				alpha = 1f;
				fadein = false;
			}
			alpha+=Time.deltaTime*speed;
			foreach(var o in secret){
				o.color = new Color(o.color.r,o.color.g,o.color.b,alpha);
			}
		}
	}
}
