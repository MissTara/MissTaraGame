/* AIPathCustom.cs
 * Author: http://arongranberg.com/
 * Last Modified By: Dexter
 * Description: 
 * A Star path finding for aliens
 * */ 
//#define ASTARDEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

/** AI for following paths.
 * This AI is the default movement script which comes with the A* Pathfinding Project.
 * It is in no way required by the rest of the system, so feel free to write your own. But I hope this script will make it easier
 * to set up movement for the characters in your game. This script is not written for high performance, so I do not recommend using it for large groups of units.
 * \n
 * \n
 * This script will try to follow a target transform, in regular intervals, the path to that target will be recalculated.
 * It will on FixedUpdate try to move towards the next point in the path.
 * However it will only move in the forward direction, but it will rotate around it's Y-axis
 * to make it reach the target.
 * 
 * \section variables Quick overview of the variables
 * In the inspector in Unity, you will see a bunch of variables. You can view detailed information further down, but here's a quick overview.\n
 * The #repathRate determines how often it will search for new paths, if you have fast moving targets, you might want to set it to a lower value.\n
 * The #target variable is where the AI will try to move, it can be a point on the ground where the player has clicked in an RTS for example.
 * Or it can be the player object in a zombie game.\n
 * The 
 * is self-explanatory, so is turningSpeed, however #slowdownDistance might require some explanation.
 * It is the approximate distance from the target where the AI will start to slow down. Note that this doesn't only affect the end point of the path
 * but also any intermediate points, so be sure to set #forwardLook and #pickNextWaypointDist to a higher value than this.\n
 * #pickNextWaypointDist is simply determines within what range it will switch to target the next waypoint in the path.\n
 * #forwardLook will try to calculate an interpolated target point on the current segment in the path so that it has a distance of #forwardLook from the AI\n
 * Below is an image illustrating several variables as well as some internal ones, but which are relevant for understanding how it works.
 * Note that the #forwardLook range will not match up exactly with the target point practically, even though that's the goal.
 * \shadowimage{aipath_variables.png}
 * This script has many movement fallbacks.
 * If it finds a NavmeshController, it will use that, otherwise it will look for a character controller, then for a rigidbody and if it hasn't been able to find any
 * it will use Transform.Translate which is guaranteed to always work.
 */
[RequireComponent(typeof(Seeker))]
public class AIPathCustom : MonoBehaviour, ICombat
{
    AIStates AnimControl;

    /** Determines how often it will search for new paths. 
     * If you have fast moving targets or AIs, you might want to set it to a lower value.
     * The value is in seconds between path requests.
     */
    public float repathRate = 0.5F;

    /** Target to move towards.
     * The AI will try to follow/move towards this target.
     * It can be a point on the ground where the player has clicked in an RTS for example, or it can be the player object in a zombie game.
     */
    public Transform target;

    /** Enables or disables searching for paths.
     * Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
     * \see #canMove
     */
    public bool canSearch = true;

    /** Enables or disables movement.
      * \see #canSearch */
    public bool canMove = true;

    /** Maximum velocity.
     * This is the maximum speed in world units per second.
     */
    public float speed = 3;

    /** Rotation speed.
     * Rotation is calculated using Quaternion.SLerp. This variable represents the damping, the higher, the faster it will be able to rotate.
     */
    public float turningSpeed = 5;

    /** Distance from the target point where the AI will start to slow down.
     * Note that this doesn't only affect the end point of the path
     * but also any intermediate points, so be sure to set #forwardLook and #pickNextWaypointDist to a higher value than this
     */
    public float slowdownDistance = 0.6F; 

    /** Determines within what range it will switch to target the next waypoint in the path */
    public float pickNextWaypointDist = 2;

    /** Target point is Interpolated on the current segment in the path so that it has a distance of #forwardLook from the AI.
      * See the detailed description of AIPath for an illustrative image */
    public float forwardLook = 1;

