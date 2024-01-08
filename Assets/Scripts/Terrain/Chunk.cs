using System.Collections;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using UnityEngine;
using System.Threading;

public class Chunk {

	private Perlin PerlinNoiseGenerator;

	public Vect2i Pos { get; set; }

	private Terrain Terrain { get; set; }

	private ChunkSettings Settings { get; set; }

	private NoiseProvider Noise { get; set; }

	public TerrainChunkNeighborhood Neighborhood { get; set; }

	private float[,] Heightmap { get; set; }
	private object HeightmapThreadLockObject { get; set; }

	public Chunk (ChunkSettings pSettings, NoiseProvider pNoiseProvider, int pX, int pZ)
	{
		this.Settings = pSettings;
		this.Noise = pNoiseProvider;
		this.Pos = new Vect2i (pX, pZ);
		Neighborhood = new TerrainChunkNeighborhood();
		HeightmapThreadLockObject = new object();
	}

	public void CreateTerrain()
	{
		var terrainData = new TerrainData();
		terrainData.heightmapResolution = Settings.HeightmapResolution;
		terrainData.alphamapResolution = Settings.AlphamapResolution;

		var heightmap = this.GetHeightmap();
		terrainData.SetHeights(0, 0, heightmap);
		terrainData.size = new Vector3(Settings.Length, Settings.Height, Settings.Length);

		var newTerrainGameObject = Terrain.CreateTerrainGameObject(terrainData);
		newTerrainGameObject.transform.position = new Vector3(Pos.X * Settings.Length, 0, Pos.Z * Settings.Length);
		Terrain = newTerrainGameObject.GetComponent<Terrain>();
		Terrain.Flush();
	}

	private float[,] GetHeightmap()
	{
		var heightmap = new float[Settings.HeightmapResolution, Settings.HeightmapResolution];

		for (var zRes = 0; zRes < Settings.HeightmapResolution; zRes++)
		{
			for (var xRes = 0; xRes < Settings.HeightmapResolution; xRes++)
			{
				var xCoordinate = Pos.X + (float)xRes / (Settings.HeightmapResolution - 1);
				var zCoordinate = Pos.Z + (float)zRes / (Settings.HeightmapResolution - 1);

				heightmap[zRes, xRes] = Noise.GetValue(xCoordinate, zCoordinate);
			}
		}

		return heightmap;
	}

	public void SetNeighbors(Chunk chunk, TerrainNeighbor direction)
	{
		if (chunk != null)
		{
			switch (direction)
			{
			case TerrainNeighbor.XUp:
				Neighborhood.XUp = chunk;
				break;

			case TerrainNeighbor.XDown:
				Neighborhood.XDown = chunk;
				break;

			case TerrainNeighbor.ZUp:
				Neighborhood.ZUp = chunk;
				break;

			case TerrainNeighbor.ZDown:
				Neighborhood.ZDown = chunk;
				break;
			}
		}
	}

	public void GenerateHeightmap()
	{
		var thread = new Thread(GenerateHeightmapThread);
		thread.Start();
	}

	private void GenerateHeightmapThread()
	{
		lock (HeightmapThreadLockObject)
		{
			var heightmap = new float[Settings.HeightmapResolution, Settings.HeightmapResolution];

			for (var zRes = 0; zRes < Settings.HeightmapResolution; zRes++)
			{
				for (var xRes = 0; xRes < Settings.HeightmapResolution; xRes++)
				{
					var xCoordinate = Pos.X + (float)xRes / (Settings.HeightmapResolution - 1);
					var zCoordinate = Pos.Z + (float)zRes / (Settings.HeightmapResolution - 1);

					heightmap[zRes, xRes] = Noise.GetValue(xCoordinate, zCoordinate);
				}
			}

			Heightmap = heightmap;
		}
	}

	public bool IsHeightmapReady()
	{
		return Terrain == null && Heightmap != null;
	}

	public float GetTerrainHeight(Vector3 worldPosition)
	{
		return Terrain.SampleHeight(worldPosition);
	}

	public void Remove()
	{
		Heightmap = null;
		Settings = null;

		if (Neighborhood.XDown != null)
		{
			Neighborhood.XDown.RemoveFromNeighborhood(this);
			Neighborhood.XDown = null;
		}
		if (Neighborhood.XUp != null)
		{
			Neighborhood.XUp.RemoveFromNeighborhood(this);
			Neighborhood.XUp = null;
		}
		if (Neighborhood.ZDown != null)
		{
			Neighborhood.ZDown.RemoveFromNeighborhood(this);
			Neighborhood.ZDown = null;
		}
		if (Neighborhood.ZUp != null)
		{
			Neighborhood.ZUp.RemoveFromNeighborhood(this);
			Neighborhood.ZUp = null;
		}

		if (Terrain != null)
			GameObject.Destroy(Terrain.gameObject);
	}

	public void RemoveFromNeighborhood(Chunk chunk)
	{
		if (Neighborhood.XDown == chunk)
			Neighborhood.XDown = null;
		if (Neighborhood.XUp == chunk)
			Neighborhood.XUp = null;
		if (Neighborhood.ZDown == chunk)
			Neighborhood.ZDown = null;
		if (Neighborhood.ZUp == chunk)
			Neighborhood.ZUp = null;
	}

	public void UpdateNeighbors()
	{
		if (Terrain != null)
		{
			var xDown = Neighborhood.XDown == null ? null : Neighborhood.XDown.Terrain;
			var xUp = Neighborhood.XUp == null ? null : Neighborhood.XUp.Terrain;
			var zDown = Neighborhood.ZDown == null ? null : Neighborhood.ZDown.Terrain;
			var zUp = Neighborhood.ZUp == null ? null : Neighborhood.ZUp.Terrain;
			Terrain.SetNeighbors(xDown, zUp, xUp, zDown);
			Terrain.Flush();
		}
	}
}
