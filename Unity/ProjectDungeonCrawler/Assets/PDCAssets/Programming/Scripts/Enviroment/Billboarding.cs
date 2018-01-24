using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour {

	Transform player;

	void Awake(){
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	void Update (){
		var lookPos = player.position - transform.position;
		lookPos.y = 0;
		var rotation = Quaternion.LookRotation(lookPos);
		transform.rotation = rotation;
		transform.Rotate(90,0,-90);
;	}
}
