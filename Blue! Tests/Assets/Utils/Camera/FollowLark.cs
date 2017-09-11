using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLark : MonoBehaviour {

    public Transform Lark;

    private Vector3 offset;
	// Use this for initialization
	void Start () {
        offset = gameObject.transform.position - Lark.position;
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = Lark.position + offset;
	}
}
