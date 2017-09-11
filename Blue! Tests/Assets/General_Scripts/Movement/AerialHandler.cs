using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialHandler {

    private float height;
    private float vel_not;
    private float grav;
    private float current_time;

    public float delta_height;

    public float Height { get { return height; } set { height = value; } }
    public float JumpStrength { get { return vel_not; } set { vel_not = value; } }
    public float Gravity { get { return grav; } set { grav = value; } }
    public float AirTime { get { return current_time; } }


    public AerialHandler()
    {
        height = 0.0f;
        vel_not = 0.0f;
        grav = 0.0f;
        current_time = 0.0f;
    }

    public void CalcHeight()
    {
        current_time += Time.deltaTime;

        delta_height = height;
        height = -(grav) * (current_time * current_time) + vel_not * current_time;
        delta_height = height - delta_height;

        //Debug.Log("height: " + height);
    }

    public void Reset()
    {
        height = 0.0f;
        vel_not = 0.0f;
        grav = 0.0f;
        current_time = 0.0f;
    }
}
