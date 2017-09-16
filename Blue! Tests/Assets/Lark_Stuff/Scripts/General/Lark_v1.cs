using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lark_v1 : MonoBehaviour {

    public Transform Camera; //Reference to the camera's transform to be used mostly for rotating the player's input
    public Transform Ground_Check; //Reference to the ground check's transform for, well, checking for ground

    public GameObject System; //Reference to the system game object. To be used to hook the input function from this script

    public float default_max_dist_from_ground;
    public float default_max_speed;
    public float default_min_speed;
    public float default_accel;
    public float default_sharp_turn_speed;
    public float braking_pow;
    public float max_reverse_speed;

    //Enumerate player states. Player states change how Lark reacts to player input.
    //An idea of how each state would work:
    /*
     * GROUND: Lark is able to run and jump.
     *          If she jumps, go to JUMP state. If she happens to fall off a ledge, go to FALL state.
     *          
     * JUMP: Lark is unable to do much in this state except do special actions such as a homing attack.
     * 
     * FALL: Practically the same as jump
     * 
     * INACTIVE: A catch-all state where Lark is doing nothing, really.
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
    private bool GROUNDED;
    private bool REVERSE;

    private bool WAIT_FOR_NEW_JUMP;

    /******************
     * Helper classes
     ******************/
    Air_Handler Jump_Manager;
    Ground_Move_Handler Run_Manager;

    /*****************
     * Other scripts to reference
     ****************/
    SysCore SYS;

    // Use this for initialization
    void Start () {

        Jump_Manager = new Air_Handler(0.0f, 0.0f);
        Run_Manager = new Ground_Move_Handler(0.0f, 0.0f, 0.0f, 0.0f);

        JUMP_DOWN = false;
        JUMP_UP = false;
        ALLOW_JUMP_LEEWAY = false;
        GROUNDED = false;

        WAIT_FOR_NEW_JUMP = true;


        CURRENT_STATE = (int)PLAYER_STATE.GROUND;

        ActionBuffer = new List<string>();

        SYS = (SysCore)System.GetComponent(typeof(SysCore));
        SYS.AddKeyHook("Lark", "NORMAL_PLAY", new SysCore.PlayerKeyHook(GetInput));

        gameObject.transform.position = new Vector3(0.000001f, 0.0f, 0.0f);

        Grounded_State_Handler.move_direction = gameObject.transform.forward;
    }
	
	// Update is called once per frame
	void Update () {

        //Check if grounded. If not, go to fall state (unless the not grounded state is from a jump state).
        //If so, go to grounded state.
        CheckForGround();
        //Do the according state function
        State_Func(CURRENT_STATE);


        ActionBuffer.Clear();
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

    private void SwitchState(int state)
    {
        switch(state)
        {
            case (int)PLAYER_STATE.GROUND: CURRENT_STATE = (int)PLAYER_STATE.GROUND; break;
            case (int)PLAYER_STATE.FALL: CURRENT_STATE = (int)PLAYER_STATE.FALL; break;
            case (int)PLAYER_STATE.JUMP: CURRENT_STATE = (int)PLAYER_STATE.JUMP; break;
        }
    }

    private void GetInput(string action_name)
    {
        Debug.Log("I got input! " + action_name);
        ActionBuffer.Add(action_name);
    }

    private void CheckForGround()
    {
        //This raycast should only check for ground layers
        LayerMask ground = 1 << LayerMask.NameToLayer("Ground");
        RaycastHit[] hit_info;

        hit_info = Physics.RaycastAll(Ground_Check.position, -Ground_Check.up, Mathf.Infinity);

        if(hit_info.Length > 0)
        {
            RaycastHit closest = hit_info[0];
            for(int i = 0; i < hit_info.Length; i++)
            {
                if(hit_info[i].distance < closest.distance)
                {
                    closest = hit_info[i];
                }
            }

            float dist_1 = Vector3.Distance(closest.point, transform.position);
            float dist_2 = Vector3.Distance(Ground_Check.position, transform.position);

            //If the transform is close enough to the point of impact, simply teleport to the point of impact
            //and set grounded to true.
            if(dist_1 <= default_max_dist_from_ground && Jump_Manager.AirSpeed == 0.0f)
            {
                transform.position = closest.point;
                GROUNDED = true;
            }
            //If the point of impact is in between the ground check and the transform, then simply
            //teleport to the point of impact and set grounded to true.
            else if(dist_2 > closest.distance && Jump_Manager.AirSpeed < 0.0f)
            {
                transform.position = closest.point;
                GROUNDED = true;
            }
            else
            {
                GROUNDED = false;
            }
        }

        Debug.Log("GROUNDED? " + GROUNDED);
    }

    private void HandleGrounded()
    {
        Debug.DrawRay(gameObject.transform.position, Grounded_State_Handler.move_direction * 10.0f, Color.blue);
        Debug.DrawRay(gameObject.transform.position, Grounded_State_Handler.total_input_direction * 10.0f, Color.yellow);

        //Reset the input direction for a fresh calculation
        Grounded_State_Handler.total_input_direction = new Vector3();
        Run_Manager.MaxSpeed = default_max_speed;
        Run_Manager.MinSpeed = default_min_speed;
        Run_Manager.Acceleration = default_accel;

        gameObject.transform.forward = Grounded_State_Handler.move_direction;

        //If the player isn't grounded, ignore any inputs for now. Just switch to the falling state
        if(!GROUNDED)
        {
            SwitchState((int)PLAYER_STATE.FALL);

            return;
        }

        //Read the action buffer
        foreach(string act in ActionBuffer)
        {
            Grounded_State_Handler.ReadInput(act);
        }

        //If a jump was detected...
        if (JUMP_DOWN)
        {
            //...if a new jump is allowed, then just go to the jump state. We can handle inputs over there.
            if(JumpIsValid())
            {
                SwitchState((int)PLAYER_STATE.JUMP);
                return;
            }
        }
        else if(JUMP_UP)
        {
            //If the jump is not pressed, set this flag to false so the player is allowed to jump again
            WAIT_FOR_NEW_JUMP = false;
        }

        //If the player did not input anything, then slow down if needed. Also, rotate the character
        //if needed. Then just leave the function. Nothing else to do here.
        if(Grounded_State_Handler.total_input_direction == Vector3.zero)
        {
            Debug.Log("Nothing to move");

            if(Run_Manager.Speed > 0.0f)
            {
                Debug.Log("Gotta slow down");
                Run_Manager.SlowDown();
            }
               
            return;
        }

        //Calculate forward relative to the camera
        Vector3 cam_forward = Grounded_State_Handler.CalculateForward(Camera, gameObject.transform);

        //Rotate the input vector to this camera forward
        Vector3 real_dir = Grounded_State_Handler.AlignVectorToForward(Grounded_State_Handler.total_input_direction, cam_forward, Vector3.forward, gameObject.transform.up).normalized;

        Debug.Log("Camera Forward: " + cam_forward);
        Debug.Log("Real input direction: " + real_dir);

        //If the player's speed is greater than 0, then we need to handle movement in such a way
        //where the player's speed is affected by what angle they want to move.
        //For example, a sharp right turn should have Lark slow down instead of turning like a truck
        if(Run_Manager.Speed > 0.0f)
        {
            //Calculate the angle between the character's facing direction and the direction the player
            //intends for the character to go.
            var angle = Vector3.Angle(Grounded_State_Handler.move_direction, real_dir);

            bool hard_break = false;

            //If the angle is within the range <-60, 60>, then we consider this a forward movement,
            //and the player character will just speed up
            if(Mathf.Abs(angle) <= 60.0f)
            {
                Run_Manager.SpeedUp();
            }
            //If the angle is within the ranges <-105, -60) or (60, 105>, then we consider this a sharp
            //turn, and the player character's speed will either need to be reduced to a turn speed
            //or capped to a turn speed depending on whether the character is above said speed or below,
            //respectively.
            else if(Mathf.Abs(angle) <= 105.0f)
            {
                if(Run_Manager.Speed < default_sharp_turn_speed)
                {
                    Run_Manager.MaxSpeed = default_sharp_turn_speed;
                    Run_Manager.SlowDown(braking_pow);
                }
                else if(Run_Manager.Speed > default_sharp_turn_speed)
                {
                    Run_Manager.MinSpeed = default_sharp_turn_speed;
                    Run_Manager.SpeedUp();
                }
            }
            //If the angle is outside either of those ranges, then we consider this direction to be
            //towards the back, meaning that the player will need to make a hard break to quickly slow
            //down before turning to said direction
            else
            {
                if (!REVERSE)
                {
                    hard_break = true;
                    Run_Manager.SlowDown(braking_pow * 1.5f);
                }
                else
                {
                    Run_Manager.MaxSpeed = max_reverse_speed;
                    Run_Manager.SpeedUp();
                }
            }

            if (!hard_break)
                Grounded_State_Handler.TurnMoveDirTowards(real_dir, angle, Run_Manager.Speed/Run_Manager.MaxSpeed);
        }
        else
        {
            //Calculate the angle between the camera's facing direction and the direction the player
            //intends for the character to go.
            var angle = Vector3.SignedAngle(cam_forward, real_dir, gameObject.transform.up);

            //Reverse if the player is holding roughly towards behind the camera
            if (Mathf.Abs(angle) > 105.0f)
            {
                REVERSE = true;
            }
 
            Run_Manager.SpeedUp();
        }
    }

    private void HandleFall()
    {

    }

    private void HandleJump()
    {

    }

    private bool JumpIsValid()
    {
        return !WAIT_FOR_NEW_JUMP;
    }

    private class Grounded_State_Handler
    {
        public static Vector3 total_input_direction;
        public static Vector3 move_direction;

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

        public static void TurnMoveDirTowards(Vector3 new_dir, float angle, float speed_percentage)
        {
            if(angle <= 1.0f)
            {
                move_direction = new_dir;
            }
            float turn_percentage = Mathf.Min(Mathf.Max(1 - (speed_percentage), 0.05f), 0.1f) * Mathf.Max(0.9f, 1.0f - (180.0f / angle));
            move_direction = Vector3.Slerp(move_direction, new_dir, turn_percentage);
        }

        public static void ReadInput(string action)
        {
            if(action == "jump")
            {

            }
            else if(action == "forward")
            {
                total_input_direction += Vector3.forward;
            }
            else if(action == "right")
            {
                total_input_direction += Vector3.right;
            }
            else if(action == "back")
            {
                total_input_direction += -Vector3.forward;
            }
            else if(action == "left")
            {
                total_input_direction += -Vector3.right;
            }
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
        private float _max_height; //The maximum height the jump should reach
        private float _max_air_time; //The maximum amount of time the player should be in the air, assuming that the jump is on flat ground. This is used internally to calculate the gravity value.


        private float _jump_strength; //The initial speed of the player's jump. Jump strength is to be calculated internally by the air handler, but this can be overrided.
        private float _gravity; //The gravity value will be deducted from the player's jumpstrength. Gravity is to be calculated internally by the air handler, but this can be overrided.

        private float _current_air_speed; //Current aerial speed of the player

        public float MaxHeight { get { return _max_height; } set { _max_height = value; } }
        public float MaxAirTime { get { return _max_air_time; } set { _max_air_time = value; } }
        public float AirSpeed { get { return _current_air_speed; } }
        public float JumpStrength { get { return _jump_strength; } }
        public float Gravity { get { return _gravity; } }

        public Air_Handler(float max_height, float max_air_time)
        {
            _current_air_speed = 0.0f;
            _max_height = max_height;
            _max_air_time = max_air_time;

            CalculateGravity();
            CalculateJumpStrength();
        }

        public void CalcJump(bool new_jump = false)
        {
            if(new_jump)
            {
                _current_air_speed = _jump_strength;
                return;
            }

            _current_air_speed -= _gravity;
        }
        
        public void OverrideGravity(float new_grav)
        {
            _gravity = new_grav;
        }

        public void OverrideJumpStrength(float new_jump_strength)
        {
            _jump_strength = new_jump_strength;
        }

        private void CalculateGravity()
        {
            _gravity = _max_height / (_max_air_time / 2);
        }

        private void CalculateJumpStrength()
        {
            _jump_strength = (_max_height + _gravity * Mathf.Pow(_max_air_time / 2, 2)) / (_max_air_time / 2);
        }

        
    }

    private class Ground_Move_Handler
    {
        private float _maximum_speed;
        private float _current_speed;
        private float _minimum_speed;
        private float _acceleration;
        private float _friction;

        public float Speed { get { return _current_speed; } }
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
