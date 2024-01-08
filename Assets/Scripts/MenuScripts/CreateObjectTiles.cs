/*
 * Author: Miroslav Tchakalov
 * Project: Physics Builder
 * Project Description: A Virtual reality building app, user can put objects together in order to build something
 * Date : 11.06.2018
 * Class: CreateObjectTiles.cs
 * Desc: This script creates the tiles in the menu of avaible objects 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class CreateObjectTiles : MonoBehaviour
{
	// The model of a tile, a prefab object which can be any tile
	private GameObject Prefab; 

	// a list of all the prefab names
	List<string> PrefabsFiles;

	/// <summary>
	/// Populate the menu with the all the tiles
	/// </summary>
	/// <param name="path">Path of the object from whom to create tiles.</param>
	/// <param name="parentTransform">The transform component of the parent gameObject </param>
	public void Populate(string path, Transform parentTransform, bool isObj)
	{
		GameObject newObj; // Creates a tile

		Prefab = (GameObject)Resources.Load("TilePrefab");

		List<string> PrefabsName = new List<string>();
		PrefabsFiles = Directory.GetFiles(Directory.GetCurrentDirectory() + path).Where(name => !name.EndsWith(".meta")).ToList();
		foreach (string prefabPath in PrefabsFiles) 
		{
			PrefabsName.Add (Path.GetFileNameWithoutExtension (prefabPath));
		}


		for (int i = 0; i < PrefabsName.Count; i++)
		{
			// Create new instances of our prefab until we've created as many as we specified
			newObj = (GameObject)Instantiate(Prefab, parentTransform);
			newObj.GetComponent<Text> ().text = PrefabsName [i];
			newObj.GetComponent<ObjTilesHandler> ().isObject = isObj;
		}

	}

}
