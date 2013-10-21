//[NEED TO BE UPDATED]
// Ask Graeme for details
using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
public class MenuMain : GUIBase {
	
	private static MenuMain m_Instance = null;
    
	public static MenuMain Get(){
        if (m_Instance == null)
            m_Instance = (MenuMain)FindObjectOfType(typeof(MenuMain));
        return m_Instance;
    }
	
	GUIStyle menuText, IconAvailable, IconUnavailable, infoText;
	private ResourceManager utiliRM;
	private SaveLoad		utiliSL;
	public bool show = false;
	private int currentItemIndex = 0;
	
	private bool isSpinning = false;
	private bool WeaponBool = true;
	private bool ArmourBool = false;
	private bool MiscBool = false;
	
	
	// Use this for initialization
	void Start () {
		utiliRM = ResourceManager.Get();
		utiliSL = SaveLoad.Get();
		if (utiliRM == null){
			print("Resource Manager is invalid");
			this.enabled = false;
		}
		if (utiliSL = null){
			print("SaveLoad is invalid");
			this.enabled = false;
		}
	}
	


protected override void OnGUI (){
		if (!show)
			return;
		
		
		//GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), utiliRM.tex_Black);
		menuText = new GUIStyle(GUI.skin.button);
		menuText.fontSize = 18;
		menuText.fontStyle = FontStyle.Bold;
		
		
		infoText = new GUIStyle(GUI.skin.label);
		infoText.fontSize = 22;
		infoText.fontStyle = FontStyle.Bold;
		
		IconAvailable = new GUIStyle(GUI.skin.button);
		IconAvailable.imagePosition = ImagePosition.ImageAbove;
		base.OnGUI ();
		
	}
	
