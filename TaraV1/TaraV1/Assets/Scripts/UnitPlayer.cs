/* UnitPlayer.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Main Player Class derived from Unit Calss.
 * Handles basic behaviours of the player. 
 * e.g. React to user inputs & play animations
 * 
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UnitPlayer : Unit,ICombat {
	// Not used
    public enum states {
        Idle1,
        Idle2,
        Sprint,
        Run, 
        Attack1,
        Attack1Combo,
        Attack2,
        Attack2Combo,
        AttackComboLoop,
        AttackComboEnd
    };
    public states PlayerState = states.Idle1;
	private static UnitPlayer m_Instance = null;
    public static UnitPlayer Get()
    {
        if (m_Instance == null)
            m_Instance = (UnitPlayer)FindObjectOfType(typeof(UnitPlayer));
        return m_Instance;
    }
	public bool 	isControllable 	= true;				// If the unit is controllable
	[HideInInspector]
	public float attackAnimationTime = 1.2f;
	[HideInInspector]
	public bool _isAttacking = false;
	public bool isAttacking{
		get { return _isAttacking; }
		set { _isAttacking = value; }
	}
	public List<BattleCore.Factions> _factionSelf;
	public List<BattleCore.Factions> _factionHostile;
	public List<BattleCore.Factions> FactionSelf{
		get { return _factionSelf; }
		set { _factionSelf = value; }
	}
	public List<BattleCore.Factions> FactionHostile{
		get { return _factionHostile; }
		set { _factionHostile = value;}
	}
	public Transform particleJump;   //Deprecated
	public Transform playerAvatar;
	public GameObject playerMesh;
	public float speedMomentumDecay = 0.9f;
	private Animator animator;
	private float attackTriggerTime;
	float tmpForwardX = 0, tmpForwardZ = 1;
	float tmpForwardDegree = 0;
	float offsetHor = 0;			// Get Move Offset X
	float offsetVer = 0;
	float transparency = 0;
	
    // Combo System
    private int comboCount = 0;
    private float lastComboDelay = 0;
    private string callBackAnimation = "";
	private string callBackRun = "";
    private bool atkButtonDown = false;
	public ColliderWeapon wepFistRight, wepFistLeft, wepHead;
	// Use this for initialization
	void Awake(){
		GameManager.isPaused = false;
		GameManager.MainPlayer = this;
		GameManager.Get().objPlayer = this.gameObject;
	}
	public override void Start () {
		animator = playerAvatar.GetComponent<Animator>();
		audioJump = ResourceManager.Get().se_PlayerJump;
        
		base.Start();
	}
	
	public override void Update () {
		if (dead && !CameraController.Get().fadeOut)
			Application.LoadLevel(0);
		if (GameManager.isPaused)
			return;
		// Stop updating while pausing
		if (GameManager.isPaused)
			return;
		if (isControllable){
			UpdateControllerInput();
			UpdatePlayerRotation();
			UpdatePlayerMovement();
			UpdatePlayerAttack();
        }
		UpdateShooting();
        UpdateState();
        UpdateAnimation();
		base.Update();
	}
	private void UpdateControllerInput(){
		if (controller.isGrounded){
			offsetHor = script_vcontroller.Get().getOffsetHorizontal();			// Get Move Offset X
			offsetVer = script_vcontroller.Get().getOffsetVertical();			// Get Move Offset Y
		}
		else{
			offsetHor = script_vcontroller.Get().getOffsetHorizontal() != 0 ? script_vcontroller.Get().getOffsetHorizontal() : offsetHor;			// Get Move Offset X
			offsetVer = script_vcontroller.Get().getOffsetVertical() != 0 ? script_vcontroller.Get().getOffsetVertical() : offsetVer;			// Get Move Offset Y
			if (speedMomentumDecay > 1 || speedMomentumDecay <= 0)
				speedMomentumDecay = 1;
			offsetHor *= speedMomentumDecay;
			offsetVer *= speedMomentumDecay;

		}
		if (canWalk)
			move = new Vector3(offsetHor, 0f, offsetVer);
		else
			move = Vector3.zero;
		move.Normalize();
		move =  Camera.mainCamera.transform.TransformDirection(move);
		GameObject cameraSelf = GameObject.Find("CameraSelf");
		cameraSelf.transform.TransformDirection(move);
		move.y = 0;
	}
	private void UpdatePlayerRotation(){
		if (move != Vector3.zero)
			targetForward = new Vector3(move.x, 0f, move.z);
		tmpForwardDegree = Mathf.LerpAngle(CalculateDegree(transform.forward.x,transform.forward.z) / Mathf.PI * 180,CalculateDegree (targetForward.x, targetForward.z) / Mathf.PI * 180,5 *  	Time.deltaTime);
		tmpForwardDegree = tmpForwardDegree / 180 * Mathf.PI;
		transform.forward = new Vector3(Mathf.Cos(tmpForwardDegree) , 0f,  Mathf.Sin(tmpForwardDegree));
	}
	private void UpdatePlayerMovement(){
		// Movement
		// Disabled Jump
		if (false && (Input.GetKeyDown(KeyCode.Space) || script_vcontroller.isJump()) && controller.isGrounded){
			jump = false;
			if (particleJump != null)
				Instantiate(particleJump,new Vector3(transform.position.x,transform.position.y - 2,transform.position.z), new Quaternion(0,180,0,0));
		}
		run = Input.GetKey(KeyCode.LeftShift);
	}
    private bool isPlayingAttackAnimation() {
        return playerMesh.animation.IsPlaying("Swipe1") || playerMesh.animation.IsPlaying("Swipe2") || playerMesh.animation.IsPlaying("SwipeCombo1") || playerMesh.animation.IsPlaying("SwipeCombo2") || playerMesh.animation.IsPlaying("SwipeComboEnd") || playerMesh.animation.IsPlaying("SwipeComboLoop") || playerMesh.animation.IsPlaying("SwipeCombo3End");
    }
	private void UpdatePlayerAttack(){
        if (!isPlayingAttackAnimation())
        {
            _isAttacking = false;
            lastComboDelay += Time.deltaTime;
            if ((Input.GetKey(KeyCode.Z) || script_vcontroller.isATK()) && !atkButtonDown)
            {
                atkButtonDown = true;
                lastComboDelay = 0;
                switch (comboCount)
                {
                    case 0:
                        playerMesh.animation.Play("Swipe1");
                        break;
                    case 1:
                        playerMesh.animation.Play("SwipeCombo1");
                        callBackAnimation = "Swipe2";
                        break;
                    case 2:
                        playerMesh.animation.Play("SwipeCombo2");
                        callBackAnimation = "SwipeComboEnd";
                        break;
                    case 3:
                        playerMesh.animation.Play("SwipeComboLoop");
                        callBackAnimation = "Swipe2";
                        break;
                    default:
                        if (comboCount % 2 == 0)
                        {
                            playerMesh.animation.Play("SwipeCombo2");
                            callBackAnimation = "SwipeComboEnd";
                        }
                        else
                        {
                            playerMesh.animation.Play("SwipeComboLoop");
                            callBackAnimation = "Swipe2";
                        }
                        break;
                }
                comboCount++;
            }
        }
        else
            _isAttacking = true;
		if (playerMesh.animation.IsPlaying("SwipeComboEnd")){
		}
		else
			wepHead.enabled = false;
		
        if (!(Input.GetKey(KeyCode.Z) || script_vcontroller.isATK()))
            atkButtonDown = false;
        if (lastComboDelay > 0.1f)
        {
            comboCount = 0;
            if (callBackAnimation != "")
            {
                playerMesh.animation.Play(callBackAnimation);
                callBackAnimation = "";
            }
        }
	}
    private void FixedUpdate() {
        
    }
    private void UpdateState() {
        if (isAttacking)
            PlayerState = states.Idle2;
    }
	private void UpdateAnimation(){
        if (move != Vector3.zero)
        {
            if (!isPlayingAttackAnimation() && callBackAnimation == "") {
                playerMesh.animation.Play("Run");
            }
        }
        else if (!isPlayingAttackAnimation() && callBackAnimation == "")
        {
            playerMesh.animation.Play("Idle");
        }
		if (callBackRun != ""){
			playerMesh.animation.Play(callBackRun);
			callBackRun = "";
		}
	}
	public void hurt(ItemWeapon weapon)
	// When player get hurt
	{
		int BaseAttack = weapon.BaseDamage;
		BattleCore.elements Ele = weapon.Element;
		PopoutNum pop = PopoutNum.Get();
		int Damage = BattleCore.CalculateDamage(BaseAttack,Ele,this.Armour.BaseArmour,this.Armour.Element);
		if (pop != null){
			pop.popupText(this.transform.position,Damage, (int)Ele);
		}
		CurHP-= Damage;
		transparency = 0.5f;
		if (CurHP == 0){
			// Death
			//animator.SetBool("Attack", false);
			//animator.SetBool("idle2ToDeath",true);
			GameManager.GameOver();
			CameraController.Get().fadeIn = false;
			CameraController.Get().fadeOut = true;
			GameManager.isPaused = true;
			dead = true;
		}
	}
	void OnGUI(){
		if (GameManager.isPaused)
			return;
		if (transparency > 0){
			GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,transparency) ;
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), ResourceManager.Get().tex_HurtScreen);
			transparency = Mathf.Max(0, transparency - 0.05f);
		}
	}
	protected override void die ()
	{
		
		print("Player Die");
	}
	public void AddImpact(Vector3 dir, float force){}
	public void onAttack(Vector3 contactPoint){
		base.Attack(contactPoint);
	}
	public void UpdateShooting()
	// Shoot System
	{
		if ((Input.GetKeyDown(KeyCode.Space) || script_vcontroller.isJump())){
			GameObject bullet = Instantiate(ResourceManager.Get().preBullet,this.transform.position + this.transform.TransformDirection(Vector3.up * 3) + this.transform.TransformDirection(Vector3.forward * 3), Quaternion.identity) as GameObject;
			if (bullet != null){
				ColliderProjectile projectile = bullet.GetComponent<ColliderProjectile>();
				if (projectile != null){
					if (targetForward != Vector3.zero)
						projectile.gameObject.transform.forward = targetForward;
					else
						projectile.gameObject.transform.forward = this.transform.forward;
					projectile.Activate(60,505);
					//rigidbody.velocity = transform.forward * 2000;
				}
			}
		}
	}
}
