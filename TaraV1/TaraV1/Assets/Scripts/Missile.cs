/*  Missile.cs
	Description: Used for when the mech's missile hits it's target
*/

using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

    float amountDamage = 2.0f;

    //private static Missile m_Instance = null;

	void OnTriggerEnter(Collider other){
		if(other.tag == "Target"){
            Instantiate(ResourceManager.Get().preExplosion, this.gameObject.transform.position, this.gameObject.transform.rotation);
			Destroy(other.gameObject);
			Destroy(this.gameObject);
		}

        if (other.tag == "Player")
        {
            ((UnitPlayer)LevelLoader.Get().mainPlayer.GetComponent(typeof(UnitPlayer))).hurt(amountDamage);
        }
	}
	
}
