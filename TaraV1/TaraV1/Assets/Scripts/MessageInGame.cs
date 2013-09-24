/* MessageInGame.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Floating 3D messages in game.
 * For more, please refer to Game Programming Document(https://docs.google.com/document/d/19QkioYumM3PjzJSFAbHVXL0d5xUcmSsBxOV8StHmEj4/edit?usp=sharing)
 * */
using UnityEngine;
using System.Collections;

public class MessageInGame : MonoBehaviour {
	public float speed = -2f, speedPopup = 4f,speedDisappear = 4f;
	
	public bool isAnimating = true, canDisappear = true;
	public bool isDisappearing = false, isAppearing = false; // Public for debug
	
	public bool autoOffset = true, autoHide = true,autoDisappearOnCollision = true;
	public float autoDisappearSeconds = 0f;	// If it is not 0, it will disappear automatically depends on the value of this variable in seconds.
	private bool isAutoDisappearing = false;
	private float scale=1f;
	public string Text{
		get {
			if (tM != null)
				return tM.text;
			else
				return "";
		}
		set {
			if (tM != null)
				tM.text = value;
		}
	}
	TextMesh tM;
	private float originalHeight = 0f;
	// Use this for initialization
	void Start () {
		originalHeight = transform.position.y;
		tM = GetComponent<TextMesh>();
		if (autoHide){
			transform.localScale = new Vector3(0,0,0);
			scale = 0f;
		}
		else{
			scale = 1f;
		}
		//transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (canDisappear && autoDisappearSeconds != 0f && !isAutoDisappearing && !isAppearing && !isDisappearing && scale > 0){
			isAutoDisappearing = true;
			StartCoroutine(autoDisappear());
		}
		if (isAnimating && !isAppearing && !isDisappearing){
			if (Mathf.Abs( originalHeight - transform.position.y) > 0.8 && Mathf.Abs( originalHeight - transform.position.y - speed) > Mathf.Abs( originalHeight - transform.position.y)){speed *= -1f;}
			float offset = (speed - Mathf.Abs(originalHeight - transform.position.y) * (speed / Mathf.Abs(speed)));

			if (autoOffset)
				offset += 0.01f * Random.Range(-10f,10f) * offset;
			//print("3DText Offset:" + offset);
			transform.position = new Vector3(transform.position.x,transform.position.y + offset * Time.deltaTime,transform.position.z);
		}
		if (isAppearing){
			if (scale < 1){
				scale += speedPopup * Time.deltaTime;
				if (scale > 1)
					scale = 1;
				transform.localScale = new Vector3(scale,scale,scale);
			}
			else{
				isAppearing = false;
			}
		}
		if (isDisappearing){
			if (scale > 0){
				scale -= speedDisappear * Time.deltaTime;
				if (scale < 0) 
					scale = 0;
				transform.localScale = new Vector3(scale,scale,scale);
			}
			else{
				isDisappearing = false;
				
				
			}
		}
	}
	public void Popup(){
		isAppearing = true;
		isDisappearing = false;
	}
	public void Disappear(){
		if (!canDisappear)
			return;
		isDisappearing = true;
		isAppearing = false;
	}
	private IEnumerator autoDisappear(){
		
		yield return new WaitForSeconds(autoDisappearSeconds);
		isDisappearing = true;
		isAutoDisappearing = false;
	}
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player" && autoDisappearOnCollision){
			isDisappearing = true;
		}
	}
}
