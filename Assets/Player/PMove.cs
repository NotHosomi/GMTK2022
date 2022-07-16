using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PMove : MonoBehaviour
{
    //Controller 
    public float mv_gravity;
    public float mv_friction;
    public float mv_maxSpeed;
    public float mv_acceleration;
    public float mv_airacceleration;
    public float mv_deceleration;
    public float mv_airControl; //obsolete
    public float mv_strafeSpeed; //obsolete
    public float mv_jumpSpeed;
    //Camera controls
    float rotX = 0;
    float rotY = 0;
    //components
    private Rigidbody rb;
    private Transform cam_transform;
    private Vector3 vel = Vector3.zero;
    Vector3 wishDir;
    // Jumping
    private bool wishJump = false;
    public bool isGrounded = false;

    LayerMask LM;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        cam_transform = Camera.main.transform;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.Log("no rb");
        }

        LM = ~LayerMask.GetMask("Player_hull", "Player_hitbox", "Projectiles", "3D UI");
    }

    // Update is called once per frame
    void Update()
    {
        //Lock cursor
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetButtonDown("Fire1"))
                Cursor.lockState = CursorLockMode.Locked;
        }

        UnitsPerSecond();
        CameraRotation();
        Movement();
    }

    private void LateUpdate()
    {
        
    }

    private void CameraRotation()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        rotX -= Input.GetAxisRaw("Mouse Y") * 1f;
        rotY += Input.GetAxisRaw("Mouse X") * 1f;
        
        if (rotX < -90)
            rotX = -90;
        else if (rotX > 90)
            rotX = 90;

        Camera.main.transform.rotation = Quaternion.Euler(rotX, rotY, 0);
    }


    /*****************
    *
    *     MOVEMENT 
    * 
    \*****************/
    private void Movement()
    {
        vel = rb.velocity;

        wishDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
            wishDir.z += 1;
        if (Input.GetKey(KeyCode.S))
            wishDir.z -= 1;
        if (Input.GetKey(KeyCode.D))
            wishDir.x += 1;
        if (Input.GetKey(KeyCode.A))
            wishDir.x -= 1;

        CheckGrounded();
        if (isGrounded)
            GroundMove(wishDir);
        else if (!isGrounded)
            AirMove(wishDir);

        rb.velocity = vel;
    }

    private void GroundMove(Vector3 wishDir)
    {
        if(Input.GetKeyDown(KeyCode.Space))
            wishJump = true;
        if(Input.GetKeyUp(KeyCode.Space))
            wishJump = false;
           
        if(vel.y < 0)
            vel.y = 0;
        if (wishJump)
        {
            vel.y = mv_jumpSpeed;
            wishJump = false;
            return;
        }
        Accelerate(wishDir);
        ApplyFriction();
        if (vel.magnitude > mv_maxSpeed)
            vel *= (mv_maxSpeed / vel.magnitude);

    }
    
    private void AirMove(Vector3 wishDir)
    {
        AirAccelerate(wishDir);

        Ray upCast = new Ray(transform.position, Vector3.up);
        bool headBump = Physics.SphereCast(upCast, 0.49f, 0.52f, 9);
        if (headBump)
        {
            if (vel.y > 0)
            {
                vel.y = 0;
            }
        }

        // Apply gravity
        vel.y -= mv_gravity * Time.deltaTime;
    }

    private void Accelerate(Vector3 wishdir)
    {
        //rotate input by camera
        float cam_angle = cam_transform.rotation.eulerAngles.y;
        wishdir = Quaternion.Euler(0, cam_angle, 0) * wishdir;
        wishdir.Normalize();

        //TODO re make
        float currentSpeed = Vector3.Dot(vel.normalized, wishdir) * vel.magnitude;
        float addSpeed = mv_acceleration;
        if (currentSpeed + addSpeed > mv_maxSpeed)
            addSpeed = mv_maxSpeed - currentSpeed;

        Vector3 wishVel = wishdir * addSpeed;
        vel += new Vector3(wishVel.x, vel.y, wishVel.z);

        Debug.DrawRay(transform.position, wishdir * 1f, Color.blue);
        Debug.DrawRay(transform.position, vel.normalized * 3f, Color.yellow);
        Debug.DrawRay(transform.position, wishVel, Color.green);
    }

    private void AirAccelerate(Vector3 wishdir)
    {
        //rotate input by camera
        float cam_angle = cam_transform.rotation.eulerAngles.y;
        wishdir = Quaternion.Euler(0, cam_angle, 0) * wishdir;
        wishdir.Normalize();

        float currentSpeed = Vector3.Dot(vel.normalized, wishdir) * vel.magnitude;
        float addSpeed = mv_airacceleration;
        if (currentSpeed + addSpeed > mv_maxSpeed)
            addSpeed = mv_maxSpeed - currentSpeed;

        Vector3 wishVel = wishdir * addSpeed;
        vel += new Vector3(wishVel.x, vel.y, wishVel.z);

        Debug.DrawRay(transform.position, wishdir * 1f, Color.blue);
        Debug.DrawRay(transform.position, vel.normalized * 3f, Color.yellow);
        Debug.DrawRay(transform.position, wishVel, Color.green);
    }

    private void ApplyFriction()
    {
        Vector3 vec = vel;
        float speed;
        float newspeed;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;

        drop = speed < mv_deceleration ? mv_deceleration : speed;      //c_deceleration if (speed < c_deceleration) is true, speed if (speed < c_deceleration) is false
        drop = drop * mv_friction * Time.deltaTime;

        newspeed = speed - drop;
        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;

        vel.x *= newspeed;
        vel.z *= newspeed;
        newspeeddebug = newspeed;
    }
    float newspeeddebug;

    private void CheckGrounded()
    {
        Collider[] hits = Physics.OverlapBox(transform.position - new Vector3(0, 1, 0),
                                    new Vector3(0.49f, 0.01f, 0.49f),
                                    Quaternion.identity,
                                    LM);
        isGrounded = hits.Length > 0;
    }

    float UPS_timer = 0;
    float UPS;
    float UPF_sum = 0;
    float UPS_rate = 4;
    void UnitsPerSecond()
    {
        UPF_sum += vel.magnitude * Time.deltaTime;
        UPS_timer += Time.deltaTime;
        if (UPS_timer > 1.0 / UPS_rate)
        {
            UPS = Mathf.Round((UPF_sum / UPS_timer)*100)/100;
            UPF_sum = 0;
            UPS_timer -= 1.0f / UPS_rate;
        }
    }


    public GUIStyle guiStyle;
    void OnGUI()
    {
        GUI.Label(new Rect(new Vector2(5,5), new Vector2(300,300)), "UPS: " + UPS + "\nIsGrounded: " + isGrounded, guiStyle);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position - new Vector3(0, 1, 0), new Vector3(0.49f, 0.01f, 0.49f) * 2);
    }
}