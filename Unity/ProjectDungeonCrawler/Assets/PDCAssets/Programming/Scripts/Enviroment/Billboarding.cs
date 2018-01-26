using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour {

	Transform player;
	public Vector3 rotationOffset = new Vector3(90,0,-90);

	void Update (){
		if(player != null){
			var lookPos = player.position - transform.position;
			lookPos.y = 0;
			var rotation = Quaternion.LookRotation(lookPos);
			transform.rotation = rotation;
			transform.Rotate(rotationOffset);
		}
	}

    public void Setup(Transform target)
    {
        player = target;
    }
}
