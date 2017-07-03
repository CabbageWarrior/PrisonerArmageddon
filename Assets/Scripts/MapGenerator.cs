using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	/*
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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CalcNoise()
	{
		float y = 0f;
		while(x < mapProfile.width)
		{
			float xCoord = x / mapWidth * featuresScale;
			float yCoord = y / mapWidth * featuresScale;
			float sample = Mathf.PerlinNoise (xCoord, yCoord);
			pixels [x][y] = sample;
			x++;
		}

		y++;
	}
	*/
}

// Experiments...
/*
public struct Coord : IStructuralEquatable<Coord>
{
	public Coord(int n, int m) : this()
	{
		N = n;
		M = m;
	}

	public int N{ get; private set;}
	public int M{ get; private set;}
}


public class Cluster
{
	public Cluster(IEnumerable<Coord> coords)
	{
		if (coords == null)
			throw new ArgumentNullException ("coords");

		coords = coords;
	}

	public IEnumerable<Coord> Coords { get; private set;}

}
*/