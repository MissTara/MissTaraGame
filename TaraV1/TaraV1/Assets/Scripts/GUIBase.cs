/* script_GUIBase.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * This class adjusts 2D drawing stuffs to any screen sizes and ratios.
 * It will adjust all drawings into the ratio of 16:9.
 * Please regard the screen resolution as 1366*768 pixels while drawing 
 * things in the class which derives this class.
 * 
 * IMPORTANT: You must derive this class if you are going to draw 
 * using GUI.xxx unless you have special adjustments for the coordinates. 
 * OR it will not be friendly to different screen sizes.
 * 
 * How to use this class?
 * 1. Derive from this class e.g. public class Sample : GUIBase
 * 2. Override the function OnDraw and write your GUI.xxx stuffs within the OnDraw() function.
 * 3. if you would like to have a black background override OnGUI and use the sample code in GUIBaseLoading.
 * 
 * GUIBaseLoading is the sample of using this class.
 * */
using UnityEngine;
using System.Collections;

public class GUIBase : MonoBehaviour {
	private float originalWidth = 1366f;
	private float originalHeight = 768f;
	private Matrix4x4 originalMatrix;
	private Vector3 scale;
	// Use this for initialization
	protected virtual void OnGUI(){
		
		BeginJustifyScreenSize();
		OnDraw();
		EndJustifuScreenSize();
	}
	protected virtual void OnDraw(){
		
	}
	private void BeginJustifyScreenSize(){
		scale = new Vector3();
		float screenWidth = Screen.width,screenHeight = Screen.height;
		Vector2 tSize = fitSize(new Vector2(screenWidth, screenHeight));
		screenWidth = tSize.x;
		screenHeight = tSize.y;
		scale.x = screenWidth / originalWidth;
		scale.y = screenHeight / originalHeight;
		scale.z = 1;
		originalMatrix = GUI.matrix;
		Vector3 tVect3 = new Vector3((Screen.width - screenWidth) / 2,(Screen.height - screenHeight) / 2,0f);
		GUI.matrix = Matrix4x4.TRS (tVect3, Quaternion.identity, scale);
		
	}
	private Vector2 fitSize(Vector2 orisize){
		float newX = orisize.x,newY = orisize.x / 16 * 9;
		if (newX > orisize.x || newY > orisize.y){
			newX = orisize.y / 9 * 16;
			newY = orisize.y;
		}
		return new Vector2(newX, newY);
	}
	private	void EndJustifuScreenSize(){
		GUI.matrix = originalMatrix;
	}
}
