using UnityEngine;
using System.Collections;

public class GogoDancer : MonoBehaviour {
/* Steven:
 * This script will handle all the movement for the gogo dancers.
 * To do: Before the cutscene (which is auto triggered), have them just playing their dancing animation.
 * Once the cutscene has ended, move them toward the camera (thats where the door is apparently) and switch to running animation.
 * Once they're out of the camera view, remove them.
 * */
	public bool running,started = false;
	
	CharacterController controller;
	void Start () {
		controller = GetComponent<CharacterController>();
	}
	void Update () {
		if(!GameManager.isPaused){
			if(running){
				animation.Play("WolfWalk");
				controller.Move(new Vector3(-0.2f,0.0f,0.0f));
				if(started){
					StartCoroutine("kill");
					started = false;
				}
			}else{
				animation.Play("WolfDance");
			}
		}
	}
	
	public void cutsceneOver(){
		running = true;
		this.transform.Rotate(Vector3.right);
		started = true;
	}
	
	IEnumerator kill(){
		yield return new WaitForSeconds(6.0f);
		Remove();
	}
	void Remove(){
		//Delete the gogo Dancer
		Destroy(this.gameObject);
	}
}
