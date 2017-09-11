using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_v1 : MonoBehaviour {

    private Dictionary<string, string> LARK_CONTROLS = new Dictionary<string, string>();

    private Dictionary<string, string> ACTIVE_CONTROLS;

    private Dictionary<string, INPUT_CONTEXTS> String_To_Contexts = new Dictionary<string, INPUT_CONTEXTS>();

    //Hooks
    private delegate void Player_Button_Hook(List<string> action_buffer);
    private delegate void Player_Mouse_Move_Hook(Vector2 position_delta);

    //Set up input delegates
    private Player_Button_Hook _Lark_Input;
    private Player_Mouse_Move_Hook _Camera_Target_Input;

    //Helper variables
    private Player_Button_Hook _Active_Input_Hook;
    private Player_Mouse_Move_Hook _Active_Mouse_Move_Hook;


    //Objects we want to hook into
    //Lark scripts
    public GameObject Lark;
    public GameObject Camera_Target;

    //Different contexts for the player's input
    private enum INPUT_CONTEXTS
    {
        NORMAL_PLAY
    }


	// Use this for initialization
	void Start () {

        //Set up default controls
        LARK_CONTROLS.Add("forward", "w");
        LARK_CONTROLS.Add("left", "a");
        LARK_CONTROLS.Add("back", "s");
        LARK_CONTROLS.Add("right", "d");

        //Add mappings to the contexts
        String_To_Contexts.Add("NORMAL_PLAY", INPUT_CONTEXTS.NORMAL_PLAY);

        //Get the functions from other scripts that we want to hook into
        //Lark
        Shit_Move_Lark_2 Lark_Movement_Script = (Shit_Move_Lark_2)Lark.GetComponent(typeof(Shit_Move_Lark_2));
        _Lark_Input = new Player_Button_Hook(Lark_Movement_Script.HandleKeyPressBuffer);

        //Camera Target (Lark's Head)
        Lark_Floater_Script Camera_Target_Script = (Lark_Floater_Script)Camera_Target.GetComponent(typeof(Lark_Floater_Script));
        _Camera_Target_Input = new Player_Mouse_Move_Hook(Camera_Target_Script.GetMouseMovement);

        //Switch to the normal play context on game start
        _SwitchContext(INPUT_CONTEXTS.NORMAL_PLAY);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update () {
        _Read_Player_Buttons();
        _Read_Player_Mouse();
	}

    void _Read_Player_Buttons()
    {
        List<string> action_buffer = new List<string>();

        action_buffer.Add("");

        foreach(string action in ACTIVE_CONTROLS.Keys)
        {
            if(Input.GetKey(ACTIVE_CONTROLS[action]))
            {
                action_buffer.Add(action);
            }
        }

        _Active_Input_Hook(action_buffer);

    }

    void _Read_Player_Mouse()
    {
        _Active_Mouse_Move_Hook(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    }

    public void _SwitchContext(string context)
    {
        _SwitchContext(String_To_Contexts[context]);
    }

    private void _SwitchContext(INPUT_CONTEXTS context)
    {
        switch(context)
        {
            case INPUT_CONTEXTS.NORMAL_PLAY:
                _SwitchToNormalPlay();
                break;
        }
    }

    private void _SwitchActiveInputHook(Player_Button_Hook hook)
    {
        _Active_Input_Hook = hook;
    }

    private void _SwitchActiveMouseMoveHook(Player_Mouse_Move_Hook hook)
    {
        _Active_Mouse_Move_Hook = hook;
    }

    private void _SwitchToNormalPlay()
    {
        _SwitchActiveInputHook(_Lark_Input);
        _SwitchActiveMouseMoveHook(_Camera_Target_Input);

        ACTIVE_CONTROLS = LARK_CONTROLS;
    }
}