    /** Distance to the end point to consider the end of path to be reached.
     * When this has been reached, the AI will not move anymore until the target changes and OnTargetReached will be called.
     */
    public float endReachedDistance = 0.2F;

    /** Distance to the target to start moving.
     * should utilize canMove bool
     */
    //public float endTriggerDistance = 5.0F;

    /** Do a closest point on path check when receiving path callback.
     * Usually the AI has moved a bit between requesting the path, and getting it back, and there is usually a small gap between the AI
     * and the closest node.
     * If this option is enabled, it will simulate, when the path callback is received, movement between the closest node and the current
     * AI position. This helps to reduce the moments when the AI just get a new path back, and thinks it ought to move backwards to the start of the new path
     * even though it really should just proceed forward.
     */
    public bool closestOnPathCheck = true;

    protected float minMoveScale = 0.05F;

    /** Cached Seeker component */
    protected Seeker seeker;

    /** Cached Transform component */
    protected Transform tr;

    /** Time when the last path request was sent */
    private float lastRepath = -9999;

    /** Current path which is followed */
    protected Path path;

    /** Cached CharacterController component */
    protected CharacterController controller;

    /** Cached NavmeshController component */
    protected NavmeshController navController;


    /** Cached Rigidbody component */
    protected Rigidbody rigid;

    /** Current index in the path which is current target */
    protected int currentWaypointIndex = 0;

    /** Holds if the end-of-path is reached
     * \see TargetReached */
    protected bool targetReached = false;

    /** Only when the previous path has been returned should be search for a new path */
    protected bool canSearchAgain = true;

    /** Alfred Lai - Custom animation state booleans */
    protected bool isRunning = false;
    public bool isIdle = true;

    /** Returns if the end-of-path has been reached
     * \see targetReached */
    public bool TargetReached
    {
        get
        {
            return targetReached;
        }
    }

    /** Holds if the Start function has been run.
     * Used to test if coroutines should be started in OnEnable to prevent calculating paths
     * in the awake stage (or rather before start on frame 0).
     */
    private bool startHasRun = false;

    /** Initializes reference variables.
     * If you override this function you should in most cases call base.Awake () at the start of it.
      * */
    protected virtual void Awake()
    {
        seeker = GetComponent<Seeker>();

        AnimControl = GetComponent<AIStates>();

        //This is a simple optimization, cache the transform component lookup
        tr = transform;

        //Make sure we receive callbacks when paths complete
        seeker.pathCallback += OnPathComplete;

        //Cache some other components (not all are necessarily there)
        controller = GetComponent<CharacterController>();
        navController = GetComponent<NavmeshController>();
        rigid = rigidbody;
    }

    /** Starts searching for paths.
     * If you override this function you should in most cases call base.Start () at the start of it.
     * \see OnEnable
     * \see RepeatTrySearchPath
     */
    protected virtual void Start()
    {
		if (target == null)
			target = GameManager.Get().objPlayer.transform;
        startHasRun = true;
        OnEnable();
		InitializeEnemy();
    }

    /** Run at start and when reenabled.
     * Starts RepeatTrySearchPath.
     * 
     * \see Start
     */
    protected virtual void OnEnable()
    {
        if (startHasRun) StartCoroutine(RepeatTrySearchPath());
    }

    /** Tries to search for a path every #repathRate seconds.
      * \see TrySearchPath
      */
    public IEnumerator RepeatTrySearchPath()
    {
        while (true)
        {
            TrySearchPath();
            yield return new WaitForSeconds(repathRate);
        }
    }

    /** Tries to search for a path.
     * Will search for a new path if there was a sufficient time since the last repath and both
     * #canSearchAgain and #canSearch are true.
     * Otherwise will start WaitForPath function.
     */
    public void TrySearchPath()
    {
        if (Time.time - lastRepath >= repathRate && canSearchAgain && canSearch)
        {
            SearchPath();
        }
        else
        {
            StartCoroutine(WaitForRepath());
        }
    }

