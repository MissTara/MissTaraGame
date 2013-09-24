/* SaveLoad.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * It saves and loads game data including ItemDatabase and Userdata.
 * Before saveing, it will serialize the userdata class into an XML then save it to a file.
 * While loading, it will load the XML file and deserialize it to a class.
 * 
 * The database is stored at the foder Assets\Resources\ItemData.xml
 * The userdata is stored at the persistentDataPath(Please refer to unity document for details)
 * */
using UnityEngine; 
using System.Collections; 
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text;
public class SaveLoad : MonoBehaviour {
	private static SaveLoad m_Instance = null;
    public static SaveLoad Get()
    {
        if (m_Instance == null)
            m_Instance = (SaveLoad)FindObjectOfType(typeof(SaveLoad));
        return m_Instance;
    }
	string _data="";
	string _FileLocation,_FileName;
	// Use this for initialization
	void Start () {
		//this.enabled = false;
		_FileLocation=Application.persistentDataPath + "/"; 
      	_FileName="SaveData"; 
		DataManager d = new DataManager();
		DebugScreen.Get().addMsg("File Location " + _FileLocation);
		//d.Weapons.Add(new ItemWeapon("Ultimate Justice", BattleCore.elements.Earth ,10,1));
		//d.Weapons.Add(new ItemWeapon("Ash Bringer", BattleCore.elements.Earth ,10,1));
		//SaveData(d,"ItemData.xml");
		TextAsset tAsset = Resources.Load("ItemData") as TextAsset;
		d = (DataManager)DeserializeObject(tAsset.text,d);//(DataManager)LoadData("ItemData.xml",d);
		GameManager.dataManager = d;
		ItemArmour arm = new ItemArmour(BattleCore.elements.Earth,10);
		DebugScreen.Get().addMsg("User Data Initializing " + Time.realtimeSinceStartup);
		InitializeUserData();
		/*
		arm.ItemDescription = "A basic defender.";
		arm.ItemPrice = 10;
		arm.ItemID = 10;
		arm.ItemName = "Cloth Armor";
		d.Armour.Add(arm);
		SaveData(d,"ItemData.xml");
		*/
		DebugScreen.Get().addMsg("User Data Initialized " + Time.realtimeSinceStartup);
	}
	void OnGUI(){
		return;
		if (GUI.Button(new Rect(10,10,200,200),"SaveGameData")){
			GameManager.EnemyProperties tEnemyProp = new GameManager.EnemyProperties(true);
			tEnemyProp.Name = "Alien";
			tEnemyProp.Element = BattleCore.elements.Tech;
			tEnemyProp.BaseArmour = 0;
			tEnemyProp.BaseAttack = 2;
			GameManager.dataManager.Enemies.Add(tEnemyProp);
			SaveGameData();
		}
	}
	public void InitializeUserData(){
		UserData udt = new UserData();
		if (!File.Exists(_FileLocation + "UserData.xml")){
			DebugScreen.Get().addMsg("File Not Exists " + Time.realtimeSinceStartup);
			udt.initializePlayer();
			SaveData(udt,"UserData.xml");
		}
		else{
			DebugScreen.Get().addMsg("Deserializing " + Time.realtimeSinceStartup);
			udt = (UserData)DeserializeObject(LoadXML("UserData.xml"), udt);
			DebugScreen.Get().addMsg("Deserialized " + Time.realtimeSinceStartup);
		}
		if (udt == null)
			this.enabled = false;
		GameManager.userData = udt;
		udt.applyToPlayer();
		
	}
	public void SaveUserData(){
		if (GameManager.userData != null){
			SaveData(GameManager.userData,"UserData.xml");
			DebugScreen.Get().addMsg("User Data Saved " + Time.realtimeSinceStartup);
		}
	}
	public void SaveGameData(){
		// Only For Debug
		if (GameManager.dataManager != null){
			SaveData(GameManager.dataManager,"GameData.xml");
		}
	}
	public void SaveData(object data, string Filename){
		CreateXML(SerializeObject(data),Filename);
	}
	public object LoadData(string Filename, object obj){
		return DeserializeObject(LoadXML(Filename),obj);
	}
	
	
	
	object DeserializeObject(string pXmlizedString,object obj) 
   { 
      XmlSerializer xs = new XmlSerializer(obj.GetType()); 
      MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
      XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
      return xs.Deserialize(memoryStream); 
   } 
	string SerializeObject(object pObject){
		string XmlizedString = null; 
	    MemoryStream memoryStream = new MemoryStream(); 
		
	    XmlSerializer xs = new XmlSerializer(pObject.GetType()); 
	    XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
	    xs.Serialize(xmlTextWriter, pObject); 
	    memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
	    XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
	    return XmlizedString; 
	}
	void CreateXML(string FileName) 
   { 
      StreamWriter writer; 
      FileInfo t = new FileInfo(_FileLocation + FileName); 
	  
      if(!t.Exists) 
      { 
         writer = t.CreateText(); 
      } 
      else 
      { 
         t.Delete(); 
         writer = t.CreateText(); 
      } 
      writer.Write(_data); 
      writer.Close(); 
      Debug.Log("File written."); 
   } 
	void CreateXML(string data,string FileName) 
   { 
      StreamWriter writer; 
      FileInfo t = new FileInfo(_FileLocation +FileName); 
		DebugScreen.Get().addMsg("Create XML " + _FileLocation + FileName);
		print(_FileLocation + FileName);
      if(!t.Exists) 
      { 
         writer = t.CreateText(); 
      } 
      else 
      { 
         t.Delete(); 
         writer = t.CreateText(); 
      } 
      writer.Write(data); 
      writer.Close(); 
      Debug.Log("File written."); 
   } 
	string UTF8ByteArrayToString(byte[] characters) 
   {      
      UTF8Encoding encoding = new UTF8Encoding(); 
      string constructedString = encoding.GetString(characters); 
      return (constructedString); 
   } 
 
   byte[] StringToUTF8ByteArray(string pXmlString) 
   { 
      UTF8Encoding encoding = new UTF8Encoding(); 
      byte[] byteArray = encoding.GetBytes(pXmlString); 
      return byteArray; 
   } 
	void LoadXML() 
   { 
      StreamReader r = File.OpenText(_FileLocation + _FileName); 
      string _info = r.ReadToEnd(); 
      r.Close(); 
      _data=_info; 
      Debug.Log("File Read"); 
   } 
	string LoadXML(string FileName) 
   { 
      StreamReader r = File.OpenText(_FileLocation + FileName); 
      string _info = r.ReadToEnd(); 
      r.Close(); 
      return _info; 
      Debug.Log("File Read"); 
   } 
}
