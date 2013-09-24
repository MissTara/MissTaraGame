/* Item.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Base item class which privides basic fields.
 * */
using System;

public class Item
{
	protected int 		_ID				= 0;
	protected string 	_Name 			= "";
	protected string	_Description	= "";
	protected int		_Price 			= 0;
	public int Icon = 0;
	public int ItemID{
		get { return _ID; }
		set { _ID = value; }
	}
	public string ItemName{
		get { return _Name; }
		set { _Name = value; }
	}
	public string ItemDescription{
		get { return _Description; }
		set { _Description =  value; }
	}
	public int ItemPrice{
		get { return _Price; }
		set { _Price = value; }
	}
	public Item ()
	{
	}
}