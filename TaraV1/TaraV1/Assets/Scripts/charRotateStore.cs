/* charRotateStore.cs
 * Author: Graeme MacDonald
 * Last Modified By: Graeme MacDonald
 * Description: 
 * Rotates the character in the shop menu screen.
 * */
using UnityEngine;
using System.Collections;

public class charRotateStore : MonoBehaviour {
	
	private float sensitivityX = 15;
	private float sensitivityY = 15;
	private bool isSliding = false;
	private float xPos = 0;
	
	private Transform referenceCamera;
	
	
	void Start (){
	if (!referenceCamera) {
		if (!Camera.main) {
			Debug.LogError("No Camera with 'Main Camera' as its tag was found. Please either assign a Camera to this script, or change a Camera's tag to 'Main Camera'.");
			Destroy(this);
			return;
		}
		referenceCamera = Camera.main.transform;
	}
}
	
	// Update is called once per frame
	void Update (){
	xPos = Input.mousePosition.x;
	//Get how far the mouse has moved by using the Input.GetAxis().
	float rotationX  = Input.GetAxis("Mouse X") * sensitivityX;
	 //float rotationY = Input.GetAxis("Mouse Y") * sensitivityY;
	
		if(xPos < Screen.width/2){
			isSliding = true;
		transform.RotateAroundLocal( referenceCamera.up	, -Mathf.Deg2Rad * rotationX );
		}else{
			isSliding = false;
			
		}
 }
}