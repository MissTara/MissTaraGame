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
	// Use this for initialization
	void Start () {
		//generateRewards(1);
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		move = Vector3.zero;
		base.Update();
		
		if(GetComponent<Unit>().CurHP <= 0)
			die();
	}
	protected override void die ()
	{
		base.die ();
	}
	public void dropItems(int which){
		if (which == 0)
			Instantiate(ResourceManager.Get().preNoteTreble,this.transform.position,this.transform.rotation);
		else if (which == 1)
			Instantiate(ResourceManager.Get().preNoteHalf,this.transform.position,this.transform.rotation);
	}

}
