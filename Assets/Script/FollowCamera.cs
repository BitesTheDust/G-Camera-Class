using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] GameObject go_Player;
    [SerializeField] CameraRay cr_Top,
            cr_Middle,
            cr_Bottom;

    private List<CameraRay> _rays;

    [SerializeField] float f_CamDistance;
    [SerializeField] float f_CamZoomSpeed;
    [SerializeField] float f_PosLerpSpeed;
    [SerializeField] float f_RotLerpSpeed;

    [SerializeField] float f_maxYAdjust;
    [SerializeField] float f_YAdjustSpeed;

    [SerializeField] LayerMask lm_Ignore;

    private float f_currentYAdjust = 0.0f;
    private float f_CurrentCamDistance;
    private Ray r_Ray;
    private RaycastHit _hit;

    public Vector3 PlayerBehind
    {
        get { return -go_Player.transform.forward; }
    }

    private void Start()
    {
        _rays = new List<CameraRay>();
        _rays.Add(cr_Top);
        _rays.Add(cr_Middle);
        _rays.Add(cr_Bottom);
    }

	private void Update()
    {
        bool validZoom = RayToCamera( f_CurrentCamDistance );

        float suggestedDistance = ZoomAdjust( validZoom );

        if( !validZoom ) 
        {
            f_CurrentCamDistance = suggestedDistance;
        }
        else 
        {
            validZoom = RayToCamera( suggestedDistance );

            if( validZoom )
                f_CurrentCamDistance = suggestedDistance;
        }

        bool validPosition = CastRays( 3.5f );
        transform.position = Vector3.Slerp(transform.position, GetIdealPos( validPosition ), f_PosLerpSpeed);

        Vector3 direction = Vector3.Normalize(go_Player.transform.position - transform.position);
        transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), f_RotLerpSpeed);
    }

    private Vector3 GetRayPos(Vector3 rayPos)
    {
        return go_Player.transform.position + rayPos;
    }

    private Vector3 GetIdealPos( bool valid )
    {
        Vector3 newPosition = go_Player.transform.position + (PlayerBehind * f_CurrentCamDistance); 

        if( cr_Top._valid && !cr_Middle._valid && f_currentYAdjust < f_maxYAdjust ) 
        {
            f_currentYAdjust += f_YAdjustSpeed;
        }
        else if( f_currentYAdjust > 0.0f ) 
        {
            f_currentYAdjust -= f_YAdjustSpeed;
        }

        f_currentYAdjust = Mathf.Clamp( f_currentYAdjust, 0, f_maxYAdjust );
        newPosition.y += f_currentYAdjust;

        return newPosition;
    }

    private float ZoomAdjust(bool valid)
    {
        float newValue = f_CurrentCamDistance;

        if(!valid)
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
        int count = 0;

        foreach( CameraRay cr in _rays) 
        {
            r_Ray = new Ray(GetRayPos(cr._position), PlayerBehind);
            if (Physics.Raycast(r_Ray, out _hit, distance ))
            {
                Debug.DrawLine(r_Ray.origin, _hit.point, Color.black);
                cr._valid = false;
                count++;
            }
            else
            {
                Debug.DrawLine(r_Ray.origin, r_Ray.origin + (PlayerBehind * f_CamDistance), Color.green);
                cr._valid = true;
            }
        }

        if( count == 3 )
            return false;
        else
            return true;
    }

    private bool RayToCamera( float distance ) 
    {
        Vector3 cameraDirection = ( Camera.main.transform.position - go_Player.transform.position ).normalized;
        r_Ray = new Ray( go_Player.transform.position, cameraDirection );
        if( Physics.Raycast(r_Ray, out _hit, distance, lm_Ignore) ) 
        {
            Debug.DrawLine(r_Ray.origin, _hit.point, Color.red);
            return false;
        }
        else 
        {
            Debug.DrawLine(r_Ray.origin, Camera.main.transform.position , Color.green);
            return true;
        }
    }
}