    public void SearchPathAgain()
    {
        Vector3 targetPosition = target.position;

        seeker.StartPath(GetFeetPosition(), targetPosition);
    }
    /** Is WaitForRepath running */
    private bool waitingForRepath = false;

    /** Wait a short time til Time.time-lastRepath >= repathRate.
      * Then call TrySearchPath
      * 
      * \see TrySearchPath
      */
    protected IEnumerator WaitForRepath()
    {
        if (waitingForRepath) yield break; //A coroutine is already running

        waitingForRepath = true;
        //Wait until it is predicted that the AI should search for a path again
        yield return new WaitForSeconds(repathRate - (Time.time - lastRepath));

        waitingForRepath = false;
        //Try to search for a path again
        TrySearchPath();
    }

    /** Requests a path to the target */
    public void SearchPath()
    {

        if (target == null) { Debug.LogError("Target is null, aborting all search"); canSearch = false; return; }
        if (this.GetComponent<AIStates>().EnemyState == AIStates.states.Jump) return;

        lastRepath = Time.time;
        //This is where we should search to
        Vector3 targetPosition = target.position;

        canSearchAgain = false;

        //Alternative way of requesting the path
        //Path p = PathPool<Path>.GetPath().Setup(GetFeetPosition(),targetPoint,null);
        //seeker.StartPath (p);

        //We should search from the current position
        seeker.StartPath(GetFeetPosition(), targetPosition);
    }

    public virtual void OnTargetReached()
    {
        //End of path has been reached
        //If you want custom logic for when the AI has reached it's destination
        //add it here
        //You can also create a new script which inherits from this one
        //and override the function in that script
		if (gameObject.tag != "Bunny"){
	        if (!isIdle)
    	    {
        	    isIdle = true;
            	isRunning = false;
				if (gameObject.tag == "MechBoss" || gameObject.tag == "CaptainBoss"){
					if(!GetComponent<AIStates>().autoBossAttack){
						GetComponent<AIStates>().StartCoroutine("getBossAttack");
					 	AnimControl.EnemyState = AIStates.states.Attack;
					}
				}else
    	        	AnimControl.EnemyState = AIStates.states.Attack;
	        }
		}else{
			isIdle = true;
			isRunning = false;
			AnimControl.EnemyState = AIStates.states.Idle;
		}
    }

    public void OnDestroy()
    {
        if (path != null) path.Release(this);
    }

    /** Called when a requested path has finished calculation.
      * A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
      * Finally it is returned to the seeker which forwards it to this function.\n
      */
    public virtual void OnPathComplete(Path _p)
    {
        ABPath p = _p as ABPath;
        if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

        //Release the previous path
        if (path != null) path.Release(this);

        //Claim the new path
        p.Claim(this);

        //Replace the old path
        path = p;

        //Reset some variables
        currentWaypointIndex = 0;
        targetReached = false;
        canSearchAgain = true;

        //The next row can be used to find out if the path could be found or not
        //If it couldn't (error == true), then a message has probably been logged to the console
        //however it can also be got using p.errorLog
        //if (p.error)

        if (closestOnPathCheck)
        {
            Vector3 p1 = p.startPoint;
            Vector3 p2 = GetFeetPosition();
            float magn = Vector3.Distance(p1, p2);
            Vector3 dir = p2 - p1;
            dir /= magn;
            int steps = (int)(magn / pickNextWaypointDist);
            for (int i = 0; i < steps; i++)
            {
                CalculateVelocity(p1);
                p1 += dir;
            }
        }
    }

    public virtual Vector3 GetFeetPosition()
    {
        if (controller != null)
        {
            return tr.position - Vector3.up * controller.height * 0.5F;
        }
        return tr.position;
    }

