/* UserData.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * This class is used for data serlization.
 * Stores basic data for a user such as the currencies and owned stuffs.
 * 
 * */
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class UserData{
	public List<int> ownedWeapons;
	public List<int> ownedArmours;
	public List<int> ownedMisc;
	public int currency;
	public int equippedWeapon = 0,equippedArmour = 0;
	public UserData(){
		ownedWeapons = new List<int>();
		ownedArmours = new List<int>();
		ownedMisc    = new List<int>();
		currency = 0;
	}
	public void initializePlayer(){}
	public void applyToPlayer(){
		equipArmour(equippedArmour);
		equipWeapon(equippedWeapon);
	}
	public bool purchase(int cost){
		if (cost <= currency){
			currency -= cost;
			SaveLoad.Get().SaveUserData();
			return true;
		}
		else{
			return false;
		}
	}
	public int gainMoney(int amount){
		currency += amount;
		SaveLoad.Get().SaveUserData();
		return currency;
	}
	public void gainItem(Item item){
		if (item.GetType() == typeof(ItemArmour)){
			ownedArmours.Add(item.ItemID);
		}
		if (item.GetType() == typeof(ItemWeapon)){
			ownedWeapons.Add(item.ItemID);
		}
		if (item.GetType() == typeof(ItemMisc)){
			ownedMisc.Add(item.ItemID);
		}
		SaveLoad.Get().SaveUserData();
	}
	public void equipWeapon(int id){
		UnitPlayer player = UnitPlayer.Get();
		ItemWeapon w = GameManager.dataManager.getWeaponById(id);
		if (player != null &&  w != null){
			player.Weapon = w;
			equippedWeapon = id;
			SaveLoad.Get().SaveUserData();
		}
	}
	public void equipArmour(int id){
		UnitPlayer player = UnitPlayer.Get();
		ItemArmour a = GameManager.dataManager.getArmourById(id);
		if (a != null){
			player.Armour = a;
			equippedArmour = id;
			SaveLoad.Get().SaveUserData();
		}
	}

    public void useMisc(int id)
    {
        UnitPlayer player = UnitPlayer.Get();
        ItemMisc misc = GameManager.dataManager.getMiscById(id);
        if (misc != null)
        {
            player.CurHP += 100;
        }
    }
}
