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
    [System.NonSerialized] public bool isReadByBoss;
	float maxRange = 0;
	public List<BattleCore.Factions> factionsSelf = new List<BattleCore.Factions>();
	public List<BattleCore.Factions> factionsHostile = new List<BattleCore.Factions>();
	public float speed = 0;
	public bool isActive = false;
	private ItemWeapon Weapon;
	private Vector3 impact = Vector3.zero;

    [System.NonSerialized]
    public float killTime;
    public float TIME = 1;

	// Use this for initialization
	void Start () {
		spawnPos = transform.position;
		Weapon = new ItemWeapon(BattleCore.elements.Fire , 1);
        isReadByBoss = false;
        killTime = TIME;
		if(this.gameObject.layer == 10){
			maxRange = 30;
			speed = 30;
		}
		else
			maxRange = 10;
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(this.transform.position, spawnPos) > maxRange){
			Dispose();
		}
        if (isActive){
            print(rigidbody);
            if (rigidbody != null){
                print("Force Appliedx");
                rigidbody.AddForce(impact * Time.deltaTime);
            }
        }

        if (killTime <= 0)
        {
            Dispose();
        }
        else if (killTime > 0 && this.gameObject != null)
        {
            killTime -= Time.deltaTime;
        }

        if (this.gameObject != null)
			if(this.gameObject.layer == 10)
            	transform.position += transform.forward * speed * Time.deltaTime;
			else
				transform.position += transform.up * speed * Time.deltaTime;
	}
	void OnTriggerStay(Collider other){
			ICombat combatOther = other.GetComponent(typeof(ICombat)) as ICombat;
			if (combatOther == null){return;}
			if (factionsHostile.Count > 0 && BattleCore.isHostile(factionsHostile,combatOther.FactionSelf)){
				combatOther.hurt(Weapon);
				Dispose();
			}
	}
    //public void Activate( float Speed, float MaxRange){
    //    this.spawnPos = this.transform.position;
    //    this.speed = Speed;
    //    this.maxRange = MaxRange;
    //    impact = transform.forward * speed;
    //    isActive = true;
    //    rigidbody.velocity = this.transform.TransformDirection(Vector3.forward * speed);
    //}
	public void Dispose(){
		Destroy(this.transform.parent.gameObject);	
	}
}
