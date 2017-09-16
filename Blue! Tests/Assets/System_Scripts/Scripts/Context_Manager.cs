using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context_Manager {

    private SysCore.ContextSwitchFunction current_function;

    private Dictionary<string, SysCore.ContextSwitchFunction> Context_Switch_Functions;

    public Context_Manager()
    {
        current_function = null;

        Context_Switch_Functions = new Dictionary<string, SysCore.ContextSwitchFunction>();
    }

    public void DoSwitchContext(string context_name)
    {
        if(context_name != "")
            Context_Switch_Functions[context_name]();
    }

    public void SetFunction(string context_name, SysCore.ContextSwitchFunction func)
    {
        if(context_name != "")
            Context_Switch_Functions[context_name] = func;
    }
}