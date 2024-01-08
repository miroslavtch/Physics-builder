/*
 * Author: Filip Keks
 * Edited by : Miroslav Tchakalov
 * Project: Physics Builder
 * Project Description: A Virtual reality building app, user can put objects together in order to build something
 * Date : 11.06.2018
 * Class: ObjectData.cs
 * Desc: This class is enabling the user to select a color from the colorPicker
 * 
 * Changes: The script was made for the mouse and keyboard, the events and functions had to be almost entirely changed so that it could work with the vive controllers
 */

using UnityEngine;

public class Draggable : MonoBehaviour
{  
	#region Variables
	public SteamVR_TrackedObject rightController;

	public Transform minBound;

	public bool fixX;
	public bool fixY;
	public Transform thumb;	
	bool dragging;
	Ray ray;
	RaycastHit hit;
	Collider cPickerCol;
	#endregion

	#region Methods

	// When the colorpicker is created
	void Start()
	{
		cPickerCol = this.GetComponent<Collider> ();
	}

	void FixedUpdate()
	{
		// Gets the right controller
		SteamVR_Controller.Device device = SteamVR_Controller.Input ((int)rightController.index);

		//Checks if the person has clicked on the menuButton
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) 
		{
			ray = new Ray(rightController.transform.position, rightController.transform.forward);
			dragging = false;

			// This passes when the raycast touches the surface of the colorpicker
			if (cPickerCol.Raycast(ray, out hit, 100)) 
			{
				//Enables the user to drag on the colorpicker while holding the button
				dragging = true;
			}
		}

		//The user stops dragging through the colorPicker when the button is released
		if (device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)) dragging = false;

		//When the user has finished entirely pressing the button
		if (dragging && device.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			ray = new Ray(rightController.transform.position, rightController.transform.forward);

			//If it hits the colorPicker, it will set the thumb position to the point where the raycast hit
			if (cPickerCol.Raycast(ray, out hit, 100)) 
			{
				Vector3 point = hit.point;
				SetThumbPosition(point);
				SendMessage("OnDrag", Vector3.one - (thumb.localPosition - minBound.localPosition) / GetComponent<BoxCollider>().size.x);
			}


		}
	}
		
	/// <summary>
	/// Sets the drag point.
	/// </summary>
	/// <param name="point">point where the raycast hit</param>
	void SetDragPoint(Vector3 point)
	{
		point = (Vector3.one - point) * GetComponent<Collider>().bounds.size.x + GetComponent<Collider>().bounds.min;
		SetThumbPosition(point);
	}

	/// <summary>
	/// Sets the thumb to the position where the raycast hit
	/// </summary>
	/// <param name="point">Position of where to put the thumb</param>
	void SetThumbPosition(Vector3 point)
	{
		Vector3 tmp = thumb.localPosition;
		thumb.position = point;
		thumb.localPosition  = new Vector3(fixX ? tmp.x : thumb.localPosition.x, fixY ? tmp.y : thumb.localPosition.y, thumb.localPosition.z - 1f);
	}

	#endregion
}
