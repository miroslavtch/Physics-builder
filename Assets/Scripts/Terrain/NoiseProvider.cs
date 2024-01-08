using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;


public class NoiseProvider {

	private Perlin PerlinNoiseGenerator;

	public NoiseProvider()
	{
		PerlinNoiseGenerator = new Perlin();
	}

	public float GetValue(float x, float z)
	{
		return (float)(PerlinNoiseGenerator.GetValue(x, 0, z) / 2f) + 0.5f;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
