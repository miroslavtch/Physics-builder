/*
 * Author: Miroslav Tchakalov
 * Project: Physics Builder
 * Project Description: A Virtual reality building app, user can put objects together in order to build something
 * Date : 11.06.2018
 * Class: ValidateForm.cs
 * Desc: Submits the menu, when the user validates the menu, this class creates the choosen GameObject
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class validateForm : MonoBehaviour {

	private const string MATERIAL_PATH = "ObjectsPrefabs/Primitives/Materials/Materials/";
	[SerializeField] private VRInteractiveItem m_InteractiveItem;


	#region EventHandlers
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
		this.Generate ();
	}
	#endregion

	/// <summary>
	/// Is called when the btnGenerate is clicked,this method generates the chosen gameObject and puts it in the scene
	/// </summary>
	private void Generate()
	{
		Material objMaterial = (Material)Resources.Load (MATERIAL_PATH + ObjectData.MaterialName);

		//Loads the prefab in a gameObject variable
		GameObject objPrefab = (GameObject)Resources.Load(ObjectData.ObjectPath + ObjectData.ObjectName);

		//Intantiates the new object in the game
		GameObject NewObject = GameObject.Instantiate(objPrefab);
		NewObject.name = ObjectData.ObjectName;

		if (ObjectData.objType == "Primitive") {
			NewObject.GetComponent<MeshRenderer> ().material = objMaterial;
		}

		// Puts the newly created object at the same position as the right controller
		NewObject.transform.position = GameObject.Find ("Controller (right)").transform.position;


	}
}