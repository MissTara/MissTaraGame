/* DebugScreen.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Shows debug data on the screen for mobile devices.
 * */
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class DebugScreen : MonoBehaviour {
    public bool showDebug;
	private static DebugScreen m_Instance = null;
    public static DebugScreen Get()
    {
        if (m_Instance == null)
            m_Instance = (DebugScreen)FindObjectOfType(typeof(DebugScreen));
        return m_Instance;
    }
	List<String> debug_msg;
	// Use this for initialization
	void Awake(){
		debug_msg = new List<string>();
	}
	void Start () {
        showDebug = false;
	}
	void OnGUI(){
        if (showDebug)
        {
            for (int i = 0; i < debug_msg.Count; i++)
            {
                GUI.Label(new Rect(0, 100 + 40 * i, 1000, 40), debug_msg[i]);
            }
        }
	}
	// Update is called once per frame
	void Update () {
	
	}
	public void addMsg(string msg){
		if (debug_msg != null){
			debug_msg.Add(msg);
			if (debug_msg.Count > 10)
				debug_msg.RemoveAt(0);
		}
	}
}
