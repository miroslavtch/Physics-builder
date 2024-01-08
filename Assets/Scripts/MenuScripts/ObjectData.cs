/*
 * Author: Miroslav Tchakalov
 * Project: Physics Builder
 * Project Description: A Virtual reality building app, user can put objects together in order to build something
 * Date : 11.06.2018
 * Class: ObjectData.cs
 * Desc: has no methods, it is a class that contains all the useful information about an object about to be created
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectData {

	public static string ObjectName 
	{
		get;
		set;
	}

	public static string ObjectPath 
	{
		get;
		set;
	}
	public static string MaterialName {
		get;
		set;
	}
	public static string objType {
		get;
		set;
	}
	public static bool isOpaque {
		get;
		set;
	}
}
