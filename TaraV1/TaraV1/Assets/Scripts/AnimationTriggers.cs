/* AnimationTrigger.cs
 * Description: Used for when functions are called during animations
 * Only add to enemies that will use it
 * */
using UnityEngine;
using System.Collections;

public class AnimationTriggers : MonoBehaviour {
	//Alien triggers
	ColliderProjectile projectile;
	private void AlienAttack(){
		GameObject bullet = Instantiate(ResourceManager.Get().preEnemyBullet,this.transform.position + this.transform.TransformDirection(Vector3.up * 4) + this.transform.TransformDirection(Vector3.forward * 3), this.transform.rotation) as GameObject;
			if (bullet != null){
				projectile = GetComponent<ColliderProjectile>();
			}
	}
	
	
	
	//Mech's triggers
	private void mechGatlingStart(){
		transform.FindChild("left_wrist_control_grp").GetComponent<BoxCollider>().enabled = true;
		transform.FindChild("right_wrist_control_grp").GetComponent<BoxCollider>().enabled = true;
	}
	
	private void mechGatlingStop(){
		transform.FindChild("left_wrist_control_grp").GetComponent<BoxCollider>().enabled = false;
		transform.FindChild("right_wrist_control_grp").GetComponent<BoxCollider>().enabled = false;
	}
}
