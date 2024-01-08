using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {

	public Color chosenColor;
	// Use this for initialization
	void OnColorChange(HSBColor color)
	{
		this.chosenColor = color.ToColor ();
	}
}
