using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class that interprets inputs from the player. It sends the messages to the appropriate reciever for them to figure out.
public class Controller_v2 {

    private Dictionary<string, SysCore.PlayerKeyHook> KeyHooks;
    private Dictionary<string, SysCore.PlayerMouseHook> MouseHooks;


    public Controller_v2()
    {
        KeyHooks = new Dictionary<string, SysCore.PlayerKeyHook>();
        MouseHooks = new Dictionary<string, SysCore.PlayerMouseHook>();
    }

    public void AddNewHook(string name, SysCore.PlayerKeyHook func)
    {
        KeyHooks.Add(name, func);
    }

    public void AddNewHook(string name, SysCore.PlayerMouseHook func)
    {
        MouseHooks.Add(name, func);
    }

    public void ReadMouseMovement(string hook_name, Vector2 mouse_dir)
    {
        if(hook_name != null && MouseHooks.ContainsKey(hook_name))
            MouseHooks[hook_name](mouse_dir);
    }

    public void ReadKey(string hook_name, string key_name)
    {
        if(hook_name != null && KeyHooks.ContainsKey(hook_name))
            KeyHooks[hook_name](key_name);
    }
}
