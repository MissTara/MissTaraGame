using UnityEngine;
using System.Collections;



public class AIBoss : MonoBehaviour
{

    public enum State
    {
        Charge,
        Backing,
        Defending,
        Attack
    };

    // prefab
    [System.NonSerialized]
    public Transform targetPlayer;

    [System.NonSerialized]
    public Transform bullet;

    public Transform barrel;


    public State bossState = new State();

    public float health = 10;

    public float acceleration = 2f;

    public float maxSpeed = 10f;

    public float minSpeed = 2f;

    [System.NonSerializedAttribute]
    public float currentSpeed = 0;

    public float attackDamage = 10;

    public float distanceBetweenPlayerAndEnemy = 50f;

    public float distanceBetweenBulletAndEnemy = 40f;

    public float COOLDOWNBULLET = 10f;
    protected float coolDownFireBullet = 0;

    public float dodgeAngle = 5f;

    [System.NonSerializedAttribute]
    public float hitTime;

    public float RETREATTIME = 1;


    void Awake()
    {

        hitTime = RETREATTIME;

    }

    // Use this for initialization
    void Start()
    {
        coolDownFireBullet = COOLDOWNBULLET;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer == null)
        {
            targetPlayer = GetComponent<AIPathCustom>().target;
        }
        switch (bossState)
        {
            case State.Charge:
                if (targetPlayer != null)
                    moveAndRotate(targetPlayer);

                break;

            case State.Backing:
                backing();
                break;

            case State.Defending:
                if (targetPlayer != null)
                    defend(targetPlayer);
                break;

            case State.Attack:
                if (targetPlayer != null)
                    attack(targetPlayer);
                break;
        }

    }

    //states
    protected void attack(Transform player)
    {
        if (player != null)
        {
            Vector3 barrelFocus = player.transform.position;
            barrelFocus.y = barrel.transform.position.y;

            Vector3 enemyPos = player.transform.position;
            enemyPos.y = 0;

            transform.LookAt(enemyPos);

            barrel.LookAt(barrelFocus);

            if (Vector3.Distance(this.transform.position, player.transform.position) > distanceBetweenPlayerAndEnemy)
            {
                SpeedUp();
            }
            else
            {
                SlowDown();
                Debug.Log("Enemy Attack");

                coolDownFireBullet -= Time.deltaTime;

            }
            moveForward();
        }
    }

    protected void backing()
    {
        hitTime -= Time.deltaTime;

        retreating();

        if (hitTime <= 0)
        {
            this.bossState = State.Charge;
            hitTime = RETREATTIME;
        }
    }

    protected void defend(Transform player)
    {
        if (player != null)
        {
            if (Vector3.Distance(this.transform.position, player.transform.position) > distanceBetweenPlayerAndEnemy)
            {
                SpeedUp();
            }
            else
            {
                SlowDown();
                if (player.GetComponent<UnitPlayer>().projectile != null)
                {
                    if (Vector3.Distance(this.transform.position, player.GetComponent<UnitPlayer>().projectile.transform.position) < distanceBetweenBulletAndEnemy)
                    {
                        Debug.Log("Enemy Dodge");
                        if (Vector3.Angle(this.transform.position, player.GetComponent<UnitPlayer>().projectile.transform.position) > 0 &&
                            Vector3.Angle(this.transform.position, player.GetComponent<UnitPlayer>().projectile.transform.position) < 180)
                        {
                            Debug.Log("left");
                            dodge("left");
                        }
                        else
                        {
                            Debug.Log("right");
                            dodge("right");
                        }
                    }
                }
                
            }
        }
    }

    void dodge(string direction)
    {
        if (direction.Equals("left"))
            transform.Translate(this.GetComponent<AIPathCustom>().speed * dodgeAngle * Time.deltaTime, 0, this.GetComponent<AIPathCustom>().speed * Time.deltaTime);
        else
            transform.Translate(this.GetComponent<AIPathCustom>().speed * -dodgeAngle * Time.deltaTime, 0, this.GetComponent<AIPathCustom>().speed * Time.deltaTime);
    }

    protected void retreating()
    {
        SpeedUp();
        moveBackward();
    }

    protected void moveAndRotate(Transform player)
    {
        if (player != null)
        {
            Vector3 enemyPos = player.transform.position;
            enemyPos.y = 0;

            transform.LookAt(enemyPos);

            if (Vector3.Distance(this.transform.position, player.transform.position) > distanceBetweenPlayerAndEnemy)
            {
                SpeedUp();
            }
            else
            {
                SlowDown();
            }
            moveForward();
        }
    }

    //enemy movement
    void moveBackward()
    {
        transform.Translate(0, 0, -currentSpeed * Time.deltaTime);
    }

    void moveForward()
    {
        this.transform.Translate(0, 0, currentSpeed * Time.deltaTime);
    }

    void SpeedUp()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;

            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
        }
    }

    void SlowDown()
    {
        if (currentSpeed > 0)
        {
            currentSpeed -= acceleration * Time.deltaTime;
            if (currentSpeed < minSpeed)
                currentSpeed = minSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            bossState = State.Backing;
        }

    }

    void LateUpdate()
    {
    }
}
