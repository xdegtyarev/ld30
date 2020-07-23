using UnityEngine;
using System.Collections;

public class PlayerTwoHello : MonoBehaviour {
	public tk2dTextMesh text;
	public tk2dTextMesh tip2j;
	public GameObject cloud;
	public Dog dog;
	bool isFading;
	float alpha = 1f;
	public float speed = 1f;
	void OnTriggerEnter(Collider other){
		if(other.GetComponent<Human>()!=null){
			if(Input.GetJoystickNames().Length<2){
				tip2j.renderer.enabled = true;
			}
			isFading = true;
			dog.RegisterInput();
			AudioController.Play("rain");
		}
	}

	void OnTriggerStay(Collider other){
		if(other.GetComponent<Human>()!=null){
			if(Mathf.Abs(other.GetComponent<Human>().transform.position.x-dog.transform.position.x)<10f){
				Debug.Log("OnTriggerStay");
				FollowX.instance.target = dog.transform;
			}
		}
	}

	void Update(){
		if(isFading){
			alpha += speed*Time.deltaTime;
			if(alpha > 1f){
				alpha = 1f;
				isFading=false;
			}
			text.color = new Color(text.color.r,text.color.g,text.color.b,alpha);
		}
	}


}
