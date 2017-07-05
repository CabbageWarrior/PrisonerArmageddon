using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {


	//public GameObject ground;
	public int mapWidth, mapHeight;
	public float groundPercBot, waterPercLeft, waterPercRight, featuresScale, threshold;

	//private SpriteRenderer mapRenderer;
	private Texture2D mapProfile;
	private float[,] pixels;
	private int[,] pixelsMask;
	private Color[] pixelsColor;
	private float[] percThresholds;
	private ClusterFinder clusterFinder;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CalcNoise()
	{
		float x = 0f, y = 0f;
		while (y < mapProfile.height) {
			x = 0f;
			while (x < mapProfile.width) {
				float xCoord = x / mapWidth * featuresScale;
				float yCoord = y / mapHeight * featuresScale;
				float sample = Mathf.PerlinNoise (xCoord, yCoord);
				// Debug.Log (sample.ToString());
				pixels [(int)x, (int)y] = sample;
				x++;
			}
			y++;
		}
	}

	void FillColorArray()
	{	
		int idx;
		for (int i = 0; i < mapProfile.width; i++) 
		{
			for (int j = 0; j < mapProfile.height; j++) 
			{	
				idx = (i * mapProfile.width) + j;
				if (pixelsMask [i, j] == 1) {
					pixelsColor [idx] = Color.white;
				}
				else {
					pixelsColor [idx] = Color.black;
					pixelsColor [idx].a = 0f;
				}
			}
		}
	}

	void TestFillColorArray()
	{
		int idx;
		for (int i = 0; i < mapProfile.width-1; i++) 
		{
			for (int j = 0; j < mapProfile.height-1; j++) 
			{	
				idx = (i * mapProfile.height) + j;
				pixelsColor [idx] = new Color(pixels[i,j], pixels[i,j], pixels[i,j]);
			}
		}
	}

	public Texture2D GetMapTexture()
	{
		//mapRenderer = ground.GetComponent<SpriteRenderer> ();
		mapProfile = new Texture2D (mapWidth, mapHeight);
		pixels = new float[mapProfile.width, mapProfile.height];
		pixelsColor = new Color[mapProfile.width * mapProfile.height];
		percThresholds = new float[3]{groundPercBot, waterPercLeft, waterPercRight};
		CalcNoise ();
		//clusterFinder = new ClusterFinder (pixels, mapProfile.width, mapProfile.height, threshold, percThresholds);
		//pixelsMask = clusterFinder.GetMask ();
		//FillColorArray ();
		TestFillColorArray();
		mapProfile.SetPixels(pixelsColor);
		mapProfile.Apply();

		return mapProfile;
	}
}
	
public class ClusterFinder{

	private int n, m;
	private float maskThreshold;
	private float[] percThresholds;
	private float[,] matrix;
	private int[,] mask;
	private List<Cluster> allClustersList = new List<Cluster>();
	private Cluster[] allClusters;
	private Cluster currentCluster;
	private Coordinates coord;

	public ClusterFinder(float[,] matrix, int n, int m, float maskThreshold, float[] percThresholds)
	{
		this.n = n;
		this.m = m;
		this.maskThreshold = maskThreshold;
		this.matrix = matrix;
		this.percThresholds = percThresholds;
		this.percThresholds [0] = Mathf.Round(this.percThresholds [0] * m);
		this.percThresholds [1] = Mathf.Round(this.percThresholds [1] * n); 
		this.percThresholds [2] = Mathf.Round(this.percThresholds [2] * n); 
		mask = new int[matrix.GetLength(0), matrix.GetLength(1)];
		CalcMask ();
		GetClusters ();
		Debug.Log (allClustersList.Count.ToString());
		UpdateMask ();
	}

	public void CalcMask()
	{
		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < m; j++) 
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
			for (int j = 0; j < m; j++) 
			{
				if (mask [i, j] == 1) 
				{
					coord = new Coordinates(i, j);
					currentCluster = new Cluster(coord);
					FillCluster (i, j);
					currentCluster.CreateCoordinatesArray ();
					allClustersList.Add (currentCluster);
				}
			}
		}

		CreateClustersArray ();
	}

	public void FillCluster(int x, int y)
	{
		Coordinates coord;
		mask [x, y] = 0;
		Coordinates[] allNeighbours = getFirstNeighbours (x, y);

		for (int i = 0; i < allNeighbours.Length; i++)
		{
			coord = allNeighbours [i];
			if (coord.x >=0 && coord.y >=0 && coord.x < n && coord.y < m && mask [coord.x, coord.y] == 1) 
			{
				currentCluster.Add (coord);
				//mask [coord.x, coord.y] = 0;
				FillCluster (coord.x, coord.y);
				if (!currentCluster.isInThreshold && y > this.percThresholds [0] && x > this.percThresholds [1] && x < this.percThresholds [2])
					currentCluster.isInThreshold = true;
			}
		}
	}

	public Coordinates[] getFirstNeighbours(int x, int y)
	{	
		int[] all_x = new int[3];
		int[] all_y = new int[3];
		int indx;

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
				indx = (i * 3) + j;
				if (indx > 4)
					indx--;
				
				if (i != 1 || j != 1)
					allNeighbours[indx] = new Coordinates(all_x[i], all_y[j]);
			}
		}

		return allNeighbours;
	}

	public void CreateClustersArray()
	{
		allClusters = allClustersList.ToArray();
	}

	public void UpdateMask()
	{
		Cluster updateCluster;
		Coordinates coord;
		Coordinates[] updateCoordinates;
		int clustersNbr = allClusters.Length;
		int pointsNbr;
		for (int i=0; i<clustersNbr; i++)
		{
			updateCluster = allClusters[i];
			if (!updateCluster.getIsInThreshold()) 
			{	
				updateCoordinates = currentCluster.getAllCoordinates ();
				pointsNbr = updateCoordinates.Length;
				for (int j=0; j<pointsNbr; j++)
				{
					coord = updateCoordinates [j];
					mask[coord.x, coord.y] = 0;
				}
			}
		}
	}

	public int[,] GetMask()
	{	
		return mask;
	}
		
}

public class Cluster
{
	public bool isInThreshold;

	private List<Coordinates> allCoordinatesList = new List<Coordinates>();
	private Coordinates[] allCoordinates;

	public Cluster(Coordinates coord)
	{
		Add (coord);
		isInThreshold = false;
	}

	public void Add(Coordinates coord)
	{
		allCoordinatesList.Add (coord);
	}

	public void CreateCoordinatesArray()
	{
		allCoordinates = allCoordinatesList.ToArray();
	}

	public bool getIsInThreshold()
	{
		return isInThreshold;
	}

	public Coordinates[] getAllCoordinates()
	{
		return allCoordinates;
	}

}

public class Coordinates
{
	public int x, y;

	public Coordinates(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}