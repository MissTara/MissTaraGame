/* ColliderProjectile.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Simulates a projectile's behaviour.
 * Used for bullets or grenades
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ColliderProjectile : MonoBehaviour {
	Vector3 spawnPos = Vector3.zero;
	float maxRange = 0;
	public List<BattleCore.Factions> factionsSelf = new List<BattleCore.Factions>();
	public List<BattleCore.Factions> factionsHostile = new List<BattleCore.Factions>();
	public float speed = 0;
	public bool isActive = false;
	private ItemWeapon Weapon;
	private Vector3 impact = Vector3.zero;
	// Use this for initialization
	void Start () {
		spawnPos = transform.position;
		speed = 80;
		maxRange = 50;
		Weapon = new ItemWeapon(BattleCore.elements.Fire , 30);
	}
	
	// Update is called once per frame
	void Update () {
		//impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
		if (Vector3.Distance(this.transform.position, spawnPos) > maxRange){
			Dispose();
		}
		if (isActive){
			//this.transform.Translate(direction.normalized * speed * Time.deltaTime);
			print(rigidbody);
			if (rigidbody != null){
				print("Force Appliedx");
				//rigidbody.AddForce();
				rigidbody.AddForce(impact * Time.deltaTime);
				
			}
		}
	}
	void OnTriggerStay(Collider other){
			ICombat combatOther = other.GetComponent(typeof(ICombat)) as ICombat;
			if (combatOther == null){return;}
			if (factionsHostile.Count > 0 && BattleCore.isHostile(factionsHostile,combatOther.FactionSelf)){
				onAttack(other.ClosestPointOnBounds(other.transform.position));
				combatOther.hurt(Weapon);
				Dispose();
				//combatOther.AddImpact((other.transform.position - this.transform.position),500);
			}
	}
	void onAttack(Vector3 contactPoint){
		Instantiate(ResourceManager.Get().particleAttack, contactPoint,Quaternion.identity);
	}
	public void Activate( float Speed, float MaxRange){
		this.spawnPos = this.transform.position;
		this.speed = Speed;
		this.maxRange = MaxRange;
		impact = transform.forward * 1000;
		isActive = true;
		rigidbody.velocity = this.transform.TransformDirection(Vector3.forward * 100);
	}
	public void Dispose(){
		Destroy(this.gameObject);	
	}
}
