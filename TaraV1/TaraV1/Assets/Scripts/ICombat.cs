/* ICombat.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * The interface for combat units.
 * */
using System;
using System.Collections.Generic;
using UnityEngine;
public interface ICombat
{
	int CurHP{
		get;
		set;
	}
	
	ItemWeapon Weapon{
		get;
	}
	bool isAttacking{
		get;set;	
	}
	// The faction the gameobject belongs to
	List<BattleCore.Factions> FactionSelf{
		get;set;
	}
	// The hostile factions for the gameobject
	List<BattleCore.Factions> FactionHostile{
		get;set;
	}
	// Got hurt function
	void hurt(ItemWeapon weapon);
	// For knock back
	void AddImpact(Vector3 dir, float force);
	// While attacking others
	void onAttack(Vector3 contactPoint);
}