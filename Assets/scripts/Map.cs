using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {


    public Transform target;
    public int xMapSize = 64;
    public int yMapSize = 64;

    public int texSize = 64;
    private Texture2D tex;
    private FloodFiller f = new FloodFiller();
    // private byte[][] map;
    private byte[] map;

    private float speed = 20.0f;

    private Vector3 pos;
    private Vector3 oldpos;
    private Vector3 gridpos;

    private bool drawing = false;
    private Vector3 startPos; // we started drawing from here

    void Start()
    {
        map = new byte[texSize * texSize];
        //map = new byte[xMapSize][];
        //for (int i = 0; i < map.Length; i++)
        //{
        //    map[i] = new byte[yMapSize];
        //}
        tex = new Texture2D(texSize, texSize);

        target.GetComponent<Renderer>().material.mainTexture = tex;
        target.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;


        for (int y = 0; y < texSize; y++)
        {
            for (int x = 0; x < texSize; x++)
            {
                //map[x][y] = 0;
                //map[texSize * x + y] = 0;
                tex.SetPixel(x, y, new Color(0, 0, 0.2f, 1));

                //if (x == 0 || y == 0 || x == texSize - 1 || y == texSize - 1)
                //{
                //    map[x][y] = 33; // border
                //    tex.SetPixel(x, y, Color.gray);
                //}
            }
        }

        tex.Apply();
        pos = transform.position;
        gridpos = pos;
        oldpos = -pos;

    }

    void Update()
    {
        // get movements
        float moveX = Input.GetAxisRaw("Horizontal") * speed;
        float moveY = Input.GetAxisRaw("Vertical") * speed;

        // limit diagonal
        if (Mathf.Abs(moveX) > Mathf.Abs(moveY)) moveY = 0.0f; else moveX = 0.0f;

        // move
        transform.Translate(new Vector3(moveX, moveY, 0) * Time.deltaTime, Space.World);

        // get gridpos
        gridpos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y),0);

        // limit on walls
        // transform.position.x = new  Mathf.Clamp(transform.position.x, 0, texsize - 1);
        // transform.position.z = Mathf.Clamp(transform.position.z, 0, texsize - 1);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 0, texSize - 1), transform.position.y, transform.position.z);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0, texSize - 1), transform.position.z);


        // we have moved?
        if (gridpos != oldpos)
        {
           int a =  (int)(texSize * gridpos.x + gridpos.y);
            // we are in empty spot
            if (map[a] == 0)
            {
                var fwd = transform.TransformDirection(-Vector3.up);
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f, 0), fwd, 10f, 1 << /*LayerMask.NameToLayer("Objects") | */LayerMask.NameToLayer("Ground"));

                if (hit != false)
                {
                    Vector2 pixelUV = hit.point;
                    pixelUV.x *= texSize;
                    pixelUV.y *= texSize;
                    //print (pixelUV);

                    // TODO: separate maps?
                    map[texSize * Mathf.RoundToInt(pixelUV.x) + Mathf.RoundToInt(pixelUV.y)] = 10;

                    tex.SetPixel(Mathf.RoundToInt(pixelUV.x), Mathf.RoundToInt(pixelUV.y), Color.green);
                    tex.Apply();

                    if (!drawing) startPos = gridpos;
                    drawing = true;


                }
            }
            else
            { // we are in pre-filled area

                // TODO: if neighbour cells are already painted, we should floodfill (because we entered 1x1 hole?)

                // we had been drawing before coming here
                if (drawing)
                {
                    drawing = false;

                    Vector3 paintpos = oldpos;

                    Debug.DrawLine(startPos, gridpos, Color.red, 10);

                    if (startPos.z < gridpos.z) paintpos.z -= 1;

                    //if (gridpos.x) paintpos.y = gridpos.y+1;
                    //if (startPos.y<gridPos.y) paintpos.y = gridPos.y+1;


                    print("paintpos:" + paintpos + " gridpos:" + gridpos + " oldpos:" + oldpos);
                    f.FloodFill(map, (int)paintpos.x, (int)paintpos.z, 10, 99);
                    checkAreas();
                }
            }

            oldpos = gridpos;

        }


        /*
        if (Input.GetMouseButtonUp (0))
        {
        //        print ("fill");
        f.FloodFill(map, 10, 10, 10, 99);
        checkAreas();
        }
        */


    }



    void checkAreas()
    {
        bool found = false;
        int countArea = 0;
        

        for (var y = 0; y < texSize; y++)
            for (var x = 0; x < texSize; x++)
            {
                var val = map[texSize * x + y];

                // its still empty, so its inside building (or its wall?)
                if (val == 0)
                {
                    map[texSize * x + y] = 0;
                    //tex.SetPixel(x,y,new Color(0,1,0,1));

                    found = true;
                    countArea++;
                }

                //it was filled, clean it up
                if (val == 99)
                {
                    map[texSize * x + y] = 0;
                    tex.SetPixel(x, y, new Color(0, 0, 1, 1));
                }
            }

        print("countArea:" + countArea);
        tex.Apply();

    }
}
