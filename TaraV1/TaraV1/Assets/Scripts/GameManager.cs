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
    public static GameObject[] spawns,enemies;
    private static int spawnamount = 1;
    public static float checkPointX = 0f, checkPointY = 0f, checkPointZ = 0f; // Stores the last check point
    private static bool isSpawn, multiSpawn = false;	// for spawning and multispawning
	public static bool _isPaused = false;				// if the game is paused
	public static bool isDebugging = true;				// if the game is on debug mode
	public static bool useGyroScope = true;				// Deprecated
	public static bool useSwipe = true;				// Deprecated
	public static DataManager dataManager;				// Game database
	public static UserData userData;				// Current user data
	public static int enemyCount = 0;				// Holder for camera lock enemy spawn
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
	public static void getItem(int itemID, int spawnNum){
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
		}
	}
	public static void getItem(int itemID, GameObject[] spawner,GameObject[] enemy, int spawnNum){
		/* Steven:
		 * For this section, we'll add in what each trigger does in a comment section.
		 * Include what trigger in which level it is, what's being spawned, 
		 * how many enemies will spawn, and how many spawners used.
		 * If you want to, include the name of the spawner(s) in here.
		 * */
		switch(itemID){
		case 100:					// Load a script which creates two invisible walls
			if (CommandScript.Get() != null){
				List<CommandScript.BasicCommand> bCmd = new List<CommandScript.BasicCommand>();
				//bCmd.Add(new CommandScript.BasicCommand ("EnableGameObject","eventWall01"));
				//bCmd.Add(new CommandScript.BasicCommand ("EnableGameObject","eventWall02"));
				//bCmd.Add(new CommandScript.BasicCommand ("AddMessage","You just touched a trigger box"));
				//bCmd.Add(new CommandScript.BasicCommand ("AddMessage","Now There are two invisible walls are created|3"));
				//bCmd.Add(new CommandScript.BasicCommand ("AddMessage","This is a sample for the script system|3"));
				//bCmd.Add(new CommandScript.BasicCommand ("AddMessage","For details please refer to GameManager|4"));
				//bCmd.Add(new CommandScript.BasicCommand ("ShowMessage",""));
				bCmd.Add(new CommandScript.BasicCommand ("LockCamera",""));
				CommandScript.Get().InterpreteCommands(bCmd);
			}
            isSpawn = true;
			enemies = new GameObject[spawnNum];
			Array.Copy(enemy,enemies,spawnNum);
			spawns = new GameObject[spawner.Length];
			spawns[0] = spawner[0];
			spawnamount = spawnNum;
			break;
			
		case 101:
			isSpawn = true;
			multiSpawn = true;
			enemies = new GameObject[spawnNum];
			Array.Copy(enemy,enemies,spawnNum);
			spawns = new GameObject[spawner.Length];
			spawns[0] = spawner[0];
			spawns[1] = spawner[1];
			spawnamount = spawnNum;
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
			MaxHP = 2;
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
		//CreateEnemy(new Vector3(4,1,4),tEnemyProp,Quaternion.identity);
	}
	void CreateEnemy(Vector3 position,EnemyProperties enemyProp,Quaternion rotation,GameObject mob){
		/* Steven:
		 * Used by update (below) to spawn new enemies.
		 * */
		GameObject enemy;
		if (mob.tag == "Alien")
			enemy = Instantiate(RM.preEnemyAlien,position, rotation)as GameObject;
		else if (mob.tag == "Bat")
			enemy = Instantiate(RM.preEnemyBat,position, rotation)as GameObject;
		else if (mob.tag == "Bear")
			enemy = Instantiate(RM.preEnemyBear,position, rotation)as GameObject;
		else
			enemy = Instantiate(RM.preEnemyBat,position, rotation)as GameObject;
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
		// Spawn Enemy if you're allowed to
		if (objEnemies.Count < spawnamount && Time.realtimeSinceStartup - oldTimer > 1 && isSpawn == true && enemyCount != spawnamount && multiSpawn){
			CreateEnemy(spawns[0].transform.position, GameManager.dataManager.Enemies[0], Quaternion.identity,enemies[enemyCount]);
			enemyCount++;
			if (enemyCount != spawnamount){
				CreateEnemy(spawns[1].transform.position, GameManager.dataManager.Enemies[0], Quaternion.identity,enemies[enemyCount]);
				enemyCount++;
			}			
            oldTimer = Time.realtimeSinceStartup;
		}else if (objEnemies.Count < spawnamount && Time.realtimeSinceStartup - oldTimer > 2 && isSpawn == true && enemyCount != spawnamount){
			//CreateEnemy(new Vector3(UnityEngine.Random.Range(-4f,4f),1,UnityEngine.Random.Range(-4f,4f)),GameManager.dataManager.Enemies[0],Quaternion.identity);
           	CreateEnemy(spawns[0].transform.position, GameManager.dataManager.Enemies[0], Quaternion.identity, enemies[enemyCount]);
			enemyCount++;
            oldTimer = Time.realtimeSinceStartup;
		}
		if (enemyCount == spawnamount && objEnemies.Count == 0 && isSpawn){ 	
			/* Steven:
			 * Check if the max number of enemies have spawned and are all dead
			 * If so, unlock the camera and stop any more spawning*/
			objEnemies.Clear();						
			CommandScript.Get().InterpreteSingle(new CommandScript.BasicCommand ("UnlockCamera",""));
			isSpawn = false;
			if (multiSpawn) multiSpawn = false;
			enemyCount = 0;
		}
	}
}
