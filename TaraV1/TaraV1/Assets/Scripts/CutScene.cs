/* CutScene.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Plays slides as cut scenes.
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutScene : GUIBase {
	private static CutScene m_Instance = null;
    
	public static CutScene Get(){
        if (m_Instance == null)
            m_Instance = (CutScene)FindObjectOfType(typeof(CutScene));
        return m_Instance;
    }
	
    public bool isShow = false;
	public float transitionLength = 1;		// The speed of the transition
	// Standard slide structure
    public struct Slide {
        public int ID;			// Slide ID
        public float duration;	// How long will the slide be show
    }
    public List<Slide> Slides;	// The slide queue
	private float tmpTransTime;
	// Use this for initialization
	void Start () {
		tmpTransTime = transitionLength;
        Slides = new List<Slide>();
		dbg_LoadCutScenes(0,2);			// [DEBUG ONLY]Load sample cut scenes
	}
    protected override void OnDraw()
    {
        if (!isShow || Slides.Count <=0)
            return;
        Texture curSlide = ResourceManager.Get().tex_Slides[Slides[0].ID];
        if (curSlide != null){
			if (Slides[0].duration <=0){
				Color oriColor = GUI.color;
				GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b, 1 - (-Slides[0].duration) / transitionLength);
				GUI.DrawTexture(new Rect(0, 0, 1366, 768), curSlide);
				GUI.color = oriColor;
			}
			else{
				Color oriColor = GUI.color;
				GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b, 1 - tmpTransTime / transitionLength);
            	GUI.DrawTexture(new Rect(0, 0, 1366, 768), curSlide);
				GUI.color = oriColor;
			}
		}
        else
            print("CutScene->Slide is missing. ID:" + Slides[0].ID);
		if (GUI.Button(new Rect(20,20,150,80),"Skip")){
			CutSceneDispose();
		}
        base.OnDraw();
    }
    protected override void OnGUI()
    {
        if (isShow)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), ResourceManager.Get().tex_Black);
        base.OnGUI();
    }
	// Update is called once per frame
	void Update () {
        if (Slides == null || Slides.Count <= 0){
            isShow = false;
		}
        if (!isShow)
            return;
		else
			GameManager.isPaused = true;
        if (Slides.Count > 0)
        {
			if (tmpTransTime > 0){
				tmpTransTime -= Time.deltaTime;
				if (tmpTransTime < 0)
					tmpTransTime = 0;
			}
			else{
	            Slide tmpSlide;
	            tmpSlide.duration = Slides[0].duration - Time.deltaTime;
	            tmpSlide.ID = Slides[0].ID;
	            if (tmpSlide.duration <= -transitionLength){
	                Slides.RemoveAt(0);
					tmpTransTime = transitionLength;
					if (Slides.Count <= 0)
						CutSceneDispose();
				}
	            else
	                Slides[0] = tmpSlide;
			}
        }
	}
    public void AddCutScene(int ID, float duration) 
	// Add a cut scene
	// The ID should match the ID of cutscene in the ResourceManager
	{
        Slide tmpSlide;
        tmpSlide.ID = ID;
        tmpSlide.duration = duration;
        Slides.Add(tmpSlide);
    }
	private void CutSceneDispose(){
		tmpTransTime = transitionLength;
		Slides.Clear();
		isShow = false;
		GameManager.isPaused = false;
	}
	public void dbg_LoadCutScenes(int from, int to, int duration = 3)
	// For debug only
	{
        for (int it = from; it <= to; it++)
        {
            AddCutScene(it, duration);
        }
		isShow = true;
	}

}
