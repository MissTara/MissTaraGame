/* MPTestMov.cs
 * Author: Unity3D
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Joystick
 * */
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class MPTestMov : MonoBehaviour {

    public float speed = 3.0f;
    public float rotateSpeed = 3.0f;
    public MPJoystick moveJoystick;
    public MPJoystick rotateJoystick;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        CharacterController controller = this.GetComponent<CharacterController>();

        //Rotate around y-axis
        //if is true (?), do this OR(:) this
        float rotatePos2 = Input.GetAxis("Horizontal")==1 ?
            Input.GetAxis("Horizontal") : joyStickInput(rotateJoystick);
        transform.Rotate(0, rotatePos2 * rotateSpeed, 0);
   
        //Move forward/backward
        Vector3 forward = transform.TransformDirection(transform.forward);
        float movePos = Input.GetAxis("Verticle")==1 ?
            Input.GetAxis("Vertical") : joyStickInput(moveJoystick);
        float curSpeed = speed * movePos;
        controller.SimpleMove(forward * curSpeed);
	}

    //Alfred Lai - Custom joystick function
    float joyStickInput(MPJoystick joystick)
    {
        Vector2 absJoyPos = new Vector2(Mathf.Abs(joystick.position.x),
                                   Mathf.Abs(joystick.position.y));
        float xDirection = (joystick.position.x > 0) ? 1 : -1;
        float yDirection = (joystick.position.y > 0) ? 1 : -1;
        return ((absJoyPos.x > absJoyPos.y) ? absJoyPos.x * xDirection : absJoyPos.y * yDirection);
    }
}
