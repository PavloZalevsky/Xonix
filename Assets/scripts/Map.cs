using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {


    public Transform target;
    public int xMapSize = 64;
    public int yMapSize = 64;

    public int texSize = 64;
    private Texture2D tex;
    private FloodFiller f = new FloodFiller();
    private byte[][] map;
   // private byte[] map;

    private float speed = 20.0f;

    private Vector3 pos;
    private Vector3 oldpos;
    private Vector3 gridpos;

    private bool drawing = false;
    private Vector3 startPos; // we started drawing from here

    void Start()
    {
        //map = new byte[texSize * texSize];
        map = new byte[xMapSize][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new byte[yMapSize];
        }
        tex = new Texture2D(texSize, texSize);

        target.GetComponent<Renderer>().material.mainTexture = tex;
        target.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;


        for (int y = 0; y < texSize; y++)
        {
            for (int x = 0; x < texSize; x++)
            {
                map[x][y] = 0;
                tex.SetPixel(x, y, Color.blue);


                if (x == 0 || y == 0 || x == texSize - 1 || y == texSize - 1)
                {
                    map[x][y] = 33; // border
                    tex.SetPixel(x, y, Color.gray);
                }
            }
        }

        tex.Apply();
        pos = transform.position;
        gridpos = pos;
        oldpos = -pos;


		//FloodFill(map,5,5,20,20);
    }

	bool start = false;
    void Update()
    {
        // get movements
       
		if(Input.GetKey(KeyCode.U))
		{
			if(!start)
			{
				start = true;

				StartCoroutine(AutiFloodFill());
			}
		}

            float moveX = Input.GetAxisRaw("Horizontal") * speed;
        float moveY = Input.GetAxisRaw("Vertical") * speed;
        moveX *= -1;
        moveY *= -1;
        // limit diagonal
        if (Mathf.Abs(moveX) > Mathf.Abs(moveY)) moveY = 0.0f; else moveX = 0.0f;



        // move
        transform.Translate(new Vector3(moveX, moveY, 0) * Time.deltaTime, Space.World);

        // get gridpos
        gridpos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y),0);
        if (gridpos != oldpos)
        {
            byte cur = map[(int)gridpos.x][(int)gridpos.y];


        // впирання в рамку
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 0, xMapSize - 1), transform.position.y, transform.position.z);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0, yMapSize - 1), transform.position.z);

        if (cur == 1) // самі в себе
        {
				
        }

        if (cur == 33) // В ЗАБОР
		{
			//if(!start)
			//	StartCoroutine(AutiFloodFill());
        }

        if (cur != 33) // тут ми були
        {
            map[(int)gridpos.x][(int)gridpos.y] = 1;
        }
        tex.SetPixel(Mathf.RoundToInt(gridpos.x), Mathf.RoundToInt(gridpos.y), Color.green);
        tex.Apply();

        }

        oldpos = gridpos;
	}


	public void FloodFill(byte[][] map, int startX, int startY, byte XtoValue, byte YtoValue)
	{
		for (int x = startX; x < startX + XtoValue; x++) {
			for (int y = startY; y < YtoValue; y++) {
				tex.SetPixel(x, y, Color.green);
			}
		}
	}

    //else if (map[x][y] == 1 && map[x][y+1] == 1 && map[x][y - 1] == 1)
    //{
    //    tmp.Add(map[x][y]);
    //}

    List<byte> tmp = new List<byte>();

    bool git = false;
	IEnumerator AutiFloodFill()
	{
		Debug.Log("!!");
		bool git = false;
		bool draw = false;
		for (int x = 1; x < texSize; x++) {
			for (int y = 1; y < texSize; y++) {


                if(map[x][y-1] == 0 && map[x][y] == 1 && map[x][y + 1] == 1)
                {
                    tmp.Add(map[x][y]);
                }
                else if (map[x][y - 1] == 0 && map[x][y] == 1 && map[x][y + 1] == 0)
                {
                    tmp.Add(map[x][y]);
                }
                else if (map[x][y - 1] == 1 && map[x][y] == 1 && map[x][y + 1] == 0)
                {
                    tmp.Add(map[x][y]);
                }




                if (tmp.Count % 2 == 0)
                {
                    draw = true;
                }
                else
                    draw = false;

                if (draw)
				{
					tex.SetPixel(x, y, Color.green);
					tex.Apply();
				}

			}
            tmp.Clear();
            draw = false;
		}
	    start = false;


		yield return null;
	}

}
//if(draw  && map[x][y] == 1 && map[x][y +1] != 1)
//{
//	draw = false;
//}
//else if(map[x][y] == 1)
//{
//	draw = true;
//}




//if(draw && map[x][y+1] != 1)
//{
//	draw =false;
//}
//else
//{
//	draw =true;
//}