// [DEPRECATED]
using UnityEngine;
using System.Collections;

public class ColliderEnemyWeapon : MonoBehaviour {
	public AIEnemy host;
	// Use this for initialization
	void Start () {
		if (host == null)
			this.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		/* Steven:
		* Apparently, the rigidbody in the player will stop triggering collisions of it stops moving (i.e, no input)
		* This stops that*/
		if(GetComponent<Rigidbody>().IsSleeping()){
			GetComponent<Rigidbody>().WakeUp();	
		}		
	}
	void OnTriggerStay(Collider other){
		if (other.tag == "Player"){
			host.OnWeaponStay(other);
		}
	}
}
