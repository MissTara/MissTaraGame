/* ColliderWeapon.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * It has to be attached to any weapons and handles collisions of weapons and bodies.
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ColliderWeapon : MonoBehaviour {
	public GameObject self;
	public ICombat combatSelf;
	// Use this for initialization
	void Start () {		
		if (self == null){
			print("ColliderWeapon.Start->Self not found");
			this.enabled = false;
			return;
		}
		combatSelf = self.GetComponent(typeof(ICombat)) as ICombat;
		if (combatSelf == null){
			print ("ColliderWeapon.Start->combatSelf Interface not found");
			this.enabled = false;
			return;
		}		
	}
	// Update is called once per frame
	void Update () {
	
	}
	void Attack(Collider other){
		if (combatSelf == null)
			return;
		//combatSelf.isAttacking && self.GetComponent<AIStates>() || 
		if (combatSelf.isAttacking){
			//Unit pOther = other.GetComponent<Unit>();
			//AIPathCustom pOther = other.GetComponent<AIPathCustom>();
			
			ICombat combatOther = other.GetComponent(typeof(ICombat)) as ICombat;
			if (combatOther == null){return;}
			if (combatSelf.FactionHostile.Count > 0 && BattleCore.isHostile(combatSelf.FactionHostile,combatOther.FactionSelf)){
				combatSelf.onAttack(other.ClosestPointOnBounds(other.transform.position));
				combatOther.hurt(combatSelf.Weapon);
				combatOther.AddImpact((other.transform.position - this.transform.position),500);
			}
		}
	}
	void OnTriggerEnter(Collider other){
		if (GameManager.isPaused)
			return;
		Attack(other);
	}
}
