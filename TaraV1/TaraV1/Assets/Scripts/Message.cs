/* Message.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Subtitle system for cut scenes
 * 
 * This script should work with the CommandScript system and the trigger box.
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Message : MonoBehaviour {
	private static Message m_Instance = null;
    public static Message Get()
    {
        if (m_Instance == null)
            m_Instance = (Message)FindObjectOfType(typeof(Message));
        return m_Instance;
    }
	
	public bool autoDisplay = false;
	List<Msg> subtitles;
	public struct Msg{
		public string context;
		public float duration;
		public float x, y, hideTime;
		public Msg(string context, float x, float y, float duration){
			this.context = context;
			this.duration = duration;
			this.x = x;
			this.y = y;
			this.hideTime = Time.realtimeSinceStartup + duration;
		}
		
	}
	public bool debug = false;
	public GUIStyle guiStyle;
	//Rect SubtitleRect = new Rect(50,Screen.height/2 + 150,Screen.width,100);
	int subtitleIndex = 0;
	//Timer for switching subtitles, initial setting
	float subtitleSwitchTimer = 0;
	//bool to determine if the GUI label is drawn
	//bool displaySubtitle = true;
	// Use this for initialization
	void Start(){
		subtitles = new List<Msg>();
		//Adding the strings to the List
		/*
		Msg a = new Msg("Test Subtitle 1",Screen.width / 2, Screen.height / 2,2);
		subtitles.Add(a);
		subtitles.Add(new Msg("Test Subtitle 2",0, 0,2));
		subtitles.Add(new Msg("Test Subtitle 3",0, 0,2));
		subtitles.Add(new Msg("Test Subtitle 4",0, 0,2));
		subtitles.Add(new Msg("Test Subtitle 5",0, 0,2));
		subtitles.Add(new Msg("Test Subtitle 6",0, 0,2));
		
		if (autoDisplay)
			nextMsg();
			*/
	}
	// Update is called once per frame
	void Update () {
		if (debug){
			nextMsg(true);
			debug = false;
		}
		if (subtitles.Count > 0){
			if(subtitleSwitchTimer > 0){
				subtitleSwitchTimer -= Time.deltaTime;
				if(subtitleSwitchTimer <= 0){
					subtitles.RemoveAt(0);
				}
			}
			else{
				if (autoDisplay)
					nextMsg();
			}
		}
		
		
	}
	
	void OnGUI(){
		// boolean to check if the GUI gets drawn
		if(subtitles.Count > 0 && subtitleSwitchTimer > 0){
			//gui label
			GUI.Label(getRectFromMsg(subtitles[0]),subtitles[0].context, guiStyle);
			
		}
	}
	private Rect getRectFromMsg(Msg m){
		GUIContent gui = new GUIContent(m.context);
		Vector2 SubSize = guiStyle.CalcSize(gui);
		return new Rect(m.x,m.y, SubSize.x, SubSize.y);
	}
	public void nextMsg()
	// Jump to the next message
	{
		if (subtitles.Count > 0){
			subtitleSwitchTimer = subtitles[0].duration;
		}
	}
	public void nextMsg(bool a)
	// Jump to the next message
	{
		if (subtitles.Count > 1){
			if (subtitleSwitchTimer > 0)
				subtitles.RemoveAt(0);
			subtitleSwitchTimer = subtitles[0].duration;
		}
	}
	public void clearMsg()
	// Clear the Message Queue
	{
		subtitles.Clear();
	}
	public void addMsg(Msg m)
	// Add a message to the queue
	{
		subtitles.Add(m);
	}
}