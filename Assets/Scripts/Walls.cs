using UnityEngine;
using System.Collections;
using System.Linq;

public class Walls {
    public const int North = 0, East = 1, South = 2, West = 3; 

    public static float Width = 16;
    public static float Height = 16;

    public static int GetWall(Vector3 o, Vector3 d) {
        float w2 = Width/2, h2 = Height/2;

        // O + tD = z
        // tD = z - O
        // t = (z - O) / D

        float t1 = ( h2 - o.z) / d.z;
        float t2 = ( w2 - o.x) / d.x;
        float t3 = (-h2 - o.z) / d.z;
        float t4 = (-w2 - o.x) / d.x;

        var list = new float[]{t1, t2, t3, t4};
        var newlist = list.OrderBy(x => x).ToList();
        float min = newlist[2];

        if(min == t1) {
            return North;
        } else if(min == t2) {
            return East;
        } else if(min == t3) {
            return South;
        } else if(min == t4) {
            return West;
        }

        // Should never happen
        return -1;
    }
}
