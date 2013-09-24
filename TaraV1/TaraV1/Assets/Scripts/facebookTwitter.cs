using UnityEngine;
using System.Collections;

public class facebookTwitter : MonoBehaviour {

	public Texture facebookTexture;
	public Texture twitterTexture;

	
	void OnGUI () {
		
		if (GUI.Button (new Rect (750,675,175,175), facebookTexture)) {
			 Application.OpenURL("https://www.facebook.com/misstara.official?ref=br_tf");
		}
		if (GUI.Button (new Rect (950,675,175,175), twitterTexture)) {
			 Application.OpenURL("https://twitter.com/DJMissTara");
		}
	}
}