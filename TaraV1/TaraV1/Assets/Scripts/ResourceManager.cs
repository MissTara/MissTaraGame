/* ResourceManager.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * It only provides a space for developers to put resources which makes developers access them easliy.
 * 
 * We simply attach this script to a game object and drag the rescource we need into it.
 * It can be accessed by "ResourceManager.Get().texture"
 * */
using System;
using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
	private static ResourceManager m_Instance = null;
    public static ResourceManager Get()
    {
        if (m_Instance == null)
            m_Instance = (ResourceManager)FindObjectOfType(typeof(ResourceManager));
        return m_Instance;
    }
	public Texture tex_HUD_HPBar,
tex_HUD_HeadTexture,
tex_HUD_Grenade,
tex_Controller_Joystick,
tex_Controller_ATK,
tex_Controller_JMP,
	tex_HurtScreen,
	tex_Black,
	tex_MenuBack,
	tex_LoadingScreen,
	tex_Credits;
	public Texture [] tex_WeaponIcons;
	public AudioClip se_PlayerJump, se_AttackOn, se_Attack;
	
	public GameObject preEnemyAlien,preEnemyBat,preEnemyBear,preEnemyWolf,preMechBoss,preFPSMonitor,preDirectionalLight,preMainPlayer,preGround,
	preMenu,preKillZone,preJoyStick, preNoteQuarter,preNoteHalf, preNoteSharp, preNoteTreble,preNoteWhole,preBullet,preCollideWall,preEnemyBullet;
	public GameObject particleAttack, particleExplosion;

    public Texture[] tex_Slides;

	public ResourceManager ()
	{
        
	}
}