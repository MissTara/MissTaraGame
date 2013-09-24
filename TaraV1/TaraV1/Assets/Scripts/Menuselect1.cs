/* Menuselect1.cs
 * Author: Alfred Lai
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Title Screen Menu
 * */
using UnityEngine;
using System.Collections;

public class Menuselect1 : MonoBehaviour {

    private bool optionOn, toggleBool = false;
    private float hSliderValue = 0.0f;
    Rect rectOptionWin = new Rect(Screen.width / 2 - 250, 50, 700, 300);
    private Rect windowRect = new Rect(Screen.width / 2 - ((Screen.width - 100) /2),
                              Screen.height / 10, Screen.width - 100, Screen.height - 100);
    public GUISkin Taraskin;
	
	
	public bool showingCredits;
    void Start()
    {
        renderer.material.color = Color.white;
    }

    void OnGUI()
    {
        GUI.skin = Taraskin;
        if (optionOn == true)
        {
            windowRect = GUI.Window(0, windowRect, WindowFunction, "Options", "box");
        }
    }

    void WindowFunction(int windowID)
    {
        // Draw any Controls inside the window here
        //change GUI skin to custom one
        GUI.skin = Taraskin;

        //Make first button
        if (GUI.Button(new Rect(50, 200, 200, 80),
                                new GUIContent("Level 1", "Go to Test Level")))
        {
            Application.LoadLevel(1);
        }
        GUI.Label(new Rect(50, 250, 100, 20), GUI.tooltip);

        // Make the menu exit button
        if (GUI.Button(new Rect(windowRect.width / 2 - 100, windowRect.height - 90, 200, 80), "Exit Options"))
        {
            //Application.LoadLevel(2);
            optionOn = false;
        }

        //Slider Bar
        hSliderValue = GUI.HorizontalSlider(new Rect(110, 80, 500, 50), hSliderValue, 0.0f, 1.0f);
        GUI.Label(new Rect(25, 80, 100, 30), "Slider "+ hSliderValue);
        AudioListener.volume = hSliderValue;

        //Toggle Button
        toggleBool = GUI.Toggle(new Rect(25, 120, 100, 30), toggleBool, "Toggle");
    }

	
	// Update is called once per frame
	void Update () {

	}

    void OnMouseEnter()
    {
        renderer.material.color = Color.red;
        //audio.PlayOneShot(sound1);
    }

    void OnMouseExit()
    {
        renderer.material.color = Color.white;
    }

    void OnMouseUp()
    {
		if(ScreenCredits.Get().show)
			return;
        if (this.tag == "StartButton"){
			GUIBaseLoading loading = GUIBaseLoading.Get();
			if (loading != null)
				loading.show = true;
            Application.LoadLevel(1);
		}
        if (this.tag == "Options")
            optionOn = true;
		if (this.name == "menu_Credits"){
			ScreenCredits.Get().show = true;
		}
    }
}
