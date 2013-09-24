/* CommandScript.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Script system for game designers who have no programming skill.
 * It can reads a script from files.
 * But currently I did not write the script serilization system. 
 * So please according to UserData and DataManager to finish this system.
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandScript : MonoBehaviour {
	
	private static CommandScript m_Instance = null;
    public static CommandScript Get()
    {
        if (m_Instance == null)
            m_Instance = (CommandScript)FindObjectOfType(typeof(CommandScript));
        return m_Instance;
    }
	
	const bool CMD_CASE_SENSITIVE = false;
	const string CMD_ENABLE_GAMEOBJECT = "EnableGameObject";	
	const string CMD_DISABLE_GAMEOBJECT = "DisableGameObject";
	const string CMD_CAMERA_LOCK = "LockCamera";
	const string CMD_CAMERA_UNLOCK = "UnlockCamera";
	const string CMD_MESSAGE_ADD = "AddMessage";
	const string CMD_MESSAGE_SHOW = "ShowMessage";
	public struct BasicCommand{
		public string CommandName;
		public string CommandParameter;
		public BasicCommand(string name, string parameter){
			CommandName = name;
			CommandParameter = parameter;
		}
	}
	List<BasicCommand> basicCommand;
	// Use this for initialization
	void Start () {
		basicCommand = new List<BasicCommand>();
	}
	public void InterpreteCommands(List<BasicCommand> cmd){
		if (cmd != null){
			foreach (BasicCommand c in cmd){
				InterpreteSingle(c);
			} 
		}
		else {
			print("CommandScript->InterpreteCommands: The command list is null");
		}
	}
	public void InterpreteSingle(BasicCommand cmd){
		if (cmd.CommandName == CMD_ENABLE_GAMEOBJECT)
			cmdEnableGameObject(cmd.CommandParameter);
		if (cmd.CommandName == CMD_DISABLE_GAMEOBJECT)
			cmdDisableGameObejct(cmd.CommandParameter);
		if (cmd.CommandName == CMD_CAMERA_LOCK)
			cmdCameraLock();
		if (cmd.CommandName == CMD_CAMERA_UNLOCK)
			cmdCameraUnlock();
		if (cmd.CommandName == CMD_MESSAGE_ADD)
			cmdAddMessgae(cmd.CommandParameter);
		if (cmd.CommandName == CMD_MESSAGE_SHOW)
			cmdShowMessage();
	}
	// Update is called once per frame
	void Update () {
	
	}
	private void cmdEnableGameObject(string parameter){
		GameObject t = GameObject.Find(parameter);
		
		if (t != null){
			MeshRenderer tMR = t.GetComponent<MeshRenderer>();
			Collider tC = t.GetComponent<Collider>();
			//if (tMR != null)
			//	tMR.enabled = true;
			if (tC != null)
				tC.enabled = true;
		}
	}
	private void cmdDisableGameObejct(string parameter){
		GameObject t = GameObject.Find(parameter);
		if (t != null)
			t.SetActive(false);
	}
	private void cmdCameraLock(){
		if (CameraController.Get() != null)
			CameraController.Get().Locked = true;	
	}
	private void cmdCameraUnlock(){
		if (CameraController.Get() != null)
			CameraController.Get().Locked = false;
	}
	private void cmdAddMessgae(string parameter){
		string[] paras = parameter.Split('|');
		float duration = 2f;
		if (paras.GetLength(0) > 1)
			duration = float.Parse(paras[1]);
		Message.Msg msg = new Message.Msg(paras[0],100,300, duration);
		Message.Get().addMsg(msg);
	}
	private void cmdShowMessage(){
		Message.Get().nextMsg();
	}
}
