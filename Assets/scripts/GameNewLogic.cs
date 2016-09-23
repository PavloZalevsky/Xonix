using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameNewLogic : MonoBehaviour {

    public Transform target;
    public int texSize = 128;
    private Texture2D tex;
    private byte[][] map;

    private float speed = 25.0f;

    private Vector3 pos;
    private Vector3 oldpos;
    private Vector3 gridpos;

    private Vector3 startPos;

    void Start()
    {
        map = new byte[texSize][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new byte[texSize];
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
        direction = Vector2.zero;
        transform.position = Vector3.zero;
        points.Clear();
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));
    }

    bool start = false;

    private Vector2 direction;

    private void Restart()
    {
        Start();
    }

    private List<Vector3> points = new List<Vector3>();
    private List<Vector3> pointsTMP = new List<Vector3>();

    private bool isHorizontal = true;

    private bool first = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(AutiFloodFill());
        }

        bool needPoint = false;
        float moveX = Input.GetAxisRaw("Horizontal") * speed * -1f;
        float moveY = Input.GetAxisRaw("Vertical") * speed * -1f;

        if (moveX != 0f && moveX == direction.x * -1 || moveY != 0f && moveY == direction.y * -1f)
        {
            moveX = 0f;
            moveY = 0f;
        }

        // auto-movement
        var somethingPressed = moveX != 0f || moveY != 0f;
        moveX = !somethingPressed ? direction.x : moveX;
        moveY = !somethingPressed ? direction.y : moveY;

        if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            moveY = 0.0f;
        else
            moveX = 0.0f;

        if (moveX == 0f && moveY == 0f)
            return;

        if (direction.x != 0 && moveX == 0 || direction.x == 0 && moveX != 0 || direction.y != 0 && moveY == 0 || direction.y == 0 && moveY != 0)
        {
            tex.SetPixel(Mathf.RoundToInt(gridpos.x), Mathf.RoundToInt(gridpos.y), Color.black);
            points.Add(new Vector3(gridpos.x, gridpos.y));
        }
        gridpos = new Vector3(Mathf.RoundToInt(transform.position.x + moveX * Time.deltaTime), Mathf.RoundToInt(transform.position.y + moveY * Time.deltaTime), 0);
        direction = new Vector2(moveX, moveY);

        //if (gridpos == oldpos)
        //return;
        if (gridpos.x < 0 || gridpos.x >= texSize || gridpos.y < 0 || gridpos.y >= texSize)
            return;
        transform.Translate(new Vector3(moveX, moveY, 0) * Time.deltaTime, Space.World);
        gridpos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

        if (gridpos != oldpos)
        {
            byte cur = map[(int)gridpos.x][(int)gridpos.y];

            if (cur == 33) // В ЗАБОР
            {
                //if(!start)
                // StartCoroutine(AutiFloodFill());
            }
            if (cur != 33) // тут ми були
            {
                map[(int)gridpos.x][(int)gridpos.y] = 1;
            }
            tex.SetPixel(Mathf.RoundToInt(gridpos.x), Mathf.RoundToInt(gridpos.y), Color.green);
            tex.Apply();

            if (cur == 1) // самі в себе
            {
                Restart();
            }
        }
        oldpos = gridpos;
    }

    IEnumerator AutiFloodFill()
    {
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));
        for (int y = 0; y < texSize; y++)
            for (int x = 0; x < texSize; x++)
            {
                if (polyCheck(points.ToArray(), new Vector3(x, y, 0)))
                {
                    tex.SetPixel(x, y, Color.green);
                }
            }
        tex.Apply();
        yield return null;
    }

    public bool polyCheck(Vector3[] p, Vector3 v)
    {
        int j = p.Length - 1;
        bool c = false;
        for (int i = 0; i < p.Length; j = i++)
            c ^= p[i].y > v.y ^ p[j].y > v.y && v.x < (p[j].x - p[i].x) * (v.y - p[i].y) / (p[j].y - p[i].y) + p[i].x;
        return c;
    }
}
