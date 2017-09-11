using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shit_Move_Lark : MonoBehaviour {
    //This is a crappy first attempt at programming Lark's movement. Instantaneous velocity, no rotating. Hell, not even any animations.
    //I'm writing this so that I have a base starting point from which to work. Think of this like a warm-up

    //Public variables
    public float max_run_speed;
    private float acceleration;
    
    //Private variables
    private float current_run_speed;
    private float x_input;
    private float z_input;
    private Transform Lark_Transform;

    // Use this for initialization
	void Start () {
        x_input = 0.0f;
        z_input = 0.0f;

        Lark_Transform = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {

        //For now, read the inputs and map them directly to moving Lark. In reality, the inputs should change meaning in different contexts.
        //For example, Lark can grind on rails, so hitting left may mean switch to a rail on the left instead of physically moving left.
        //Furthermore, inputs in different contexts can effect finer things such as what animation to use. If I'm hitting left during Lark's
        //guard animation, then she would not tilt left or switch to a run animation with a slight left tilt. She should be keeping the same animation,
        //but swiftly skipping to the left.
        //Hell, left might mean something in a menu system instead of moving. Context matters.
        //Thus, the architecture needs to handle different contexts. We'll worry about that later.

        _ReadInputs();

        Lark_Transform.Translate(new Vector3(x_input * max_run_speed * Time.deltaTime, 0.0f, z_input * max_run_speed * Time.deltaTime));
	}

    void _ReadInputs()
    {
        x_input = Input.GetAxis("Horizontal");
        z_input = Input.GetAxis("Vertical");
    }
}
