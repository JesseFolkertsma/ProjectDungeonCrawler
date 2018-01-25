using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour {

	Transform player;
	public Vector3 rotationOffset = new Vector3(90,0,-90);

	void Awake(){
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	void Update (){
		if(player != null){
			var lookPos = player.position - transform.position;
			lookPos.y = 0;
			var rotation = Quaternion.LookRotation(lookPos);
			transform.rotation = rotation;
			transform.Rotate(rotationOffset);
		}
		else{
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}
	}
}
