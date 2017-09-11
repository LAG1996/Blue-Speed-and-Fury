using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysCore : MonoBehaviour {

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

    void Awake()
    {

        Valid_Keys = new List<string>();
        InputReader = new Controller_v2();
        ContextManager = new Context_Manager();

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
        Lark_Script = (Lark)Lark_Obj.GetComponent(typeof(Lark));
        Lark_Floater_Script = (Lark_Floater_v2)Camera_Focus_Obj.GetComponent(typeof(Lark_Floater_v2));
        Camera_Script = (Camera_Move_v1)Camera.GetComponent(typeof(Camera_Move_v1));

        _SetUpPlayerActions();

        Context_To_Switch_To = "NORMAL_PLAY";
    }

    void Update()
    {
        InputReader.ReadMouseMovement(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

        bool read_key = false;

        foreach(var key in Valid_Keys)
        {
            if(Input.GetKey(key))
            {
                read_key = true;
                InputReader.ReadKey(key);
            }
        }

        if(!read_key)
        {
            InputReader.ReadKey("");
        }

        _UpdateCamera();
    }

    void LateUpdate()
    {
        ContextManager.DoSwitchContext(Context_To_Switch_To);

        Context_To_Switch_To = "";
    }


    private void _UpdateCamera()
    {
        Camera_Script.UpdateSpeed(Lark_Script._myController.Max_Speed, Lark_Script._myController.Speed);
    }

    private void _SetUpPlayerActions()
    {
        InputReader.AddNewHook("Lark", new Controller_v2.PlayerKeyHook(Lark_Script.ReadInputHook));
        InputReader.AddNewHook("Lark_Floater", new Controller_v2.PlayerMouseHook(Lark_Floater_Script.GetMouseMovement));
        Context_Manager.ContextSwitchFunction func = () => { InputReader.SetActiveKeyHook("Lark"); InputReader.SetActiveMouseHook("Lark_Floater"); };
        ContextManager.SetFunction("NORMAL_PLAY", func);
    }
}
