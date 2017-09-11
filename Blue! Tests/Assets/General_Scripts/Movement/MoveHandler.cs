using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler {

    private float maximum_speed;
    private float current_speed;
    private float minimum_speed;
    private float acceleration;
    private float friction;

    public float Min_Speed { get { return minimum_speed; } set { minimum_speed = value; } }
    public float Max_Speed { get { return maximum_speed; } set { maximum_speed = value; } }
    public float Speed { get { return current_speed; }  set { current_speed = value; } }
    public float Accel { get { return acceleration; } set { acceleration = value; } }
    public float Friction { get { return friction; } set { Mathf.Clamp(value, 0, 1); } }

    public MoveHandler(float maximum_speed, float minimum_speed, float acceleration)
    {
        this.maximum_speed = maximum_speed;
        this.minimum_speed = minimum_speed;

        current_speed = 0.0f;

        friction = 0.0f;

        this.acceleration = acceleration;
    }

    public void SpeedUp()
    {
        current_speed += acceleration * (1 - friction) * Time.deltaTime;

        ClampSpeed();
    }

    public void SpeedUp(float impulse)
    {
        current_speed += (acceleration * (1 - friction) + impulse) * Time.deltaTime;

        ClampSpeed();
    }

    public void SlowDown()
    {
        current_speed -= acceleration * (1 - friction) * Time.deltaTime;
        ClampSpeed();

    }

    public void SlowDown(float stopping_power)
    {
        current_speed -= (acceleration * (1 - friction) + stopping_power) * Time.deltaTime;
        ClampSpeed();

    }

    private void ClampSpeed()
    {
        current_speed = Mathf.Clamp(current_speed, minimum_speed, maximum_speed);
    }
}
