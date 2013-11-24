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
	
	private void mechMissileAttack(){
		Vector3[] positions = new Vector3[2];
		for (int i = 0; i < 2; i++){
			positions[i] = new Vector3(Random.Range(this.transform.position.x-20.0f, this.transform.position.x+20.0f) ,-1.33f, Random.Range(this.transform.position.z-20.0f,this.transform.position.z+20.0f ));
			Instantiate(ResourceManager.Get().preMissileTarget,positions[i], this.transform.rotation);
			GameObject missile = Instantiate(ResourceManager.Get().preMissile,new Vector3(positions[i].x,300.0f,positions[i].z),ResourceManager.Get().preMissile.transform.rotation ) as GameObject;
			missile.rigidbody.AddForce(Vector3.down*3000.0f);
		}
	}
}
