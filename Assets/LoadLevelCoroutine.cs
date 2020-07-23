using UnityEngine;
using System.Collections;

public class LoadLevelCoroutine : MonoBehaviour {

	void Start () {
		StartCoroutine(LoadLevelC());
	}

	IEnumerator LoadLevelC(){
		yield return new WaitForSeconds(5f);
		Application.LoadLevel(Application.loadedLevel+1);
	}
}
