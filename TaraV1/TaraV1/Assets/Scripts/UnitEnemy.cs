/* [DEPRECATED] */
/* UnitEnemy.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * 		Deprecated
 * 
 * */

using UnityEngine;
using System.Collections;

public class UnitEnemy : Unit {
	private int dropChanceHealth = 1;
	private int dropChanceAmmo = 1;
	// Use this for initialization
	void Start () {
		//generateRewards(1);
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		move = Vector3.zero;
		base.Update();
	}
	protected override void die ()
	{
		//GetComponent<script_HUD>().StartCoroutine("bunnyGuage");
		base.die ();
		dropItems();	
	}
	protected void dropItems(){
		if(true){ //Health drop
			Instantiate(ResourceManager.Get().preNoteTreble,this.transform.position,this.transform.rotation);
			//Reset the counter
		}
		
		if (true){ //Ammo drop
			
		}
		//Vector3 itemLocation = (new Vector3(1,1,1)) + transform.position;
		//Instantiate (itemDrop, transform.position, transform.rotation);
		//itemLocation = (new Vector3(2,1,2)) + transform.position;
		//Instantiate (reward, itemLocation, transform.rotation);
		//itemLocation = (new Vector3(-1,1,-1)) + transform.position;
		//Instantiate (reward, itemLocation, transform.rotation);
		ColliderPickup[] tmpCP = GetComponentsInChildren<ColliderPickup>();
		foreach (ColliderPickup c in tmpCP){
			if (c.tag == "deathBonus")
			{c.Active = true;}
		}
	}
}
