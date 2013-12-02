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
	public Transform dropItem;
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
	private void generateRewards(int algoID){
		if (dropItem == null)
			return;
		Transform tmpReward, tran = this.transform;
		Transform item = dropItem;
		//print(item);
		tmpReward = Instantiate(item,tran.position + new Vector3(Random.Range(-2f,2f),1,Random.Range(-2f,2f)),tran.rotation) as Transform;
		//print(tmpReward);
		tmpReward.tag = "deathBonus";
		tmpReward.parent = tran;
		ColliderPickup tmpCP = tmpReward.GetComponent<ColliderPickup>();
		if (tmpCP){
			tmpCP.Active = false;
		}
		
	}
}
