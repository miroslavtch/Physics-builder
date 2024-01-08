/*
 * Author: Miroslav Tchakalov
 * Project: Physics Builder
 * Project Description: A Virtual reality building app, user can put objects together in order to build something
 * Date : 11.06.2018
 * Class: MenuGenerator.cs
 * Desc: This script generates the content in the menu
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class MenuGenerator : MonoBehaviour {
	
	#region Variables

	private const string  MATERIALS_PATH = "/ObjectsPrefabs/Primitives/Materials/Materials/";
	private const string  PRIMITIVES_PATH = "ObjectsPrefabs/Primitives/";
	private const string FURNITURES_PATH = "ObjectsPrefabs/Furniture/Prefabs/";
	private const string INFRASTRUCTURES_PATH = "ObjectsPrefabs/Town Creator Kit LITE/Prefabs/";
	private const string RESOURCES_PATH = "/Assets/Resources/";

	// Lists of the different lists containing the infos of the menu's components
	Transform objTilesContainer;
	Transform MatObjContainer;

	CreateObjectTiles tiles;
	#endregion

	/// <summary>
	/// This method is called when the programm starts
	/// </summary>
	void Start()
	{
		//Gets all the files from the assets and puts them in the lists
		objTilesContainer = GameObject.Find ("ObjTIlesContainer").transform;
		MatObjContainer = GameObject.Find ("MatTilesContainer").transform;
		tiles = new CreateObjectTiles ();

		tiles.Populate (RESOURCES_PATH + MATERIALS_PATH, MatObjContainer,false);

	}

	#region Generation of tiles depending on which toggle is ON

	/// <summary>
	/// Creates the primitive types tiles in the menu	
	/// </summary>
	public void CreatePrimitiveTiles()
	{
		this.clearTiles ();
		tiles.Populate (RESOURCES_PATH + PRIMITIVES_PATH,objTilesContainer,true);
		ObjectData.ObjectPath = PRIMITIVES_PATH;
		MatObjContainer.gameObject.SetActive (true);
		ObjectData.objType = "Primitive";
	}

	/// <summary>
	/// Creates the infrastructure tiles.
	/// </summary>
	public void CreateInfrastructureTiles()
	{
		this.clearTiles ();
		tiles.Populate (RESOURCES_PATH + INFRASTRUCTURES_PATH, objTilesContainer,true);
		ObjectData.ObjectPath = INFRASTRUCTURES_PATH;
		MatObjContainer.gameObject.SetActive (false);
		ObjectData.objType = "Infrastructure";
	}


	/// <summary>
	/// Creates the furniture tiles.
	/// </summary>
	public void CreateFurnitureTiles()
	{
		this.clearTiles ();
		tiles.Populate (RESOURCES_PATH + FURNITURES_PATH, objTilesContainer,true);
		ObjectData.ObjectPath = FURNITURES_PATH;
		MatObjContainer.gameObject.SetActive (false);
		ObjectData.objType = "Furniture";

	}
	#endregion

	/// <summary>
	/// Clears the tiles.
	/// </summary>
	private void clearTiles()
	{
		// Loops through all the existing tiles and destroys them.
		int childs = objTilesContainer.childCount;
		for (int i = 0; i < childs; i++)
		{
			GameObject.Destroy(objTilesContainer.GetChild(i).gameObject);
		}

	}
		
}
