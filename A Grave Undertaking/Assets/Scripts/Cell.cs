using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {

    public Color OriginalColor;

	// Use this for initialization
	void Start () {
        OriginalColor = GetComponent<Image>().color;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
