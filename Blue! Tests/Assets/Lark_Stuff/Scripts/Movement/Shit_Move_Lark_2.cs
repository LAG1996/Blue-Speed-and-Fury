using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shit_Move_Lark_2 : MonoBehaviour
{

    public float straight_terminal_vel;
    public float turning_terminal_vel;
    public float max_accel;
    public float stopping_power;
    public float turn_threshold_divisor;
    public float friction;

    public Transform Camera;
    public Animator _Lark_Animator;

    private List<Vector3> move_buffer = new List<Vector3>();

    private Vector3 primary_dir;
    private Vector3 direction_moving;

    private float current_terminal_vel;
    private float current_move_speed;
    private float current_accel;
    private float current_turn_threshold_divisor;

    //Flags
    private bool stop_to_turn;
    private bool turning;
    private bool still_holding_direction;

    //Timers
    private float time_no_direction;

    // Use this for initialization
    void Start()
    {

        stop_to_turn = false;
        turning = false;

        primary_dir = gameObject.transform.forward;
        current_move_speed = 0.0f;
        primary_dir = new Vector3();

        time_no_direction = 0.0f;
    }

    public void HandleKeyPressBuffer(List<string> buffer)
    {
        foreach (string action in buffer)
        {
            HandleKeyPress(action);
        }
    }

    public void HandleKeyPress(string action_name)
    {
        Vector3 dir = new Vector3();

        switch (action_name)
        {
            case "forward":
                dir = new Vector3(0.0f, 0.0f, 1.0f);
                break;
            case "back":
                dir = new Vector3(0.0f, 0.0f, -1.0f);
                break;
            case "left":
                dir = new Vector3(-1.0f, 0.0f);
                break;
            case "right":
                dir = new Vector3(1.0f, 0.0f);
                break;
        }

        move_buffer.Add(dir);
    }

    // Update is called once per frame
    void Update()
    {

        _ReadMovementBuffer();
        _HandleRotation();
        _HandleRunningAnimation();

        //Debug.DrawRay(gameObject.transform.position, primary_dir * 10.0f, Color.red);
    }

    private void _HandleMovement(Vector3 direction)
    {
        //Debug.Log(direction);
        Debug.DrawRay(gameObject.transform.position, direction * 10.0f, Color.green);

        if (direction.Equals(Vector3.zero))
        {
            _Slow_Down(ref current_accel, max_accel, stopping_power/4);
        }
        else
        {
            var angle = Vector3.Angle(primary_dir, direction);


            if (angle <= 100.0f)
            {
                if (current_move_speed <= 0.0f)
                    current_move_speed = 5.0f;

                _Straight_Run(ref current_accel, max_accel, ref current_terminal_vel, straight_terminal_vel);
            }
            else if(angle > 90.0f )
            {
                if (current_move_speed > (current_turn_threshold_divisor != 0.0f ? straight_terminal_vel / Mathf.Abs(current_turn_threshold_divisor) : 0.0f))
                {
                    stop_to_turn = true;
                    _Slow_Down(ref current_accel, max_accel, (stopping_power + Mathf.Abs(friction)) * 2);
                }
            }
            Debug.Log(angle);

            if (angle <= 90.0f)
                 _Handle_Turn(direction, ref primary_dir, current_move_speed, current_terminal_vel, angle);
        }

       
        _Calculate_Speed(ref current_move_speed, current_accel, current_terminal_vel, turning, false);

        //Debug.Log(current_move_speed);

        if (current_move_speed < 10.0f)
            stop_to_turn = false;

        _Move(primary_dir, current_move_speed, gameObject.transform);

        //Reset turning flag
        turning = false;
    }

    private void _Straight_Run(ref float c_accel, float m_accel, ref float c_vel, float m_vel)
    {
        c_vel = m_vel;
        c_accel = m_accel;
    }

    private void _Slow_Down(ref float c_accel, float m_accel, float fric)
    {
        c_accel = -m_accel * fric;
    }

    private void _Turn(ref float c_accel, float m_accel, ref float c_vel, float t_vel, float c_speed, float friction)
    {
        c_vel = t_vel;

        c_accel = c_speed > c_vel ? -m_accel * (1.2f + friction) : m_accel * (0.6f + friction);

    }

    private void _Handle_Turn(Vector3 target_direction, ref Vector3 current_direction, float c_speed, float c_vel, float angle = float.NaN)
    {
        if (float.IsNaN(angle))
        {
            angle = Vector3.Angle(current_direction, target_direction);
        }

        float turn_percentage = Mathf.Min(Mathf.Max(1 - (c_speed / c_vel), 0.05f), 0.1f) * Mathf.Max(0.5f, 1.0f - (180.0f / angle));

        current_direction = Vector3.Slerp(current_direction, target_direction, turn_percentage);
    }

    private void _Calculate_Speed(ref float c_speed, float accel, float m_vel, bool bypass_maximum, bool bypass_minimum)
    {
        c_speed += accel * Time.deltaTime;

        if (c_speed > m_vel && !bypass_maximum)
            c_speed = m_vel;
        else if (c_speed < 0.0f && !bypass_minimum)
            c_speed = 0.0f;

    }

    private void _Move(Vector3 direction, float speed, Transform trans)
    {
        trans.Translate(primary_dir * current_move_speed * Time.deltaTime, Space.World);
    }

    private void _ReadMovementBuffer()
    {
        Vector3 tot_dir = new Vector3();

        foreach (Vector3 input in move_buffer)
        {
            tot_dir += input;
        }

        //Convert the input to be relative to where the camera is facing
        //First get the cross product of the camera's right and the character's up direction
        Vector3 cam_forward = Vector3.Cross(Camera.right, transform.up);
        //Next, get the angle between the player's forward direction and this new front direction
        float angle = Vector3.SignedAngle(Vector3.forward, cam_forward, transform.up);

        Vector3 rot_input = Quaternion.AngleAxis(angle, transform.up) * tot_dir;

        tot_dir = rot_input;

        _HandleMovement(tot_dir);

        move_buffer.Clear();
    }

    private void _HandleRotation()
    {
        transform.forward = !primary_dir.Equals(Vector3.zero) ? primary_dir : transform.forward;
    }

    private void _HandleRunningAnimation()
    {
        _Lark_Animator.SetBool("RUN_3", false);
        _Lark_Animator.SetBool("RUN_2", false);
        _Lark_Animator.SetBool("RUN_1", false);
        _Lark_Animator.SetBool("NO_RUN", false);

        if (current_move_speed >= straight_terminal_vel - (straight_terminal_vel * 0.25f))
        {
            _Lark_Animator.SetBool("RUN_3", true);
        }
        else if (current_move_speed >= straight_terminal_vel - (straight_terminal_vel * 0.5))
        {
            _Lark_Animator.SetFloat("speed_mult", 1.5f);
            _Lark_Animator.SetBool("RUN_2", true);
        }
        else if (current_move_speed >= straight_terminal_vel - (straight_terminal_vel * 0.75))
        {
            _Lark_Animator.SetFloat("speed_mult", 1.0f);
            _Lark_Animator.SetBool("RUN_2", true);
        }
        else if (current_move_speed > 0.1f)
        {
            _Lark_Animator.SetBool("RUN_1", true);
        }
        else
        {
            _Lark_Animator.SetBool("NO_RUN", true);
        }
    }
}
