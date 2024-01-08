using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkGenerator {

	public ChunkSettings Settings { get; set; }
	private ChunkInfo CI { get; set; }
	private NoiseProvider Noise{ get; set; }

	public ChunkGenerator (ChunkSettings pSettings)
	{
		this.Settings = pSettings;
		this.Noise = new NoiseProvider ();
		CI = new ChunkInfo ();
	}

	private List<Vect2i> GetChunkPositionsInRadius(Vect2i chunkPosition, int radius)
	{
		var result = new List<Vect2i>();

		for (var zCircle = -radius; zCircle <= radius; zCircle++)
		{
			for (var xCircle = -radius; xCircle <= radius; xCircle++)
			{
				if (xCircle * xCircle + zCircle * zCircle < radius * radius)
					result.Add(new Vect2i(chunkPosition.X + xCircle, chunkPosition.Z + zCircle));
			}
		}

		return result;
	}
 	
	public void UpdateTerrain(Vector3 worldPosition, int radius)
	{
		Vect2i chunkPosition = GetChunkPosition(worldPosition);
		List<Vect2i> newPositions = GetChunkPositionsInRadius(chunkPosition, radius);

		var loadedChunks = CI.GetGeneratedChunks();
		var chunksToRemove = loadedChunks.Except(newPositions).ToList();

		var positionsToGenerate = newPositions.Except(chunksToRemove).ToList();
		foreach (Vect2i position in positionsToGenerate)
			GenerateChunk(position.X, position.Z);

		foreach (Vect2i position in chunksToRemove)
			RemoveChunk(position.X, position.Z);
	}


	private void GenerateChunk(int x, int z)
	{
		if (CI.ChunkCanBeAdded(x, z))
		{
			Chunk chunk = new Chunk(Settings, Noise, x, z);
			CI.AddNewChunk(chunk);
		}
	}

	private void RemoveChunk(int x, int z)
	{
		if (CI.ChunkCanBeRemoved(x, z))
			CI.RemoveChunk(x, z);
	}

	public Vect2i GetChunkPosition(Vector3 worldPosition)
	{
		int x = (int)Mathf.Floor(worldPosition.x / Settings.Length);
		int z = (int)Mathf.Floor(worldPosition.z / Settings.Length);

		return new Vect2i(x, z);
	}

	public bool IsTerrainAvailable(Vector3 worldPosition)
	{
		var chunkPosition = GetChunkPosition(worldPosition);
		return CI.IsChunkGenerated(chunkPosition);
	}

	public float GetTerrainHeight(Vector3 worldPosition)
	{
		Vect2i chunkPosition = GetChunkPosition(worldPosition);
		Chunk chunk = CI.GetGeneratedChunk(chunkPosition);
		float terrainHeight; 

		if (chunkPosition != null)
			terrainHeight = chunk.GetTerrainHeight (worldPosition);
		else
			terrainHeight = 0;

		return terrainHeight;
	}
}
