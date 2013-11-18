/* AIStates.cs
 * Author: Alfred Lai
 * Last Modified By: Dexter
 * Description: 
 * Provides states for enemy and plays different animations depends on different states.
 * */
using UnityEngine;
using System.Collections;

public class AIStates : MonoBehaviour
{

    public enum states { Idle, Run, Death, Attack, Hit, Dance, Jump };
    public bool hitted = false;
    AIPathCustom AIPathing;
    CharacterController controller;

    public states EnemyState;
    [System.NonSerialized] public bool died, attacking = false;
    protected float delay, timer;
    public float delayer = 5.0f;
    protected float batdelay = 3.4f;

    public Transform player;
    private float speed = 1.0f;
    private Vector3 dir;
	
	private GameManager GM;
	private UnitEnemy tmp;				//Holder for the Bat's UnitEnemy script
	public Vector3 speedSwoop;			//Speed for the attack of the bat
	/* Steven:
	 * swoop is if the bat is swooping.
	 * BatAtt is if the bat is attacking
	 * batWait is a cooldown so the bat doesnt try to swoop in succession
	 * */
	public bool swoop = false;
	private bool batAtt,batWait = false;

    void Start()
    {
        EnemyState = states.Idle;
        delay = Time.time;
        AIPathing = GetComponent<AIPathCustom>();
        controller = GetComponent<CharacterController>();
		GM= GameManager.Get();
		if (gameObject.tag == "Bat"){
			player = GM.objPlayer.transform;
			tmp = GetComponent<UnitEnemy>();
		}
    }

    void Update()
    {
        //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        if (gameObject.tag == "Alien")
            PlayAlien();
        else if (gameObject.tag == "Bat")
            PlayBat();
		else if (gameObject.tag == "Bear")
			PlayBear();
		else if (gameObject.tag == "Wolf")
			PlayWolf();
		else if (gameObject.tag == "MechBoss")
			PlayMechBoss();
    }

    private void PlayAlien()
    {
        if (EnemyState == states.Idle)
            animation.Play("AlienIdle");
        else if (EnemyState == states.Run)
            animation.Play("AlienWalk");
        else if (EnemyState == states.Attack){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("AlienAttack_copy");
	            delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("AlienAttack_copy")){
					animation.Stop("AlienAttack_copy");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
        }
        else if (EnemyState == states.Death){
            if (!died)
            {
                animation.Play("AlienDead");
				animation["AlienDead"].speed = 1.5f;
                died = true;
            }
        }
        else if (EnemyState == states.Dance){
            if (!died){
                animation.Play("AlienDance");

                if (!animation.IsPlaying("AlienDance"))
                    EnemyState = states.Death;
            }
        }

        if (hitted == true && !died){
            animation.Play("HeadHit");
            hitted = false;
        }
    }

    //Function for Bat enemies
    private void PlayBat()
    {
        if (batAtt && Time.time >= delay + batdelay && !died)
        {
			StopCoroutine("batAttack");
            EnemyState = states.Run;
            animation.Play("BatFly");
			AIPathing.canMove = true;
            AIPathing.canSearch = true;
            tmp.enabled= true;
			speedSwoop = new Vector3(0.0f,0.0f,0.0f);
            batAtt = false;
			swoop = false;
			batWait = true;
			StartCoroutine("attackDelay");
        }

        if (EnemyState == states.Idle && !batAtt)
            animation.PlayQueued("BatFly");
        else if (EnemyState == states.Run && !batAtt)
            animation.PlayQueued("BatFly");
        else if (EnemyState == states.Attack && !batAtt && !batWait)
        {
            AIPathing.canMove = false;
            AIPathing.canSearch = false;
            delay = Time.time;
            dir = player.position - transform.position; // calculate the target direction...
            speedSwoop = new Vector3(dir.x*0.06f,-0.1f,dir.z*0.06f);
			this.transform.LookAt(new Vector3(GM.objPlayer.transform.position.x,this.transform.position.y,GM.objPlayer.transform.position.z));	//Look in the direction of the player
			tmp.enabled = false;
			batAtt = true;
			swoop = true;
        }
		else if (EnemyState == states.Attack && batAtt && !died){
			AIPathing.canMove = false;
			AIPathing.canSearch = false;
			animation.Play("BatAttack");
			StartCoroutine("batAttack");			
		}
		else if (EnemyState == states.Dance){
			if (!died)
            {
			   batAtt = false;
			   StopCoroutine("batAttack");
			   speedSwoop = new Vector3(0.0f,0.0f,0.0f);
               animation.Play("BatDance");
               if (!animation.IsPlaying("BatDance"))
               {
                   EnemyState = states.Death;
               }
			}
		}
        else if (EnemyState == states.Death)
        {
            if (!died)
            {
                animation.Stop();
                animation.Play("BatDead");
				StopCoroutine("batAttack");
				if (!tmp.enabled)
					tmp.enabled = true;
				speedSwoop = new Vector3(0.0f,0.0f,0.0f);
				controller.center = new Vector3(0, 0.03f, 0);
				this.GetComponent<CharacterController>().stepOffset = 0.03f;
				died = true;
            }
        }
        else
			controller.Move(dir * speed * Time.deltaTime);
		
		if (hitted && !died){
			animation.Play("BatHit");
			hitted = false;
		}
    }
	
