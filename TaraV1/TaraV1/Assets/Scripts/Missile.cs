/*  Missile.cs
	Description: Used for when the mech's missile hits it's target
*/

using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

    float amountDamage = 50.0f;

    private static Missile m_Instance = null;

	void OnTriggerEnter(Collider other){
		if(other.tag == "Target"){
			
			Destroy(other.gameObject);
			Destroy(this.gameObject);


		}

        if (other.tag == "Player")
        {
            ((UnitPlayer)LevelLoader.Get().mainPlayer.GetComponent(typeof(UnitPlayer))).hurt(amountDamage);
        }
	}
	
}
