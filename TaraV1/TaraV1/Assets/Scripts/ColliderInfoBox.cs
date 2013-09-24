/* ColliderInfoBox.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * The information box which can pops up a 3D text guiding the player.
 * For more, please refer to Game Programming Document(https://docs.google.com/document/d/19QkioYumM3PjzJSFAbHVXL0d5xUcmSsBxOV8StHmEj4/edit?usp=sharing)
 * */
using UnityEngine;
using System.Collections;

public class ColliderInfoBox : MonoBehaviour {
	public MessageInGame msg;
	public bool ExitDisappear = true, visible = true;
	// Use this for initialization
	void Start () {
		if (msg == null){
			this.enabled = false;
		}
		if (!visible){
			MeshRenderer mr = GetComponent<MeshRenderer>();
			if (mr != null)
				mr.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
			msg.Popup();
		}
	}
	void OnTriggerExit(Collider other){
		if (other.tag == "Player" && ExitDisappear){
			msg.Disappear();
		}
	}
}
