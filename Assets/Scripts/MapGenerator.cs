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
	private Cluster currentCluster;
	private Coordinates coord;

	public ClusterFinder(float[,] matrix, int n, int m, float maskThreshold)
	{
		this.m = m;
		this.n = n;
		this.maskThreshold = maskThreshold;
		this.matrix = matrix;
		mask = new int[matrix.GetLength(0), matrix.GetLength(1)];
		GetMask ();
		GetClusters ();
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
					coord = Coordinates(x, y);
					currentCluster = new Cluster(coord);
					FillCluster (x, y);
				}
			}
		} 
	}

	public void FillCluster(int x, int y)
	{
		// Adds one point to the cluster and then set it to 0
		// looks for non-zero neighbours
		// Repeats for all neighbours until no neighbours are found

		Coordinates coord;
		mask [x, y] = 0;
		Coordinates[] allNeighbours = getFirstNeighbours (x, y);
		for (int i = 0; i < allNeighbours.Length; i++)
		{
			coord = allNeighbours [i];
			if (mask [coord.x, coord.y])
				currentCluster.Add (coord);
		}

	}

	public Coordinates[] getFirstNeighbours(int x, int y)
	{	
		int[] all_x = new int[3];
		int[] all_y = new int[3];

		int startingX = x-1;
		for (int i=0; i<3; i++)
		{
			all_x [i] = startingX + i;
		}

		int startingY = y-1;
		for (int i=0; i<3; i++)
		{
			all_y [i] = startingY + i;
		}

		Coordinates[] allNeighbours = new Coordinates[(all_x.Length * all_y.Length)-1];
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++) 
			{	
				if (i != 1 && j != 1)
					allNeighbours[i+j] = Coordinates(all_x[i], all_y[j]);
			}
		}

		return allNighbours;
	}
		
}

public class Cluster
{
	private List<Coordinates> allCoordinates = new List<Coordinates>();
	private bool isInThrehold;

	public Cluster(Coordinates coord)
	{
		Add (coord);
	}

	public void Add(Coordinates coord)
	{
		allCoordinates.Add (coord);
	}
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