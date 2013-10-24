// [DEPRECATED]
using UnityEngine;
using System.Collections;

public class AIEnemy : MonoBehaviour {
	protected CharacterController controller;		// Character Controller
	private Vector3 move = Vector3.zero;
	private float lastUPD = 0f;
	public bool Running = false;
	public bool canAttack = false;
	public float atkDuration = 2f;
	public float atkWait = 2f;
	public Transform dbg_AttackParticle;
	private float atkStartTime = 0;
	private float atkDealTime = 0;
	private bool isAttacking = false;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		if (!controller)
		{
			Debug.LogError("controller.Start() " + name + "has no character controller.");
			enabled = false;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.isPaused)
			return;
		if (canAttack){
			if (!isAttacking && Time.realtimeSinceStartup - atkStartTime > atkWait + atkDuration){
				isAttacking = true;
				atkStartTime = Time.realtimeSinceStartup;
				if (dbg_AttackParticle != null)
					Instantiate(dbg_AttackParticle,transform.position,transform.rotation);
			}
			if (Time.realtimeSinceStartup - atkStartTime >= atkDuration){
				isAttacking = false;
			}
		}
		
		if(!Running)
			return;
		float t = Time.realtimeSinceStartup;
		if (t - lastUPD > 2){
			GenenrateDir();
			lastUPD = t;
		}
		controller.Move(move * Time.deltaTime);
	}
	void GenenrateDir(){
		move = new Vector3(Random.Range(-5f,5f),0f,Random.Range(-5f,5f));
	}
	public void OnWeaponStay(Collider other){
		if (!canAttack)
			return;
		UnitPlayer pProp = other.GetComponent<UnitPlayer>();
		if (isAttacking && Time.realtimeSinceStartup - atkDealTime > 0.5f){
			if (pProp != null){
				pProp.CurHP -= 1;
				//pProp.hurt();
			}
			atkDealTime = Time.realtimeSinceStartup;
		}
	}

}
