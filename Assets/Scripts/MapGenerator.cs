using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {


	//public GameObject ground;
	public int mapWidth, mapHeight;
	public float groundPercBot, waterPercLeft, waterPercRight, featuresScale;

	//private SpriteRenderer mapRenderer;
	private Texture2D mapProfile;
	private float[,] pixels;

	// Use this for initialization
	void Start () {

		//mapRenderer = ground.GetComponent<SpriteRenderer> ();
		mapProfile = new Texture2D (mapWidth, mapHeight);
		pixels = new float[mapProfile.width, mapProfile.height];
		CalcNoise ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CalcNoise()
	{
		float x = 0f, y = 0f;
		while(x < mapProfile.width)
		{
			float xCoord = Mathf.Round(x / mapWidth * featuresScale);
			float yCoord = Mathf.Round(y / mapWidth * featuresScale);
			float sample = Mathf.PerlinNoise (xCoord, yCoord);
			pixels [(int)x, (int)y] = sample;
			x++;
		}

		y++;
	}

}
	
public class ClusterFinder{

	private int n, m;
	private float maskThreshold;
	private float[,] matrix;
	private int[,] mask;
	private Cluster[] allClusters;

	public ClusterFinder(float[,] matrix, int n, int m, float maskThreshold)
	{
		this.m = m;
		this.n = n;
		this.maskThreshold = maskThreshold;
		this.matrix = matrix;
		mask = new int[matrix.GetLength(0), matrix.GetLength(1)];
		GetMask ();
	}

	public void GetMask()
	{
		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < n; j++) 
			{
				if (matrix[i,j] > maskThreshold)
					mask[i,j] = 1;
				else
					mask[i,j] = 0;
			}
		}
	}

	public void GetClusters()
	{
		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < n; j++) 
			{
				if (mask [i, j] = 1) 
				{
					thisCluster = new Cluster();

				}
			}
		} 
	}

	public void FillCluster()
	{
		// Adds one point to the cluster and then set it to 0
		// looks for non-zero neighbours
		// Repeats for all neighbours until no neighbours are found
	}

}

public class Cluster
{
	private Coordinates[] allCoordinates;
	private bool isInThrehold;

}

public class Coordinates
{
	private int x, y;

	public Coordinates(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}