using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MovementPlayer : MonoBehaviour
{

    //Text
    public Text textoAirtime;
    //Colliders
    public GameObject standCollider;
    public GameObject crouchCollider;

    //Camera Position
    [SerializeField] private GameObject cameraPosition;

    [Header("Camera")]


    [Header("Movement")]
    [SerializeField] public float horizontalVelocity;
    [SerializeField] public float verticalVelocity;
    [SerializeField] public float velocityX;
    [SerializeField] public float velocityZ;
    [SerializeField] private float moveSpeed;
    public float maxSpeed = 10f; // Set your desired max speed here
    public float maxFallSpeed;

    public float groundDrag;
    public float airDrag;
    public float dashspeed;
    public float dashSpeedChangeFactor;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float maxYSpeed;
    [SerializeField] bool readyToJump;

    public float walkSpeed;
    public float sprintSpeed;
    public float crouchSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    [SerializeField] bool grounded;
    [SerializeField] bool crouching;

    [Header("Slope Handling")]
    public float maxSlopAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Ceiling Check")]
    public Vector3 ceilingCheckSize = new Vector3(1f, 0.1f, 1f); // Size of the box used for ceiling check
    public float ceilingCheckDistance = 1.0f;
    public LayerMask ceilingLayer;

    [Header("Crouch")]

    public float crouchYColScale;
    private float startYColScale;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    ConstantForce grav;
    [SerializeField] float gravityForce = -981;
    [SerializeField] public float gravityScale = 2.0f;


    float defaultGravity;

    public float airTime;
    [SerializeField] private float timer1 = 0;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        air,
        dashing,
        crouching
    }

    public bool dashing;

    private float desiredMoveSpeed;
    private float lastdesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    public void StateHandler()
    {
        //Mode - Dashing
        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashspeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        if (grounded && Input.GetKey(sprintKey) && state != MovementState.crouching)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (grounded && Input.GetKey(crouchKey) && state != MovementState.sprinting)
        {

            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {

            state = MovementState.air;

            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed;

        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastdesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;
        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }
        lastdesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }


    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }
        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        grav = GetComponent<ConstantForce>();
        rb.freezeRotation = true;

        readyToJump = true;
        rb.useGravity = false;
    }

    private void Update()
    {
        Vector3 boxCenter = transform.position + Vector3.down * (playerHeight * 0.5f);
        Vector3 boxHalfExtents = new Vector3(0.45f, 0.1f, 0.45f); // Adjust the y value (0.1f) based on how close to the ground you want the check to be
                                                                  /////////////////////////////////////player widht    playerwith
        grounded = Physics.CheckBox(boxCenter, boxHalfExtents, Quaternion.identity, ground);

        
        MyInput();
        SpeedControl();
        StateHandler();
        
        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
        
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        MovePlayer2();
        ChangeGravity();
        
    }
    private bool IsCeilingAbove()
    {
        Vector3 ceilingCheckPosition = transform.position + Vector3.up * (playerHeight * 0.5f + ceilingCheckDistance);
        return Physics.CheckBox(ceilingCheckPosition, ceilingCheckSize / 2, Quaternion.identity, ceilingLayer);
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded && !crouching)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if ((Input.GetKey(crouchKey)) && !Input.GetKey(sprintKey))
        {
            Vector3 crouchCamPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            cameraPosition.transform.position = crouchCamPos;


            crouchCollider.SetActive(true);
            standCollider.SetActive(false);

            crouching = true;
        }
        if ((!Input.GetKey(crouchKey)))
        {
            if (IsCeilingAbove())
            {

            }
            else
            {
                Vector3 standCamPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                cameraPosition.transform.position = standCamPos;

                crouchCollider.SetActive(false);
                standCollider.SetActive(true);

                crouching = false;
            }
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);


    }

    private void ChangeGravity()
    {
        if (rb.velocity.y < 0 && !grounded)
        {
            if (timer1 <= airTime)
            {
                timer1 = timer1 + Time.deltaTime;
                textoAirtime.text = timer1.ToString();
                grav.relativeForce = new Vector3(0, -981f, 0);
            }
            else
            {

                if (grav.relativeForce.y > maxFallSpeed)
                {
                    grav.relativeForce = new Vector3(0, grav.relativeForce.y * gravityScale, 0);
                    //print(grav.relativeForce);
                }
            }

        }
        else
        {
            grav.relativeForce = new Vector3(0, gravityForce, 0);
            //print(grav.relativeForce);
            timer1 = 0;

        }
    }
    private void MovePlayer2()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);



        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // limit player's speed to maxSpeed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        //on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        }

        //turn gravity off while on slope
        grav.enabled = !OnSlope();
        

    }
    private void CalculateVelocity()
    {
        // Create a vector for the horizontal velocity (ignore the y component)
        Vector3 horizontalVelocityTemp = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // Calculate the magnitude of the horizontal velocity vector
        horizontalVelocity = horizontalVelocityTemp.magnitude;

        //vertical velocity 
        verticalVelocity = rb.velocity.y;
        velocityX = rb.velocity.x;
        velocityZ = rb.velocity.z;
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //limit spped on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        //limit Y speed
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);

    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
    private void ResetJump()
    {
        exitingSlope = false;

        readyToJump = true;
    }
    private void OnDrawGizmosSelected()
    {
        // Ground Check Visualization
        Gizmos.color = Color.green;
        Vector3 groundCheckPosition = transform.position + Vector3.down * (playerHeight * 0.5f + 0.2f);
        Gizmos.DrawLine(transform.position, groundCheckPosition);
        Gizmos.DrawWireSphere(groundCheckPosition, 0.1f);

        // Ceiling Check Visualization
        Gizmos.color = Color.red;
        Vector3 ceilingCheckPosition = transform.position + Vector3.up * (playerHeight * 0.5f + ceilingCheckDistance);
        Gizmos.DrawWireCube(ceilingCheckPosition, ceilingCheckSize);
    }
}
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Rendering;

