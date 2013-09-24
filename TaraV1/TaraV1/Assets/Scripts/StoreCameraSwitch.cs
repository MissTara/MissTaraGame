/* StoreCameraSwitch.cs
 * Author: Graeme MacDonald
 * Last Modified By: Graeme MacDonald
 * Description: 
 * Switches camera from the main camera to the shop camera or vice versa.
 * 
 * */
using UnityEngine;
using System.Collections;
 
public class StoreCameraSwitch : MonoBehaviour {
 
	public Camera camera1;
	public Camera camera2;
 
	// Use this for initialization
	void Start () {
		camera1.enabled = true; 
		camera1.GetComponent<AudioListener>().enabled = true;
		camera2.enabled = false; 
		camera2.GetComponent<AudioListener>().enabled = false;
	}
 
	// Update is called once per frame
	void Update () {
		if (MenuMain.Get().show == true)
		{
			camera1.enabled = false;
			camera1.GetComponent<AudioListener>().enabled = false;
			camera2.enabled = true;
			camera2.GetComponent<AudioListener>().enabled = true;
		}
		if (MenuMain.Get().show == false)
		{
			camera1.enabled = true;
			camera1.GetComponent<AudioListener>().enabled = true;
			camera2.enabled = false;
			camera2.GetComponent<AudioListener>().enabled = false;
		}
	}
}