using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Map : MonoBehaviour
{
    public int DensityPixels = 10;
    public Transform Player;
    public Camera cameraOther;

    public Transform target;
    // public int texSize = 128;
    private Texture2D tex;
    private byte[][] map;

    public List<Enemy> PoolEnemy = new List<Enemy>();
    private List<Enemy> enemies = new List<Enemy>();


    private float speed = 25.0f;


    private int xSize = 0;
    private int ySize = 0;



    private Vector3 pos;
    private Vector3 gridpos;

    private Vector3 startPos;

    private bool load = false;

    private int allPixel = 0;
    private int paintedPixels = 0;


    void OnEnable()
    {
        CameraSettings();
    }


    void Start()
    {
        map = new byte[xSize][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new byte[ySize];
        }
        tex = new Texture2D(xSize, ySize);

        target.GetComponent<Renderer>().material.mainTexture = tex;
        target.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                map[x][y] = 0;
                tex.SetPixel(x, y, Color.blue);

                if (x == 0 || y == 0 || x == xSize - 1 || y == ySize - 1)
                {
                    map[x][y] = 33; // border
                    tex.SetPixel(x, y, Color.gray);
                }
            }
        }

        tex.Apply();
        paintedPixels = 0;
        pos = new Vector3(xSize / 2, 0, 0);
        gridpos = pos;
        transform.position = pos;
        Player.position = new Vector3(pos.x + 0.5f, pos.y);

        direction = Vector2.zero;
        points.Clear();
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));


        SpawnEnemies();
        load = true;
    }

    private void SpawnEnemies()
    {
        //TODO//
        foreach (var item in PoolEnemy)
        {
            item.gameObject.SetActive(false);
        }
        enemies.Clear();

        int countEnemy = UnityEngine.Random.Range(2, 6);

        for (int i = 0; i < countEnemy; i++)
        {
            PoolEnemy[i].transform.position = new Vector3(UnityEngine.Random.Range(10, xSize - 10), UnityEngine.Random.Range(10, ySize - 10), 0);
            PoolEnemy[i].gameObject.SetActive(true);
            enemies.Add(PoolEnemy[i]);
        }
    }

    void CameraSettings()
    {
        float heightMainCamera = Camera.main.orthographicSize * 2f;
        float widtMainCamerh = heightMainCamera * Screen.width / Screen.height;
        target.transform.localScale = new Vector3(widtMainCamerh, heightMainCamera, 0.1f);

        xSize = Mathf.RoundToInt(widtMainCamerh * DensityPixels);
        ySize = Mathf.RoundToInt(heightMainCamera * DensityPixels);

        cameraOther.orthographicSize = cameraOther.orthographicSize * DensityPixels;
        float heightOtherCamera = 2f * cameraOther.orthographicSize;
        float widthOtherCamera = heightOtherCamera * cameraOther.aspect;
        cameraOther.transform.position = new Vector3(cameraOther.transform.position.x + widthOtherCamera / 2, cameraOther.transform.position.y + heightOtherCamera / 2, -10);

        allPixel = xSize * ySize;

        Debug.Log(xSize + "   " + ySize + "  AllPixel " + allPixel);
    }


    private Vector2 direction;

    private void Restart()
    {
        load = false;
        Start();
    }

    private List<Vector3> points = new List<Vector3>();

    private List<Vector2> myPoins = new List<Vector2>();

    void Update()
    {
        MoveEnemy();
        //return;
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(AutoFloodFill());
        }

        float moveX = Input.GetAxisRaw("Horizontal") * speed;
        float moveY = Input.GetAxisRaw("Vertical") * speed;

        if (moveX != 0f && moveX == direction.x * -1 || moveY != 0f && moveY == direction.y * -1f)
        {
            moveX = 0f;
            moveY = 0f;
        }

        // auto-movement
        //var somethingPressed = moveX != 0f || moveY != 0f;
        //moveX = !somethingPressed ? direction.x : moveX;
        //moveY = !somethingPressed ? direction.y : moveY;

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
        Player.position = new Vector3(gridpos.x + 0.5f, gridpos.y);


        direction = new Vector2(moveX, moveY);

        //if (gridpos == oldpos)
        //return;
        if (gridpos.x < 0 || gridpos.x >= xSize || gridpos.y < 0 || gridpos.y >= ySize)
            return;
        transform.Translate(new Vector3(moveX, moveY, 0) * Time.deltaTime, Space.World);
        gridpos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);


        int x_x = (Mathf.RoundToInt(gridpos.x));
        int y_y = (Mathf.RoundToInt(gridpos.y));
        byte cur = map[x_x][y_y];

        if (cur == 33) // В ЗАБОР
        {
            StartCoroutine(AutoFloodFill());
        }
        if (cur != 33) // тут ми були
        {
            map[x_x][y_y] = 1;
            myPoins.Add(new Vector2(x_x, y_y));
        }

        tex.SetPixel(Mathf.RoundToInt(gridpos.x), Mathf.RoundToInt(gridpos.y), Color.green);
        tex.Apply();

        if (cur == 1) // самі в себе
        {
            //  Restart();
        }

    }


    void MoveEnemy()
    {
        //TODO//
        if (!load) return;

        foreach (var ene in enemies)
        {

            int x_x = (Mathf.RoundToInt(ene.transform.position.x));
            int y_y = (Mathf.RoundToInt(ene.transform.position.y));


            //if (x_x < 0 || y_y < 0 || x_x > xSize - 1 || y_y > ySize - 1)
            //{
            //    Debug.LogError(x_x + "   " + y_y);
            //    //    return;
            //}

            if (x_x < 0)
            {
                x_x = 0;
            }
            if (x_x > xSize - 1)
            {
                x_x = xSize - 1;
            }
            if (y_y < 0)
            {
                y_y = 0;
            }
            if (y_y > ySize - 1)
            {
                y_y = ySize - 1;
            }


            //if (x_x > xSize || x_x < 0)
            //{
            //    ene.directionEnemy = new Vector2(ene.directionEnemy.x, ene.directionEnemy.y * -1);
            //    continue;
            //}
            //else if(y_y > ySize || y_y < 0)
            //{
            //    ene.directionEnemy = new Vector2(ene.directionEnemy.x * -1, ene.directionEnemy.y);
            //    continue;

            //}



            if (map[x_x][y_y] == 33)
            {
                if ((y_y + 1 < ySize - 1 && map[x_x][y_y + 1] == 33) && (y_y - 1 > 0 && map[x_x][y_y - 1] == 33))
                {
                    ene.directionEnemy = new Vector2(ene.directionEnemy.x * -1, ene.directionEnemy.y);
                }
                else if ((x_x + 1 < xSize - 1 && map[x_x + 1][y_y] == 33) && (x_x - 1 > 0 && map[x_x - 1][y_y] == 33))
                {
                    ene.directionEnemy = new Vector2(ene.directionEnemy.x, ene.directionEnemy.y * -1);
                }
                else
                {
                    ene.directionEnemy = new Vector2(ene.directionEnemy.x * -1, ene.directionEnemy.y * -1);
                }
                ene.transform.Translate(ene.directionEnemy * Time.deltaTime * ene.speedEnemy, Space.World);

            }



            if (map[x_x][y_y] == 1)
            {
                Restart();
                break;
            }

            //map[x_x][y_y] = 2;

            //if (ene.LastPoz != Vector2.zero)
            //{
            //    map[(int)ene.LastPoz.x][(int)ene.LastPoz.y] = ene.LastSumbl; 
            //}

            //ene.LastPoz = new Vector2(x_x, y_y);
            //ene.LastSumbl = map[x_x][y_y];
            ene.transform.Translate(ene.directionEnemy * Time.deltaTime * ene.speedEnemy, Space.World);

        }
    }

    bool one = false;

    IEnumerator AutoFloodFill()
    {
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));
        tex.SetPixel(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Color.magenta);
        tex.Apply();

        CheckPoins();
        CreateMewBorder();
        Debug.Log(paintedPixels + "    " + (paintedPixels /(allPixel / 100)));





        yield break;
        yield return StartCoroutine(WaitForKeyPress());

        one = true;
        for (int y = 0; y < xSize; y++)
            for (int x = 0; x < ySize; x++)
            {
                if (polyCheck(points.ToArray(), new Vector3(x, y, 0)))
                {
                    tex.SetPixel(x, y, Color.green);
                }
            }
        tex.Apply();


        //  points.Clear();

        CreateMewBorder();

        yield return null;
    }

    private IEnumerator WaitForKeyPress()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R))
                yield break;
            yield return 0;
        }
    }

    private void CheckPoins()
    {
        var lastPoint = points.Last();
        var preLastPoint = points[points.Count - 2];
        var prePreLastPoint = points[points.Count - 3];
        var middle = new Vector3(Mathf.RoundToInt((lastPoint.x + preLastPoint.x + prePreLastPoint.x) / 3),
            Mathf.RoundToInt((lastPoint.y + preLastPoint.y + prePreLastPoint.y) / 3));
        StartCoroutine(FloodFillCorot(Mathf.RoundToInt(middle.x), Mathf.RoundToInt(middle.y)));
        //FloodFill(Mathf.RoundToInt(middle.x), Mathf.RoundToInt(middle.y));
        tex.Apply();
        return;
        var listToClear = new List<Vector3>();
        for (int i = 0; i < points.Count; i++)
        {
            var newList = new List<Vector3>();
            newList.AddRange(points);
            newList.Remove(newList[i]);
            if (polyCheck(newList.ToArray(), points[i]))
            {
                listToClear.Add(points[i]);
                tex.SetPixel(Mathf.RoundToInt(points[i].x), Mathf.RoundToInt(points[i].y), Color.red);
            }
        }
        tex.Apply();
        foreach (var item in listToClear)
            points.RemoveAll(vector3 => vector3 == item);
    }

    private IEnumerator FloodFillCorot(int x, int y)
    {
        var color = tex.GetPixel(x, y);
        //if (map[x][y] == 33)
        //return;
        //map[x][y

        if (color == Color.green || color == Color.magenta || color == Color.black || x < 0 || x >= xSize || y < 0 || y >= ySize)
            yield break;
        tex.SetPixel(x, y, Color.green);
        tex.Apply();
        paintedPixels++;

        yield return new WaitForSeconds(0.025f);
        StartCoroutine(FloodFillCorot(x + 1, y));
        StartCoroutine(FloodFillCorot(x - 1, y));
        StartCoroutine(FloodFillCorot(x, y + 1));
        StartCoroutine(FloodFillCorot(x, y - 1));
    }

    public void FloodFill(int x, int y)
    {
        var color = tex.GetPixel(x, y);
        //if (map[x][y] == 33)
        //return;
        //map[x][y
        if (color == Color.green)
            return;
        tex.SetPixel(x, y, Color.green);
        tex.Apply();
        FloodFill(x + 1, y);
        FloodFill(x - 1, y);
        FloodFill(x, y + 1);
        FloodFill(x, y - 1);
    }

    private void CreateMewBorder()
    {
        foreach (var item in myPoins)
        {
            map[(int)item.x][(int)item.y] = 33;
        }

        myPoins.Clear();
    }

    private void CheckForBorders()
    {
        var firstPoint = points.First();
        var lastPoint = points.Last();

        if (map[(int)lastPoint.x][(int)lastPoint.y] == 33)
        {
            var tmpY = lastPoint.y;

            if (lastPoint.x == xSize - 1 || lastPoint.x == 0)
            {
                //   FIX Y
                if (lastPoint.y < firstPoint.y || lastPoint.y > firstPoint.y)
                {

                    var dir = firstPoint.y - lastPoint.y > 0 ? 1 : -1;
                    while (tmpY > 0 && tmpY < ySize || map[(int)lastPoint.x][(int)tmpY] != 33)
                    {
                        tmpY += dir;
                        tex.SetPixel((int)lastPoint.x, (int)tmpY, Color.green);
                        if (map[(int)tmpY][(int)lastPoint.x] != 33)
                            if (map[(int)lastPoint.x][(int)tmpY] != 33)
                            {
                                points.Add(new Vector3(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(tmpY)));
                                break;
                            }
                    }
                    points.Add(new Vector3(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(tmpY)));
                }
            }
            if (lastPoint.y == ySize - 1 || lastPoint.y == 0)
            {

                //  FIX X
                if (lastPoint.x < firstPoint.x || lastPoint.x > firstPoint.x)
                {
                    var tmpX = lastPoint.x;
                    var dir = firstPoint.x - lastPoint.x > 0 ? 1 : -1;
                    while (tmpX > 0 && tmpX < xSize || map[(int)tmpX][(int)lastPoint.y] != 33)
                    {
                        tmpX += dir;
                        tex.SetPixel((int)tmpX, (int)lastPoint.y, Color.green);
                        if (map[(int)tmpY][(int)lastPoint.x] != 33)
                            if (map[(int)lastPoint.x][(int)tmpY] != 33)
                            {
                                points.Add(new Vector3(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(tmpY)));
                                break;
                            }
                    }
                    points.Add(new Vector3(Mathf.RoundToInt(tmpX), Mathf.RoundToInt(lastPoint.y)));
                }
            }
        }
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
public enum Value
{
    border = (byte)33
}