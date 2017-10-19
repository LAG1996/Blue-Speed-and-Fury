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

    public string CURRENT_CONTEXT;

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

        Valid_Keys.Add("w");
        Valid_Keys.Add("a");
        Valid_Keys.Add("s");
        Valid_Keys.Add("d");
        Valid_Keys.Add("space");

        CURRENT_CONTEXT = "NORMAL_PLAY";
       
    }

    void Start()
    {
        //Reference scripts
        //Lark_Script = (Lark)Lark_Obj.GetComponent(typeof(Lark));
       // Lark_Floater_Script = (Lark_Floater_v2)Camera_Focus_Obj.GetComponent(typeof(Lark_Floater_v2));
       // Camera_Script = (Camera_Move_v1)Camera.GetComponent(typeof(Camera_Move_v1));

        //_SetUpPlayerActions();
    }

    void Update()
    {
        InputReader.ReadMouseMovement(Context_To_Mouse_Hook[CURRENT_CONTEXT], new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

        bool read_key = false;

        Debug.Log("------------------INPUTS----------");
        foreach(var key in Valid_Keys)
        {
            if(Input.GetKey(key))
            {
                Debug.Log(key);
                read_key = true;
                InputReader.ReadKey(Context_To_Key_Hook[CURRENT_CONTEXT], key);
            }
        }
        Debug.Log("--------------END INPUTS----------");

        if (!read_key)
        {
            InputReader.ReadKey(Context_To_Key_Hook[CURRENT_CONTEXT], "");
        }
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
