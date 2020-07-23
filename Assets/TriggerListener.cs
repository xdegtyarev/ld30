using UnityEngine;

public class TriggerListener : MonoBehaviour {
	public tk2dTextMesh text;
	public GameObject selector;
	bool isFading;
	float alpha = 1f;
	public float speed = 0.1f;
	void OnTriggerEnter(Collider other){
		if(other.gameObject == selector){
			isFading = true;
		}
	}

	void Update(){
		if(isFading){
			alpha -= speed*Time.deltaTime;
			if(alpha < 0f){
				Destroy(text.gameObject);
			}
			text.color = new Color(text.color.r,text.color.g,text.color.b,alpha);
		}
	}


}
