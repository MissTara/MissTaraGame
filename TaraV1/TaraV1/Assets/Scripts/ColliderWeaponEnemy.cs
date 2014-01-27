// [Deprecated]
using UnityEngine;
using System.Collections;

public class ColliderWeaponEnemy : MonoBehaviour {
	public GameObject self;
	ICombat selfCombat;
	// Use this for initialization
	void Start () {
		if (self == null){
			print ("Error:ColliderWeaponEnemy"  + " has no host");
			this.enabled = false;
		}
		selfCombat = self.GetComponent(typeof(ICombat)) as ICombat;
		if (selfCombat == null){
			print ("Error:ColliderWeaponEnemy"  + " has no AIPathCustom");
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<Rigidbody>().IsSleeping())
			GetComponent<Rigidbody>().WakeUp();	
	}
	void Attack(Collider other){
		if(selfCombat == null)
			return;
		if (selfCombat.isAttacking){
			ICombat otherCombat = other.GetComponent(typeof(ICombat)) as ICombat;
			if (otherCombat == null){
				print("Error:ColliderWeaponEnemy player has no Script");
				return;
			}
			otherCombat.hurt(selfCombat.Weapon);
			//print("Damage Dealt:" +BattleCore.CalculateDamage(ai.BaseAttack,ai.Element,player.Armour.BaseArmour,player.Armour.Element));
			//player.CurHP -= BattleCore.CalculateDamage(ai.BaseAttack,ai.Element,player.Armour.BaseArmour,player.Armour.Element);
		}
	}
	void OnTriggerStay(Collider other){
		if (other.tag == "Player")
			Attack(other);
	}
	
	void OnTriggerEnter(Collider other){
		if (other.tag == "Sidewalls" && transform.parent.tag == "Bat" && transform.parent.GetComponent<AIStates>().swoop){
			Vector3 tmp = transform.parent.GetComponent<AIStates>().speedSwoop;
			transform.parent.GetComponent<AIStates>().speedSwoop = new Vector3(0.0f,tmp.y,0.0f);	
		}
	}
}
