using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMount : MonoBehaviour
{
    [SerializeField] GameObject go_Player;
    [SerializeField] Vector3 v_Offset;

    private float _originYDistance;

    private void Start()
    {
        _originYDistance = transform.position.y;
    }

    void LateUpdate ()
    {
        Vector3 wantedPosition = new Vector3(go_Player.transform.position.x,
                                            go_Player.transform.position.y + _originYDistance,
                                            go_Player.transform.position.z);

        wantedPosition += v_Offset;

        transform.position = Vector3.Slerp(transform.position, wantedPosition, 1);
	}
}
