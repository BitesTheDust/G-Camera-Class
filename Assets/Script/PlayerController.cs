using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc_PlayerControl;
    private float f_jumpVelocity;
    private Vector3 v_velocity;
    private Vector3 v_direction;

    [SerializeField] private float f_moveSpeed;
    [SerializeField] private float f_jumpPower;
    [SerializeField] private float f_gravity;
    [SerializeField] private float f_turnSpeed;
    [SerializeField] private float f_maxAngle;


    private void Start ()
    {
        cc_PlayerControl = GetComponent<CharacterController>();
        v_velocity = Vector3.zero;
	}
	
	private void Update ()
    {
        v_velocity = new Vector3( Input.GetAxis("Horizontal") * f_moveSpeed, v_velocity.y, Input.GetAxis("Vertical") * f_moveSpeed );

        v_direction = new Vector3( v_velocity.x, 0, v_velocity.z );

        if(v_direction != Vector3.zero) 
        {
            Quaternion destinyRotation =  Quaternion.LookRotation(v_direction, Vector3.up);
            float angle = Quaternion.Angle( transform.rotation, destinyRotation );
            if( angle > f_maxAngle ) 
            {
                v_velocity = new Vector3( 0, v_velocity.y, 0 );
            }

            transform.localRotation = Quaternion.Slerp(transform.rotation, destinyRotation, f_turnSpeed * Time.deltaTime);
        }

        if (cc_PlayerControl.isGrounded)
        {
            if ( Input.GetKeyDown( KeyCode.Space ) ) 
            {
                v_velocity.y = 0;
                v_velocity.y = f_jumpPower;
            }
        }
        else 
        {
            v_velocity.y -= ( f_jumpPower * 0.95f ) * Time.deltaTime;
        }

        v_velocity.y -= f_gravity * Time.deltaTime;
    }

    private void FixedUpdate() 
    {
        cc_PlayerControl.Move( v_velocity * Time.fixedDeltaTime );
    }
}
