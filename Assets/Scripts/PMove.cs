using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PMove : MonoBehaviour
{
    //Controller 
    public float g_gravity = 20.0f;
    public float g_friction = 6;
    public float mv_maxSpeed = 15.0f;
    public float mv_acceleration = 14.0f;
    public float mv_deceleration = 10.0f;
    public float mv_airControl = 0.3f; //obsolete
    public float mv_strafeSpeed = 1.0f; //obsolete
    public float mv_jumpSpeed = 8.0f;
    //Camera controls
    float rotX = 0;
    float rotY = 0;
    //frame shit
    public float fpsDisplayRate = 4.0f; // 4 updates per sec
    private int frameCount = 0;
    private float dt = 0.0f;
    //components
    private Rigidbody rb;
    private Transform cam_transform;
    private Vector3 vel = Vector3.zero;
    //Q3 Jump Queue
    private bool wishJump = false;
    public bool isGrounded = false;

    public Animator c_animator;

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

        CameraRotation();

        Movement();
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

        Vector3 wishDir = new Vector3(0, 0, 0);
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
           

        vel.y = 0;
        if (wishJump)
        {
            vel.y = mv_jumpSpeed;
            wishJump = false;
            return;
        }
        Accelerate(wishDir);
        float magni = rb.velocity.magnitude;
        if (wishDir.sqrMagnitude == 0)
            ApplyFriction();
        else if (magni > mv_maxSpeed)
            rb.velocity *= (mv_maxSpeed / magni);

    }
    
    private void AirMove(Vector3 wishDir)
    {
        Accelerate(wishDir);

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
        vel.y = vel.y - (g_gravity * Time.deltaTime);
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

    private void ApplyFriction()
    {
        rb.velocity *= 0;
        return;

        Vector3 vec = vel;
        float speed;
        float newspeed;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        //Only if the player is on the ground apply friction
        if (isGrounded)
        {
            drop = speed < mv_deceleration ? mv_deceleration : speed;      //c_deceleration if (speed < c_deceleration) is true, speed if (speed < c_deceleration) is false
            drop = drop * g_friction * Time.deltaTime;
        }

        newspeed = speed - drop;
        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;

        vel.x *= newspeed;
        vel.y *= newspeed;
    }

    private void CheckGrounded()
    {
        LayerMask LM = LayerMask.GetMask("Player", "Projectiles");
        Collider[] hits = Physics.OverlapBox(transform.position - new Vector3(0, -1, 0),
                                    new Vector3(0.49f, 0.01f, 0.49f),
                                    Quaternion.identity,
                                    LM);
        isGrounded = hits.Length > 0;
    }


    public GUIStyle guiStyle;
    public GameObject UPS_bar;
    void OnGUI()
    {
        Vector3 v = rb.velocity;
        v.y = 0;
        GUI.Label(new Rect(new Vector2(5,5), new Vector2(300,300)), "UPS: " + v.magnitude, guiStyle);
        Vector3 s = UPS_bar.transform.localScale;
        s.x = 0.001f * v.magnitude;
        UPS_bar.transform.localScale = s;
    }
}