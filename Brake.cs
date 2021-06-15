using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brake : MonoBehaviour {
   // bool is_click = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
      //  is_click = false;
    }

    public void OnClick()
    { // 必ず public にする
        Debug.Log("clicked");
     //   is_click = true;
        //return true;
    }
}

