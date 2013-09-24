/* ScreenCredits.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Shows our lovely credits
 * */
using UnityEngine;
using System.Collections;

public class ScreenCredits : GUIBase {
	private static ScreenCredits m_Instance = null;
    public static ScreenCredits Get()
    {
        if (m_Instance == null)
            m_Instance = (ScreenCredits)FindObjectOfType(typeof(ScreenCredits));
        return m_Instance;
    }
	
	public Texture texCreditsTitle;
	public Texture Credits;
	public bool show = false;
	public float speed = 10f;
	public float fadeSpeed = 0.05f;
	
	private float offset = 0f;
	private float opacity = 0f;
	private bool interruptSign = false;
	private bool startUp = true;
	private float titleX = 0f;
	float titleSpeed = 2f;
	// Use this for initialization
	void Start () {
		
	}
	protected override void OnDraw ()
	{
		if (Credits == null || !show)
			return;
		if (startUp){
			GUI.DrawTexture(new Rect(titleX,200,400,250),texCreditsTitle);
			return;
		}
		Color t = GUI.color;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,opacity);
		GUI.DrawTexture(new Rect(0,0 - offset,1366,2048),Credits);
		GUI.color = t;
		base.OnDraw ();
	}
	protected override void OnGUI ()
	{
		if (Credits == null || !show)
			return;
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), ResourceManager.Get().tex_Black);
		base.OnGUI ();
	}
	// Update is called once per frame
	void Update () {
		if (Credits == null || !show)
			return;
		if (startUp){
			if (titleX < 383){
				titleSpeed -= 0.05f;
			}
			else if (titleX > 383 && titleX < 583){
				titleSpeed = 0.5f;
			}
			else{
				titleSpeed += 2f;
			}
			titleX += titleSpeed * 800 * Time.deltaTime;
			if (titleX >= 1366)
				startUp = false;
			return;
		}
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || Input.touchCount > 0){
			interruptSign = true;
		}
		offset += speed * Time.deltaTime * 100;
		if (offset >= 2048 - 768)
		{
			offset = 2048 - 768;
			opacity-= fadeSpeed;
			
		}
		else if (interruptSign == true){
			opacity-= fadeSpeed;
		}
		else if (opacity <= 1){
			opacity += fadeSpeed;
			if (opacity > 1)
				opacity = 1;
		}
		if (opacity <= 0){
			offset = 0;
			show = false;
			interruptSign = false;
			startUp = true;
			titleSpeed = 2f;
			titleX = 0;
		}
	}
}