//public class MovementPlayer : MonoBehaviour
//{
//    // Colliders
//    public GameObject standCollider;
//    public GameObject crouchCollider;

//    // Camera Position
//    [SerializeField] private GameObject cameraPosition;

//    [Header("Camera")]


//    [Header("Movement")]
//    private float moveSpeed;
//    public float maxSpeed = 10f; // Set your desired max speed here
//    public float maxFallSpeed;

//    public float groundDrag;
//    public float airDrag;
//    public float dashspeed;
//    public float dashSpeedChangeFactor;
//    public float jumpForce;
//    public float jumpCooldown;
//    public float airMultiplier;
//    [SerializeField] bool readyToJump;

//    public float walkSpeed;
//    public float sprintSpeed;
//    public float crouchSpeed;

//    [Header("Keybinds")]
//    public KeyCode jumpKey = KeyCode.Space;
//    public KeyCode sprintKey = KeyCode.LeftShift;
//    public KeyCode crouchKey = KeyCode.C;

//    [Header("Ground Check")]
//    public float playerHeight;
//    public LayerMask ground;
//    [SerializeField] bool grounded;
//    [SerializeField] bool crouching;

//    [Header("Crouch")]
//    public float crouchYColScale;
//    private float startYColScale;

//    [Header("Ceiling Check")]
//    public Vector3 ceilingCheckSize = new Vector3(1f, 0.1f, 1f); // Size of the box used for ceiling check
//    public float ceilingCheckDistance = 1.0f;
//    public LayerMask ceilingLayer;

//    public Transform orientation;

//    float horizontalInput;
//    float verticalInput;

//    Vector3 moveDirection;

//    Rigidbody rb;
//    ConstantForce grav;
//    [SerializeField] float gravityForce = -981;
//    [SerializeField] public float gravityScale = 2.0f;

//    float defaultGravity;

//    public MovementState state;
//    public enum MovementState
//    {
//        walking,
//        sprinting,
//        air,
//        dashing,
//        crouching
//    }

//    public bool dashing;

//    private float desiredMoveSpeed;
//    private float lastdesiredMoveSpeed;
//    private MovementState lastState;
//    private bool keepMomentum;

//    public void StateHandler()
//    {
//        // Mode - Dashing
//        if (dashing)
//        {
//            state = MovementState.dashing;
//            desiredMoveSpeed = dashspeed;
//            speedChangeFactor = dashSpeedChangeFactor;
//        }

//        if (grounded && Input.GetKey(sprintKey) && state != MovementState.crouching)
//        {
//            state = MovementState.sprinting;
//            desiredMoveSpeed = sprintSpeed;
//        }
//        else if (grounded && Input.GetKey(crouchKey) && state != MovementState.sprinting)
//        {
//            state = MovementState.crouching;
//            desiredMoveSpeed = crouchSpeed;
//        }
//        else if (grounded)
//        {
//            state = MovementState.walking;
//            desiredMoveSpeed = walkSpeed;
//        }
//        else
//        {
//            state = MovementState.air;

