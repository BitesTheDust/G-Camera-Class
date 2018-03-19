using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] GameObject go_Player;

    [SerializeField] Vector3 v_RayPositionOne,
            v_RayPositionTwo, 
            v_RayPositionThree;

    private List<Vector3> _rayPositions;

    [SerializeField] float f_CamDistance;
    [SerializeField] float f_CamZoomSpeed;
    [SerializeField] float f_PosLerpSpeed;
    [SerializeField] float f_RotLerpSpeed;

    private float f_CurrentCamDistance;
    private Ray r_Ray;
    private RaycastHit _hit;

    public Vector3 PlayerBehind
    {
        get { return -go_Player.transform.forward; }
    }

    private void Start()
    {
        _rayPositions = new List<Vector3>();
        _rayPositions.Add(v_RayPositionOne);
        _rayPositions.Add(v_RayPositionTwo);
        _rayPositions.Add(v_RayPositionThree);
    }

	private void Update()
    {
        bool validCamPos = CastRays( f_CurrentCamDistance );

        float suggestedDistance = ZoomAdjust(validCamPos);

        if( !validCamPos ) 
        {
            f_CurrentCamDistance = suggestedDistance;
        }
        else 
        {
            validCamPos = CastRays( suggestedDistance );

            if( validCamPos )
                f_CurrentCamDistance = suggestedDistance;
        }

        transform.position = Vector3.Slerp(transform.position, GetIdealPos(), f_PosLerpSpeed);

        Vector3 direction = Vector3.Normalize(go_Player.transform.position - transform.position);
        transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), f_RotLerpSpeed);
    }

    private Vector3 GetRayPos(Vector3 rayPos)
    {
        return go_Player.transform.position + rayPos;
    }

    private Vector3 GetIdealPos()
    {
        return go_Player.transform.position + (PlayerBehind * f_CurrentCamDistance);
    }

    private float ZoomAdjust(bool valid)
    {
        float newValue = f_CurrentCamDistance;

        if (!valid)
        {
            newValue -= f_CamZoomSpeed;
        }
        else if(f_CurrentCamDistance < f_CamDistance)
        {
            newValue += f_CamZoomSpeed;
        }

        return newValue = Mathf.Clamp(newValue, 0.1f, f_CamDistance);
    }

    private bool CastRays( float distance ) 
    {
        bool validPos = true;

        foreach(Vector3 rayPos in _rayPositions)
        {
            r_Ray = new Ray(GetRayPos(rayPos), PlayerBehind);

            if (Physics.Raycast(r_Ray, out _hit, distance ))
            {
                Debug.DrawLine(r_Ray.origin, _hit.point, Color.black);
                validPos = false;
            }
            else
            {
                Debug.DrawLine(r_Ray.origin, r_Ray.origin + (PlayerBehind * f_CamDistance), Color.green);
            }
        }

        return validPos;
    }
}
