using UnityEngine;
using System.Collections;

public class FollowX : MonoBehaviour {
	public static FollowX instance;
	public Transform target;
	public GameObject help;
	// Use this for initialization
	void Awake () {
		instance = this;
	}

	void Update(){
		if(Input.GetKeyUp(KeyCode.Space)){
			help.SetActive(!help.activeInHierarchy);
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {

		transform.position = Vector3.right*target.transform.position.x;
	}
}