//            if (desiredMoveSpeed < sprintSpeed)
//                desiredMoveSpeed = walkSpeed;
//            else
//                desiredMoveSpeed = sprintSpeed;
//        }

//        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastdesiredMoveSpeed;
//        if (lastState == MovementState.dashing) keepMomentum = true;
//        if (desiredMoveSpeedHasChanged)
//        {
//            if (keepMomentum)
//            {
//                StopAllCoroutines();
//                StartCoroutine(SmoothlyLerpMoveSpeed());
//            }
//            else
//            {

//                moveSpeed = desiredMoveSpeed;
//            }
//        }
//        lastdesiredMoveSpeed = desiredMoveSpeed;
//        lastState = state;
//    }

//    private float speedChangeFactor;
//    private IEnumerator SmoothlyLerpMoveSpeed()
//    {
//        float time = 0;
//        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
//        float startValue = moveSpeed;

//        float boostFactor = speedChangeFactor;

//        while (time < difference)
//        {
//            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

//            time += Time.deltaTime * boostFactor;

//            yield return null;
//        }
//        moveSpeed = desiredMoveSpeed;
//        speedChangeFactor = 1f;
//        keepMomentum = false;
//    }

//    private void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        grav = GetComponent<ConstantForce>();
//        rb.freezeRotation = true;

//        readyToJump = true;
//        rb.useGravity = false;
//    }

//    private void Update()
//    {
//        // ground check
//        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

//        MyInput();
//        SpeedControl();
//        StateHandler();

//        // handle drag
//        if (grounded)
//            rb.drag = groundDrag;
//        else
//            rb.drag = airDrag;
//    }

//    private void FixedUpdate()
//    {
//        MovePlayer2();
//        ChangeGravity();
//    }

//    private void MyInput()
//    {
//        horizontalInput = Input.GetAxisRaw("Horizontal");
//        verticalInput = Input.GetAxisRaw("Vertical");

//        // when to jump
//        if (Input.GetKey(jumpKey) && readyToJump && grounded && !crouching)
//        {
//            readyToJump = false;

//            Jump();

//            Invoke(nameof(ResetJump), jumpCooldown);
//        }

//        if ((Input.GetKey(crouchKey)) && !Input.GetKey(sprintKey))
//        {
//            Vector3 crouchCamPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
//            cameraPosition.transform.position = crouchCamPos;

//            crouchCollider.SetActive(true);
//            standCollider.SetActive(false);

//            crouching = true;
//        }
//        if ((Input.GetKeyUp(crouchKey)))
//        {
//            if (!IsCeilingAbove())
//            {
//                Vector3 standCamPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
//                cameraPosition.transform.position = standCamPos;

//                crouchCollider.SetActive(false);
//                standCollider.SetActive(true);

//                crouching = false;
//            }
//        }
//    }

//    private bool IsCeilingAbove()
//    {
//        Vector3 ceilingCheckPosition = transform.position + Vector3.up * (playerHeight * 0.5f + ceilingCheckDistance);
//        return Physics.CheckBox(ceilingCheckPosition, ceilingCheckSize / 2, Quaternion.identity, ceilingLayer);
//    }

//    private void MovePlayer()
//    {
//        // calculate movement direction
//        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

//        // on ground
//        if (grounded)
//            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

//        // in air
//        else if (!grounded)
//            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
//    }

//    private void ChangeGravity()
//    {
//        if (rb.velocity.y < 0 && !grounded)
//        {
//            if (grav.relativeForce.y > maxFallSpeed)
//            {
//                grav.relativeForce = new Vector3(0, grav.relativeForce.y * gravityScale, 0);
//                print(grav.relativeForce);
//            }
//        }
//        else
//        {
//            grav.relativeForce = new Vector3(0, gravityForce, 0);
//            print(grav.relativeForce);
//        }
//    }

//    private void MovePlayer2()
//    {
//        // calculate movement direction
//        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

//        // on ground
//        if (grounded)
//            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

//        // in air
//        else if (!grounded)
//            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

//        // limit player's speed to maxSpeed
//        if (rb.velocity.magnitude > maxSpeed)
//        {
//            rb.velocity = rb.velocity.normalized * maxSpeed;
//        }
//    }

//    private void SpeedControl()
//    {
//        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

//        // limit velocity if needed
//        if (flatVel.magnitude > moveSpeed)
//        {
//            Vector3 limitedVel = flatVel.normalized * moveSpeed;
//            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
//        }
//    }

//    private void Jump()
//    {
//        // reset y velocity
//        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

//        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
//    }

//    private void ResetJump()
//    {
//        readyToJump = true;
//    }
//}