	protected override void OnDraw (){
		GUI.DrawTexture(new Rect(0,0,1366,800),utiliRM.tex_MenuBack);
		string funcText = "";
		
		
		
		
		if (GUI.Button(new Rect(780,250,150,100), "Weapons",menuText)){
			WeaponBool = true;
			ArmourBool = false;
			MiscBool = false;
	
			currentItemIndex = 0;
		}
		
		if (GUI.Button(new Rect(955,250,150,100), "Armour",menuText)){
			WeaponBool = false;
			ArmourBool = true;
			MiscBool = false;
			currentItemIndex = 0;
		}
		
		if (GUI.Button(new Rect(1130,250,150,100), "Misc",menuText)){
			WeaponBool = false;
			ArmourBool = false;
			MiscBool = true;
			currentItemIndex = 0;
			
		}
		
		if (GUI.Button(new Rect(955,625,150,100), "Back To Title",menuText)){
			Application.LoadLevel(0);
		}
		if (GUI.Button(new Rect(1130,625,150,100),"Resume",menuText)){
			GameManager.isPaused = false;
			this.show = false;
		};
		
		if (WeaponBool == true){
		int column = 0;
		if (GameManager.userData.ownedWeapons.Contains(GameManager.dataManager.Weapons[currentItemIndex].ItemID))
			funcText = "Equip";
		else
			funcText = "Buy";
			
			if (GUI.Button(new Rect(780,625,150,100), funcText,menuText)){
			if (funcText == "Buy")
			{
				if (GameManager.userData.purchase(GameManager.dataManager.Weapons[currentItemIndex].ItemPrice)){
					GameManager.userData.ownedWeapons.Add(GameManager.dataManager.Weapons[currentItemIndex].ItemID);
					utiliSL.SaveGameData();
					print("Item Purchased");
				}
			}
			else if (funcText == "Equip"){
				
				GameManager.userData.equipWeapon(GameManager.dataManager.Weapons[currentItemIndex].ItemID);
				print("Equipped" + UnitPlayer.Get().Weapon.ItemID);
			}
		}
			
			
			for (int i = 0; i < GameManager.dataManager.Weapons.Count;i++){
			String text = "Buy Now";
			if (GameManager.userData.ownedWeapons.Contains(GameManager.dataManager.Weapons[i].ItemID))
				text = "";
			if (UnitPlayer.Get().Weapon.ItemID == GameManager.dataManager.Weapons[i].ItemID)
				text = "Equipped";
			
			GUIContent guiC = new GUIContent(text,utiliRM.tex_WeaponIcons[GameManager.dataManager.Weapons[i].Icon]);
			
			if(GUI.Button(new Rect(770 + column * 96 + column * 40,375 + 120 * (int)(i / 4),110,110), guiC, IconAvailable)){
				currentItemIndex = i;
			}
			column++;
			column  %= 4;
		}
		}
		
		if (ArmourBool == true){
		int column = 0;
			if (GameManager.userData.ownedArmours.Contains(GameManager.dataManager.Armours[currentItemIndex].ItemID))
			funcText = "Equip";
		else
			funcText = "Buy";
			
			if (GUI.Button(new Rect(700,650,150,100), funcText,menuText)){
			if (funcText == "Buy")
			{
				if (GameManager.userData.purchase(GameManager.dataManager.Armours[currentItemIndex].ItemPrice)){
					GameManager.userData.ownedArmours.Add(GameManager.dataManager.Armours[currentItemIndex].ItemID);
					utiliSL.SaveGameData();
					print("Item Purchased");
				}
			}
			else if (funcText == "Equip"){
				
				GameManager.userData.equipArmour(GameManager.dataManager.Armours[currentItemIndex].ItemID);
				print("Equipped" + UnitPlayer.Get().Weapon.ItemID);
			}
		}
			
			for (int i = 0; i < GameManager.dataManager.Armours.Count;i++){
				
			String text = "Buy Now";
			if (GameManager.userData.ownedArmours.Contains(GameManager.dataManager.Armours[i].ItemID))
				text = "";
			if (UnitPlayer.Get().Armour.ItemID == GameManager.dataManager.Armours[i].ItemID)
				text = "Equipped";
			
			GUIContent guiC = new GUIContent(text,utiliRM.tex_WeaponIcons[GameManager.dataManager.Armours[i].Icon]);
			
			if(GUI.Button(new Rect(800 + column * 96 + column * 40,375 + 120 * (int)(i / 4),110,110), guiC, IconAvailable)){
				currentItemIndex = i;
			}
			column++;
			column  %= 4;
		}
		}
		
		if (MiscBool == true)
        {
		    int column = 0;
			if (GameManager.userData.ownedMisc.Contains(GameManager.dataManager.Misc[currentItemIndex].ItemID))
			    funcText = "Use";
		    else
			    funcText = "Buy";
			
			if (GUI.Button(new Rect(700,650,150,100), funcText,menuText))
            {
			    if (funcText == "Buy")
			    {
				    if (GameManager.userData.purchase(GameManager.dataManager.Misc[currentItemIndex].ItemPrice))
                    {
					    GameManager.userData.ownedMisc.Add(GameManager.dataManager.Misc[currentItemIndex].ItemID);
					    utiliSL.SaveGameData();
					    print("Item Purchased");
				    }
			    }
			    else if (funcText == "Use")
                {
                    if (UnitPlayer.Get().CurHP < UnitPlayer.Get().MaxHP)
                    {
                        // add health to player UnitPlayer.Get().gainHealth
                    }
			    }
		    }
			
			
			for (int i = 0; i < GameManager.dataManager.Misc.Count;i++)
            {
				
			    String text = "Buy Now";
			    if (GameManager.userData.ownedMisc.Contains(GameManager.dataManager.Misc[i].ItemID))
				    text = "";
				    /*
			    if (UnitPlayer.Get().Misc.ItemID == GameManager.dataManager.Misc[i].ItemID)
				    text = "Equipped";
			    */
			    GUIContent guiC = new GUIContent(text,utiliRM.tex_WeaponIcons[GameManager.dataManager.Misc[i].Icon]);
			
			    if(GUI.Button(new Rect(800 + column * 96 + column * 40,375 + 120 * (int)(i / 4),110,110), guiC, IconAvailable))
                {
				    currentItemIndex = i;
			    }
			    column++;
			    column  %= 4;
		    }
		}
		
	
		GUI.Label(new Rect(750,210,300,200),"Currency:" + GameManager.userData.currency, infoText);
		GUI.Label(new Rect(750,50,300,100),GameManager.dataManager.Weapons[currentItemIndex].ItemName, infoText);
		GUI.Label(new Rect(750,100,600,100),GameManager.dataManager.Weapons[currentItemIndex].ItemDescription, infoText);
		GUI.Label(new Rect(950,210,300,200),"Price:" + GameManager.dataManager.Weapons[currentItemIndex].ItemPrice, infoText);
		base.OnDraw ();
	}
	
	
	// Update is called once per frame
	void Update () {
		Vector3 vTmp = new Vector3(1,1,1);
		//vTmp = vTmp + Camera.mainCamera.transform.forward;
		
	}
	
	/*void TestFunction(){
		int column = 0;
		
		for (int i = 0; i < GameManager.dataManager.Weapons.Count;i++){
			String text = "Buy Now";
			if (GameManager.userData.ownedWeapons.Contains(GameManager.dataManager.Weapons[i].ItemID))
				text = "";
			if (UnitPlayer.Get().Weapon.ItemID == GameManager.dataManager.Weapons[i].ItemID)
				text = "Equipped";
			GUIContent guiC = new GUIContent(text,utiliRM.tex_WeaponIcons[GameManager.dataManager.Weapons[i].Icon]);
		
			if(GUI.Button(new Rect(800 + column * 96 + column * 20,300 + 120 * (int)(i / 4),110,110), guiC, IconAvailable)){
				currentItemIndex = i;
			}
			column++;
			column  %= 4;
		}	
	}*/
}