	IEnumerator batAttack(){
		yield return new WaitForSeconds(1.9f);					//Delay for the bat to attack
		if (swoop && !died && EnemyState != states.Dance){
			this.transform.position += speedSwoop;
			speedSwoop.y += 0.15f*Time.deltaTime;
		}		
	}
	
	IEnumerator attackDelay(){				//Cooldown on the bat's attack so he doesn't swoop in quick succession
		yield return new WaitForSeconds(1.0f);
		batWait = false;
	}
	
	
	private void PlayBear(){				
		if (EnemyState == states.Attack && !died)
        {
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("BearAttack");
	            delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("BearAttack")){
					animation.Stop("BearAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
        }else if (EnemyState == states.Idle && !attacking)
            animation.Play("BearIdle");
        else if (EnemyState == states.Run && !attacking)
            animation.Play("BearWalk");
        else if (EnemyState == states.Death)
        {
            if (!died)
            {
                animation.Stop();
                animation.Play("BearDead");
                Debug.Log(states.Death.ToString());
                died = true;
            }
        }
        else if (EnemyState == states.Dance)
        {
            if (!died)
            {
                animation.Play("BearDance");

                if (!animation.IsPlaying("BearDance"))
                {
                    EnemyState = states.Death;
                }
            }
        }
        if (hitted == true && !died)
        {
            animation.Play("BearHit");
            hitted = false;
        }
	}
	
	private void PlayWolf(){
		/* Steven:
		 * It's the wolf for level 1 (or whichever level)
		 * */
		if (EnemyState == states.Attack && !died){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("WolfAttack");
		        delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("WolfAttack")){
					animation.Stop("WolfAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("WolfIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("WolfWalk");
        else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("WolfDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("WolfDance");
    	        if (!animation.IsPlaying("WolfDance"))
          	        EnemyState = states.Death;
	        }
	    }
        /*if (hitted == true && !died){
           	animation.Play("WolfHit");
            hitted = false;
	    }*/	
	}
	
	private void PlayMechBoss(){
		/* Steven:
		 * Boss of level 1.
		 * */ 
		
		/*To do (ideas...for now):
		* 
		* Idle:
		* That's easy...
		* 
		* Move mech:
		* Either the same as the regular enemies, or have it only walk on the X or Z plane
		* 
		* Attack: Missiles:
		* Random number generated, if it is the one for missiles, stand still, and launch at the player's position
		* when it was decided
		* (Shoot missiles up (however many), after a second or so, randomize positions in the play area for
		* the missiles to land. Put a marker at those spots to identify where they will land)
		* 
		* Death:
		* That's easy too...
		*/
		if (EnemyState == states.Attack && !died){
			if (false){ 								//Gatling attack
				if(!attacking){
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					animation.Play("MechAttack1_copy");
					animation["MechAttack1_copy"].speed = 0.5f;
					attacking = true;
				}else{							
					if(!animation.IsPlaying("MechAttack1_copy")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						animation.Stop("MechAttack1_copy");
						EnemyState = states.Run;
						attacking = false;
					}
				}
			}else{
				if(!attacking){
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					animation.Play("MechAttack2_copy");
					animation["MechAttack2_copy"].speed = 0.5f;
					attacking = true;
				}else{							
					if(!animation.IsPlaying("MechAttack2_copy")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						animation.Stop("MechAttack2_copy");
						EnemyState = states.Run;
						attacking = false;
					}
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("MechIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("MechWalk");
        else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
				animation["MechDead"].speed = 0.5f;
                animation.Play("MechDead");
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
	    }
        else if (EnemyState == states.Jump)
        {
            animation.Play("MechJump");
        }
	}
}
