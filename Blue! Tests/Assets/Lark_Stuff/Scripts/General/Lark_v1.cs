using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lark_v1 : MonoBehaviour {



    //Enumerate player states. Player states change how Lark reacts to player input.
    //An idea of how each state would work:
    /*
     * GROUND: Lark is able to run and jump.
     *          If she jumps, go to JUMP state. If she happens to fall off a ledge, go to FALL state.
     *          
     * JUMP: Lark is unable to do much in this state except do special actions such as a homing attack.
     * 
     * FALL: Practically the same as jump
     */ 
    private enum PLAYER_STATE
    {
        GROUND,
        FALL,
        JUMP,
        INACTIVE
    }

    private int CURRENT_STATE;

    /***************
     * Lists
     ***************/
    private List<string> ActionBuffer; // A list for the action buffer. Typically, it is only one action per frame, but we want to stay on the safe side.


    /******************
     * Flags
     ******************/
    private bool JUMP_DOWN;
    private bool JUMP_UP;
    private bool ALLOW_JUMP_LEEWAY;

    /******************
     * Helper classes
     ******************/



	// Use this for initialization
	void Start () {

        CURRENT_STATE = (int)PLAYER_STATE.GROUND;

        ActionBuffer = new List<string>();
    }
	
	// Update is called once per frame
	void Update () {

        //Check if grounded. If not, go to fall state (unless the not grounded state is from a jump state).
        //If so, go to grounded state.

        //Do the according state function
        State_Func(CURRENT_STATE);

	}

    private void State_Func(int state)
    {
        switch(state)
        {
            case (int)PLAYER_STATE.GROUND: HandleGrounded();  break;
            case (int)PLAYER_STATE.FALL: HandleFall();  break;
            case (int)PLAYER_STATE.JUMP: HandleJump();  break;
        }
    }

    private void GetInput(string action_name)
    {
        ActionBuffer.Add(action_name);
    }

    private void CheckForGround()
    {

    }

    private void HandleGrounded()
    {

    }

    private void HandleFall()
    {

    }

    private void HandleJump()
    {

    }

    private class Grounded_State_Handler
    {

        public Grounded_State_Handler()
        {}

        public static Vector3 CalculateForward(Transform relativeTo, Transform obj)
        {
            return Vector3.Cross(relativeTo.right, obj.up);
        }

        public static Vector3 AlignVectorToForward(Vector3 vec, Vector3 to_forward, Vector3 from_forward, Vector3 axis)
        {
            float angle = Vector3.SignedAngle(to_forward, from_forward, axis);

            return Quaternion.AngleAxis(angle, axis) * vec;
        }
    }

    private class Jump_State_Handler
    {
        public Jump_State_Handler()
        {

        }
    }


    private class Air_Handler
    {
        private float _height;
        private float _delta_height;
        private float _gravity;
        private float _current_air_time;
        private float _jump_strength;

        public float CurrHeight { get { return _height; } }
        public float DeltaHeight { get { return _delta_height; } }
        public float AirTime { get { return _current_air_time; } }
        public float Gravity { get { return _gravity; } set { _gravity = value; } }
        public float JumpStrength { get { return _jump_strength; } }

        public Air_Handler(float jump_strength, float gravity)
        {
            _jump_strength = jump_strength;
            _gravity = gravity;
            Reset();
        }

        public void Reset()
        {
            _height = 0.0f;
            _delta_height = 0.0f;
            _current_air_time = 0.0f;
        }

        public float CalcHeight(float time)
        {
            _delta_height = _height;
            _height = -(_gravity) * Mathf.Pow(time, 2.0f) + _jump_strength * time;
            _delta_height = _height - _delta_height;

            return _height;
        }

        public float CalcHeight()
        {
            _current_air_time += Time.deltaTime;

            _height = -(_gravity) * Mathf.Pow(_current_air_time, 2.0f) + _jump_strength * _current_air_time;
            _delta_height = _height - _delta_height;

            return _height;
        }
    }

    private class Ground_Move_Handler
    {
        private float _maximum_speed;
        private float _current_speed;
        private float _minimum_speed;
        private float _acceleration;
        private float _friction;

        public float MinSpeed { get { return _minimum_speed; } set { _minimum_speed = value; } }
        public float MaxSpeed { get { return _maximum_speed; } set { _maximum_speed = value; } }
        public float Acceleration { get { return _acceleration; } set { _acceleration = value; } }
        public float Friction { get { return _friction; } set { _friction = value; } }

        public Ground_Move_Handler(float max_speed, float min_speed, float accel, float friction)
        {

            _maximum_speed = max_speed;
            _minimum_speed = min_speed;
            _acceleration = accel;
            _friction = friction;

            Reset();

        }

        public void Reset()
        {
            _current_speed = _minimum_speed;
        }

        public void SpeedUp()
        {
            _current_speed += _acceleration * (1 - _friction);
            ClampSpeed();
        }

        public void SpeedUp(float impulse)
        {
            _current_speed += _acceleration * (1 - _friction) + impulse;
            ClampSpeed();
        }

        public void SlowDown()
        {
            _current_speed -= _acceleration * (1 - _friction);
            ClampSpeed();
        }

        public void SlowDown(float brake_power)
        {
            _current_speed -= _acceleration * (1 - _friction) + brake_power;
            ClampSpeed();
        }

        private void ClampSpeed()
        {
            _current_speed = Mathf.Clamp(_current_speed, _minimum_speed, _maximum_speed);
        }
    }
}
