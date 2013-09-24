/* ItemArmour.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Stores single armour data.
 * */
using System;
using UnityEngine;
public class ItemArmour: Item
{
	protected BattleCore.elements _Element;
	protected int _BaseArmour = 0;
	public BattleCore.elements Element{
		get { return _Element; }
		set { _Element = value; }
	}
	public int BaseArmour{
		get { return _BaseArmour; }
		set { _BaseArmour = value; }
	}
	public ItemArmour ()
	{
	}
	public ItemArmour(BattleCore.elements Element, int BaseArmour){
		this.Element = Element;
		this.BaseArmour = BaseArmour;
	}
}