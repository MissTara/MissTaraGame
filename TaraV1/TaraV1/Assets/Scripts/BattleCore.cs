/* BattleCore.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Alfred Lai
 * Description: 
 * Provides the basic battle functions.
 * Calculates damage.
 * Determines if a character can attack another one.
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleCore : MonoBehaviour {
	// Defines elements for weapons and armours
	public enum elements{
		Physical = 0,
		Earth = 1,
		Fire = 2,
		Lightning = 3,
		Tech = 4,
		Snoic = 5,
		Ice = 6,
		Water = 7,
		None = 8
	}
	// Defines factions
	// Factions are used for the game to decide if one can hurt another.
	// Just like the fractions in the World Of Warcraft(Alliance & Horde & Shen'dralar)
	public enum Factions{
		human = 0,
		alien = 1
	}
	// Calculates damage depends on weapons and armours
	public static int CalculateDamage(Unit attacker, Unit defenser){
		int elementalMultiplier = 1 + CalcEleMulti(attacker.Weapon.Element,defenser.Armour.Element);
		int damage = (attacker.Damage * elementalMultiplier) - defenser.Armour.BaseArmour;
		return damage;
	}
	public static int CalculateDamage(int atkDamage, elements atkEle, int defArmour, elements defEle){
		int elementalMultiplier = 1 + CalcEleMulti(atkEle,defEle);
		int damage = (atkDamage * elementalMultiplier) - defArmour;
		return damage;
	}
	private static int CalcEleMulti(elements ele1, elements ele2){
		if (ele1 == elements.None || ele2 == elements.None)
			return 1;
		int eleOffset = Mathf.Abs( ele1 - ele2);
		if (eleOffset <= 4 )
			return eleOffset;
		else
			return 8 - eleOffset;
	}
	// Current Opponent Evaluation:
	// If one faction that the defenser has is not the opponent of the attacker, the defenser is not an opponent
	public static bool isHostile(List<Factions> atk, List<Factions> def){
		foreach (Factions f in def){
			if (!atk.Contains(f)){
				return false;
			}
		}
		return true;
	}
}
