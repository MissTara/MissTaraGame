/* PopoutNum.cs
 * Author: Xi Song(Sissi)
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Pops up numbers wherever we like in different colours.
 * */
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class PopoutNum : MonoBehaviour {
	/*
	public GameObject hitPlayer;
	public int HP;
	public GameObject Indicator = new GameObject();
	public TextMesh textMesh = Indicator.AddComponent<TextMesh>();
	public  MeshRenderer meshRenderer = Indicator.AddComponent<meshRenderer>();
	*/
	private static PopoutNum m_Instance = null;
    public static PopoutNum Get()
    {
        if (m_Instance == null)
            m_Instance = (PopoutNum)FindObjectOfType(typeof(PopoutNum));
        return m_Instance;
    }
	// Luke
	public bool dbg_trigger = false;
	public Transform text3d;
	struct textPopup{
		public string text;
		public float duration;
		public Transform obj;
	}
	List<textPopup> listPopup;
	// Use this for initialization
	void Start () {
		listPopup = new List<textPopup>();
	}
	
	// Update is called once per frame
	void Update () {
		if (dbg_trigger){
			dbg_trigger = false;
			popupText(this.transform.position ,99,2);
		}
		for (int i = 0; i < listPopup.Count;i++){
			textPopup tmp;
			tmp.duration = listPopup[i].duration - Time.deltaTime;
			tmp.text = listPopup[i].text;
			tmp.obj = listPopup[i].obj;
			tmp.obj.transform.renderer.material.color = new Color(
				tmp.obj.transform.renderer.material.color.r,
				tmp.obj.transform.renderer.material.color.g,
				tmp.obj.transform.renderer.material.color.b,
				tmp.obj.transform.renderer.material.color.a * 0.99f);
			listPopup[i] = tmp;
			tmp.obj.transform.Translate((Vector3.up*2*Time.deltaTime));
			if (listPopup[i].duration < 0){
				Destroy(listPopup[i].obj.gameObject);
				listPopup.RemoveAt(i);
				i--;
			}
			
		}
		

	}
	public void popupText(Vector3 pos,int number, int textColor)
	// Pops up a numper at the provided position with the provided colour
	{
		textPopup tmp;
		pos += Vector3.up * 8;
		tmp.text = number.ToString();
		tmp.duration = 1.5f;
		tmp.obj = Instantiate(text3d ,pos, Camera.mainCamera.transform.rotation) as Transform;
		tmp.obj.GetComponent<TextMesh>().text = number.ToString();
		Color color;
		switch(textColor){
		case 0:
			color = Color.magenta;
			break;
		case 1:
			color = Color.green;
			break;
		case 2:
			color = Color.red;
			break;
		case 3:
			color = Color.yellow;
			break;
		case 4:
			color = Color.blue;
			break;
		case 5:
			color = Color.gray;
			break;
		case 6:
			color = Color.white;
			break;
		case 7:
			color = Color.cyan;
			break;
		case 8:
			color = Color.black;
			break;
		default:
			color = Color.black;
			break;
		}
		
		tmp.obj.transform.renderer.material.SetColor("_Color", color);
		listPopup.Add(tmp);

		
	}
}
