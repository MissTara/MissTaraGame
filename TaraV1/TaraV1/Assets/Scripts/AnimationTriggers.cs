﻿/* AnimationTrigger.cs
 * Description: Used for when functions are called during animations
 * Only add to enemies that will use it
 * */
using UnityEngine;
using System.Collections;

public class AnimationTriggers : MonoBehaviour {
	//Alien triggers
	ColliderProjectile projectile;

	private void AlienAttack(){
		GameObject bullet;
		if(this.tag == "GunAlien")
			bullet = Instantiate(ResourceManager.Get().preEnemyBullet,this.transform.position + this.transform.TransformDirection(Vector3.up * 3) + this.transform.TransformDirection(Vector3.left *2), this.transform.rotation) as GameObject;
		else
			bullet = Instantiate(ResourceManager.Get().preEnemyBullet,this.transform.position + this.transform.TransformDirection(Vector3.up * 3) + this.transform.TransformDirection(Vector3.left *2), this.transform.rotation) as GameObject; // + this.transform.TransformDirection(Vector3.forward)
			if (bullet != null){
				projectile = GetComponent<ColliderProjectile>();
			}
	}
    public int missilesToFire = 2;
	
	//Regular enemy triggers
	
	private void enemyAttackStart(){
		transform.FindChild("Attackbox").GetComponent<BoxCollider>().enabled = true;
	}
	
	private void enemyAttackStop(){
		transform.FindChild("Attackbox").GetComponent<BoxCollider>().enabled = false;
	}
	
	//Mech's triggers
	private void mechSlowDown(){
		animation["MechAttack1"].speed = 0.1f;	
	}
	
	private void mechGatlingStart(){
		transform.FindChild("left_wrist_control_grp").GetComponent<BoxCollider>().enabled = true;
		transform.FindChild("right_wrist_control_grp").GetComponent<BoxCollider>().enabled = true;
		animation["MechAttack1"].speed = 0.1f;
	}
	
	private void mechGatlingStop(){
		transform.FindChild("left_wrist_control_grp").GetComponent<BoxCollider>().enabled = false;
		transform.FindChild("right_wrist_control_grp").GetComponent<BoxCollider>().enabled = false;
		animation["MechAttack1"].speed = 0.1f;
	}
	
	private void mechMissileAttack(){
        int howmanyMissiles = Mathf.Clamp(missilesToFire, this.GetComponent<AIBoss>().missilesToFireMin, this.GetComponent<AIBoss>().missilesToFireMax);

        Vector3[] positions = new Vector3[howmanyMissiles];

        positions[0] = LevelLoader.Get().mainPlayer.transform.position;
        positions[0].y = -1.33f;
        Instantiate(ResourceManager.Get().preMissileTarget, positions[0], this.transform.rotation);
        GameObject missile = Instantiate(ResourceManager.Get().preMissile, new Vector3(positions[0].x, 20.0f, positions[0].z), ResourceManager.Get().preMissile.transform.rotation) as GameObject;
        missile.transform.localScale = missile.transform.localScale * 1.5f;
        missile.rigidbody.AddForce(Vector3.down * 10.0f);

        for (int i = 1; i < howmanyMissiles; i++)
        {
            positions[i] = new Vector3(Random.Range(this.transform.position.x - 15.0f, this.transform.position.x + 15.0f), -1.33f, Random.Range(this.transform.position.z - 15.0f, this.transform.position.z + 15.0f));
			Instantiate(ResourceManager.Get().preMissileTarget,positions[i], this.transform.rotation);
			missile = Instantiate(ResourceManager.Get().preMissile,new Vector3(positions[i].x,20.0f,positions[i].z),ResourceManager.Get().preMissile.transform.rotation ) as GameObject;
            missile.transform.localScale = missile.transform.localScale * 1.5f;
            missile.rigidbody.AddForce(Vector3.down * 10.0f);
		}

        missilesToFire += 1;
	}
	
	
	//Captain's triggers
	
	private void captainAttack1Swing(){
		animation["captainBossAttack1"].speed = 1.0f;
		transform.FindChild("polySurface1").GetComponent<BoxCollider>().enabled = true;
	}
	
	private void captainAttack2Hold(){
		animation["captainBossAttack2"].speed = 0.1f;
	}
	
	private void captainAttack2Slam(){
		animation["captainBossAttack2"].speed = 1.0f;
		transform.FindChild("polySurface1").GetComponent<BoxCollider>().enabled = true;
	}
	
	private void captainAttackDone(){
		transform.FindChild("polySurface1").GetComponent<BoxCollider>().enabled = false;
	}

    private void captainAttackAir()
    {
        Instantiate(ResourceManager.Get().preAirEffect, transform.FindChild("polySurface1").transform.position - new Vector3(0,2.0f,0), this.gameObject.transform.rotation);
        Instantiate(ResourceManager.Get().preAir, transform.FindChild("polySurface1").transform.position, this.gameObject.transform.rotation);
    }
}
