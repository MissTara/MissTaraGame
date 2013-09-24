//[DEPRECATED]
using UnityEngine;
using System.Collections;
public class script_GUIBase : MonoBehaviour {
	private float originalWidth = 1366f;
	private float originalHeight = 768f;
	private Matrix4x4 originalMatrix;
	private Vector3 scale;
	// Use this for initialization
	private void OnGUI(){
		//BeginJustifyScreenSize();
		OnDraw();
		//EndJustifuScreenSize();
	}
	protected virtual void OnDraw(){
		
	}
	private void BeginJustifyScreenSize(){
		scale = new Vector3();
		scale.x = Screen.width / originalWidth;
		scale.y = Screen.height / originalHeight;
		scale.z = 1;
		originalMatrix = GUI.matrix;
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scale);
		
	}
	private	void EndJustifuScreenSize(){
		GUI.matrix = originalMatrix;
	}
}
