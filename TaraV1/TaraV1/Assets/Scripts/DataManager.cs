/* DataManager.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Graeme MacDonald
 * Description: 
 * Stores the weapon, armour and misc data.
 * */
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class DataManager
{
	// Stores a list of items
	public List<ItemWeapon> Weapons;
	public List<ItemArmour> Armours;
	public List<ItemMisc> Misc;
	// Enemy Properties Data
	// Different enemies can have different damage, Element, HP and etc.
	public List<GameManager.EnemyProperties> Enemies;
	public DataManager ()
	{
		Weapons = new List<ItemWeapon>();
		Armours = new List<ItemArmour>();
		Misc	= new List<ItemMisc>();
		Enemies = new List<GameManager.EnemyProperties>();
	}
	// Get item from the database
	public ItemWeapon getWeaponById(int id){
		foreach (ItemWeapon w in Weapons){
			if (w.ItemID == id){
				return w;
			}
		}
		return null;
	}
	// Get item from the database
	public ItemArmour getArmourById(int id){
		foreach (ItemArmour a in Armours){
			if (a.ItemID == id){
				return a;
			}
		}
		return null;
	}
	// Get item from the database
	public ItemMisc getMiscById(int id){
		foreach (ItemMisc m in Misc){
			if (m.ItemID == id){
				return m;
			}
		}
		return null;
	}
}

