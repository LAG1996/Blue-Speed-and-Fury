using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lark_Floater_v2 : MonoBehaviour {

    public GameObject Lark;
    public GameObject System;

    public float x_speed = 100.0f;
    public float y_speed = 100.0f;

    public float y_minimum = -20.0f;
    public float y_maximum = 80.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    private SysCore SYS;

    // Use this for initialization
    void Start () {

        SYS = (SysCore)System.GetComponent(typeof(SysCore));

        SYS.AddMouseHook("LARK_CAM_FOCUS", "NORMAL_PLAY", new SysCore.PlayerMouseHook(GetMouseMovement));

	}
	
	// Update is called once per frame
	void Update () {

        _SetRotation();
		
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
        if (angle < -360f)
        {
            angle += 360;
        }
        else if (angle > 360f)
        {
            angle -= 360;
        }

        return Mathf.Clamp(angle, max, min);
    }
}
