using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class that interprets inputs from the player. It sends the messages to the appropriate reciever for them to figure out.
public class Controller_v2 {

    private Dictionary<string, SysCore.PlayerKeyHook> KeyHooks;
    private Dictionary<string, SysCore.PlayerMouseHook> MouseHooks;

    private SysCore.PlayerKeyHook active_key_hook;
    private SysCore.PlayerMouseHook active_mouse_hook;

    private Dictionary<string, string> Key_To_Action;

    public Controller_v2()
    {
        KeyHooks = new Dictionary<string, SysCore.PlayerKeyHook>();
        MouseHooks = new Dictionary<string, SysCore.PlayerMouseHook>();
        Key_To_Action = new Dictionary<string, string>();


        Key_To_Action.Add("", "no_action");

        active_key_hook = null;
        active_mouse_hook = null;
    }

    public void AddNewHook(string name, SysCore.PlayerKeyHook func)
    {
        KeyHooks.Add(name, func);
    }

    public void AddNewHook(string name, SysCore.PlayerMouseHook func)
    {
        MouseHooks.Add(name, func);
    }

    public void SetActiveKeyHook(string name)
    {
        if(name != null)
            active_key_hook = KeyHooks[name];
    }

    public void SetActiveMouseHook(string name)
    {
        if(name != null)
            active_mouse_hook = MouseHooks[name];
    }

    public void SetKeyAction(string key, string action)
    {
        Key_To_Action.Add(key, action);
    }

    public void ReadMouseMovement(Vector2 mouse_dir)
    {
        if(active_mouse_hook != null)
            active_mouse_hook(mouse_dir);
    }

    public void ReadKey(string key_name)
    {
        if(active_key_hook != null)
            active_key_hook(Key_To_Action[key_name]);
    }
}
