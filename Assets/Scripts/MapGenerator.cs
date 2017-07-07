using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour {


	//public GameObject ground;
	public int mapWidth, mapHeight;
	public float groundPercBot, waterPercLeft, waterPercRight, featuresScale, threshold;
    public GameObject background;

	//private SpriteRenderer mapRenderer;
	private Texture2D mapProfile, origMapProfile;
	private float[,] pixels;
	private int[,] pixelsMask;
	private Color[] pixelsColor;
	private float[] percThresholds;
	private float randomX, randomY;
	private ClusterFinder clusterFinder;
    private Sprite bkgSprite;
    private SpriteRenderer bkgRenderer;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void CalcNoise()
	{
		randomX = Random.Range (0f, 100f);
		randomY = Random.Range (0f, 100f);

		float x = 0f, y = 0f;
		while (y < mapProfile.height) {
			x = 0f;
			while (x < mapProfile.width) {
				float xCoord = randomX + x / mapWidth * featuresScale;
				float yCoord = randomY + y / mapHeight * featuresScale;
				float sample = Mathf.PerlinNoise (xCoord, yCoord);
				if (sample < threshold)
					sample = 0f;
				pixels [(int)x, (int)y] = sample;
				x++;
			}
			y++;
		}
	}

	void FillColorArray()
	{	
		int idx, idx2, pxTexture;
        Color[] basePixels = origMapProfile.GetPixels();

        Color colorTransparent = new Color(0f, 0f, 0f, 0f);
        pxTexture = (int)Mathf.Sqrt(basePixels.Length);

        for (int i = 0; i < mapProfile.height; i++) 
		{
			for (int j = 0; j < mapProfile.width; j++) 
			{
                idx = (i * mapProfile.width) + j;
                if (pixelsMask[j, i] == 1)
                {
                    idx2 = (i % pxTexture) * pxTexture + j % pxTexture;
                    pixelsColor[idx] = basePixels[idx2];
                }
                //else if (pixelsMask[j, i] == 2)
                //{
                //    pixelsColor[idx] = Color.red;
                //}
                else
                {
                    pixelsColor[idx] = Color.black;
                    pixelsColor[idx].a = 0f;
                }


            }
		}
	}

	void TestFillColorArray()
	{
		int idx;
		for (int j = 0; j < mapProfile.height; j++) 
		{
			for (int i = 0; i < mapProfile.width; i++) 
			{	
				idx = (j * mapProfile.width) + i;
				pixelsColor [idx] = new Color(pixels[i,j], pixels[i,j], pixels[i,j]);
			}
		}
	}

	public Texture2D GetMapTexture()
	{
        //mapRenderer = ground.GetComponent<SpriteRenderer> ();
        int bkgNbr = Random.Range(0, 3);
        int colorNbr = Random.Range(0, 4);
        bkgRenderer = background.GetComponent<SpriteRenderer>();

        switch (bkgNbr)
        {
            case 0:
                bkgSprite = Resources.Load<Sprite>("background_faded_light");
                bkgRenderer.sprite = bkgSprite;
                break;
            case 1:
                bkgSprite = Resources.Load<Sprite>("background_faded_light_2");
                bkgRenderer.sprite = bkgSprite;
                break;
            case 2:
                bkgSprite = Resources.Load<Sprite>("background_faded_light_3");
                bkgRenderer.sprite = bkgSprite;
                break;
            default:
                break;
        }

        background.transform.localScale += new Vector3(0.5f, 0f, 0f);

        switch (colorNbr)
        {
            case 0:
                origMapProfile = (Texture2D)Resources.Load("rock_01");
                break;
            case 1:
                origMapProfile = (Texture2D)Resources.Load("rock_02");
                break;
            case 2:
                origMapProfile = (Texture2D)Resources.Load("rock_03");
                break;
            case 3:
                origMapProfile = (Texture2D)Resources.Load("rock_04");
                break;
            default:
                break;
        }

        mapProfile = new Texture2D(mapWidth, mapHeight);
        pixels = new float[mapProfile.width, mapProfile.height];
        pixelsColor = new Color[mapProfile.width * mapProfile.height];
        percThresholds = new float[3] { groundPercBot, waterPercLeft, waterPercRight };
        CalcNoise();
        clusterFinder = new ClusterFinder(pixels, mapProfile.width, mapProfile.height, percThresholds);
        pixelsMask = clusterFinder.GetMask();
        FillColorArray();
        //TestFillColorArray();
        mapProfile.SetPixels(pixelsColor);
        mapProfile.anisoLevel = 16;
		mapProfile.Apply(true);

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
    private List<MaskMatrixElement> clusterElements = new List<MaskMatrixElement>();
    private Cluster[] allClusters;
	private Cluster currentCluster;
	private Coordinates coord;
    private MaskMatrix maskMatrix;

	public ClusterFinder(float[,] matrix, int n, int m, float[] percThresholds)
	{
		this.n = n;
		this.m = m;
		this.matrix = matrix;
		this.percThresholds = percThresholds;
		this.percThresholds [0] = Mathf.Round(this.percThresholds [0] * m / 100f);
		this.percThresholds [1] = Mathf.Round(this.percThresholds [1] * n / 100f); 
		this.percThresholds [2] = Mathf.Round(n - this.percThresholds [2] * n / 100f); 
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
				if (matrix[i,j] != 0)
					mask[i,j] = 1;
				else
					mask[i,j] = 0;
			}
		}

        maskMatrix = new MaskMatrix(mask);
	}

	public void GetClusters()
	{
        MaskMatrixElement currentMatrixElement;

		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < m; j++) 
			{

                currentMatrixElement = maskMatrix.matrix[i, j];
                
                if (!currentMatrixElement.coord.isPicked && currentMatrixElement.value == 1) 
				{
                    currentMatrixElement.setIsPicked(true);
                    currentCluster = new Cluster(currentMatrixElement.coord);
					FillCluster (i, j);
                    if (currentCluster.isAtEdge == true)
                        currentCluster.isInThreshold = false;
					currentCluster.CreateCoordinatesArray ();
					allClustersList.Add (currentCluster);
				}
			}
		}

        CreateClustersArray();
	}

	public void FillCluster(int x, int y)
	{
		Coordinates coord;
        Coordinates[] allNeighbours;
        MaskMatrixElement currentMatrixElement, neighbourMatrixElement;

        currentMatrixElement = maskMatrix.matrix[x, y];
        clusterElements.Add(maskMatrix.matrix[x, y]);

        int i = 0;
        while (i < clusterElements.Count)
        {
            currentMatrixElement = clusterElements.ElementAt(i);
            allNeighbours = getFirstNeighbours(currentMatrixElement.coord.x, currentMatrixElement.coord.y);

            for (int j = 0; j < allNeighbours.Length; j++)
            {
                coord = allNeighbours[j];
                if (coord.x >= 0 && coord.y >= 0 && coord.x < n && coord.y < m)
                {
                    neighbourMatrixElement = maskMatrix.matrix[coord.x, coord.y];

                    if (!neighbourMatrixElement.coord.isPicked && neighbourMatrixElement.value == 1)
                    {
                        neighbourMatrixElement.setIsPicked(true);
                        currentCluster.Add(neighbourMatrixElement.coord);
                        clusterElements.Add(neighbourMatrixElement);
                        if (!currentCluster.isInThreshold && coord.y < percThresholds[0] && coord.x > percThresholds[1] && coord.x < percThresholds[2])
                            currentCluster.isInThreshold = true;
                        if (coord.x == 0 || coord.x == n-1 || coord.y == m-1)
                            currentCluster.isAtEdge = true;
                    }
                    else
                    {
                        currentMatrixElement.setIsPicked(true);
                    }

                }

            }

            i++;
        }

        clusterElements.Clear();

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
        int nbrOfDeletedClusters = 0, nbrOfDeletedPoints = 0;
		for (int i=0; i<clustersNbr; i++)
		{
			updateCluster = allClusters[i];
			if (!updateCluster.getIsInThreshold()) 
			{
                nbrOfDeletedClusters += 1;
                updateCoordinates = updateCluster.getAllCoordinates ();
				pointsNbr = updateCoordinates.Length;
				for (int j=0; j<pointsNbr; j++)
				{
					coord = updateCoordinates [j];
					//mask[coord.x, coord.y] = 0;
                    mask[coord.x, coord.y] = 2;
                    nbrOfDeletedPoints += 1;

                }
			}
		}
        Debug.Log(nbrOfDeletedClusters.ToString());
        Debug.Log(nbrOfDeletedPoints.ToString());
    }

	public int[,] GetMask()
	{	
		return mask;
	}
		
}

public class Cluster
{
	public bool isInThreshold, isAtEdge;

	private List<Coordinates> allCoordinatesList = new List<Coordinates>();
	private Coordinates[] allCoordinates;

	public Cluster(Coordinates coord)
	{
		Add (coord);
		isInThreshold = false;
        isAtEdge = false;

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
    public bool isPicked = false;

	public Coordinates(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

public class MaskMatrix
{
    public MaskMatrixElement[,] matrix;
  
    private int xdim, ydim;

    public MaskMatrix(int[,] mask)
    {
        int value;
        Coordinates coord;
        xdim = mask.GetLength(0);
        ydim = mask.GetLength(1);

        matrix = new MaskMatrixElement[xdim, ydim];
        for (int i=0; i < mask.GetLength(0); i++)
        {
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                value = mask[i,j];
                coord = new Coordinates(i, j);
                matrix[i, j] = new MaskMatrixElement(coord, value);
            }
        }
    }
}

public class MaskMatrixElement
{
    public Coordinates coord;
    public float value;

    public MaskMatrixElement(Coordinates coord, int value)
    {
        this.coord = coord;
        this.value = value;
    }

    public void setIsPicked(bool isPicked)
    {
        coord.isPicked = isPicked;
    }
}