using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class PlaceAtDogX : MonoBehaviour {
	public Transform dog;
	public GameObject endGame;
	void OnEnable(){
		transform.position = Vector3.right*dog.transform.position.x;
		AudioController.Play("portal");
		StartCoroutine(Finish());
	}

	IEnumerator Finish(){
		yield return new WaitForSeconds(2f);
		endGame.SetActive(true);
		yield return new WaitForSeconds(5f);
		endGame.SetActive(false);
	}
}
