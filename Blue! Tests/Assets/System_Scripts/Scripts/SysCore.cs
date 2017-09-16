using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysCore : MonoBehaviour {

    //Some delegates that we can use globally
    public delegate void FunctionRef<T>(T input);
    public delegate void ContextSwitchFunction();
    public delegate void PlayerKeyHook(string action);
    public delegate void PlayerMouseHook(Vector2 dir);
    public delegate void InputReaderMessageSend();

    //All game objects that the player will interact with
    public GameObject Lark_Obj;
    public GameObject Camera_Focus_Obj;

    //Some game objects that the player indirectly controls
    public GameObject Camera;

    //Scripts that we are going to reference
    private Lark Lark_Script;
    private Lark_Floater_v2 Lark_Floater_Script;
    private Camera_Move_v1 Camera_Script;

    //Some public objects
    //A manager for the contexts that the game will be in
    private Context_Manager ContextManager;

    //An interpreter for the player's input
    private Controller_v2 InputReader;

    public static List<string> Valid_Keys;

    public string Context_To_Switch_To;

    private Dictionary<string, ContextSwitchFunction> Context_To_Function;
    private Dictionary<string, string> Context_To_Key_Hook;
    private Dictionary<string, string> Context_To_Mouse_Hook;

    void Awake()
    {
        Context_To_Function = new Dictionary<string, ContextSwitchFunction>();
        Context_To_Key_Hook = new Dictionary<string, string>();
        Context_To_Mouse_Hook = new Dictionary<string, string>();
        Valid_Keys = new List<string>();
        InputReader = new Controller_v2();
        ContextManager = new Context_Manager();

        Context_To_Key_Hook.Add("NORMAL_PLAY", null);
        Context_To_Mouse_Hook.Add("NORMAL_PLAY", null);
        Context_To_Function.Add("NORMAL_PLAY", null);

        InputReader.SetKeyAction("w", "forward");
        InputReader.SetKeyAction("a", "left");
        InputReader.SetKeyAction("s", "back");
        InputReader.SetKeyAction("d", "right");
        InputReader.SetKeyAction("space", "jump");

        Valid_Keys.Add("w");
        Valid_Keys.Add("a");
        Valid_Keys.Add("s");
        Valid_Keys.Add("d");
        Valid_Keys.Add("space");

        Context_To_Switch_To = "";
       
    }

    void Start()
    {
        //Reference scripts
        //Lark_Script = (Lark)Lark_Obj.GetComponent(typeof(Lark));
       // Lark_Floater_Script = (Lark_Floater_v2)Camera_Focus_Obj.GetComponent(typeof(Lark_Floater_v2));
       // Camera_Script = (Camera_Move_v1)Camera.GetComponent(typeof(Camera_Move_v1));

        //_SetUpPlayerActions();

        Context_To_Switch_To = "NORMAL_PLAY";

        Context_To_Function["NORMAL_PLAY"] = () => {

            InputReader.SetActiveKeyHook(Context_To_Key_Hook["NORMAL_PLAY"]);
            InputReader.SetActiveMouseHook(Context_To_Mouse_Hook["NORMAL_PLAY"]);

        };

        Context_To_Function["NORMAL_PLAY"]();
    }

    void Update()
    {
        InputReader.ReadMouseMovement(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

        bool read_key = false;

        Debug.Log("------------------INPUTS----------");
        foreach(var key in Valid_Keys)
        {
            if(Input.GetKey(key))
            {
                Debug.Log(key);
                read_key = true;
                InputReader.ReadKey(key);
            }
        }
        Debug.Log("--------------END INPUTS----------");

        if (!read_key)
        {
            InputReader.ReadKey("");
        }

        //_UpdateCamera();
    }

    void LateUpdate()
    {
        //ContextManager.DoSwitchContext(Context_To_Switch_To);

        Context_To_Switch_To = "";
    }


    private void _UpdateCamera()
    {
        Camera_Script.UpdateSpeed(Lark_Script._myController.Max_Speed, Lark_Script._myController.Speed);
    }

    private void _SetUpPlayerActions()
    {
        InputReader.AddNewHook("Lark", new PlayerKeyHook(Lark_Script.ReadInputHook));
        InputReader.AddNewHook("Lark_Floater", new PlayerMouseHook(Lark_Floater_Script.GetMouseMovement));
        ContextSwitchFunction func = () => { InputReader.SetActiveKeyHook("Lark"); InputReader.SetActiveMouseHook("Lark_Floater"); };
        ContextManager.SetFunction("NORMAL_PLAY", func);
    }

    public void AddKeyHook(string hook_name, string context, PlayerKeyHook func)
    {
        InputReader.AddNewHook(hook_name, func);

        Context_To_Key_Hook[context] =  hook_name;
    }

    public void AddMouseHook(string hook_name, string context, PlayerMouseHook func)
    {
        InputReader.AddNewHook(hook_name, func);

        Context_To_Mouse_Hook[context] = hook_name;
    }

    public void SwitchContext(string to_context)
    {
        Context_To_Function[to_context]();
    }
}
