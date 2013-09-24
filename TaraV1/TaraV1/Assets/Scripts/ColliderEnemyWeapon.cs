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
		
	}
	void OnTriggerStay(Collider other){
		if (other.tag == "Player"){
			host.OnWeaponStay(other);
		}
	}
}
