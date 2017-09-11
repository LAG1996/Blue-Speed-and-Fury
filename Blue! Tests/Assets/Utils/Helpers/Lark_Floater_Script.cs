using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lark_Floater_Script : MonoBehaviour {

    public Transform Lark;

    public float x_speed = 100.0f;
    public float y_speed = 100.0f;

    public float y_minimum = -20.0f;
    public float y_maximum = 80.0f;

    private float x = 0.0f;
    private float y = 0.0f;
    public float last_input_time;

	// Use this for initialization
	void Start () {
        last_input_time = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
    }

    private void FixedUpdate()
    {
        _SetRotation();

        Debug.DrawRay(transform.position, -(transform.forward * 5.0f), Color.green);
    }

    public void GetMouseMovement(Vector2 direction)
    {

        x += direction.x * x_speed * Time.deltaTime;
        y -= direction.y * y_speed * Time.deltaTime;

        x = ClampAngle(x, -360f, 360f);
        y = ClampAngle(y, y_minimum, y_maximum);

    }

    private void _SetRotation()
    {
        gameObject.transform.rotation = Quaternion.Euler(y, x, 0);
    }

    private float ClampAngle(float angle, float max, float min)
    {
        if(angle < -360f)
        {
            angle += 360;
        }
        else if(angle > 360f)
        {
            angle -= 360;
        }

        return Mathf.Clamp(angle, max, min);
    }
}
