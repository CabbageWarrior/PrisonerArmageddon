using UnityEngine;

public class GroundController : MonoBehaviour
{

    public GameObject gameManager, prisonerSpawner;

    // Reference variable for the SpriteRenderer component.
    private SpriteRenderer spriteRenderer;

    // Entire SpriteRenderer dimensions.
    private float groundTotalWidth, groundTotalHeight;

    // Ground image dimensions in pixel.
    private int groundImgPixelWidth, groundImgPixelHeight;

    // Utilities: Transparent color.
    private Color colorTransparent;


    private SpawnerController spawner;

    /// <summary>
    /// Start method.
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spawner = prisonerSpawner.GetComponent<SpawnerController>();

        Texture2D textureOriginal = gameManager.GetComponent<MapGenerator>().GetMapTexture();

        // Resources.Load("nome_do_arquivo") carrega um arquivo localizado
        // em Assets/Resources
        Texture2D tex_clone = (Texture2D)Instantiate(textureOriginal);
        // Criamos uma Texture2D clone de tex para nao alterarmos a imagem original 
        spriteRenderer.sprite = Sprite.Create(tex_clone,
                                  new Rect(0f, 0f, tex_clone.width, tex_clone.height),
                                  new Vector2(0.5f, 0.5f), 100f);
        colorTransparent = new Color(0f, 0f, 0f, 0f);
        InitSpriteDimensions();
        BulletController.groundController = this;

        ResetPolygonCollider2D();
        
        // Spawn Players after ground creation.
        for (int i = 0; i < spawner.playersPerTeam; i++)
        {
            spawner.CreateNewPrisoner(
                -groundTotalWidth / 2,  // xMin
                groundTotalWidth / 2,   // xMax
                -groundTotalHeight / 2, // yMin
                groundTotalHeight / 2,  // yMax
                1,                      // teamNumber
                i
            );

            spawner.CreateNewPrisoner(
                -groundTotalWidth / 2,  // xMin
                groundTotalWidth / 2,   // xMax
                -groundTotalHeight / 2, // yMin
                groundTotalHeight / 2,  // yMax
                2,                      // teamNumber
                i
            );
        }

        gameManager.GetComponent<TurnManager>().SetActivePlayer(Random.Range(1, 2), Random.Range(0, spawner.playersPerTeam));
    }

    /// <summary>
    /// Initializes global ground dimensions.
    /// </summary>
    private void InitSpriteDimensions()
    {
        groundTotalWidth = spriteRenderer.bounds.size.x;
        groundTotalHeight = spriteRenderer.bounds.size.y;
        groundImgPixelWidth = spriteRenderer.sprite.texture.width;
        groundImgPixelHeight = spriteRenderer.sprite.texture.height;
    }

    ///// <summary>
    ///// Update method.
    ///// </summary>
    //void Update()
    //{

    //}

    /// <summary>
    /// Destroys part of the ground based on the CircleCollider2D dimension.
    /// </summary>
    /// <param name="destroyerCircleCollider">Collider that defines the area to destroy.</param>
    public void DestroyGround(CircleCollider2D destroyerCircleCollider)
    {
        // Center coordinates of the collider.
        V2int colliderCenter = World2Pixel(destroyerCircleCollider.bounds.center.x, destroyerCircleCollider.bounds.center.y);

        // Collider ray.
        int colliderRay = Mathf.RoundToInt(destroyerCircleCollider.bounds.size.x * groundImgPixelWidth / groundTotalWidth);

        int x, y, px, nx, py, ny, verticalCathetus;

        // Moving X from 0 to the collider ray length...
        for (x = 0; x <= colliderRay; x++)
        {
            // Every X defines a triangle.
            // --> 0 to X is the horizontal cathetus.
            // --> colliderRay is the hypotenuse.
            // We need to calculate the vertical cathetus.
            verticalCathetus = (int)Mathf.RoundToInt(Mathf.Sqrt(colliderRay * colliderRay - x * x));

            // Moving Y from 0 to the vertical cathetus length...
            for (y = 0; y <= verticalCathetus; y++)
            {
                // I am considering only one quarter, but with that I'm going to work in all the four ones.
                // I need positive and negative coordinates of X and Y.
                px = colliderCenter.x + x;
                nx = colliderCenter.x - x;
                py = colliderCenter.y + y;
                ny = colliderCenter.y - y;

                // For every singlecoordinate found by combining positive and negative X's and Y's,
                // I am going to override the pixel positioned in that coordinate with
                // the transparent color.
                if ((px >= 0 && px <= groundImgPixelWidth) && (py >= 0 && py <= groundImgPixelHeight)) { spriteRenderer.sprite.texture.SetPixel(px, py, colorTransparent); }
                if ((nx >= 0 && nx <= groundImgPixelWidth) && (py >= 0 && py <= groundImgPixelHeight)) { spriteRenderer.sprite.texture.SetPixel(nx, py, colorTransparent); }
                if ((px >= 0 && px <= groundImgPixelWidth) && (ny >= 0 && ny <= groundImgPixelHeight)) { spriteRenderer.sprite.texture.SetPixel(px, ny, colorTransparent); }
                if ((nx >= 0 && nx <= groundImgPixelWidth) && (ny >= 0 && ny <= groundImgPixelHeight)) { spriteRenderer.sprite.texture.SetPixel(nx, ny, colorTransparent); }
            }
        }
        // Now I'll apply those updates on the texture. That's why I operate on a clone
        // and not on the original texture.
        spriteRenderer.sprite.texture.Apply();

        // Now it's time to refresh the collider, that will ignore transparent pixels and fit
        // the new texture.
        ResetPolygonCollider2D();
    }

    /// <summary>
    /// Gets the pixel coordinates from the position in the plane.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns></returns>
    private V2int World2Pixel(float x, float y)
    {
        V2int v = new V2int();

        float dx = x - transform.position.x;
        v.x = Mathf.RoundToInt(0.5f * groundImgPixelWidth + dx * groundImgPixelWidth / groundTotalWidth);

        float dy = y - transform.position.y;
        v.y = Mathf.RoundToInt(0.5f * groundImgPixelHeight + dy * groundImgPixelHeight / groundTotalHeight);

        return v;
    }

    /// <summary>
    /// Resets the collider in order to fit the texture.
    /// </summary>
    private void ResetPolygonCollider2D()
    {
        // Simple and funny: destroy and recreate.
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }
}