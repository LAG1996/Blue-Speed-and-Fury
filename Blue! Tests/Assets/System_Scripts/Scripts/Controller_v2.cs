using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class that interprets inputs from the player. It sends the messages to the appropriate reciever for them to figure out.
public class Controller_v2 {

    public delegate void PlayerKeyHook(string action);
    public delegate void PlayerMouseHook(Vector2 dir);

    private Dictionary<string, PlayerKeyHook> KeyHooks;
    private Dictionary<string, PlayerMouseHook> MouseHooks;

    private PlayerKeyHook active_key_hook;
    private PlayerMouseHook active_mouse_hook;

    private Dictionary<string, string> Key_To_Action;

    public Controller_v2()
    {
        KeyHooks = new Dictionary<string, PlayerKeyHook>();
        MouseHooks = new Dictionary<string, PlayerMouseHook>();
        Key_To_Action = new Dictionary<string, string>();


        Key_To_Action.Add("", "no_action");

        active_key_hook = null;
        active_mouse_hook = null;
    }

    public void AddNewHook(string name, PlayerKeyHook func)
    {
        KeyHooks.Add(name, func);
    }

    public void AddNewHook(string name, PlayerMouseHook func)
    {
        MouseHooks.Add(name, func);
    }

    public void SetActiveKeyHook(string name)
    {
        active_key_hook = KeyHooks[name];
    }

    public void SetActiveMouseHook(string name)
    {
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
