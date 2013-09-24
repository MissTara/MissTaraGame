using UnityEngine;
using System;

public class AssetDestuction{
	
private bool hasAppeared = false;
	
	void Start()
    {
        hasAppeared = false;
		
    }
	
	void Update()
    {
		if(Renderer.IsVisible){
			hasAppeared = false;
		}
			
        if(!Renderer.IsVisible){
			Destroy(GameObject);
		}
		
    }
	
	public AssetDestuction (){
		
		
	}
} 


