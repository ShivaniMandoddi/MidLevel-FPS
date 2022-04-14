using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ZombieController : MonoBehaviour
{
    public Animator anim;
    public GameObject target;
    NavMeshAgent agent;
    
    public float walkingSpeed;
    public float runningSpeed;
    public GameObject ragDollPrefab;
    public enum STATE { IDLE, WONDER, CHASE, ATTACK, DEAD }
    public STATE state = STATE.IDLE; // default state
                              // public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        //anim.SetBool("isWalking", true);
        agent = this.GetComponent<NavMeshAgent>();
        //state = STATE.CHASE;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        agent.SetDestination(target.transform.position);
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttacking", false);
        }
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", true);
        }
       
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("isWalking", true);
        }
        else
        if (Input.GetKey(KeyCode.R))
        {
            anim.SetBool("isRunning", true);
        }
        else
        if (Input.GetKey(KeyCode.A))
        {
            anim.SetBool("isAttacking", true);
        }
        else
        if (Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isDead", true);
        }
        */
        if(Input.GetKeyDown(KeyCode.R))
        {
            /*GameObject tempRD = Instantiate(ragDollPrefab, this.transform.position, this.transform.rotation);
            tempRD.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward*10000);
            Destroy(this.gameObject);
            */
            return;
        }
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            return;
        }
        

        switch (state)
        {

            case STATE.IDLE:
                state = STATE.WONDER;
               /* if (CanSeePlayer())
                {
                    state = STATE.CHASE;
                }*/
              
                break;
            case STATE.WONDER:
                if (!agent.hasPath)
                {
                    float randValueX = transform.position.x + Random.Range(-5f, 5f);
                    float randValueZ = transform.position.z + Random.Range(-5f, 5f);
                    float ValueY = Terrain.activeTerrain.SampleHeight(new Vector3(randValueX, 0f, randValueZ));
                    Vector3 destination = new Vector3(randValueX, ValueY, randValueZ);
                    agent.SetDestination(destination);
                    agent.stoppingDistance = 0f;
                    agent.speed = walkingSpeed;
                    TurnOffAllTriggerAnim();
                    anim.SetBool("isWalking", true);
                }
                if (CanSeePlayer())
                {
                    state = STATE.CHASE;
                    //agent.ResetPath();
                }
                /*else if (Random.Range(0, 1000) < 7)
                {
                    state = STATE.IDLE;
                    TurnOffAllTriggerAnim();
                    agent.ResetPath();
                }*/
                break;
            case STATE.CHASE:
                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 2f;
                TurnOffAllTriggerAnim();
                anim.SetBool("isRunning", true);
                agent.speed = runningSpeed;
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    state = STATE.ATTACK;
                }
                if (CanNotSeePlayer())
                {
                    state = STATE.WONDER;
                    agent.ResetPath();
                }
                break;
            case STATE.ATTACK:
                TurnOffAllTriggerAnim();
                anim.SetBool("isAttacking", true);
                transform.LookAt(target.transform.position);          // zomnies should look at player;
                if (DistanceToPlayer() > agent.stoppingDistance + 2f)
                {
                    state = STATE.CHASE;
                }
                //Debug.Log("this is attack state");
                break;
            case STATE.DEAD:
                //GameObject tempRD = Instantiate(ragDollPrefab, this.transform.position, this.transform.rotation);
                //tempRD.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10000);
                Destroy(agent);
                this.GetComponent<SinkToGround>().ReadyToSink();

                break;
            default:
                break;
        }

        //Randomly generate the zombie , mixed zombies ,putting health, ammonation
    }


    public void TurnOffAllTriggerAnim()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isDead", false);
    }


    public bool CanSeePlayer()
    {
        if (DistanceToPlayer() < 10f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float DistanceToPlayer()
    {
        return Vector3.Distance(target.transform.position, this.transform.position);
    }
    public bool CanNotSeePlayer()
    {
        if (DistanceToPlayer() > 15f)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public void KillZombie()
    {
        TurnOffAllTriggerAnim();
        anim.SetBool("isDead", true);
        state = STATE.DEAD;
    }
    int damageAmount = 5;
    public void DamagePlayer()
    {
        target.GetComponent<PlayerController>().TakeHit(damageAmount);
    //create a method name random sound,when player takes damageS   
    }
}
// FPS
// Any fps game with using 
// Navmesh,fps controller , animations, object pool,terrain, 
// finaite state machines and sounds
