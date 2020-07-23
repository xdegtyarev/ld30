using UnityEngine;
using System.Collections;

public class DoginputTrig : MonoBehaviour {
	public tk2dTextMesh text;
	public tk2dTextMesh addText;
	public Renderer rain;
	public tk2dSprite cloud;
	public GameObject selector;
	bool isFading;
	float alpha = 1f;
	public float speed = 0.1f;
	void OnTriggerEnter(Collider other){
		if(other.gameObject == selector){
			isFading = true;
		}
		AudioController.Stop("rain");
	}

	void Update(){
		if(isFading){
			alpha -= speed*Time.deltaTime;
			if(alpha < 0f){
				Destroy(text.gameObject);
			}
			text.color = new Color(text.color.r,text.color.g,text.color.b,alpha);
			addText.color= new Color(text.color.r,text.color.g,text.color.b,alpha);
			cloud.color= new Color(cloud.color.r,cloud.color.g,cloud.color.b,alpha);
			rain.material.color = new Color(1f,1f,1f,alpha);
		}
	}


}
