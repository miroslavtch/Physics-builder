/*
 * Author: Miroslav Tchakalov
 * Project: Physics Builder
 * Project Description: A Virtual reality building app, user can put objects together in order to build something
 * Date : 11.06.2018
 * Class: Tiles.cs
 * Desc: Handles the events on the tiles, this script is add to each tile.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class ObjTilesHandler : MonoBehaviour {
	private Color onColor;
	private Color offColor;
	[SerializeField] private VRInteractiveItem m_InteractiveItem;
	public bool isObject {
		get;
		set;
	}


	//this method is called when the tiles are generated and enabled.
	private void OnEnable()
	{
		m_InteractiveItem.OnClick += HandleClick;
		this.GetComponent<Toggle> ().group = this.GetComponentInParent<ToggleGroup> ();
		onColor = Color.cyan;
		offColor = Color.gray;
	}

	//This method is called when the tiles are desabled
	private void OnDisable()
	{
		m_InteractiveItem.OnClick -= HandleClick;
	}

	//Handles the onClickEvent on the tile
	private void HandleClick()
	{
		if (isObject) {
			//Adds the name of the object to the objectData class
			ObjectData.ObjectName = m_InteractiveItem.GetComponent<Text> ().text;
		} 
		else 
		{
			//Adds the name of the material to the objectData class
			ObjectData.MaterialName = m_InteractiveItem.GetComponent<Text> ().text;
		}

	
		this.GetComponent<Toggle> ().isOn = true;
	}
	void Update()
	{
		//Changes the color of the selected and unselected tiles
		if (this.GetComponent<Toggle> ().isOn) {
			this.GetComponentInChildren<Image> ().color = onColor;
		} else {
			this.GetComponentInChildren<Image> ().color = offColor;
		}
	}
}
