using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float playerSpeed;
    public float playerJumpForce;
    public float playerRotationSpeed;
    Rigidbody rb;
    CapsuleCollider colliders;
    Quaternion playerRotation;
    Quaternion camRotation;
    public Camera cam;
    public float minX=-90f;
    public float maxX = 90f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
       
      
    }
    private void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal") * playerSpeed;
        float inputz = Input.GetAxis("Vertical") * playerSpeed;
        
        transform.position += new Vector3(inputX, 0f, inputz);
        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * playerJumpForce);
        }
        float mouseX = Input.GetAxis("Mouse X")*playerRotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y")*playerRotationSpeed;
        Debug.Log(mouseY);
        playerRotation = Quaternion.Euler(0f, mouseX, 0f)*playerRotation;
        camRotation = Quaternion.Euler(-mouseY, 0f,0f)*camRotation;
        camRotation = ClampRotationPlayer(camRotation);
        this.transform.localRotation = playerRotation;
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
}
