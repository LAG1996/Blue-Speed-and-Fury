using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context_Manager {

    public delegate void ContextSwitchFunction();

    private ContextSwitchFunction current_function;

    private Dictionary<string, ContextSwitchFunction> Context_Switch_Functions;

    public Context_Manager()
    {
        current_function = null;

        Context_Switch_Functions = new Dictionary<string, ContextSwitchFunction>();
    }

    public void DoSwitchContext(string context_name)
    {
        if(context_name != "")
            Context_Switch_Functions[context_name]();
    }

    public void SetFunction(string context_name, ContextSwitchFunction func)
    {
        if(context_name != "")
            Context_Switch_Functions[context_name] = func;
    }
}