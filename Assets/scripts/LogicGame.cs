using UnityEngine;
using System.Collections;

public class LogicGame : MonoBehaviour
{
    public Transform target;
    private Texture2D tex;
    private int texsize = 64;

    private FloodFiller f = new FloodFiller();

    private int MAP_SIZE = 64; // only powers of two are supported
    private byte[] map = new byte[64 * 64];

    private float speed = 20.0f;

    private Vector3 pos;
    private Vector3 oldpos;
    private Vector3 gridpos;

    private bool drawing = false;
    private Vector3 startPos; // we started drawing from here

    void Start()
    {
        tex = new Texture2D(texsize, texsize);
        target.GetComponent<Renderer>().material.mainTexture = tex;
        target.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;

        for (var y = 0; y < texsize; y++)
            for (var x = 0; x < texsize; x++)
            {
                map[texsize * x + y] = 0;
                tex.SetPixel(x, y, new Color(0, 0, 0.2f, 1));

                if (x == 0 || y == 0 || x == texsize - 1 || y == texsize - 1)
                {
                    map[texsize * x + y] = 33; // border
                    tex.SetPixel(x, y, Color.gray);
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
        transform.Translate(new Vector3(moveX, 0, moveY) * Time.deltaTime, Space.World);

        // get gridpos
        gridpos = new Vector3(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.z));

        // limit on walls
        // transform.position.x = new  Mathf.Clamp(transform.position.x, 0, texsize - 1);
       // transform.position.z = Mathf.Clamp(transform.position.z, 0, texsize - 1);

        transform.position = new Vector3( Mathf.Clamp(transform.position.x, 0, texsize - 1),transform.position.y,transform.position.z);
        transform.position = new Vector3(transform.position.x,transform.position.y, Mathf.Clamp(transform.position.z, 0, texsize - 1));


        // we have moved?
        if (gridpos != oldpos)
        {

            // we are in empty spot
            if (map[(int)(texsize *gridpos.x + gridpos.z)] == 0)
            {
                var fwd = transform.TransformDirection(-Vector3.up);
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f, 0), fwd,10f, 1 << /*LayerMask.NameToLayer("Objects") | */LayerMask.NameToLayer("Ground"));
               
                if (hit != false)
                {
                    Vector2 pixelUV = hit.point;
                    pixelUV.x *= texsize;
                    pixelUV.y *= texsize;
                    //print (pixelUV);

                    // TODO: separate maps?
                    map[texsize * Mathf.RoundToInt(pixelUV.x) + Mathf.RoundToInt(pixelUV.y)] = 10;

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

        for (var y = 0; y < texsize; y++)
            for (var x = 0; x < texsize; x++)
            {
                var val = map[texsize * x + y];

                // its still empty, so its inside building (or its wall?)
                if (val == 0)
                {
                    map[texsize * x + y] = 0;
                    //tex.SetPixel(x,y,new Color(0,1,0,1));

                    found = true;
                    countArea++;
                }

                //it was filled, clean it up
                if (val == 99)
                {
                    map[texsize * x + y] = 0;
                    tex.SetPixel(x, y, new Color(0, 0, 1, 1));
                }
            }

        print("countArea:" + countArea);
        tex.Apply();

    }
}
