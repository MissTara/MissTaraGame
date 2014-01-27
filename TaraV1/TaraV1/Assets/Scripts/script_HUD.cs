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
	private Texture texCharPortrait, texHP, texBunny, texBunnyBG;
	Rect rectCharPortrait, rectSrcCharPortrait, rectHP, rectBunny, rectBunnyBG;
	// HP Flow
	float intHPFlow, intHPRange;
	private float range = 0.0f;
	public float rangeBunny = 1.0f;
	public int ammo = 5;
	private int maxAmmo = 10;
	// Use this for initialization
	void Start () {
		// Load Textures
		//texCharPortrait = (Texture)Resources.Load("texCharPortrait");
		//texHPBG = (Texture)Resources.Load("texHP");
		//texHP = (Texture)Resources.Load("texHP");
		//texGrenadeIcon = (Texture)Resources.Load("texGrenade");
		//Calculate Positions
		texCharPortrait = ResourceManager.Get().tex_HUD_HeadTexture;
		texHP = ResourceManager.Get().tex_HUD_HPBar;
		texBunny = ResourceManager.Get().tex_HUD_BunnyBar;
		texBunnyBG = ResourceManager.Get().tex_HUD_BunnyBarBG;
		rectCharPortrait = new Rect(10,10, texCharPortrait.width / 4, texCharPortrait.height / 4);
		rectSrcCharPortrait = new Rect(0,0, 1, 1);
		//rectSrcCharPortrait = new Rect(0,0, 0.2f, 1);
		rectHP = new Rect(80, 20, texHP.width * 0.5f, 40.0f);
		rectBunny = new Rect(80,70,texBunny.width,texBunny.height);
		rectBunnyBG = new Rect(80,70,texBunnyBG.width, texBunnyBG.height);
	}
	void OnGUI(){
		if (GameManager.isPaused)
			return;
		// Draw The Pause Button
		/*if (GUI.Button(new Rect(Screen.width / 2 - 25,25,50,50), "II")){
			GameManager.isPaused = true;
			MenuMain.Get().show = true;
		}*/
		//GUI.DrawTexture(rectCharPortrait,texCharPortrait);
		GUI.DrawTextureWithTexCoords(rectCharPortrait,texCharPortrait,rectSrcCharPortrait);
		//GUI.DrawTextureWithTexCoords(rectHPBG,texHPBG, rectSrcHPBG);
		//GUI.DrawTextureWithTexCoords(rectHP,texHP,rectSrcHP);
		
		//Bunny bar
		GUI.DrawTexture(rectBunnyBG,texBunnyBG);
		GUI.BeginGroup(new Rect(80,70,rectBunny.width, 36.0f));
		GUI.DrawTexture(new Rect(-texBunny.width * rangeBunny, 0.0f, rectBunny.width, 36.0f),texBunny);
		GUI.EndGroup();

		//Health bar
		GUI.BeginGroup(new Rect(80,20,rectHP.width, 40.0f));
		GUI.DrawTexture(new Rect(-rectHP.width * range, 0.0f, rectHP.width, 40.0f),texHP);
		GUI.EndGroup();
	}
	// Update is called once per frame
	void Update () {
		// Stop updating while pausing
		if (GameManager.isPaused)
			return;
		SyncPlayerStatus();
	}
	void SyncPlayerStatus(){
        if (LevelLoader.Get().mainPlayer != null)
        {
            UnitPlayer player = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitPlayer>();
            if (player != null){
                range = (((float)player.CurHP / (float)player.MaxHP) - 1.0f) * -1.0f;
            }
        }
	}
	public void changeAmmo(int ammount){
		if(ammo + ammount >= maxAmmo)
			ammo = maxAmmo;
		else
			ammo += ammount;	
	}
	
	IEnumerator bunnyGuage(){
		if (rangeBunny-0.1f < 0.0f)
			rangeBunny = 0.0f;	
		else
			rangeBunny -= 0.1f;
		yield return new WaitForSeconds(0.1f);
		StopCoroutine("bunnyGuage");
	}
	
	public void resetGuage(){
		rangeBunny = 1.0f;	
	}
}
