/*
 * Author: Miroslav Tchakalov
 * Project: Physics Builder
 * Project Description: A Virtual reality building app, user can put objects together in order to build something
 * Date : 11.06.2018
 * Class: ControllerInteraction.cs
 * Desc: This script is the center of all scripts, represents a controller and handles all the interactions.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ControllerInteraction : MonoBehaviour {


	#region Variables, Properties

	//Buttons of the controller
	private Valve.VR.EVRButtonId btnTrigger = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private Valve.VR.EVRButtonId btnMenu = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
	private Valve.VR.EVRButtonId btnColorPicker = Valve.VR.EVRButtonId.k_EButton_Grip;

	//The controller and the trackedObject
	private SteamVR_Controller.Device Controller { get{ return SteamVR_Controller.Input((int)ObjTracked.index); } }
	private SteamVR_TrackedObject ObjTracked;

	//the interactable objects variables
	HashSet<InteractableObject> ObjectsHovering = new HashSet<InteractableObject>();
	private InteractableObject nearestItem;
	private InteractableObject currentObject;

	private GameObject MenuCanvas;
	private bool canTeleport;
	ScrollRect TilesScroll;
	ColorManager cm;

	private GameObject ColorPicker;

	#endregion

	#region Methods
	// Use this for initialization
	void Awake()
	{
		ObjTracked = GetComponent<SteamVR_TrackedObject>();
		MenuCanvas = GameObject.Find ("Menu_Canvas");
		ColorPicker = GameObject.Find ("ColorPicker");
	}
	
	// Update is called once per frame
	void Update () {
		if (Controller == null) 
		{
			Debug.Log ("Controller is not initialized");

		}


		// checks if the trigger buton is pressed down
		if (Controller.GetPressDown(btnTrigger)) 
		{
			pickupObject();
		}

		// Checks if the user has an object in hand, drops it if the trigger button is released
		if (Controller.GetPressUp(btnTrigger) && currentObject != null) 
		{
			currentObject.EndInteraction (this);
			currentObject = null;

		}
			

		// Checks if the Menu is enabled, the user cannot teleport if this is the case
		if (MenuCanvas) 
		{
			if (this.name == "Controller (left)" && MenuCanvas.activeSelf) 
			{
				if (ObjectData.objType == "Primitive") {
					EnableScrolling (GameObject.Find ("ScrViewMaterials").GetComponent<ScrollRect> ());
				}
				else 
				{
					EnableScrolling (GameObject.Find ("ScrViewPrefabs").GetComponent<ScrollRect> ());
				}
			}

			// The condition to close or open the menu
			if (Controller.GetPressUp(btnMenu) && this.name == "Controller (left)") 
			{
				MenuCanvas.SetActive (!MenuCanvas.activeSelf);
			}

		}


		if (Controller.GetPressUp(btnColorPicker) && this.name == "Controller (left)") 
		{
			
			ColorPicker.SetActive (!ColorPicker.activeSelf);
		}
		
	
		// Checks if the edition mode can be enabled
		if (currentObject != null && currentObject.isInteracting ()) 
		{
			editionMode ();
		} 
		ToggleTeleport ();

	}

	/// <summary>
	/// Picks up an object and starts an interaction.
	/// </summary>
	private void pickupObject()
	{
		// Loops through all the objects and checks which one is the nearest object
		ObjectsHovering.ToList ().ForEach (GrippedObject => calculateDistance (GrippedObject));

		// The current picked up object is the nearest one to the controller
		currentObject = nearestItem;

		//Checks if there is a current object to pick up
		if (currentObject) 
		{
			//Adds the second controller (for scaling) if the object is picked up by two controllers
			if (currentObject.isInteracting()) 
			{
				currentObject.addSecondController (this);

			}	
			else
			{
				currentObject.BeginInteraction (this);
			}
		}
	}

	/// <summary>
	/// Activates the edition mode and handles all the editing action that can be done to an object
	/// </summary>
	private void editionMode()
	{
		// Checks if the user has clicked on the controller's touchpad
		if (Controller.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad)) {

			// The upper button of the touchpad (Creates joint if the current object touches another object)
			if (Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y > 0.3f) {
				if (currentObject.collisionedObj != null) {
					currentObject.CreateJoint ();
				}
			}

			// The lower button of the touchpad (Destroys the joint of the current object)
			if (Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y < -0.3f) {
				currentObject.DestroyJoint ();
			}

			// The right button of the touchpad (Clones the current object)
			if (Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x > 0.5f) {
				GameObject.Instantiate (currentObject.gameObject);

			}

			// The left button of the touchpad (Deletes the current object and all the connections that is has)
			if (Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x < -0.5f) {
				currentObject.EndInteraction (this);
				ObjectsHovering.Remove (currentObject);
				Destroy (currentObject.gameObject);
				currentObject = null;
			}
		}
	}

	// Finds the nearest object among all the objects surrounding the user
	private void calculateDistance(InteractableObject pGripObj)
	{
		float minDistance = float.MaxValue;
		float distance;

		distance = (pGripObj.transform.position - this.transform.position).sqrMagnitude;

		if (distance < minDistance) 
		{
			minDistance = distance;
			nearestItem = pGripObj;
		}

	}

	// Activates and deactivates the ability of a user to teleport
	private void ToggleTeleport()
	{
		bool canTeleport = false;
		TeleportVive teleporter = GameObject.Find ("Camera (eye)").GetComponent<TeleportVive> ();

			// User can teleport if the menu isn't open and he holds no object in hands
			if (MenuCanvas && !MenuCanvas.activeSelf) 
			{
				canTeleport = !currentObject || !currentObject.isInteracting();
			}
			

		teleporter.enabled = canTeleport;

	}

	// Event triggered when the controller enters the collider of an object.
	private void OnTriggerEnter(Collider col)
	{
		// adds the collidedItem in the list of objects
		InteractableObject collidedItem = col.GetComponent<InteractableObject> ();
		if (collidedItem) {
			ObjectsHovering.Add (collidedItem);
		}

	}

	// Event triggered when the controller exits the collider of an object
	private void OnTriggerExit(Collider col)
	{
		
		InteractableObject collidedItem = col.GetComponent<InteractableObject> ();
		if (collidedItem) {
			ObjectsHovering.Remove (collidedItem);
		}

	
	}

	// Enables the scrolling in the tiles window contained in the menu
	private void EnableScrolling(ScrollRect pView)
	{
		TilesScroll = pView;

		// How much will the menu scroll per quick (0.0 .. 1.0)
		float scrollOffset = 0.05f;


		// When the buttons up and down of the touchpad is pressed, the scrolling will happen
		if (Controller.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad)) {
			if (Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y > 0.5f) {
				TilesScroll.verticalNormalizedPosition -= scrollOffset;

			}
			if (Controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y < -0.3f) {
				TilesScroll.verticalNormalizedPosition += scrollOffset;
			}

		}

		// Scrolls through the tiles.
		if (TilesScroll.verticalNormalizedPosition >= 1.0f) 
		{
			TilesScroll.verticalNormalizedPosition = 0.0f;	
		}

	}
	#endregion
}
