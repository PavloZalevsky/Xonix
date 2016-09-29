using System;
using System.Diagnostics;
using System.Linq;

// orig: http://pastebin.com/KHD8axSL

//namespace FloodFill
//{
//internal class Program
public class FloodFiller
{
    //        private const int MAP_SIZE = 1024; // only powers of two are supported

    /*
    private static void Main()
    {
    var map = new byte[MAP_SIZE * MAP_SIZE];

    var stopwatch = Stopwatch.StartNew();
    {
    FloodFill(map, startX: 123, startY: 123, fromValue: 0, toValue: 1);
    }
    stopwatch.Stop();

    Console.WriteLine(stopwatch.ElapsedMilliseconds);

    // Test results
    if (map.Any(item => item == 0))
    {
    Console.WriteLine("Error");
    }
    else
    {
    Console.WriteLine("Ok");
    }

    Console.ReadLine();
    }
    */

    public void FloodFill(byte[] map, int startX, int startY, byte fromValue, byte toValue)
    {
        int shift = (int)Math.Round(Math.Log(map.Length, 4)); // if the array's length is (2^x * 2^x), then shift = x
        int startIndex = startX + (startY << shift);

        if (map[startIndex] >= fromValue)
        {
            return;
        }



        // initialize flood fill
        int size = 1 << shift;
        int sizeMinusOne = size - 1;
        int xMask = size - 1;
        int minIndexForVerticalCheck = size;
        int maxIndexForVerticalCheck = map.Length - size - 1;

        // initialize queue
        int capacity = size * 2;
        int mask = capacity - 1;
        uint tail = 0;
        uint head = 0;
        var queue = new int[capacity];

        map[startIndex] = toValue;
        queue[tail++ & mask] = startIndex;

        while (tail - head > 0)
        {
            int index = queue[head++ & mask];
            int x = index & xMask;

            //if (x > 0 && map[index - 1] == fromValue)
            if (x > 0 && map[index - 1] < fromValue)
            {
                map[index - 1] = toValue;
                queue[tail++ & mask] = index - 1;
            }
            //if (x < sizeMinusOne && map[index + 1] == fromValue)
            if (x < sizeMinusOne && map[index + 1] < fromValue)
            {
                map[index + 1] = toValue;
                queue[tail++ & mask] = index + 1;
            }
            //if (index >= minIndexForVerticalCheck && map[index - size] == fromValue)
            if (index >= minIndexForVerticalCheck && map[index - size] < fromValue)
            {
                map[index - size] = toValue;
                queue[tail++ & mask] = index - size;
            }
            //if (index <= maxIndexForVerticalCheck && map[index + size] == fromValue)
            if (index <= maxIndexForVerticalCheck && map[index + size] < fromValue)
            {
                map[index + size] = toValue;
                queue[tail++ & mask] = index + size;
            }
        }




    }
}
//}
 
