using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class that holds character information.
public class Character_v1 {

    protected GameObject myObject;
    protected Transform myTransform;

    protected Vector3 my_position;
    protected Vector3 move_direction;
    protected Vector3 aerial_direction;

    public MoveHandler move_handle;
    public AerialHandler air_handle;

    public float Speed { get { return move_handle.Speed; } set { move_handle.Speed = value; } }
    public float Max_Speed { get { return move_handle.Max_Speed; } set { move_handle.Max_Speed = value; } }
    public float Min_Speed { get { return move_handle.Min_Speed; } set { move_handle.Min_Speed = value; } }
    public float Friction { get { return move_handle.Friction; } set { move_handle.Friction = value; } }
    public float Acceleration { get { return move_handle.Accel; } set { move_handle.Accel = value; } }

    public Vector3 MovingDirection { get { return move_direction; } }
    public Vector3 AerialDirection { get { return aerial_direction; } }

    public Character_v1(GameObject obj, Transform trans, float maximum_speed, float minimum_speed, float acceleration, float friction)
    {
        myObject = obj;
        myTransform = trans;

        my_position = myTransform.position;

        move_direction = myTransform.forward;
        aerial_direction = myTransform.up;

        move_handle = new MoveHandler(maximum_speed, minimum_speed, acceleration);
        air_handle = new AerialHandler();
    }

    public void SetMoveDirection(Vector3 dir)
    {
        move_direction = dir;
    }

    public void SetAerialDirection(Vector3 dir)
    {
        aerial_direction = dir;
    }

    public virtual void ReadInput(Vector3 direction)
    {
        move_direction = direction;
    }

    public virtual void ReadInput(string action)
    {}

    public virtual void Move()
    {
        myTransform.Translate(move_direction * move_handle.Speed * Time.deltaTime, Space.World);
    }

    public virtual void HandleAir()
    {
        myTransform.Translate(aerial_direction * air_handle.delta_height, Space.World);
    }
}