    public virtual void Update(){
		if (GameManager.isPaused)
			return;
		EnemyUpdate();
		//print (Vector3.Distance(target.position,transform.position));
		impact = Vector3.Lerp(impact,Vector3.zero, 5 * Time.deltaTime);
        if (!canMove) { return; }
        /*
		if (!canMove|| Vector3.Distance(target.position,transform.position) > detectRange || GameManager.isPaused){
			if (!dead)
				AnimControl.EnemyState = AIStates.states.Idle;
			return;
		}*/
		Vector3 dir = CalculateVelocity(GetFeetPosition());
		
		dir += impact * Time.deltaTime;
        //Rotate towards targetDirection (filled in by CalculateVelocity)
        if (targetDirection != Vector3.zero){
            RotateTowards(targetDirection);
        }
        if (navController != null){
            navController.SimpleMove(GetFeetPosition(), dir);
        }else if (controller != null){
            if (gameObject.tag == "Bat")
                controller.Move(dir * Time.deltaTime);
            else
                controller.SimpleMove(dir);
        }else if (rigid != null){
            rigid.AddForce(dir);
        }else{
            transform.Translate(dir * Time.deltaTime, Space.World);
        }
    }

    /** Point to where the AI is heading.
      * Filled in by #CalculateVelocity */
    protected Vector3 targetPoint;
    /** Relative direction to where the AI is heading.
     * Filled in by #CalculateVelocity */
    protected Vector3 targetDirection;

    protected float XZSqrMagnitude(Vector3 a, Vector3 b)
    {
        float dx = b.x - a.x;
        float dz = b.z - a.z;
        return dx * dx + dz * dz;
    }

