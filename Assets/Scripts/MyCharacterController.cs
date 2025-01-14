using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.3f;
    [SerializeField] private float playerGravity = 0.04f;
    [SerializeField] private float groundedRadius = 1.1f;
    [SerializeField] private float JumpStrength = 0.22f;
    [SerializeField] private float JumpFalloff = 0.06f;
    [SerializeField] private float JumpHang = 0.02f;
    [SerializeField] private int CoyoteFrames = 2;

    private CharacterController ctrl;
    private bool grounded;
    private bool canJump;
    private bool jumping;
    private int remCoyote;
    private float remJump;
    private Vector3 playerVelocity;

    // Start is called before the first frame update
    void Start()
    {
        ctrl = gameObject.GetComponent<CharacterController>();
        if (ctrl == null) Debug.LogError("Character missing charactercontroller: " + gameObject.name);
        jumping = false;
    }

    // Read jump key
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            Debug.Log("jump!");
            remJump = JumpStrength;
            jumping = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            jumping = false;
        }


    }

    // Do physics
    private void FixedUpdate()
    {
        grounded = Grounded();// only need one cast per physics frame

        //if grounded, reset jump and coyote time. If not, apply gravity acceleration
        if (grounded)
        {
            canJump = true;
            remCoyote = CoyoteFrames;
            Debug.Log("Character grounded");
            playerVelocity.y = -playerGravity;
        }
        else if( CoyoteFrames < 1) playerVelocity.y -= playerGravity;
        else CoyoteFrames--;

        // calculate vertical movement
        if (jumping)
        {
            canJump = false; //ensure we can't start another jump until grounded and button released
            playerVelocity.y += remJump;
            remJump = Mathf.Max(remJump -= JumpFalloff, JumpHang);
            Debug.Log("Jumping");
        }

        // calculate side movement
        playerVelocity.x = Input.GetAxis("Horizontal") * moveSpeed;

        //if in 3D, do forward movement
        //TODO

        // do calculated movement
        ctrl.Move(playerVelocity);
    }

    // Replacement for unreliable or non-working CharacterController collision detection
    private bool Grounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, groundedRadius)) return true;
        return false;
    }
}
