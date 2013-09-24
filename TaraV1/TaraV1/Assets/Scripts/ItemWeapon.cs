/* ItemWeapon.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Stores single weapon data.
 * */
using System;
using UnityEngine;
public class ItemWeapon : Item
{
	protected BattleCore.elements _Element;
	protected int _BaseDamage = 0;
	protected float _AttackInterval = 1f;					// Deprecated
	protected Vector3 _BounceBack = new Vector3(0, 10,-10); // Deprecated
	
	public string audioAttack = "096-Attack08";
	public BattleCore.elements Element{
		get { return _Element; }
		set { _Element = value; }
	}
	public int BaseDamage{
		get { return _BaseDamage; }
		set { _BaseDamage = value; }
	}
	// Deprecated?
	public float AttackInterval{
		get { return _AttackInterval; }
		set { _AttackInterval = value; }
	}
	// Deprecated
	public Vector3 BounceBack{
		get { return _BounceBack;}
		set { _BounceBack = value;}
	}
	
	// Constructors
	public ItemWeapon(){}
	public ItemWeapon(BattleCore.elements Element, int BaseDamage, float AttackInterval){
		this._Element = Element;
		this._BaseDamage = BaseDamage;
		this._AttackInterval = AttackInterval;
	}
	public ItemWeapon(string name,BattleCore.elements Element, int BaseDamage, float AttackInterval){
		this._Name = name;
		this._Element = Element;
		this._BaseDamage = BaseDamage;
		this._AttackInterval = AttackInterval;
	}
	public ItemWeapon(BattleCore.elements Element, int BaseDamage){
		this._Element = Element;
		this._BaseDamage = BaseDamage;
	}
}