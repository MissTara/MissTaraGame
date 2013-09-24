/* GUIBaseLoading.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Loading Screen
 * */
using UnityEngine;
using System.Collections;

public class GUIBaseLoading : GUIBase {
	private static GUIBaseLoading m_Instance = null;
    public static GUIBaseLoading Get()
    {
        if (m_Instance == null)
            m_Instance = (GUIBaseLoading)FindObjectOfType(typeof(GUIBaseLoading));
        return m_Instance;
    }
	Texture loadingScreen;
	Texture blackBackground;
	ResourceManager rsManager;
	public bool show = false;
	// Use this for initialization
	void Start () {
		rsManager = ResourceManager.Get();
		if (rsManager != null){
			loadingScreen = rsManager.tex_LoadingScreen;
			blackBackground = rsManager.tex_Black;
		}
		if (loadingScreen == null || blackBackground == null)
			this.enabled = false;
	}
	protected override void OnGUI ()
	// it is not necessary to override this function unless you would like the background to be black
	{
		if (!show)
			return;
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), blackBackground);
		base.OnGUI ();
	}
	protected override void OnDraw ()
	{
		if (loadingScreen)
			GUI.DrawTexture(new Rect(0,0, 1366, 768), loadingScreen);
		base.OnDraw ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
