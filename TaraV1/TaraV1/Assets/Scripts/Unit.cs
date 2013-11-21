/* Unit.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * The base class for UnitPlayer.cs
 * Provides basic switches and behavoiurs for a unit.
 * 
 * */
using UnityEngine;
using System.Collections;
[RequireComponent(typeof(CharacterController))]
public class Unit : MonoBehaviour {
	// Switches & Constants
	public bool 	canWalk 		= true;				// If the unit can walk
	public bool 	canRun 			= true;				// If the unit can run
	public bool 	canJump 		= true;				// If the unit can jump
	public bool 	canAttack		= true;				// If the unit can attack
	public bool 	canBeAttacked	= true;				// If the unit can be attacked
	public bool 	dead			= false;			// If the unit is dead
	public float 		MaxHP			= 10;				// The maximum HP of the unit
	public float speedMove 		= 10f;				// The basic move speed of the unit
	public float speedRun 		= 20f;				// The running speed of the unit
	public float speedJump 		= 10f;				// The jump height(speed) of the unit
	public float speedTurn		= 8f;
	// Particles Deprecated
	public Transform particleAttack;
	public Transform particleBeingAttacked;
	public Transform particleDestoried;
	
	protected AudioClip audioJump,audioRun;
	// Unit Data
	public ItemWeapon _Weapon;
	public ItemWeapon Weapon{
		get { return _Weapon; }
		set { _Weapon = value; }
	}
	public ItemArmour Armour;
	public float _CurHP		= 0;				// The default current HP which is equal to the maximum
    public float CurHP
    {								// The current HP which should between 0 and the maximum HP inclusive
		get {return _CurHP;}
		set	{
			if (value < 0)
				_CurHP = 0;
			else if (value > MaxHP)
				_CurHP = MaxHP;
			else
				_CurHP = value;
		}
	}
	public int Damage{
		get {
			if (Weapon != null)
				return _BaseDamage + Weapon.BaseDamage;
			return _BaseDamage;
		}
	}
	public int BaseDamage{
		get {return _BaseDamage; }
		set {_BaseDamage = value;}
	}

    

    public void gainHealth(float gainAmountOfHealth)
    {
        if (this._CurHP + gainAmountOfHealth > this.MaxHP)
        {
            float healhToMax = this.MaxHP - gainAmountOfHealth;
            this._CurHP += healhToMax;
        }
        else
        {
            this._CurHP += gainAmountOfHealth;
        }
    }
	protected int _BaseDamage 	= 1;
	private float lastAttackTime;
	
	
	// Character State
	protected bool jump 		= false;			// The status of jump	[Trigger]
	protected bool run 			= false;			// The status of run	[Trigger]
	
	// Physics & Movement
	protected Vector3 gravity = Vector3.zero ;		// The gravity
	protected CharacterController controller;		// Character Controller
	protected Vector3 move = Vector3.zero;			// The movement vector
	protected Vector3 targetForward = Vector3.zero; 
	protected float targetDegree = 0;
	protected bool deadAlready = false;
 protected	float CalculateDegree(float x, float z){
		if (x == 0 && z == 0)
			return 0;
		if (x == 0 && z > 0)
			return Mathf.PI * 0.5f;
		if (x == 0 && z < 0)
			return Mathf.PI * 1.5f;
		if (z == 0 && x > 0)
			return 0;
		if (z == 0 && x < 0)
			return Mathf.PI;
		if (x > 0)
			return Mathf.Atan(z / x);
		else
			return Mathf.Atan(z / x) + Mathf.PI;
	}
	
	// Use this for initialization
	public virtual void Start () {
		
		controller = GetComponent<CharacterController>();
		
		if (!controller)
		{
			Debug.LogError("Unit.Start() " + name + "has no character controller.");
			enabled = false;
		}
		// Load Resources
		
		// Initialize Data
		_CurHP = MaxHP;
		Weapon = new ItemWeapon(BattleCore.elements.None,1,1f);
		Armour = new ItemArmour(BattleCore.elements.None,0);
		targetForward = this.transform.forward;
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
		// Stop updating while pausing
		if (GameManager.isPaused)
			return;
		if (dead && !deadAlready)
			die ();
		else if (!dead && deadAlready)
			revive();
		// Determine speed
		if (run && canRun && controller.isGrounded)
			move *= speedRun;
		else
			move *= speedMove;
		if (!controller.isGrounded && this.tag != "Bat"){
			move *= 0.8f;
			gravity += Physics.gravity * Time.deltaTime;
		}
		else {
			move.y = -controller.stepOffset / Time.deltaTime;
			gravity = Vector3.zero;
			if (jump && canJump){
				if (audioJump != null)
					audio.PlayOneShot(audioJump);
				move.y = 0;
				gravity.y = speedJump;
				jump = false;
			}
		}
		if(this.tag == "Bat" && GetComponent<AIStates>().swoop){
			move = Vector3.zero;
		}else
			move += gravity;
		controller.Move(move * Time.deltaTime);
		
	}
	public bool Attack(Vector3 contactPoint){
		float now = Time.realtimeSinceStartup;										// Get the current time
		//if (canAttack && now - lastAttackTime > 1.2f){									// If time span more than the attack interval
			lastAttackTime = now;													// Update the lastAttackTime
			Instantiate(particleAttack, contactPoint,transform.rotation);		// Create attack particle
		//print("Attack" +Time.realtimeSinceStartup);
			return true;
		//}
		return false;
	}
	public void BeingAttacked(Unit Attacker){
		CurHP -= 1; // Place Holder

        AudioClip atkOn = ResourceManager.Get().se_AttackOn;
			if (atkOn != null)
				audio.PlayOneShot(atkOn);
		Instantiate(particleBeingAttacked, transform.position,transform.rotation);
		if (CurHP == 0 && !dead){
			dead = true;
		}
	}
	protected virtual void die(){
		print("UnitDie");
		deadAlready = true;
		CurHP = 0;
		if (particleDestoried){
			Transform tempParticle;
			tempParticle = Instantiate(particleDestoried, transform.position,transform.rotation) as Transform;
			tempParticle.parent = transform;
		}
		AIEnemy AI = GetComponent<AIEnemy>();
		
		if (AI != null)
			Destroy(AI);
		controller.enabled = false;
	}
	protected virtual void revive(){
		// Reset
	}
}
