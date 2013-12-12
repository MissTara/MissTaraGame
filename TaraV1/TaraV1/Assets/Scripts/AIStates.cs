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
	 * attackWait is a cooldown on all attacks so they dont attack in quick succession
	 * */
	public bool swoop,autoBossAttack = false;
	private bool batAtt,attackWait = false;
	public int bossAttack = 0;
	public float bossAutoDelay = 12.0f;

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
		}else if (gameObject.tag == "MechBoss"){
			StartCoroutine("bossAutoAttack");
		}
    }

    void Update()
    {
		if (gameObject.tag == "Bunny")
			PlayBunny();
        else if (gameObject.tag == "Alien")
            PlayAlien();
        else if (gameObject.tag == "Bat")
            PlayBat();
		else if (gameObject.tag == "Bear")
			PlayBear();
		else if (gameObject.tag == "Wolf")
			PlayWolf();
		else if (gameObject.tag == "Spear")
			PlaySpear();
		else if (gameObject.tag == "MechBoss")
			PlayMechBoss();
		else if (gameObject.tag == "CaptainBoss")
			playCaptainBoss();
    }
	
	private void PlayBunny(){
		if (EnemyState == states.Idle)
            animation.Play("LBunnyIdle");
        else if (EnemyState == states.Run)
			animation.Play("LBunnyHop");
	}

    private void PlayAlien()
    {
        if (EnemyState == states.Idle)
            animation.Play("AlienIdle");
        else if (EnemyState == states.Run)
            animation.Play("AlienWalk");
        else if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("AlienAttack");
	            delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("AlienAttack")){
					animation.Stop("AlienAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
					StartCoroutine("attackDelay");
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
			attackWait = true;
			StartCoroutine("attackDelay");
        }

        if (EnemyState == states.Idle && !batAtt)
            animation.PlayQueued("BatFly");
        else if (EnemyState == states.Run && !batAtt)
            animation.PlayQueued("BatFly");
        else if (EnemyState == states.Attack && !batAtt && !attackWait)
        {
            AIPathing.canMove = false;
            AIPathing.canSearch = false;
            delay = Time.time;
            dir = player.position - transform.position; // calculate the target direction...
            speedSwoop = new Vector3(dir.x*0.03f,-0.1f,dir.z*0.03f);
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
		attackWait = false;
	}	
	
	private void PlayBear(){				
		if (EnemyState == states.Attack && !died && !attackWait){
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
					StartCoroutine("attackDelay");
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
        else if (EnemyState == states.Dance){
            if (!died){
                animation.Play("BearDance");
                if (!animation.IsPlaying("BearDance"))
                    EnemyState = states.Death;
            }
        }
        if (hitted == true && !died){
            animation.Play("BearHit");
            hitted = false;
        }
	}
	
	private void PlayWolf(){
		if (EnemyState == states.Attack && !died && !attackWait){
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
					StartCoroutine("attackDelay");
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
				animation["WolfDead"].speed = 0.5f;
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
        if (hitted == true && !died){
           	animation.Play("WolfHit");
            hitted = false;
	    }
	}
	
	private void PlaySpear(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("SpearAttack");
		        delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("SpearAttack")){
					animation.Stop("SpearAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("SpearIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("SpearWalk");
        else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("SpearDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("SpearDance");
    	        if (!animation.IsPlaying("SpearDance"))
          	        EnemyState = states.Death;
	        }
	    }
        if (hitted == true && !died){
           	animation.Play("SpearHit");
            hitted = false;
	    }
	}
	
	private void PlayMechBoss(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if (bossAttack == 1){ 								//Gatling attack
				if(!attacking){
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					animation.Play("MechAttack1");
					animation["MechAttack1"].speed = 0.5f;
					attacking = true;
					StopCoroutine("bossAutoAttack");
					if(autoBossAttack)
						autoBossAttack = false;
				}else{							
					if(!animation.IsPlaying("MechAttack1")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						animation.Stop("MechAttack1");
						EnemyState = states.Run;
						attacking = false;
						StartCoroutine("bossAutoAttack");
						StartCoroutine("attackDelay");
					}
				}
			}else{
				if(!attacking){									//Missile attack
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					animation.Play("MechAttack2");
					animation["MechAttack2"].speed = 0.5f;
					attacking = true;
					StopCoroutine("bossAutoAttack");
					if(autoBossAttack)
						autoBossAttack = false;
				}else{							
					if(!animation.IsPlaying("MechAttack2")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						animation.Stop("MechAttack2");
						EnemyState = states.Run;
						attacking = false;
						StartCoroutine("bossAutoAttack");
						StartCoroutine("attackDelay");
					}
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("MechIdle");
	    else if (EnemyState == states.Run){
    	    animation.Play("MechWalk");
			animation["MechWalk"].speed = 0.5f;
		}else if (EnemyState == states.Death){
            if (animation.IsPlaying("MechJump")){
                animation.Stop();
                animation.Play("MechDead");
            }
    	    if (!died){
            	animation.Stop();
				animation["MechDead"].speed = 0.5f;
                animation.Play("MechDead");
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
			 if (!died)
				animation.Play("MechDance");
	    }
        else if (EnemyState == states.Jump)
            animation.Play("MechJump");
	}
	
	IEnumerator getBossAttack(){
		bossAttack = Random.Range(0,2);
		yield return new WaitForSeconds(0.2f);
		StopCoroutine("getBossAttack");
	}
	
	IEnumerator bossAutoAttack(){
		if(!GameManager.isPaused){
			yield return new WaitForSeconds(bossAutoDelay);
			if(!autoBossAttack){
				autoBossAttack = true;
				StartCoroutine("getBossAttack");
				EnemyState = states.Attack;
			}
			autoBossAttack = false;
		}
	}
	
	private void playCaptainBoss(){
		/* Steven:
		 * Boss of stage 2.
		 * Attack1: Left hook: Slow at the start to give the player time to react (like 1/4 speed), then return to normal speed
		 * Attack2: Slam: Normal speed for the windup, then slow it down at the apex (1/10 speed), then back to normal for the attack
		 * */
		if (EnemyState == states.Attack && !died && !attackWait){
			if (bossAttack == 1){ 								
				if(!attacking){
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					
					animation.Play("captainBossAttack1");
					animation["captainBossAttack1"].speed = 0.25f;
					attacking = true;
				}else{							
					if(!animation.IsPlaying("captainBossAttack1")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						transform.FindChild("polySurface1").GetComponent<BoxCollider>().enabled = false;
						EnemyState = states.Run;
						attacking = false;
						StartCoroutine("attackDelay");
					}
				}
			}else{
				if(!attacking){									
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					animation.Play("captainBossAttack2");
					attacking = true;
				}else{							
					if(!animation.IsPlaying("captainBossAttack2")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						transform.FindChild("polySurface1").GetComponent<BoxCollider>().enabled = false;
						EnemyState = states.Run;
						attacking = false;
						StartCoroutine("attackDelay");
					}
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("captainBossIdle");
	    else if (EnemyState == states.Run){
    	    animation.Play("captainBossWalk");
		}else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("captainBossDead");
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
			 if (!died)
				animation.Play("captainBossDance");
	    }
	}
}
