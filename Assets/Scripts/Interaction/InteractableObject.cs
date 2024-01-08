/*
 * Author: Miroslav Tchakalov
 * Project: Physics Builder
 * Project Description: A Virtual reality building app, user can put objects together in order to build something
 * Date : 11.06.2018
 * Class: InteractableObject.cs
 * Desc: Represents an object with which a user can interact, theese objects are all the objects that the user can pick up, scale and edit.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {

	#region Variables

	//Informations about the object
	public Rigidbody rb;
	private Transform gripPoint;
	private bool inInteraction;

	//Controllers that can be attached to the object
	private ControllerInteraction AttachedController;
	private ControllerInteraction AttachedController2;


	// All the factors that need to be calculated so that the object will behave normally when interacted with.
	private Vector3 posDelta;
	private Vector3 origDeltaControllers;
	private Quaternion rotDelta;
	private float angle;
	private Vector3 axis;
	private float rotationFactor = 400f;
	private float velocityFactor = 20000f;

	public GameObject collisionedObj;

	private Vector3 origObjScale;
	#endregion

	// Use this for initialization
	void Start () {
		
		rb = GetComponent<Rigidbody> ();
		gripPoint = new GameObject ().transform;
		velocityFactor /= rb.mass;
		rotationFactor /= rb.mass;
	}
	
	// Update is called once per frame
	void Update () {

		// Checks if the object is interacting with at least one controller
		if (AttachedController && isInteracting()) 
		{
			//Calculates the distance between the object and the controller
			posDelta = AttachedController.transform.position - gripPoint.position;

			//Sets the velocity of the object (the speed on which it will follow the controller)
			this.rb.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

			// Checks if the object is interacting with a second controller
			if (AttachedController2) 
			{	
				ScaleObject ();
			} 
			else 
			{
				rotDelta = AttachedController.transform.rotation * Quaternion.Inverse (gripPoint.rotation);
				rotDelta.ToAngleAxis (out angle, out axis);

				if (angle > 180) {
					angle -= 360;
				}
				
				this.rb.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
			}
		}

	}

	/// <summary>
	/// Scales this object.
	/// </summary>
	private void ScaleObject()
	{
		// Puts the object between the two controllers
		this.transform.position  = (AttachedController.transform.position + AttachedController2.transform.position ) / 2;
		Vector3 posDeltaController = new Vector3 ();
		this.transform.rotation = new Quaternion ();

		// Scales the object, depending on the distance between the two controllers
		posDeltaController = AttachedController.transform.position - AttachedController2.transform.position - origDeltaControllers;
		this.transform.localScale = origObjScale + posDeltaController;
	}

	/// <summary>
	/// Begins the interaction between a controller and an object
	/// </summary>
	/// <param name="pController">Controller which interacts with the object</param>
	public void BeginInteraction(ControllerInteraction pController)
	{
		// Checks if the object isn't already in interaction with any controller
		if (AttachedController != pController && AttachedController2 != pController) 
		{
			AttachedController = pController;
			setInteractionPoint ();
			inInteraction = true;

		}

	}

	/// <summary>
	/// Ends the interaction between an object and a controller
	/// </summary>
	/// <param name="pController">P controller.</param>
	public void EndInteraction(ControllerInteraction pController)
	{
		if (pController == AttachedController) {
			AttachedController = null;
			if (AttachedController2) {
				AttachedController = AttachedController2;
			}

		}	
		if (pController == AttachedController2 || AttachedController == AttachedController2) {
			this.AttachedController2 = null;
		}

		if (!AttachedController && !AttachedController2) 
		{
			inInteraction = false;
		}
		else
		{
			this.transform.position = AttachedController.transform.position;
		}


	}

	/// <summary>
	/// Matches the position and rotation of the object to that of the controller.
	/// </summary>
	private void setInteractionPoint()
	{
		gripPoint.position = AttachedController.transform.position;
		gripPoint.rotation = AttachedController.transform.rotation;
		gripPoint.SetParent (this.transform,true);
	}

	/// <summary>
	/// Verifies if the object is the interacting.
	/// </summary>
	/// <returns><c>true</c>, the object is interacting <c>false</c> The object is not interacting.</returns>
	public bool isInteracting()
	{
		return inInteraction;
	}

	/// <summary>
	/// EventHandler called whenever an object is destroyed	
	/// </summary>
	public void OnDestroy()
	{
		if (gripPoint) {
			Destroy (gripPoint.gameObject);
		}	
	}

	/// <summary>
	/// Adds the second controller (for scaling).
	/// </summary>
	/// <param name="pController">Second controller to be added to the object.</param>
	public void addSecondController(ControllerInteraction pController)
	{
		this.AttachedController2 = pController;
		origDeltaControllers = this.AttachedController.transform.position - AttachedController2.transform.position;
		origObjScale = this.transform.localScale;
	}

	/// <summary>
	/// Raises the collision enter event.
	/// </summary>
	/// <param name="collision">Collision object</param>
	void OnCollisionEnter (Collision collision)
	{
		collisionedObj = collision.gameObject;
	}

	/// <summary>
	/// Raises the collision exit event.
	/// </summary>
	void OnCollisionExit()
	{
		collisionedObj = null;
	}

	/// <summary>
	/// Creates a joint on the current object to attach it to the object with which the object is in collision
	/// </summary>
	public void CreateJoint ()
	{
		// If the object is in collision with another object, this object creates a fixed joint and connects it to the other object
		if (collisionedObj.GetComponent<Rigidbody>() != null) {
			gameObject.AddComponent<FixedJoint> ();  
			gameObject.GetComponent<FixedJoint> ().connectedBody = collisionedObj.GetComponent<Rigidbody>();
		}
	}

	/// <summary>
	/// Destroies the joint on this object
	/// </summary>
	public void DestroyJoint()
	{
		if (this.GetComponent<FixedJoint> () != null) 
		{
			Destroy (this.GetComponent<FixedJoint> ());
		}

	}
}