    /** Calculates desired velocity.
     * Finds the target path segment and returns the forward direction, scaled with speed.
     * A whole bunch of restrictions on the velocity is applied to make sure it doesn't overshoot, does not look too far ahead,
     * and slows down when close to the target.
     * /see speed
     * /see endReachedDistance
     * /see slowdownDistance
     * /see CalculateTargetPoint
     * /see targetPoint
     * /see targetDirection
     * /see currentWaypointIndex
     */
    protected Vector3 CalculateVelocity(Vector3 currentPosition)
    {
        if (path == null || path.vectorPath == null || path.vectorPath.Count == 0) return Vector3.zero;

        List<Vector3> vPath = path.vectorPath;
        //Vector3 currentPosition = GetFeetPosition();

        if (vPath.Count == 1)
        {
            vPath.Insert(0, currentPosition);
        }

        if (currentWaypointIndex >= vPath.Count) { currentWaypointIndex = vPath.Count - 1; }

        if (currentWaypointIndex <= 1) currentWaypointIndex = 1;

        while (true)
        {
            if (currentWaypointIndex < vPath.Count - 1)
            {
                //There is a "next path segment"
                float dist = XZSqrMagnitude(vPath[currentWaypointIndex], currentPosition);
                if (!isRunning)
                {
            	    isRunning = true;
        	        isIdle = false;
	                AnimControl.EnemyState = AIStates.states.Run;
                }
                //Mathfx.DistancePointSegmentStrict (vPath[currentWaypointIndex+1],vPath[currentWaypointIndex+2],currentPosition);
                if (dist < pickNextWaypointDist * pickNextWaypointDist)
                {
                    currentWaypointIndex++;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        Vector3 dir = vPath[currentWaypointIndex] - vPath[currentWaypointIndex - 1];
        Vector3 targetPosition = CalculateTargetPoint(currentPosition, vPath[currentWaypointIndex - 1], vPath[currentWaypointIndex]);
        //vPath[currentWaypointIndex] + Vector3.ClampMagnitude (dir,forwardLook);
        dir = targetPosition - currentPosition;
        dir.y = 0;
        float targetDist = dir.magnitude;

        float slowdown = Mathf.Clamp01(targetDist / slowdownDistance);

        this.targetDirection = dir;
        this.targetPoint = targetPosition;
		//targetDist = Vector3.Distance(this.transform.position, target.position);
		//print("Dist:" + targetDist + " EndDistance:" + endReachedDistance);
		//		print(this.GetInstanceID() + " " + (currentWaypointIndex == vPath.Count - 1) + " " + (targetDist <= endReachedDistance));

		//print(currentWaypointIndex.ToString() + " | " + (vPath.Count - 1).ToString());
		if (currentWaypointIndex == vPath.Count - 1 && targetDist <= endReachedDistance)
        {
            if (!targetReached) { targetReached = true; OnTargetReached(); }

            //Send a move request, this ensures gravity is applied
            return Vector3.zero;
        }

        Vector3 forward = tr.forward;
        float dot = Vector3.Dot(dir.normalized, forward);
        float sp = speed * Mathf.Max(dot, minMoveScale) * slowdown;

        if (Time.deltaTime > 0)
        {
            sp = Mathf.Clamp(sp, 0, targetDist / (Time.deltaTime * 2));
        }

        return forward * sp;
    }

    /** Rotates in the specified direction.
     * Rotates around the Y-axis.
     * \see turningSpeed
     */
    protected virtual void RotateTowards(Vector3 dir)
    {
        Quaternion rot = tr.rotation;
        Quaternion toTarget = Quaternion.LookRotation(dir);

        rot = Quaternion.Slerp(rot, toTarget, turningSpeed * Time.fixedDeltaTime);
        Vector3 euler = rot.eulerAngles;
        euler.z = 0;
        euler.x = 0;
        rot = Quaternion.Euler(euler);

        tr.rotation = rot;
    }

    /** Calculates target point from the current line segment.
     * \param p Current position
     * \param a Line segment start
     * \param b Line segment end
     * The returned point will lie somewhere on the line segment.
     * \see #forwardLook
     * \todo This function uses .magnitude quite a lot, can it be optimized?
     */
    protected Vector3 CalculateTargetPoint(Vector3 p, Vector3 a, Vector3 b)
    {
        a.y = p.y;
        b.y = p.y;

        float magn = (a - b).magnitude;
        if (magn == 0) return a;

        float closest = Mathfx.Clamp01(Mathfx.NearestPointFactor(a, b, p));
        Vector3 point = (b - a) * closest + a;
        float distance = (point - p).magnitude;

        float lookAhead = Mathf.Clamp(forwardLook - distance, 0.0F, forwardLook);

        float offset = lookAhead / magn;
        offset = Mathf.Clamp(offset + closest, 0.0F, 1.0F);
        return (b - a) * offset + a;
    }
	//==========================================================
	/* Luke Enemy */
	//==========================================================
	public float detectRange = 5;
	public bool canAttack = true;
	public bool dead = false;
	public float MaxHP = 2;
	private float _CurHP = 0;
	public BattleCore.elements Element = BattleCore.elements.None;
	
	public List<BattleCore.Factions> _factionSelf = new List<BattleCore.Factions>();
	public List<BattleCore.Factions> _factionHostile = new List<BattleCore.Factions>();
	public int BaseArmour = 0, BaseAttack = 0;
	public ItemWeapon Weapon{
		get { return new ItemWeapon( Element, BaseAttack); }
		set { print("Cannot Set Weapons of an Enemy");}
	}
	public float mass = 3;
	private Vector3 impact = Vector3.zero;
	public bool isAttacking{
		get { return AnimControl.EnemyState == AIStates.states.Attack; }
		set { print("Cannot Set isAttacking of an Enemy"); }
	}
	public float CurHP{
		get { return _CurHP; }
		set {
			if (value  < 0)
				_CurHP = 0;
			else if (value > MaxHP){
				_CurHP = MaxHP;
			}
			else{
				_CurHP = value;
			}
		}
	}
	public List<BattleCore.Factions> FactionSelf{
		get{ return _factionSelf; }
		set{ _factionSelf = value; }
	}
	public List<BattleCore.Factions> FactionHostile{
		get { return _factionHostile; }
		set { _factionHostile = value; }
	}
	// This got called in the Start(){}
	public void InitializeEnemy(){
		CurHP = MaxHP;
		if (_factionSelf.Count < 1)
			_factionSelf.Add(BattleCore.Factions.alien);
		if (_factionHostile.Count < 1)
			_factionHostile.Add(BattleCore.Factions.human);
	}
	public void AddImpact(Vector3 dir, float force){
		dir.Normalize();
		if (dir.y < 0)
			dir.y = - dir.y;
		impact += dir.normalized * force / mass;
	}
	public void onAttack(Vector3 contactPoint){}
	public void hurt(ItemWeapon weapon){ 
		if (!dead){
			//AnimControl.EnemyState = AIStates.states.AlienHit;
			int damage = BattleCore.CalculateDamage(weapon.BaseDamage,weapon.Element,BaseArmour,Element);
			PopoutNum pop = PopoutNum.Get();
			if (pop != null){
				pop.popupText(this.transform.position,damage,(int)weapon.Element);
			}
			else
				print("AIPathCustom->No Popup in the scene");
			CurHP -= damage;
			if (CurHP == 0){
				GameObject.Find("CameraSelf").GetComponentInChildren<script_HUD>().StartCoroutine("bunnyGuage");
				Die();
			}
		}
	}

    public virtual void KillMe()
    {
        if (gameObject.tag == "Alien" || gameObject.tag == "Bat" || gameObject.tag == "Bear" || gameObject.tag == "Wolf" || gameObject.tag == "MechBoss" || gameObject.tag == "CaptainBoss")
            this.Dance();
    }

    public void Dance()
    {
        canSearch = false;
        canMove = false;
        AnimControl.EnemyState = AIStates.states.Dance;
		if (gameObject.tag != "MechBoss" || gameObject.tag == "captainBoss")
        	StartCoroutine("PlayDance");
		else
			StartCoroutine("PlayDanceBoss");
    }

    IEnumerator PlayDance()
    {
        yield return new WaitForSeconds(3.0f);
        this._CurHP = 0;
        this.dead = true;
        this.Die();
    }
	
	IEnumerator PlayDanceBoss()
    {
		if(gameObject.tag == "MechBoss")
        	yield return new WaitForSeconds(9.0f);
		else
			yield return new WaitForSeconds(2.0f);
        this._CurHP -= 50.0f;
		canSearch = true;
		canMove = true;
		AnimControl.EnemyState = AIStates.states.Run;
    }

	public void Die(){
		dead = true;
		AnimControl.EnemyState = AIStates.states.Death;
		canSearch = false;
		canMove = false;
		
	}
	public void LateDie(){
		GameManager.Get ().objEnemies.Remove(this.gameObject);
		this.gameObject.SetActive(false);
		controller.enabled = false;
        Destroy(this.gameObject);
	}
	public void EnemyUpdate(){
		if (dead && !AnimControl.animation.isPlaying)
			LateDie();
	}
	public void dropItems(){
		
	}
	public void ApplyProperties(GameManager.EnemyProperties prop){
		this.canSearch = prop.CanSearch;
		this.canMove = prop.CanMove;
		this.canAttack = prop.CanAttack;
		//this.speed = prop.Speed;
		this.turningSpeed = prop.TurningSpeed;
		this.endReachedDistance = prop.EndReachedDistance;
		this.detectRange = prop.DetectRange;
		//this.MaxHP = prop.MaxHP;
		this.CurHP = this.MaxHP;
		this.Element = prop.Element;
		this.FactionSelf = prop.FactionSelf;
		this.FactionHostile = prop.FactionHostile;
		this.BaseArmour = prop.BaseArmour;
		this.BaseAttack = prop.BaseAttack;
		this.mass = prop.Mass;
	}
}
