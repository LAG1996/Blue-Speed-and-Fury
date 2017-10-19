using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Move_v1 : MonoBehaviour {

    public Transform focus;
    public float min_distance;
    public float max_distance;

    public float cam_damp_time;


    public static float follow_speed;
    public static float follow_max_speed;

    private float cur_distance;

    private Vector3 last_position;
    private Vector3 myVelocity;

	// Use this for initialization
	void Start () {

        last_position = focus.position;
        cur_distance = min_distance;

        myVelocity = Vector3.zero;
	}
	
	// Update is called once per frame
    void Update()
    {
        _CalculateCameraPosition();
    }

    private void _CalculateCameraPosition()
    {
        if(follow_max_speed > 0.0f)
        {
            //Debug.Log("Max speed: " + character_max_speed);
            //Debug.Log("Current speed: " + character_speed);
            cur_distance = Mathf.Lerp(min_distance, max_distance, follow_speed / follow_max_speed);
        }

        //Debug.Log("my distance: " + cur_distance);

        transform.position = Vector3.SmoothDamp(transform.position, ((focus.forward * -cur_distance) + focus.position), ref myVelocity, cam_damp_time);

        transform.LookAt(focus);
    }
}
