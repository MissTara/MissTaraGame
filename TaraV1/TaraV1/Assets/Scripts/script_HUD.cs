/* script_HUD.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * This class the basic UI of the gameplay system including the health bar, currencies and pause button.
 * [This class should be fixed in the future]
 * Now it is not derived from GUIBase which makes it look awful in some low resolution screens.
 * */
using UnityEngine;
using System.Collections;

public class script_HUD : MonoBehaviour {
	private Texture texCharPortrait, texHPBG, texHP, texGrenadeIcon;
	Rect rectCharPortrait, rectSrcCharPortrait, rectHPBG, rectSrcHPBG, rectHP, rectSrcHP, rectGrenadeIcon;
	// HP Flow
	float intHPFlow = 0, intHPRange;
	// Use this for initialization
	void Start () {
		// Load Textures
		//texCharPortrait = (Texture)Resources.Load("texCharPortrait");
		//texHPBG = (Texture)Resources.Load("texHP");
		//texHP = (Texture)Resources.Load("texHP");
		//texGrenadeIcon = (Texture)Resources.Load("texGrenade");
		// Calculate Positions
		texCharPortrait = ResourceManager.Get().tex_HUD_HeadTexture;
		texHPBG = ResourceManager.Get().tex_HUD_HPBar;
		texHP = ResourceManager.Get().tex_HUD_HPBar;
		texGrenadeIcon = ResourceManager.Get().tex_HUD_Grenade;
		rectCharPortrait = new Rect(10,10, texCharPortrait.width / 4, texCharPortrait.height / 4);
		rectSrcCharPortrait = new Rect(0,0, 1, 1);
		//rectSrcCharPortrait = new Rect(0,0, 0.2f, 1);
		rectHPBG = new Rect(130, 30, texHPBG.width * 0.5f, texHPBG.height * 0.5f * 5);
		rectSrcHPBG = new Rect(0, 0, 1, 0.5f);
		rectHP = new Rect(130, 30, texHP.width * 0.5f, texHP.height * 0.5f * 5);
		rectSrcHP = new Rect(0, 0.5f, 1, 0.5f);
		rectGrenadeIcon = new Rect(130, 55, texGrenadeIcon.width, texGrenadeIcon.height);
		
	}
	void OnGUI(){

		if (GameManager.isPaused)
			return;
		// Draw The Pause Button
		if (GUI.Button(new Rect(Screen.width / 2 - 25,25,50,50), "II")){
			GameManager.isPaused = true;
			MenuMain.Get().show = true;
		}
		//GUI.DrawTexture(rectCharPortrait,texCharPortrait);
		GUI.DrawTextureWithTexCoords(rectCharPortrait,texCharPortrait,rectSrcCharPortrait);
		GUI.DrawTextureWithTexCoords(rectHPBG,texHPBG, rectSrcHPBG);
		GUI.DrawTextureWithTexCoords(rectHP,texHP,rectSrcHP);
		GUI.DrawTexture(rectGrenadeIcon, texGrenadeIcon);
		GUI.Label(new Rect(130,80,100,100),"Coins:" + GameManager.userData.currency);
	}
	// Update is called once per frame
	void Update () {
		// Stop updating while pausing
		if (GameManager.isPaused)
			return;
		// HP Bar flowing
		intHPFlow += 5 * Time.deltaTime * 60;
		if (intHPFlow >= texHP.width / 3 * 2)
			intHPFlow = 0;
		rectSrcHP = new Rect(intHPFlow / texHP.width, 0.5f, 1, 0.5f);
		// Update the HP Bar
		SyncPlayerStatus();
	}
	void SyncPlayerStatus(){
	 	UnitPlayer player = GameObject .FindGameObjectWithTag("Player").GetComponent<UnitPlayer>();
		if (player != null){
			rectHP = new Rect(130, 30, texHP.width * 0.5f / player.MaxHP * player.CurHP, texHP.height * 0.5f * 5);
		}
	}
}
