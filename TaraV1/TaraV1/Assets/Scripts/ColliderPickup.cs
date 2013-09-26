/* ColliderPickup.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * This script is for pickup items in the game such as notes(Currencies)and loots.
 * It also implements special stuffs like:
 * Trigger box: Triggers an event in the GameManager while collide with player.
 * Kill zone: a layer below the ground to prevent player from falling off the ground. If a player does, 
 * it will respawn them at the proper area. 
 * Check Point(Deprecated): Player will respawn at the check point after death or touches the killzone.
 * 
 * For more, please refer to Game Programming Document(https://docs.google.com/document/d/19QkioYumM3PjzJSFAbHVXL0d5xUcmSsBxOV8StHmEj4/edit?usp=sharing)
 * */
using System;
using UnityEngine;
public class ColliderPickup : MonoBehaviour
{
	
	public Transform particlePickedup;	// The particle effect plays while it is triggered
	public Transform particleStandBy;	// The default particle effect that keeps playing while stading by
	public bool StaticItem = false;		// A static item does not disappear after interacting with the player such as killzone
	public bool Magnetic = false;		// A magnetic item will come to player when they are close enough [This function requires the Magnetic Collider on the player]
	public bool AutoRotate = false;		// The item will rotates automatically while standing by
	public int ItemID = 1;				// This determines which event it triggers in the GameManager
	public bool _Active = true;			// When the item is not active, it will not show and interact with anything
	public bool playPickupSE = true;	// If it is true, a Sound Effect(if provided) will be played when interacting with the player
	public AudioClip audioPickupSE;		// The Sound Effect for pick up
	
	public GameObject spawner;
	public int spawnNum;
	public bool Active{
		set { 
			_Active = value; 
			MeshRenderer tmpMesh = GetComponent<MeshRenderer>();
			if (tmpMesh)
				tmpMesh.enabled = value;
		}
		get { return _Active; }
	}
	private bool Activated = false;
	private float degree = 0f;
	private bool isDestorying = false;
	void Start(){
		if (particleStandBy != null)
			Instantiate(particleStandBy, this.transform.position, this.transform.rotation);
	}
	void OnTriggerEnter(Collider other){
		if (!Active)
			return;
		DebugScreen.Get().addMsg( this.name + " | " +  other.tag);
		if (other.tag == "Player"){
			PickUp();
		}
		
		
	}
	void OnTriggerStay(Collider other){
		if (Magnetic && other.tag == "MagCollider"){
			this.transform.position = Vector3.MoveTowards(this.transform.position, other.transform.position,20* Time.deltaTime) ;
		}
	}
	void Update(){
		if (!Active)
			return;
		if (AutoRotate){
			if(degree > 720)
				degree = 0;
			degree += 120 * Time.deltaTime;
			transform.rotation = Quaternion.Euler(new Vector3(0,degree,0));
		}
	}
	void PickUp(){
		DebugScreen.Get().addMsg("Picked Up");
		if (isDestorying && !StaticItem)
			return;
		isDestorying = true;
		if (ItemID < 100)
		     GameManager.getItem(ItemID,null,spawnNum);
		else
		     GameManager.getItem(ItemID,spawner,spawnNum);	

		if (!Activated){
			if (particlePickedup != null)
				Instantiate(particlePickedup, this.transform.position, this.transform.rotation);
					if (audioPickupSE != null && playPickupSE){
			audio.PlayOneShot(audioPickupSE);
		}
			Activated = true;
		}
		
		if (!StaticItem){
			renderer.enabled = false;
			Destroy (gameObject, audioPickupSE.length);
		}
		
	}
}