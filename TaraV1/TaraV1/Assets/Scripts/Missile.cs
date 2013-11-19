/*  Missile.cs
	Description: Used for when the mech's missile hits it's target
*/

using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Target"){
			
			Destroy(other.gameObject);
			Destroy(this.gameObject);
		}
	}
	
}
