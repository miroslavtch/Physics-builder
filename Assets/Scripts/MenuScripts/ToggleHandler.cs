/*
 * Author: Miroslav Tchakalov
 * Project: Physics Builder
 * Project Description: A Virtual reality building app, user can put objects together in order to build something
 * Date : 11.06.2018
 * Class: ToggleHandler.cs
 * Desc: Handles the click event on the toggles that change the tiles
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class ToggleHandler : MonoBehaviour {

	[SerializeField] private VRInteractiveItem m_InteractiveItem;

	#region eventHandlers
	private void OnEnable()
	{
		m_InteractiveItem.OnClick += HandleClick;
	}


	private void OnDisable()
	{

		m_InteractiveItem.OnClick -= HandleClick;
	}

	//Handle the Click event
	private void HandleClick()
	{
		m_InteractiveItem.GetComponent<Toggle> ().isOn = !m_InteractiveItem.GetComponent<Toggle> ().isOn;

				
	}
	#endregion
}
