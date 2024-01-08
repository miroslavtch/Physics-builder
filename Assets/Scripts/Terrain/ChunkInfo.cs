using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;


public class ChunkInfo {
	private readonly int MaxChunkThreads = 3;

	private Dictionary<Vect2i, Chunk> RequestedChunks { get; set; }

	private Dictionary<Vect2i, Chunk> ChunksBeingGenerated { get; set; }

	private Dictionary<Vect2i, Chunk> LoadedChunks { get; set; }

	private HashSet<Vect2i> ChunksToRemove { get; set; }

	public OnChunkGeneratedDelegate OnChunkGenerated { get; set; }

	public ChunkInfo()
	{
		RequestedChunks = new Dictionary<Vect2i, Chunk>();
		ChunksBeingGenerated = new Dictionary<Vect2i, Chunk>();
		LoadedChunks = new Dictionary<Vect2i, Chunk>();
		ChunksToRemove = new HashSet<Vect2i>();
	}

	public void Update()
	{
		TryToDeleteQueuedChunks();

		GenerateHeightmapForAvailableChunks();
		CreateTerrainForReadyChunks();
	}

	public void AddNewChunk(Chunk chunk)
	{
		RequestedChunks.Add(chunk.Pos, chunk);
		GenerateHeightmapForAvailableChunks();
	}

	public void RemoveChunk(int x, int z)
	{
		ChunksToRemove.Add(new Vect2i(x, z));
		TryToDeleteQueuedChunks();
	}

	public bool ChunkCanBeAdded(int x, int z)
	{
		Vect2i key = new Vect2i(x, z);
		return
			!(RequestedChunks.ContainsKey(key)
				|| ChunksBeingGenerated.ContainsKey(key)
				|| LoadedChunks.ContainsKey(key));
	}

	public bool ChunkCanBeRemoved(int x, int z)
	{
		Vect2i key = new Vect2i(x, z);
		return
			RequestedChunks.ContainsKey(key)
			|| ChunksBeingGenerated.ContainsKey(key)
			|| LoadedChunks.ContainsKey(key);
	}

	public bool IsChunkGenerated(Vect2i chunkPosition)
	{
		return GetGeneratedChunk(chunkPosition) != null;
	}

	public Chunk GetGeneratedChunk(Vect2i chunkPosition)
	{
		if (LoadedChunks.ContainsKey(chunkPosition))
			return LoadedChunks[chunkPosition];

		return null;
	}

	public List<Vect2i> GetGeneratedChunks()
	{
		return LoadedChunks.Keys.ToList();
	}

	private void GenerateHeightmapForAvailableChunks()
	{
		List<KeyValuePair<Vect2i,Chunk>> requestedChunks = RequestedChunks.ToList();
		if (requestedChunks.Count > 0 && ChunksBeingGenerated.Count < MaxChunkThreads)
		{
			var chunksToAdd = requestedChunks.Take(MaxChunkThreads - ChunksBeingGenerated.Count);
			foreach (var chunkEntry in chunksToAdd)
			{
				ChunksBeingGenerated.Add(chunkEntry.Key, chunkEntry.Value);
				RequestedChunks.Remove(chunkEntry.Key);

				chunkEntry.Value.GenerateHeightmap();
			}
		}
	}

	private void CreateTerrainForReadyChunks()
	{
		var anyTerrainCreated = false;

		var chunks = ChunksBeingGenerated.ToList();
		foreach (var chunk in chunks)
		{
			if (chunk.Value.IsHeightmapReady())
			{
				ChunksBeingGenerated.Remove(chunk.Key);
				LoadedChunks.Add(chunk.Key, chunk.Value);

				chunk.Value.CreateTerrain();

				anyTerrainCreated = true;
				if (OnChunkGenerated != null)
					OnChunkGenerated.Invoke(ChunksBeingGenerated.Count);

				SetChunkNeighborhood(chunk.Value);
			}
		}

		if (anyTerrainCreated)
			UpdateAllChunkNeighbors();
	}

	private void TryToDeleteQueuedChunks()
	{
		var chunksToRemove = ChunksToRemove.ToList();
		foreach (var chunkPosition in chunksToRemove)
		{
			if (RequestedChunks.ContainsKey(chunkPosition))
			{
				RequestedChunks.Remove(chunkPosition);
				ChunksToRemove.Remove(chunkPosition);
			}
			else if (LoadedChunks.ContainsKey(chunkPosition))
			{
				var chunk = LoadedChunks[chunkPosition];
				chunk.Remove();

				LoadedChunks.Remove(chunkPosition);
				ChunksToRemove.Remove(chunkPosition);
			}
			else if (!ChunksBeingGenerated.ContainsKey(chunkPosition))
				ChunksToRemove.Remove(chunkPosition);
		}
	}

	private void SetChunkNeighborhood(Chunk chunk)
	{
		Chunk xUp;
		Chunk xDown;
		Chunk zUp;
		Chunk zDown;

		LoadedChunks.TryGetValue(new Vect2i(chunk.Pos.X + 1, chunk.Pos.Z), out xUp);
		LoadedChunks.TryGetValue(new Vect2i(chunk.Pos.X - 1, chunk.Pos.Z), out xDown);
		LoadedChunks.TryGetValue(new Vect2i(chunk.Pos.X, chunk.Pos.Z + 1), out zUp);
		LoadedChunks.TryGetValue(new Vect2i(chunk.Pos.X, chunk.Pos.Z - 1), out zDown);

		if (xUp != null)
		{
			chunk.SetNeighbors(xUp, TerrainNeighbor.XUp);
			xUp.SetNeighbors(chunk, TerrainNeighbor.XDown);
		}
		if (xDown != null)
		{
			chunk.SetNeighbors(xDown, TerrainNeighbor.XDown);
			xDown.SetNeighbors(chunk, TerrainNeighbor.XUp);
		}
		if (zUp != null)
		{
			chunk.SetNeighbors(zUp, TerrainNeighbor.ZUp);
			zUp.SetNeighbors(chunk, TerrainNeighbor.ZDown);
		}
		if (zDown != null)
		{
			chunk.SetNeighbors(zDown, TerrainNeighbor.ZDown);
			zDown.SetNeighbors(chunk, TerrainNeighbor.ZUp);
		}
	}

	private void UpdateAllChunkNeighbors()
	{
		foreach (var chunkEntry in LoadedChunks)
			chunkEntry.Value.UpdateNeighbors();
	}

}
