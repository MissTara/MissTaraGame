/* script_vcontroller.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * This class handles the input of the user including the virtual buttons on mobile devices.
 * 
 * */
using UnityEngine;
using System.Collections;

public class script_vcontroller : MonoBehaviour {
	private static script_vcontroller m_Instance = null;
    public static script_vcontroller Get()
    {
        if (m_Instance == null)
            m_Instance = (script_vcontroller)FindObjectOfType(typeof(script_vcontroller));
        return m_Instance;
    }
	// Joystick
	public MPJoystick joystick;
	//public Texture circle;
	public float horizontalOffset = 0;
	public float verticalOffset = 0;
	public float rotateX{
		get {
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)	
				return Input.gyro.rotationRate.x;
			else
				return Input.GetAxis("Mouse Y");
		}
	}
	public float rotateY{
		get {
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)	
				return Input.gyro.rotationRate.y;
			else
				return -Input.GetAxis("Mouse X");
		}
	}
	private float axisOffset = 0.02f;
	private float diameter;	
	private float screenHeight;
	/*
	float leftStickRadius;
	private float leftStickRadiusSquared;
	private float halfScreenWidth = Screen.width * 0.5f;
	private bool leftStickDown = false;
	private Vector2 leftStickCenter;
	private int leftStickTouchId = -1;
	*/
	// Jump Button
	public Texture jumpButton;
	private Rect jumpButtonRect;
	private float jumpButtonRadius;
	private float jumpButtonDiameter;
	private float jumpButtonId = -1;
	private bool jumpButtonDown = false;
	private static bool jumpButtonPressed = false;
	// Attack Button
	public Texture atkButton;
	private Rect atkButtonRect;
	private float atkButtonRadius;
	private float atkButtonDiameter;
	private float atkButtonId = -1;
	private bool atkButtonDown = false;
	private static bool atkButtonPressed = false;
    // SpecialAttack Button
    public Texture specialAtkButton;
    private Rect specialAtkButtonRect;
    private float specialAtkButtonRadius;
    private float specialAtkButtonDiameter;
    private float specialAtkButtonId = -1;
    private bool specialAtkButtonDown = false;
    private static bool specialAtkButtonPressed = false;
	// Use this for initialization
	void Start () {
		if (joystick == null){
			joystick = GameObject.FindObjectOfType(typeof(MPJoystick)) as MPJoystick;
		}
		
		// Try to enable gyroscope
		Input.gyro.enabled = true;
		
		screenHeight = Screen.height;
		// Load Textures
		//circle = ResourceManager.Get().tex_Controller_Joystick;
		jumpButton = ResourceManager.Get().tex_Controller_JMP;
		atkButton = ResourceManager.Get().tex_Controller_ATK;
        specialAtkButton = ResourceManager.Get().tex_Controller_SPECIALATTACK;
		/*
		if (circle != null){
			diameter = circle.width;
			leftStickRadius = diameter * 0.5f;
			leftStickRadiusSquared = leftStickRadius * leftStickRadius;
		}
		*/
        if (specialAtkButton != null)
        {
            specialAtkButtonDiameter = specialAtkButton.width;
            specialAtkButtonRadius = specialAtkButtonDiameter * 0.5f;
            specialAtkButtonRect = new Rect(Screen.width - 150, screenHeight - 450, specialAtkButtonDiameter, specialAtkButtonDiameter);
        }
		if (jumpButton != null){
			jumpButtonDiameter = jumpButton.width;
			jumpButtonRadius = jumpButtonDiameter * 0.5f;
			jumpButtonRect = new Rect(Screen.width - 150, screenHeight - 300, jumpButtonDiameter, jumpButtonDiameter);
		}
		if (atkButton != null){
			atkButtonDiameter = atkButton.width;
			atkButtonRadius = atkButtonDiameter * 0.5f;
			atkButtonRect = new Rect(Screen.width - 150, screenHeight - 150, atkButtonDiameter, atkButtonDiameter);
		}
	}
	protected void OnGUI ()
	{
		if (GameManager.isPaused)
			return;
		/*
		if (leftStickDown && false){
			float rectLeft = leftStickCenter.x - leftStickRadius;
			float rectTop = screenHeight - leftStickCenter.y - leftStickRadius;
			GUI.DrawTexture(new Rect(rectLeft, rectTop, diameter, diameter), circle);
		}
		*/
		if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)){
			GUI.DrawTexture(jumpButtonRect, jumpButton);
			GUI.DrawTexture(atkButtonRect, atkButton);
            GUI.DrawTexture(specialAtkButtonRect, specialAtkButton);
		}
	}
	// Update is called once per frame
	void Update () {
		if (GameManager.isPaused)
			return;
		int count = Input.touchCount;
		for (int i = 0; i < count; i++){
			Touch touch = Input.GetTouch(i);
			UpdateJumpButton(touch);
			UpdateAtkButton(touch);
            UpdateSpecialAttackButton(touch);
			//UpdateSwipe(touch);
		}
		horizontalOffset = joystick.position.x;
		verticalOffset = joystick.position.y;
		//Debug.Log("[Axis] X:" + horizontalOffset + "Y:" + verticalOffset);
	}
	/*
	public float swipeMinDistance;
	int swipeID = -1;
	bool swipeDown = false;
	Vector2 swipeTouchPos = Vector2.zero, swipeReleasePos= Vector2.zero;
	void UpdateSwipe(Touch touch){
		
	}
	*/
	
	
	
	/*
	bool swipeDown = false;
	Vector2 swipeLastPosition = Vector2.zero;
	int swipeID;
	public Vector2 swipeOffset = Vector2.zero;
	void UpdateSwipe(Touch touch){
		
		Vector2 touchPosition = touch.position;
		Vector2 guiTouchPosition = new Vector2(touch.position.x, screenHeight - touch.position.y);
		if (touch.phase == TouchPhase.Began &&
			!swipeDown &&
			!MPJoystick.Get().HitTest(touch.position) &&
			!atkButtonRect.Contains(guiTouchPosition ) &&
			!jumpButtonRect.Contains(guiTouchPosition)){
			swipeDown = true;
			swipeLastPosition = guiTouchPosition;
			swipeID = touch.fingerId;
		}
		else if(swipeDown && touch.fingerId == swipeID){
			if (touch.phase == TouchPhase.Moved){
				if (Mathf.Sign(swipeOffset.x) != Mathf.Sign((guiTouchPosition - swipeLastPosition).x))
					swipeOffset = new Vector2(0,swipeOffset.y);
				swipeOffset += (guiTouchPosition - swipeLastPosition);
				swipeLastPosition = guiTouchPosition;
			}
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
				if (swipeDown && touch.fingerId == swipeID){
					swipeDown = false;
				}
			}
		}
	}
	*/
	
	/*
	void UpdateJoyStick(Touch touch){
		Vector2 touchPosition = touch.position;
		if (touch.phase == TouchPhase.Began
			&& !leftStickDown
			&& touchPosition.x < halfScreenWidth){
			leftStickDown = true;
			leftStickCenter = touchPosition;
			leftStickTouchId = touch.fingerId;
		}
		else if (leftStickDown && touch.fingerId == leftStickTouchId){
			if (touch.phase == TouchPhase.Moved){
				Vector2 lDiff = touchPosition - leftStickCenter;
				if (lDiff.sqrMagnitude > leftStickRadiusSquared)
					leftStickCenter = touchPosition - (new Vector2(lDiff.x,lDiff.y)).normalized * leftStickRadius;
			}
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
				if (leftStickDown && touch.fingerId == leftStickTouchId)
					leftStickDown = false;
			}
		}
	}
	*/
    void UpdateSpecialAttackButton(Touch touch)
    {
        Vector2 touchPosition = touch.position;
        if (touch.phase == TouchPhase.Began
                && !specialAtkButtonDown
                && specialAtkButtonRect.Contains(new Vector2(touchPosition.x, Screen.height - touchPosition.y)))
        {
            specialAtkButtonDown = true;
            script_vcontroller.specialAtkButtonPressed = true;
            specialAtkButtonId = touch.fingerId;
        }
        else if (specialAtkButtonDown && touch.fingerId == specialAtkButtonId)
        {
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (specialAtkButtonDown && touch.fingerId == specialAtkButtonId)
                {
                    script_vcontroller.specialAtkButtonPressed = false;
                    specialAtkButtonDown = false;
                }
            }
        }
    }
	void UpdateJumpButton(Touch touch){
		Vector2 touchPosition = touch.position;
		if (touch.phase == TouchPhase.Began
				&& !jumpButtonDown
				&& jumpButtonRect.Contains (new Vector2(touchPosition.x, Screen.height - touchPosition.y))){
				jumpButtonDown = true;
				script_vcontroller.jumpButtonPressed = true;
				jumpButtonId = touch.fingerId;
			}
			else if (jumpButtonDown && touch.fingerId == jumpButtonId){
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
					if (jumpButtonDown && touch.fingerId == jumpButtonId){
						script_vcontroller.jumpButtonPressed = false;
						jumpButtonDown = false;
					}
				}
			}
	}
	void UpdateAtkButton(Touch touch){
		Vector2 touchPosition = touch.position;
		if (touch.phase == TouchPhase.Began
				&& !atkButtonDown
				&& atkButtonRect.Contains (new Vector2(touchPosition.x, Screen.height - touchPosition.y))){
				atkButtonDown = true;
				script_vcontroller.atkButtonPressed = true;
				atkButtonId = touch.fingerId;
			}
			else if (atkButtonDown && touch.fingerId == atkButtonId){
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
					if (atkButtonDown && touch.fingerId == atkButtonId){
						script_vcontroller.atkButtonPressed = false;
						atkButtonDown = false;
					}
				}
			}
	}
	public static bool isJump(){
		if (jumpButtonPressed){
			jumpButtonPressed = false;
			return true;
		}
		else{
			return false;
		}
	}
	public static bool isATK(){
		if (atkButtonPressed){
			atkButtonPressed = false;
			return true;
		}
		else{
			return false;
		}
	}

    public static bool isSpecialAtk()
    {
        if (specialAtkButtonPressed)
        {
            specialAtkButtonPressed = false;
            return true;
        }
        else
        {
            return false;
        }
    }

	public float getOffsetHorizontal(){
		if (Input.GetAxis("Horizontal") != 0)
			return Input.GetAxis("Horizontal");
		return horizontalOffset;
	}
	public float getOffsetVertical(){
		if (Input.GetAxis("Vertical") != 0)
			return Input.GetAxis("Vertical");
		return verticalOffset;
	}
	Vector2 joyStickInput(MPJoystick joystick)
    {
        float xDirection = (joystick.position.x > 0) ? 1 : -1;
        float yDirection = (joystick.position.y > 0) ? 1 : -1;
        return new Vector2(xDirection,yDirection);
    }
}
