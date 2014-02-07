/* CameraController.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Controls the camera
 * 
 * There are many redundant codes in this script.
 * */
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Transform 	cameraTarget;
	public Transform 	cameraSelf;
	public float		targetHeight		= 1.0f;
	public LayerMask	collisionLayers		= -1;
	public float		distance			= 8.0f;
	public float		xSpeed				= 80.0f;
	public float		ySpeed				= 120.0f;
	public float		yMinLimit			= -12f;
	public float		yMaxLimit			= 30f;
	public float		rotationSpeed		= 3.0f;
	public float		zoomMinLimit		= 0f;
	public float		zoomMaxLimit		= 6f;
	public float		zoomDampening		= 5.0f;
	public float		offsetFromWall		= 0.1f;
	public bool 		Locked = false;
	private Vector3 oriRotation;
	/* Camera Fading */
	public bool fadeIn = false, fadeOut = false;
	public float fadeSpeed = 1f;
	private float fadeIdentifier = 1;
	private float fadeAlpha = 1;
	private bool isFading{
		get { return fadeAlpha != 0 && fadeAlpha != 1;}
	}
	private Rect fadeBox = new Rect(0,0,Screen.width,Screen.height);
	/*===============*/

    public float oldZ;
	public Vector3 dstPos;

	public float x = 0.0f;
	public float y = 0.0f;
	public bool debug = false;
	private float currentDistance;
	private float desiredDistance;
	private float correctedDistance;
	//float offsetHor = 0;			// Get Move Offset X
	//float offsetVer = 0;
	Vector2 offsetVector = Vector2.zero;
	private static CameraController m_Instance = null;
    public static CameraController Get()
    {
        if (m_Instance == null)
            m_Instance = (CameraController)FindObjectOfType(typeof(CameraController));
        return m_Instance;
    }
	// Use this for initialization
	void Start () {
		//cameraTarget = GameManager.Get().objPlayer.transform;
        oldZ = cameraSelf.position.z;
		Vector2 angles = cameraTarget.eulerAngles;
		x = angles.y - 30;
		y = angles.x;
		currentDistance = distance;
		desiredDistance = distance;
		correctedDistance = distance;
		oriRotation = this.transform.forward;
		fadeIn = true;
        fadeOut = false;
	}
	void OnGUI(){
		FadingDraw();
	}
	void FadingUpdate(){
		if (isFading || fadeIn || fadeOut){
			if (fadeIn)
				fadeAlpha -= (fadeSpeed * fadeIdentifier) * Time.deltaTime;
			else
				fadeAlpha += (fadeSpeed * fadeIdentifier) * Time.deltaTime;
			if (fadeAlpha > 1){
				fadeAlpha = 1;
				fadeIn = false;
				fadeOut = false;
			}
			else if(fadeAlpha  < 0){
				fadeAlpha = 0;
				fadeIn = false;
				fadeOut = false;
			}
		}
	}
	void FadingDraw(){
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,fadeAlpha);
		GUI.DrawTexture(fadeBox, ResourceManager.Get().tex_Black);
	}
	void Update(){
		FadingUpdate();
		if (GameManager.isPaused)
			return;
        if (Input.GetKey(KeyCode.I)){
            fadeIn = true;
            fadeOut = false;
        }
        else if (Input.GetKey(KeyCode.O)){
            fadeIn = false;
            fadeOut = true;
        }
	}
	// Update is called once per frame
	void LateUpdate () {
        if (LevelLoader.Get().boolSetNewLevel == false)
            oldZ = cameraSelf.position.z;

		if (GameManager.isPaused && LevelLoader.Get().boolSetNewLevel == true)
			return;
		//if(LevelLoader.Get().bossLevel)
			//dstPos = new Vector3(cameraSelf.position.x,GameObject.Find("bossView").transform.position.y,cameraTarget.position.z);
		//else
			dstPos = new Vector3(cameraSelf.position.x,cameraTarget.position.y,cameraTarget.position.z);
		
		if (Vector3.Distance(cameraSelf.position, dstPos) > 0.5f && !Locked)
        {
			if(LevelLoader.Get().bossLevel){ 
				if((cameraTarget.position.z - oldZ) < 0.0f){
					//GameObject.Find("bossView").transform.position += new Vector3(0.0f, 0.0f,-Time.deltaTime * 2);
					cameraSelf.position = Vector3.Lerp(cameraSelf.position, dstPos, Time.deltaTime * 5);
				}else if ((cameraTarget.position.z - oldZ) > 0.0f){
//					GameObject.Find("bossView").transform.position += new Vector3(0.0f, 0.0f,Time.deltaTime * 2);
            		cameraSelf.position = Vector3.Lerp(cameraSelf.position, dstPos, Time.deltaTime * 5);
				}
			}else{
				cameraSelf.position = Vector3.Lerp(cameraSelf.position, dstPos, Time.deltaTime * 10);
			}
            if (!GameManager.isPaused && (cameraTarget.position.z - oldZ) < 0.0f && !Locked && !LevelLoader.Get().boolSetNewLevel){
				// If the camera has moved, move the player boundary with it, but stop if the camera gets locked
                if (LevelLoader.Get().collideWall != null && (cameraTarget.position.z - oldZ) < 0.0f){
                    ((Wall)(LevelLoader.Get().collideWall.GetComponent(typeof(Wall)))).Move(oldZ);
                }
            }
			if(!LevelLoader.Get().bossLevel){
	            if (cameraSelf.position.z > oldZ){
                	cameraSelf.position = new Vector3(cameraSelf.position.x, cameraSelf.position.y, oldZ);
				}
			}
        }
        if (Locked){}
        else{
            this.transform.forward = Vector3.Lerp(this.transform.forward, oriRotation, Time.deltaTime * 10);
        }
	}

    public void Reset()
    {
        cameraSelf.transform.localPosition = new Vector3(-27, 0, 0);
        oldZ = -10.0f;
    }
}
