/* ItemMisc.cs
 * Author: Graeme MacDonald
 * Last Modified By: Graeme MacDonald
 * Description: 
 * Stores single misc data.
 * */
using System;
using UnityEngine;
public class ItemMisc: Item
{
	protected BattleCore.elements _Element;
	protected int _BaseAmount = 0;
	public BattleCore.elements Element{
		get { return _Element; }
		set { _Element = value; }
	}
	public int BaseAmount{
		get { return _BaseAmount; }
		set { _BaseAmount = value; }
	}
	public ItemMisc ()
	{
	}
	public ItemMisc(BattleCore.elements Element, int BaseAmount){
		this.Element = Element;
		this.BaseAmount = BaseAmount;
	}
}