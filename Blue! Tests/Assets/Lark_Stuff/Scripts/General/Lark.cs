using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lark : MonoBehaviour {

    public float temp_max_speed;
    public float temp_accel;
    public float temp_friction;
    public float temp_stopping_power;
    public float temp_max_vel_up;
    public float temp_max_vel_down;
    public float temp_jump_strength;
    public float max_stand_dist_from_ground;
    public float temp_gravity;

    public Animator _Lark_Animator;
    public GameObject Camera;
    public Transform Ground_Check;

    public LarkController _myController;

    private Vector3 _Facing_Dir;

    private float time_holding_jump;
    private float distance_from_ground;
    private float time_falling;

    private bool just_jumped;
    private bool just_let_go_of_jump;
    private bool allow_jump_leeway;

    private List<string> _Animator_Triggers;

    //Some flags
    private Dictionary<string, bool> Flags;

    // Use this for initialization
    void Start() {

        just_jumped = false;
        allow_jump_leeway = false;
        just_let_go_of_jump = true;

        time_holding_jump = 0.0f;
        time_falling = 0.0f;

        _myController = new LarkController(gameObject, transform, temp_max_speed, 0.0f, temp_accel, temp_friction);
        _Animator_Triggers = new List<string>();

        _myController.SetMoveDirection(gameObject.transform.forward);
        _myController.SetAerialDirection(gameObject.transform.up);

        Flags = new Dictionary<string, bool>();

        Flags.Add("HARD_BREAK", false);
        Flags.Add("SOFT_BREAK", false);
        Flags.Add("JUMP_DOWN", false);
        Flags.Add("JUMP_UP", false);
        Flags.Add("DO_JUMP", false);
        Flags.Add("GROUNDED", false);

       // _Animator_Triggers.Add("HOP_1");
       // _Animator_Triggers.Add("HOP_2");
        _Animator_Triggers.Add("RUN_1");
        _Animator_Triggers.Add("RUN_2");
        _Animator_Triggers.Add("RUN_3");
        _Animator_Triggers.Add("HARD_BREAK");
        _Animator_Triggers.Add("NO_RUN");
        _Animator_Triggers.Add("STOP");
        _Animator_Triggers.Add("JUMP");
        _Animator_Triggers.Add("GROUNDED");

        _Facing_Dir = transform.forward;

        gameObject.transform.Translate(transform.forward * 0.00001f);
    }

    // Update is called once per frame
    void Update() {

        _DoGroundCheck();

        foreach (string action in _myController.GetActionBuff())
        {
            _HandleAction(action);
        }

        _myController.ClearActionBuff();



        _Rotate_To_Cam();

        _HandleJump();

        _myController.HandleAir();
        _myController.Move();
    }

    private void LateUpdate()
    {

        if (_Lark_Animator)
        {
            _HandleRunningAnimation();
        }

        _ClearFlags();
    }

    public void ReadInputHook(string action)
    {
        _myController.ReadInput(action);
    }

    private void _DoGroundCheck()
    {
        _myController.air_handle.CalcHeight();
        //Debug.Log(_myController.air_handle.delta_height);
        //Debug.DrawRay(transform.position, -_myController.AerialDirection * 10, Color.yellow, Mathf.Infinity);
        LayerMask ground = 1 << LayerMask.NameToLayer("Ground");
        RaycastHit[] hit_info;

        hit_info = Physics.RaycastAll(Ground_Check.position, -Ground_Check.up, Mathf.Infinity, ground.value);

       // Debug.Log("Registered " + hit_info.Length + " hits ");

        if (hit_info.Length > 0)
        {
            RaycastHit closest = hit_info[0];
            for (int i = 0; i < hit_info.Length; i++)
            {
               // Debug.Log("Reading collisions at " + hit_info[i].distance);
                if (hit_info[i].distance < closest.distance)
                {
                    closest = hit_info[i];
                }
            }

            float dist = Vector3.Distance(closest.point, transform.position);
            //Debug.Log("Closest hit is " + closest.distance + " from ground check");
            //Debug.Log("Closest hit is: " + (dist) + " from transform");
           // Debug.Log("Delta is: " + (_myController.air_handle.delta_height));
           // Debug.Log("Colliding at: " + closest.point);

            Debug.DrawRay(Ground_Check.position, closest.distance * -Ground_Check.up, Color.yellow);

            if (dist <= max_stand_dist_from_ground && _myController.air_handle.delta_height == 0.0f)
            {
                Debug.Log("Whyyy");
                //Debug.Log("asdsadsad");
                transform.position = closest.point;
                Flags["GROUNDED"] = true;
            }
            else if (Vector3.Distance(Ground_Check.position, transform.position) > closest.distance && _myController.air_handle.delta_height <= 0.0f)
            {
                Debug.Log("FFFFUUUUUUUUUUUUUCK");
                transform.position = closest.point;
                Flags["GROUNDED"] = true;
            }
            else
            {
                Flags["GROUNDED"] = false;
            }
        }
    }

    private void _ClearFlags()
    {
        string [] f = new string[Flags.Keys.Count];
        Flags.Keys.CopyTo(f, 0);
        foreach (string flag in f)
        {
            Flags[flag] = false;
        }
    }

    private void _HandleJump()
    {

        if(Flags["GROUNDED"])
        {
            _myController.air_handle.Reset();
            just_jumped = false;
            time_falling = 0.0f;
        }
        else if(!just_jumped)
        {
            _myController.air_handle.Gravity = temp_gravity * 2.0f;

            if(time_falling <= 0.2f)
            {
                time_falling += Time.deltaTime;
                allow_jump_leeway = true;
            }
            else
            {
                allow_jump_leeway = false;
            }
        }

        if (Flags["JUMP_DOWN"] && (Flags["GROUNDED"] || allow_jump_leeway) && just_let_go_of_jump)
        {
            Debug.Log("pressing down on jump...");

            Flags["DO_JUMP"] = true;

            _myController.air_handle.Reset();
            _myController.air_handle.Gravity = temp_gravity;
            _myController.air_handle.JumpStrength = temp_jump_strength;

            time_holding_jump += Time.fixedDeltaTime;

            just_jumped = true;
            just_let_go_of_jump = false;
        }
        else if (Flags["JUMP_UP"])
        {
            if(just_jumped && !Flags["GROUNDED"])
            {
                Debug.Log("let go of jump after " + _myController.air_handle.AirTime + " secs");
                
                just_jumped = false;

                float new_grav = _CalculateNewGravity(_myController.air_handle.JumpStrength, _myController.air_handle.AirTime);

                Debug.Log("New gravity: " + new_grav);

                float vel = 0.0f;
                _myController.air_handle.Gravity = Mathf.SmoothDamp(_myController.air_handle.Gravity, new_grav, ref vel, 0.1f);

                time_holding_jump = 0.0f;
            }

            just_let_go_of_jump = true;

        }

    }

    private float _CalculateNewGravity(float jump_strength, float time_in_air)
    {
        return jump_strength / (2 * time_in_air);
    }

    private void _HandleAction(string action)
    {
        if (action == "no_action")
        {
            Flags["SOFT_BREAK"] = true;
            _myController.move_handle.SlowDown();
        }

        Flags["JUMP_UP"] = true;

        if (action == "jump")
        {
            Flags["JUMP_UP"] = false;
            Flags["JUMP_DOWN"] = true;
        }

        if(Flags["GROUNDED"])
        {
            if (action == "forward")
            {
                _myController.move_handle.SpeedUp();
            }
            else if (action == "back")
            {
                Flags["HARD_BREAK"] = true;
                _myController.move_handle.SlowDown(temp_stopping_power);
            }
        }
    }

    private Vector3 _CalculateForward()
    {
        //Convert the input to be relative to where the camera is facing
        return Vector3.Cross(Camera.transform.right, transform.up);
    }

    private void _Rotate_To_Cam()
    {
        if(_myController.Speed > 0.0f)
        {       
            if(!Flags["HARD_BREAK"] && !Flags["SOFT_BREAK"])
            {
                Vector3 vel = new Vector3();
                transform.forward = Vector3.SmoothDamp(transform.forward, _CalculateForward(), ref vel, 0.1f);

                _myController.SetMoveDirection(transform.forward);
            }

            if (Flags["HARD_BREAK"])
            {
                Vector3 cam_facing = _CalculateForward();

                float angle = Vector3.Angle(transform.forward, cam_facing);


                //Debug.Log(angle);
                _Lark_Animator.SetFloat("brake_angle", angle);
            }
        }
    }

    private void _ClearAnimatorValues()
    {
        foreach(string value in _Animator_Triggers)
        {
            _Lark_Animator.SetBool(value, false);
        }
    }

    private void _HandleRunningAnimation()
    {
        _ClearAnimatorValues();

        if(Flags["DO_JUMP"])
        {
            Debug.Log("DO JUMP???");
            _Lark_Animator.SetBool("JUMP", true);
        }

        _Lark_Animator.SetBool("GROUNDED", Flags["GROUNDED"]);

        if(!Flags["GROUNDED"])
        {
            return;
        }

        if (Flags["HARD_BREAK"])
        {
            _Lark_Animator.SetBool("NO_RUN", true);

            if (_myController.Speed <= 0.0f)
            {
                _Lark_Animator.SetBool("STOP", true);
            }
            else if(Flags["HARD_BREAK"])
            {
                _Lark_Animator.SetBool("HARD_BREAK", true);
            }
        }
        else if (_myController.Speed >= _myController.Max_Speed - (_myController.Max_Speed * 0.25f))
        {
            _Lark_Animator.SetBool("RUN_3", true);
        }
        else if (_myController.Speed >= _myController.Max_Speed - (_myController.Max_Speed * 0.5))
        {
            _Lark_Animator.SetFloat("speed_mult", 1.5f);
            _Lark_Animator.SetBool("RUN_2", true);
        }
        else if (_myController.Speed >= _myController.Max_Speed - (_myController.Max_Speed * 0.75))
        {
            _Lark_Animator.SetFloat("speed_mult", 1.0f);
            _Lark_Animator.SetBool("RUN_2", true);
        }
        else if (_myController.Speed > 0.1f)
        {
            _Lark_Animator.SetBool("RUN_1", true);
        }
        else
        {
            _Lark_Animator.SetBool("NO_RUN", true);
        }
    }

    public class LarkController : Character_v1
    {
        private List<string> _Action_Buff;

        public bool needTurning;

        public LarkController(GameObject obj, Transform trans, float maximum_speed, float minimum_speed, float acceleration, float friction) : base(obj, trans, maximum_speed, minimum_speed, acceleration, friction)
        {
            _Action_Buff = new List<string>();
        }

        public override void ReadInput(string action)
        {
            needTurning = false;

            _Action_Buff.Add(action);
        }

        public List<string> GetActionBuff()
        {
            return _Action_Buff;
        }

        public void ClearActionBuff()
        {
            _Action_Buff.Clear();
        }
    }
}