using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float playerSpeed;
    public float playerJumpForce;
    public float playerRotationSpeed;
    public Transform bulletLaunch;
    Rigidbody rb;
    CapsuleCollider colliders;
    Quaternion camRotation;
    public Camera cam;
    Quaternion playerRotation;
    float minX=-90f;
    float maxX = 90f;
    public Animator animator;
    float inputX;
    float inputz;
    int ammo = 50;
    int medical = 100;
    int maxAmmo = 100;
    int maxMedical = 100;
    int reloadAmmo = 0;
    int maxReloadAmmo = 10;
    public GameObject steveModelPrefab;
    int deathCount;
    public Slider health;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponent<CapsuleCollider>();
        
        //animator = GetComponent<Animator>();
        //audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        health.value = (float)medical / 100f;
        Debug.Log(health.value);
       if(Input.GetKeyDown(KeyCode.F))
        {
            animator.SetBool("IsAiming", !animator.GetBool("IsAiming"));
        }
       if(Input.GetMouseButtonDown(0) && !animator.GetBool("IsFiring"))
        {
            if (ammo > 0)
            {
                //animator.SetBool("IsFiring", !animator.GetBool("IsFiring"));
                animator.SetTrigger("IsFiring");
                WhenZombieGotHit();
                ammo = Mathf.Clamp(ammo - 1, 0, maxAmmo);
               // Debug.Log("Ammo Fire Value: "+ammo);
            }
            else
            {
                // Trigger the sound for empty bullets 
            }
        }
       if(Input.GetKeyDown(KeyCode.R))
        {
            
            animator.SetTrigger("IsReload");
            int amountAmmoNeeded = maxReloadAmmo-reloadAmmo;
            
            int ammoAvailable = amountAmmoNeeded < ammo ? amountAmmoNeeded: ammo;
            
            reloadAmmo += ammoAvailable;
            ammo -= ammoAvailable;
            Debug.Log("Ammo Loaded Value: "+ammo);
            Debug.Log("Ammo Reloaded Value: " + reloadAmmo);
        }
        if(Mathf.Abs(inputX)>0|| Mathf.Abs(inputz)>0)
        {
            if(!animator.GetBool("IsWalking"))
                animator.SetBool("IsWalking", true);
        }
        else if(animator.GetBool("IsWalking"))
        {
            animator.SetBool("IsWalking", false);
        }
      
    }

    private void WhenZombieGotHit()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(bulletLaunch.position,bulletLaunch.forward,out hitInfo,100f))
        {
            GameObject hitZombie = hitInfo.collider.gameObject;
            if(hitZombie.tag=="Zombie")
            {
                deathCount++;
                Debug.Log("DeathCount: " + deathCount);
                if (UnityEngine.Random.Range(0, 5) < 3)
                {
                    /*   GameObject tempRD = Instantiate(ragDollPrefab, this.transform.position, this.transform.rotation);
                       tempRD.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10000);
                       Destroy(this.gameObject);*/
                    GameObject tempRD = hitZombie.GetComponent<ZombieController>().ragDollPrefab;
                    GameObject newTempRD = Instantiate(tempRD, hitZombie.transform.position, hitZombie.transform.rotation);
                    newTempRD.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10000);
                    Destroy(hitZombie);
                }
                else
                {
                    hitZombie.GetComponent<ZombieController>().KillZombie();
                }
                if(deathCount==GameObject.Find("SpawnPoint").GetComponent<SpawnManager>().number)
                {
                    ZombiesCompleted();
                }

            }
        }
    }

    private void ZombiesCompleted()
    {
        Debug.Log("GameOver");
        Vector3 position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(this.transform.position), transform.position.z);
        GameObject tempSteve = Instantiate(steveModelPrefab, position, this.transform.rotation);
        tempSteve.GetComponent<Animator>().SetTrigger("Dance");
        GameStart.isGameOver = true;
        Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        inputX = Input.GetAxis("Horizontal") * playerSpeed;
        inputz = Input.GetAxis("Vertical") * playerSpeed;

        //transform.position += new Vector3(inputX , 0f, inputz);
        transform.Translate(inputX, 0f, inputz);
        
        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * playerJumpForce);
           
        }
        float mouseX = Input.GetAxis("Mouse X")*playerRotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y")*playerRotationSpeed;

        playerRotation = Quaternion.Euler(0f, mouseX, 0f);//*playerRotation;
        camRotation = Quaternion.Euler(-mouseY, 0f,0f)*camRotation;
        camRotation = ClampRotationPlayer(camRotation);
        //this.transform.localRotation = playerRotation;
       // Debug.Log(playerRotation);
        //transform.localRotation = playerRotation*transform.localRotation;
        transform.rotation = playerRotation * transform.rotation;
        cam.transform.localRotation = camRotation;
       
    }
    public bool IsGrounded()
    {
        RaycastHit rayCastHit;
        if(Physics.SphereCast(transform.position,colliders.radius,Vector3.down,out rayCastHit,(colliders.height/2)-colliders.radius+0.1f))
        {
            return true;
        }
        else
            return false;
       
    }
    public Quaternion ClampRotationPlayer(Quaternion n)
    {
       
        n.w = 1f;
        n.x /=n.w;
        n.y /= n.w;
        n.z /= n.w;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(n.x);

        angleX = Mathf.Clamp(angleX, minX, maxX);
        n.x = Mathf.Tan(Mathf.Deg2Rad * angleX*0.5f);
        return (n);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Ammo" && ammo<maxAmmo) 
        {
            Debug.Log("Collected box");
            //ammo += 10;
            ammo = Mathf.Clamp(ammo + 10, 0, maxAmmo);
            Debug.Log("Ammo: " + ammo);
            collision.gameObject.SetActive(false);
        }
        if(collision.gameObject.tag == "Medical" && medical<maxMedical)
        {
            Debug.Log("Collected medical box");
            //medical += 10;
            medical = Mathf.Clamp(medical + 10, 0, maxMedical);
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag=="Lava")
        {
            // Need to Trigger dead sound , when medical is zero
            medical = Mathf.Clamp(medical - 10, 0, maxMedical);
            Debug.Log("Medical: "+medical);
        }
    }
    public void TakeHit(int value)
    {
        medical = Mathf.Clamp(medical - value, 0, maxMedical); // medical=Health
        print("Health: " + medical);
        if (medical <= 0)
        {
            Vector3 position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(this.transform.position), transform.position.z);
            GameObject tempSteve = Instantiate(steveModelPrefab, position, this.transform.rotation);
            tempSteve.GetComponent<Animator>().SetTrigger("Death");
            GameStart.isGameOver = true;
            Destroy(this.gameObject);
        }
            
    }
    
}

