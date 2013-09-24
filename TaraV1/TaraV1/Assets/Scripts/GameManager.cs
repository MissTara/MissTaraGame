/* GameManager.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * The GameManager manages the game state and contains a database for item events.
 * */
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
public class GameManager : MonoBehaviour
{
	//static script_player m_Player, m_weapon;
	//static script_camera m_Camera;
	public static UnitPlayer MainPlayer;				// Deprecated
	public static float checkPointX = 0f, checkPointY = 0f, checkPointZ = 0f; // Stores the last check point
	private static bool _isPaused = false;				// if the game is paused
	public static bool isDebugging = true;				// if the game is on debug mode
	public static bool useGyroScope = true;				// Deprecated
	public static bool useSwipe = true;					// Deprecated
	public static DataManager dataManager;				// Game database
	public static UserData userData;					// Current user data
	public static bool isPaused{
		get {return _isPaused;}
		set {
			_isPaused = value;
		}
	}
	
	//public static int Coins = 0; Deprecated
	
	public static bool onMobileDevices()
	// Detects if the game is running on the mobile platform
	{
		return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
	}
	// Item event database
	public static void getItem(int itemID){
		switch(itemID){
		case 0:
			break;
		case 1:						// Single Coin
			getCoin(1);
			break;
		case 2:						// Double Coin
			getCoin(2);
			break;
		case 3:						// Set Check Point
			setCheckPoint();
			break;
		case 4:						// Kill Zone
			KillZone();
			break;
		case 99:					// Load default cut scene
			CutScene cutScene = CutScene.Get();
			if(cutScene != null)
				cutScene.dbg_LoadCutScenes();
			break;
		case 100:					// Load a script which creates two invisible walls
			if (CommandScript.Get() != null){
				List<CommandScript.BasicCommand> bCmd = new List<CommandScript.BasicCommand>();
				bCmd.Add(new CommandScript.BasicCommand ("EnableGameObject","eventWall01"));
				bCmd.Add(new CommandScript.BasicCommand ("EnableGameObject","eventWall02"));
				bCmd.Add(new CommandScript.BasicCommand ("AddMessage","You just touched a trigger box"));
				bCmd.Add(new CommandScript.BasicCommand ("AddMessage","Now There are two invisible walls are created|3"));
				bCmd.Add(new CommandScript.BasicCommand ("AddMessage","This is a sample for the script system|3"));
				bCmd.Add(new CommandScript.BasicCommand ("AddMessage","For details please refer to GameManager|4"));
				bCmd.Add(new CommandScript.BasicCommand ("ShowMessage",""));
				//bCmd.Add(new CommandScript.BasicCommand ("LockCamera",""));
				CommandScript.Get().InterpreteCommands(bCmd);
			}
			break;
		}
	}
	private static void getCoin(int amount)
	// Gain money
	{
		userData.currency +=amount;
		SaveLoad.Get().SaveUserData();
	}
	public static void setCheckPoint()
	// Update check point
	{
		if (MainPlayer == null)
			return;
		MainPlayer.CurHP = MainPlayer.MaxHP;
		checkPointX = MainPlayer.transform.position.x;
		checkPointY = MainPlayer.transform.position.y;
		checkPointZ = MainPlayer.transform.position.z;
		Debug.Log("[Player Check Point Saved]x:" + checkPointX + " y:" + checkPointY + " z:" + checkPointZ);
	}
	public static void KillZone()
	// Teleports the player back to the last check point
	{
		MainPlayer.transform.position = new Vector3(checkPointX ,checkPointY ,checkPointZ);
	}
	public static void GameOver()
	// GameOver and saves player's data
	{
		SaveLoad.Get().SaveUserData();
	}
	//===============[ Static Stuffs End ]=================
	
	
	// A interface to allow user to access the GameManager
	private static GameManager m_Instance = null;
    public static GameManager Get()
    {
        if (m_Instance == null)
            m_Instance = (GameManager)FindObjectOfType(typeof(GameManager));
        return m_Instance;
    }
	private ResourceManager RM;
	public GameObject objPlayer;
	public List<GameObject> objEnemies;
	public float oldTimer = 0;
	public int MaxEnemyQueue = 5;
	public struct EnemyProperties{
		public string Name;
		public bool CanSearch;
		public bool CanMove;
		public float Speed;
		public float TurningSpeed;
		public float EndReachedDistance;
		public float DetectRange;
		public bool CanAttack;
		public int MaxHP;
		public BattleCore.elements Element;
		public List<BattleCore.Factions> FactionSelf;
		public List<BattleCore.Factions> FactionHostile;
		public int BaseArmour;
		public int BaseAttack;
		public float Mass;
		public EnemyProperties(bool initialize){
			Name = "NoName";
			CanSearch = true;
			CanMove = true;
			Speed = 3;
			TurningSpeed = 5;
			EndReachedDistance = 2f;
			DetectRange = 10;
			CanAttack = true;
			MaxHP = 20;
			Element = BattleCore.elements.Earth;
			FactionSelf = new List<BattleCore.Factions>();
			FactionSelf.Add( BattleCore.Factions.alien );
			FactionHostile = new List<BattleCore.Factions>();
			FactionHostile.Add( BattleCore.Factions.human );
			BaseArmour = 0;
			BaseAttack = 1;
			Mass = 3;
		}
	}
	void Awake(){
		RM = ResourceManager.Get();
		if (RM == null){
			print("Game Manager->Resource Manager is missing");
			this.enabled = false;
			return;
		}
		print("Game Initializing");
		objEnemies = new List<GameObject>();
		// Initialize GameObjects
		CreateMenu();
		CreateJoyStick();
		CreateKillZone();
		CreateFPSMonitor();
		//CreateGround();
		//CreatePlayer();
		CreateLight();
		CreateEnemy();
	}
	void CreateMenu(){
		Instantiate(RM.preMenu);
	}
	void CreateJoyStick(){
		Instantiate(RM.preJoyStick);
	}
	void CreateKillZone(){
		Instantiate(RM.preKillZone);
	}
	void CreatePlayer(){
		objPlayer = Instantiate(RM.preMainPlayer,new Vector3(0,1,0),this.transform.rotation) as GameObject;
		if (objPlayer == null){
			print("Game Manager->Failed Initializing Player");
			this.enabled = false;
		}
	}
	void CreateGround(){
		Instantiate(RM.preGround);
	}
	void CreateEnemy(){
		EnemyProperties tEnemyProp = new EnemyProperties(true);
		CreateEnemy(new Vector3(4,1,4),tEnemyProp,Quaternion.identity);
	}
	void CreateEnemy(Vector3 position,EnemyProperties enemyProp,Quaternion rotation){
		GameObject enemy = Instantiate(RM.preEnemyAlien,position, rotation)as GameObject;
		if (enemy != null){
			AIPathCustom ai = enemy.GetComponent<AIPathCustom>();
			ai.ApplyProperties(enemyProp);
		}
		objEnemies.Add(enemy);
	}
	void CreateLight(){
		//Instantiate(RM.preDirectionalLight);
	}
	void CreateFPSMonitor(){
		Instantiate(RM.preFPSMonitor);
	}
	void CreateCurriencies(){
		
	}
	void Update(){
		// Spawn Enemy
		if (objEnemies.Count < 5 && Time.realtimeSinceStartup - oldTimer > 2){
			CreateEnemy(new Vector3(UnityEngine.Random.Range(-4f,4f),1,UnityEngine.Random.Range(-4f,4f)),GameManager.dataManager.Enemies[0],Quaternion.identity);
			oldTimer = Time.realtimeSinceStartup;
		}
	}
}
