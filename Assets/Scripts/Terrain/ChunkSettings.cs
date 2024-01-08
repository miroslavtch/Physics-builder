using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;

public class ChunkSettings {

	public int HeightmapResolution { get; private set; }
	public int AlphamapResolution { get; private set; }

	public int Length { get; private set; }
	public int Height { get; private set; }

	public ChunkSettings (int pHMP, int pAMP, int pLength, int pHeight)
	{
		this.HeightmapResolution = pHMP;
		this.AlphamapResolution = pAMP;
		this.Length = pLength;
		this.Height = pHeight;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
