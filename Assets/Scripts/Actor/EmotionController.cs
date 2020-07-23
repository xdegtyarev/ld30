using UnityEngine;
using System.Collections.Generic;

public class EmotionController : MonoBehaviour {
	[SerializeField] List<GameObject> emotions;
	public void OnEmotionChange(int emotionID){
		if(emotionID>emotions.Count-1){
			emotionID = 0;
		}
		for(int i = 0; i<emotions.Count; i++){
			if(i!=emotionID){
				if(emotions[i]!=null){
					emotions[i].SetActive(false);
				}
			}else{
				if(emotions[i]!=null){
					emotions[i].SetActive(true);
				}
			}
		}
	}
}
