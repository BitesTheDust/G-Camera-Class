using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc_PlayerControl;

    private int i_jumps = 1;

    private float f_jumpVelocity;

    private Vector3 v_velocity;

    [SerializeField] private float f_moveSpeed;
    [SerializeField] private float f_jumpPower;
    [SerializeField] private float f_turnSpeed;


    private void Start ()
    {
        cc_PlayerControl = GetComponent<CharacterController>();
	}
	
	private void Update ()
    {
        v_velocity = Vector3.zero;

        v_velocity.x = (Input.GetAxis("Horizontal") * f_moveSpeed * Time.deltaTime);
        v_velocity.z = (Input.GetAxis("Vertical") * f_moveSpeed * Time.deltaTime);
        v_velocity.y = 0;

        cc_PlayerControl.Move(v_velocity);

        Vector3 direction = Vector3.Normalize(v_velocity - transform.position);

        if(v_velocity != Vector3.zero)
            transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(v_velocity, Vector3.up), f_turnSpeed * Time.deltaTime);

        RaycastHit hit;
        Ray downwards = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(downwards, out hit))
        {
            if (hit.distance < 2)
            {
                if(i_jumps < 1)
                    i_jumps = 1;

                f_jumpVelocity = 0;
            }
            else
            {
                f_jumpVelocity -= (f_jumpPower * 2) * Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && i_jumps > 0)
        {
            i_jumps--;
            f_jumpVelocity = f_jumpPower;
        }

        cc_PlayerControl.Move(new Vector3(0, f_jumpVelocity, 0));
    }

    private void Jump()
    {
        
    }
}
