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
		GameObject bullet = Instantiate(ResourceManager.Get().preBullet,this.transform.position + this.transform.TransformDirection(Vector3.up * 3) + this.transform.TransformDirection(Vector3.forward * 3), Quaternion.identity) as GameObject;
			if (bullet != null){
				projectile = GetComponent<ColliderProjectile>();
				if (projectile != null){
					if (this.transform.forward != Vector3.zero)
						projectile.gameObject.transform.forward = Vector3.zero;
					else
						projectile.gameObject.transform.forward = this.transform.forward;
					projectile.Activate(50,600);
					//rigidbody.velocity = transform.forward * 2000;
				}
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
