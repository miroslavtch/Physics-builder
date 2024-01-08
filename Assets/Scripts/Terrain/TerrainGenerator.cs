using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour {

	private const int Radius = 4;
	private Chunk terrain;
	public ChunkGenerator Generator;
	public Transform Player;
	private Vect2i PreviousPlayerChunkPosition;

	private Vect2i playerChunkPosition;

	// Use this for initialization
	void Start () {
		var settings = new ChunkSettings(129, 129, 100, 5);
		var noiseProvider = new NoiseProvider();
		var terrain = new Chunk(settings, noiseProvider, -10, -10);
		terrain.CreateTerrain();
		Generator = new ChunkGenerator (settings);


		for (var i = 0; i < 4; i ++)
			for (var j = 0; j < 4; j++)
				new Chunk(settings, noiseProvider, i, j).CreateTerrain();
	}

	private IEnumerator InitializeCoroutine()
	{
		bool canActivateCharacter = false;

		Generator.UpdateTerrain(Player.position, Radius);

		do
		{
			var exists = Generator.IsTerrainAvailable(Player.position);
			if (exists)
				canActivateCharacter = true;
			yield return null;
		} while (!canActivateCharacter);

		PreviousPlayerChunkPosition = Generator.GetChunkPosition(Player.position);
		Player.position = new Vector3(Player.position.x, Generator.GetTerrainHeight(Player.position) + 0.5f, Player.position.z);
		Player.gameObject.SetActive(true);
	}

	// Update is called once per frame
	void Update () 
	{
		if (Player.gameObject.activeSelf)
		{
			playerChunkPosition = Generator.GetChunkPosition(Player.position);
			if (!playerChunkPosition.Equals(PreviousPlayerChunkPosition))
			{
				Generator.UpdateTerrain(Player.position, Radius);
				PreviousPlayerChunkPosition = playerChunkPosition;
			}
		}
	}
}