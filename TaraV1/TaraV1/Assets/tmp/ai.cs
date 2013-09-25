using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ai : MonoBehaviour {
	public Rect gridRect = new Rect(-5,-5,5,5);
	public float cellLength = 0.5f;
	public bool visible = true;
	public List<Vector2> dbg_vPath;
	public int [,] actualMap = new int[40,40];
	private int [,] tMap = {
{1,1,0,1,1,1,1,0,0,0,1,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0},
{0,0,0,1,0,1,1,0,1,0,1,0,0,0,0,0,0,0,0,0},
{0,0,0,1,0,1,0,0,1,0,1,0,0,0,0,0,0,0,0,0},
{0,0,0,1,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
{0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0},
{0,0,0,1,0,0,1,1,1,1,1,1,0,0,0,1,0,0,0,0},
{0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0},
{0,0,0,1,0,0,0,0,1,0,0,1,0,0,0,1,0,0,0,0},
{0,0,0,1,0,0,0,0,1,0,0,1,1,0,0,1,1,1,1,0},
{0,0,0,1,1,1,1,1,1,0,0,0,1,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
};
	private int [,] tDrawMap;
	private Vector2 tStart = new Vector2(19,0), tDest = new Vector2(0,19);
		// Use this for initialization
	void Start () {
		dbg_vPath =AStar(tStart,tDest,tMap);
		//print ();
		tDrawMap = (int[,])tMap.Clone();
		foreach (Vector2 v in dbg_vPath){
			tDrawMap[(int)v.x,(int)v.y] = 2;
		}
		tDrawMap[(int)tStart.x,(int)tStart.y] = 3;
		tDrawMap[(int)tDest.x,(int)tDest.y] = 3;
}
	void OnGUI(){
		for (int x = 0; x < tDrawMap.GetLength(0);x++){
			for (int y = 0; y < tDrawMap.GetLength(1);y++){
				Color c = Color.white;
				GUIStyle gs = new GUIStyle();
				
				switch (tDrawMap[y,x]){
				case 0:
					c = Color.white;
					break;
				case 1:
					c = Color.red;
					break;
				case 2:
					c = Color.green;
					break;
				case 3:
					c = Color.cyan;
					break;
				default:
					c = new Color(0,255,0);
					break;
				}
				gs.normal.textColor = c;
				GUI.Label(new Rect(x * 10, y * 10, 10,10), "@",gs);
			}
		}
	}
	void OnCollisionStay(Collision collisionInfo){
		List<int> gridChanges = new List<int>();
		foreach (ContactPoint contact in collisionInfo.contacts) {
			print(contact.normal);
			for (int x = 0; x < actualMap.GetLength(0);x++){
				for(int y = 0; y < actualMap.GetLength(1);y++){
					if (Mathf.Abs(x * cellLength - contact.point.x) < cellLength && Mathf.Abs(y * cellLength - contact.point.z) < cellLength)
						actualMap[x,y] = 1;
					}
			}
            //Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
	}
	// Update is called once per frame
	void OnDrawGizmos(){
			for (int x = 0; x < actualMap.GetLength(0);x++){
				for(int y = 0; y < actualMap.GetLength(1);y++){
					if (actualMap[x,y] == 0)
						drawline(new Vector3(x * cellLength - 0.1f,-0.1f,y * cellLength - 0.1f),new Vector3(x * cellLength + 0.1f,0.1f,y * cellLength + 0.1f), Color.blue);	
					else
						drawline(new Vector3(x * cellLength - 0.1f,-0.1f,y * cellLength - 0.1f),new Vector3(x * cellLength + 0.1f,0.1f,y * cellLength + 0.1f), Color.red);	
				}		
			}
	}
	void Update () {
		
		//Graphics.DrawMesh(null,
	}
	void drawline(Vector3 start, Vector3 end, Color color){
		if (visible){
			Gizmos.color = color;
			Gizmos.DrawLine(start , end);
		}
		else{
			Debug.DrawLine (start, end, color);
		}
		
	}
	List<Vector2> neighbour_nodes(int [,] map, Vector2 pos){
		List<Vector2> tVector = new List<Vector2>();
		int xBound = map.GetLength(0) - 1, yBound = map.GetLength(1) - 1;
		if (pos.x - 1 >= 0)
			tVector.Add(new Vector2(pos.x - 1, pos.y));
		if (pos.x + 1 <= xBound)
			tVector.Add(new Vector2(pos.x + 1, pos.y));
		if (pos.y - 1 >= 0)
			tVector.Add(new Vector2(pos.x, pos.y - 1));
		if (pos.y + 1 <= yBound)
			tVector.Add(new Vector2(pos.x, pos.y + 1));
		if (pos.x - 1 >= 0 && pos.y - 1 >= 0)
			tVector.Add(new Vector2(pos.x - 1, pos.y - 1));
		if (pos.x - 1 >= 0 && pos.y + 1 <= yBound)
			tVector.Add(new Vector2(pos.x - 1, pos.y + 1));
		if (pos.x + 1 <= xBound && pos.y - 1 >= 0)
			tVector.Add(new Vector2(pos.x + 1, pos.y - 1));
		if (pos.x + 1 <= xBound && pos.y + 1 <= yBound)
			tVector.Add(new Vector2(pos.x + 1, pos.y + 1));
		return tVector;
	}
	List<Vector2> AStar(Vector2 start, Vector2 goal, int[,] map){
		List<Vector2> openset = new List<Vector2>();
		List<Vector2> closedset = new List<Vector2>();
		Dictionary<Vector2, Vector2> came_from = new Dictionary<Vector2, Vector2>();
		openset.Add(start);
		Vector2 current = Vector2.zero;
		while (openset.Count > 0){
			current = openset[0];
			foreach(Vector2 v in openset){
				if (f_score(v,start,goal) < f_score(current,start,goal)){
					current = v;
				}
			}
			if (current == goal){
				print ("Path Found!!!");
				return reconstruct_path(came_from,goal);
			}
			openset.Remove(current);
			closedset.Add(current);
			Vector2 formerNeighbour = Vector2.zero;
			foreach (Vector2 neighbour in neighbour_nodes(map, current)){
				if (openset.Contains(neighbour))
					continue;
				float tentative_g_score = g_score(current,start) + dist_between(current, neighbour);
				
				if (closedset.Contains(neighbour) && tentative_g_score >= g_score(neighbour, start))
					continue;
				/*
				bool tentative_is_better  = false;
				if (!openset.Contains(neighbour)){
					openset.Add(neighbour);
					tentative_uois_better = true;
				}
				else if (tentative_g_score < g_score(neighbour,start)){
					tentative_is_better = true;
				}
				else{
					tentative_is_better = false;
				}
				if (tentative_is_better && map[(int)neighbour.x,(int)neighbour.y]!= 1){
					came_from[neighbour] = current;
				}
				*/
				
				if ((!closedset.Contains(neighbour) || tentative_g_score < g_score(neighbour, start)) && map[(int)neighbour.x,(int)neighbour.y]!= 1){
					came_from[neighbour] = current;
					
					
					if (!openset.Contains(neighbour)){
						openset.Add(neighbour);
					}
				}
			}
		}
		print("Fail Finding Path");
		return null;
	}
	List<Vector2> reconstruct_path(Dictionary<Vector2, Vector2> came_from, Vector2 current_node){
		List<Vector2> tVector = new List<Vector2>();
		Vector2 former_node = current_node;
		while (came_from.ContainsKey(former_node)){
			tVector.Add(came_from[former_node]);	
			former_node = came_from[former_node];
		}
		tVector.RemoveAt(tVector.Count - 1);
		tVector.Reverse();
		return tVector;
	}
	float dist_between(Vector2 x, Vector2 y){
		return Vector2.Distance(x,y);
	}
	float g_score(Vector2 pos, Vector2 start){
		return Vector2.Distance(pos,start);
	}
	float h_score(Vector2 pos, Vector2 goal){
		return Vector2.Distance(pos, goal);
	}
	float f_score(Vector2 pos, Vector2 start, Vector2 goal){
		return g_score(pos,start) + h_score(pos, goal);
	}
	int[,] pathFinding(Vector2 start, Vector2 destination, int[,] map){
		int [,] tmpMap = new int[map.GetLength(0), map.GetLength(1)];
		List<Vector2> openset = new List<Vector2>();
		List<Vector2> closedset = new List<Vector2>();
		int x = (int)start.x, y = (int)start.y, xBound = map.GetLength(0), yBound = map.GetLength(1), xOffset = 0, yOffset = 0;
		tmpMap[x,y] = -1;
		if (futureDistance(map,start ,destination ) > -1){
			openset.Add(start);
			print ("Path Found! Perfect Condition");
			return null;
		}
		if (start.x < destination.x)
			xOffset = 1;
		else if (start.x > destination.x)
			xOffset = -1;
		if (start.y < destination.y)
			yOffset = 1;
		else if (start.y > destination.y)
			yOffset = -1;
		while (x != destination.x || y != destination.y){
			Vector2 tVector = new Vector2(x,y);
			if (futureDistance(map,tVector,destination) > -1){
				openset.Add(tVector);
				print("Path Found!!!");
				return null;
			}
			/*if (futureDistance(map,))*/
			
		}
		return null;
	}
	
	int futureDistance(int [,] map, Vector2 pos, Vector2 destination){
		int tmpDistance = 0;
		int safe = 0;
		int x = 0, y = 0,xOffset = 0,yOffset = 0, xBound = map.GetLength(0), yBound = map.GetLength(1);
		if (pos.x < 0 || pos.x > xBound - 1 || pos.y < 0 || pos.y > yBound - 1){
			return -1;
		}
		if (pos.x < destination.x)
			xOffset = 1;
		else if (pos.x > destination.x)
			xOffset = -1;
		if (pos.y < destination.y)
			yOffset = 1;
		else if (pos.y > destination.y)
			yOffset = -1;
		while (pos.x + x != destination.y || pos.y + y != destination.y){
			print ("X:" + (pos.x + x) + " Y:" + (pos.y + y));
			if (((int)pos.x + x + xOffset > xBound - 1 && (int)pos.x + x + xOffset < 0) && ((int)pos.y + y + yOffset > yBound - 1 && (int)pos.y + y + yOffset < 0))
				return tmpDistance;
			if (map[(int)pos.x + x, Mathf.Max(0, Mathf.Min(yBound - 1, (int)pos.y + y + yOffset))] == 1){
				if (map[Mathf.Max(0, Mathf.Min((int)pos.x + x + xOffset,xBound - 1)), (int)pos.y + y] == 1){
					return -1;
				}
			}
			if (((int)pos.x + x + xOffset >=0 && (int)pos.x + x + xOffset < xBound) && map[(int)pos.x + x + xOffset, (int)pos.y + y] == 0){
				x += xOffset;
				tmpDistance++;
			}
			if (((int)pos.y + y + yOffset >=0 && (int)pos.y + y + yOffset < yBound) && map[(int)pos.x + x, (int)pos.y + y + yOffset] == 0){
				y += yOffset;
				tmpDistance++;
			}
			safe++;
			if (safe > 100)
				return 0;
		}
		return tmpDistance;
	}
	int calcCost(int [,] path){
		int tmp = 0;
		foreach (int x in path){
			if (x == 1)
				tmp++;
		}
		return tmp;
	}
}
