using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoard : MonoBehaviour {

    //Public variables
    public Dictionary<string, string> Controls = new Dictionary<string, string>();

    public GameObject Lark;

    //Private variables
    //Game components that the message board has to send messages to.

    private delegate void DelegatePlayerInput(string action_name);
    //Set up input delegates
    private DelegatePlayerInput _Lark;

    private DelegatePlayerInput _ActiveDelegate_Input;

    //The different contexts that the player's input may be in 
    private enum INPUT_CONTEXTS
    {
        LARK
    }

    //A list of basic messages for the message board to interpret
    private enum INPUT_MESSAGES
    {
        MOVE_LARK
    }

    private int _current_context;

	// Use this for initialization
	void Start () {
        _current_context = (int)INPUT_CONTEXTS.LARK;

        //Set player input controls
        Controls.Add("forward", "w");
        Controls.Add("left", "a");
        Controls.Add("back", "s");
        Controls.Add("right", "d");

        //Set up delegates
        try
        {
            Shit_Move_Lark_2 Lark_Movement_Script = (Shit_Move_Lark_2)Lark.GetComponent(typeof(Shit_Move_Lark_2));
            _Lark = new DelegatePlayerInput(Lark_Movement_Script.HandleKeyPress);
        }
        catch (UnityException e)
        {
            Debug.LogError("'Lark' not found or function HandleKeyPress() does not exist!");
        }
    }
	
	// Update is called once per frame
	void Update () {
        _SetInputDelegate();

        _Process_Messages();
	}

    void _Process_Messages()
    {
        _ReadPlayerInput();
    }

    void _ReadPlayerInput()
    {
        bool key_pressed = false;
        foreach(string key in Controls.Keys)
        {
            if(Input.GetKey(Controls[key]))
            {
                //Debug.Log("Pressed " + Controls[key]);

                key_pressed = true;
                _ActiveDelegate_Input(key);
            }
        }

        if(!key_pressed)
        {
            _ActiveDelegate_Input("");
        }
    }

    void _SetInputDelegate()
    {
        switch(_current_context)
        {
            case (int)INPUT_CONTEXTS.LARK:
                _ActiveDelegate_Input = _Lark;
                break;
        }
    }
}
