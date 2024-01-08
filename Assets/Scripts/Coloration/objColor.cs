using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objColor : MonoBehaviour {

	public ColorManager cm;

	// Update is called once per frame
	void Update () 
	{
		if (GameObject.Find("ColorPicker") && this.GetComponent<InteractableObject>().isInteracting()) {
			cm = GameObject.Find("ColorPicker").GetComponent<ColorManager> ();
			GetComponent<Renderer> ().material.color = cm.chosenColor;
		}

	}
}