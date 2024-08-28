 
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Physics;
//using Unity.Physics.Systems;
//using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
 
 
using csDelaunay;
using System;

public class VoronoiDiagram : MonoBehaviour {
 
    // The number of polygons/sites we want
    public int polygonNumber = 10;
 
    // This is where we will store the resulting data
    private Dictionary<System.Numerics.Vector2, Site> sites;
    private List<csDelaunay.Edge> edges;
 
    void Start() {
        // Create your sites (lets call that the center of your polygons)
        List<System.Numerics.Vector2> points = CreateRandomPoint();
       
        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        Rectf bounds = new Rectf(0,0,512,512);
       
        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        Voronoi voronoi = new Voronoi(points,bounds, 0);
 
        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);
 
        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        

        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;

        //check the edges of these objects  and find site that is nearest to each edge, then add in a new edge
        Vertex Vert_TL = new Vertex(0, 512); //TOP LEFT
        System.Numerics.Vector2 closest_site_TL = GetClosestSiteIndexDict(Vert_TL, sites);

        //get the two vertexes which 


        foreach (KeyValuePair<System.Numerics.Vector2, csDelaunay.Site> site in sites)
        {
            Debug.Log(" ------------------------------ checking new site!!");





            foreach (var edge in site.Value.Edges)
            {

                if (edge.ClippedEnds == null) continue;
 
                //DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);

                var vertex_L = ReflectPointBothAxes(edge.ClippedEnds[LR.LEFT].X, edge.ClippedEnds[LR.LEFT].Y, 512, 512);
                var vertex_R = ReflectPointBothAxes(edge.ClippedEnds[LR.RIGHT].X, edge.ClippedEnds[LR.RIGHT].Y, 512, 512);

                //System.Numerics.Vector2 vert_L = TranslatePointUnity(512 , edge.ClippedEnds[LR.LEFT]);

                //System.Numerics.Vector2 vert_L = TranslatePointUnity(512 , edge.ClippedEnds[LR.LEFT]);
                //System.Numerics.Vector2 vert_R = TranslatePointUnity(512 , edge.ClippedEnds[LR.RIGHT]);
                Debug.Log("vert_L: (" + vertex_L.x + ", " + vertex_L.y + ")  |  vert_R: (" + vertex_R.x + ", " + vertex_R.y + ")");
            }
            
        }

        //var length = sites[0].Edges.Length;
 
        DisplayVoronoiDiagram();
    }

    public static void FixVoronoiEdgesAndCorners(){
        // DEVELOP THS SUCH THAT IT DOES 1 PASS OVER THE DATA STRUCTURE

        // Corners will have vertexes that have two different max/min component on their vertexes ex: (edge): (0, 436) and (400, 512) -> 0 and 512 are bounds (works only if 1 corner)
        // sides will have vertexes that have only 1 type of max/min component on their vetexes: ex: (edge): (0, 436) and (0, 512) (works only if 1 corner)
        
        //  what about sites with 2 corner?
        // if a site contains vertexes with two min/maxes on the same axis that means it has two corners!

        //  what about sites with 3 corner?
        // .. must use distance


        //new 

        // loop through every site and find the sites which are the closet to the corners, save them to a list, the number or corners a one site denotes the number of edges to add (2 * corners - 1) -> create corners 
        // loop through every site, every site that is closed is ignored, every site that is open and not in the corner list gets 1 edge added between the first and last edge
        // *the order of the edges in the list does not have guarenteed ordering* as vertex is found, add to list along with bool = false, on new vertex check list, if not found then add to list. at the end, any values with a value of false will mean an edge, make an edge with all the false vertexes.



        // !cannot usually have a vertex with (0,512) as that would represent a corner (two different max/min component on the same vertex) -> this could happen but very rare
    }

    public static System.Numerics.Vector2 GetClosestSiteIndex(Vertex point, List<Site> sites){
        float distance = CalculateDistanceVertex(point, sites[0].Coord);
        float new_distance;
        System.Numerics.Vector2 index = sites[0].Coord;

        for (int x = 1; x < sites.Count; x++)
        {
            new_distance = CalculateDistanceVertex(point, sites[x].Coord);

            if(new_distance < distance){
                distance = new_distance;
                index = sites[x].Coord;
            }
        }

        return index;
    }

    public static System.Numerics.Vector2 GetClosestSiteIndexDict(Vertex point, Dictionary<System.Numerics.Vector2, Site> sites)
{
    var enumerator = sites.GetEnumerator();
    enumerator.MoveNext(); // Move to the first element

    // Initialize the closest site using the first element of the dictionary
    float distance = CalculateDistanceVertex(point, enumerator.Current.Key);
    System.Numerics.Vector2 closestCoord = enumerator.Current.Key;

    // Iterate over the rest of the dictionary
    while (enumerator.MoveNext())
    {
        System.Numerics.Vector2 currentCoord = enumerator.Current.Key;
        float newDistance = CalculateDistanceVertex(point, currentCoord);

        if (newDistance < distance)
        {
            distance = newDistance;
            closestCoord = currentCoord;
        }
    }

    return closestCoord;
}

    public static float CalculateDistanceVertex(Vertex point1, System.Numerics.Vector2 point2)
    {
        // Calculate the differences in x and y coordinates
        float dx = point2.X - point1.x;
        float dy = point2.Y - point1.y;

        // Calculate the distance using the Euclidean distance formula
        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        
        return distance;
    }

    // creates the corners
    public static void CompleteEdges(){

    }


    /*
    public static List<Triangle> TriangulateConvexPolygonFromEdges(Site site)
    {
        List<Triangle> triangles = new List<Triangle>();

        List<Vertex> convexHullpoints



        for (int i = 2; i < convexHullpoints.Count; i++)
        {
            Vertex a = convexHullpoints[0];
            Vertex b = convexHullpoints[i - 1];
            Vertex c = convexHullpoints[i];

            triangles.Add(new Triangle(a, b, c));
        }

        return triangles;
    }
    

    public static List<Triangle> TriangulateConvexPolygon(List<Vertex> convexHullpoints)
    {
        List<Triangle> triangles = new List<Triangle>();

        for (int i = 2; i < convexHullpoints.Count; i++)
        {
            Vertex a = convexHullpoints[0];
            Vertex b = convexHullpoints[i - 1];
            Vertex c = convexHullpoints[i];

            triangles.Add(new Triangle(a, b, c));
        }

        return triangles;
    }*/

    public static (float x, float y) ReflectPointBothAxes(float x, float y, float imageWidth, float imageHeight)
{
    float axisX = imageWidth / 2;
    float axisY = imageHeight / 2;
    float reflectedX = 2 * axisX - x;
    float reflectedY = 2 * axisY - y;
    return (reflectedX, reflectedY);
}

    private System.Numerics.Vector2 TranslatePointUnity (float image_width, System.Numerics.Vector2 vector){
        float middle_distance = image_width/2;
        System.Numerics.Vector2 new_vector = new System.Numerics.Vector2();

        //x axis
        float distance_to_middle_x = vector.X - middle_distance;
        if(Mathf.Sign(distance_to_middle_x) > 0){
            //posative distance
            new_vector.X = vector.X + distance_to_middle_x * 2;
        }else{
            //negative distance
            new_vector.X = vector.X - distance_to_middle_x * 2;
        }

        //y axis
        float distance_to_middle_y = vector.Y - middle_distance;
        if(Mathf.Sign(distance_to_middle_y) > 0){
            //posative distance
            new_vector.Y = vector.Y + distance_to_middle_y * 2;
        }else{
            //negative distance
            new_vector.Y = vector.Y - distance_to_middle_y * 2;
        }

        return new_vector;
    }
   
    private List<System.Numerics.Vector2> CreateRandomPoint() {
        // Use Vector2f, instead of Vector2
        // Vector2f is pretty much the same than Vector2, but like you could run Voronoi in another thread
        List<System.Numerics.Vector2> points = new List<System.Numerics.Vector2>();
        for (int i = 0; i < polygonNumber; i++) {
            points.Add(new System.Numerics.Vector2( UnityEngine.Random.Range(0,512), UnityEngine.Random.Range(0,512)));
        }
 
        return points;
    }
 
    // Here is a very simple way to display the result using a simple bresenham line algorithm
    // Just attach this script to a quad
    private void DisplayVoronoiDiagram() {
        Texture2D tx = new Texture2D(512,512);
        foreach (KeyValuePair<System.Numerics.Vector2,Site> kv in sites) {
            tx.SetPixel((int)kv.Key.X, (int)kv.Key.Y, Color.red);
        }
        foreach (csDelaunay.Edge edge in edges) {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;
 
            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();

        var renderer =  GetComponent<Renderer>();
        renderer.material.mainTexture = tx;
 
        //this.renderer.material.mainTexture = tx;
    }
 
    // Bresenham line algorithm
    private void DrawLine(System.Numerics.Vector2 p0, System.Numerics.Vector2 p1, Texture2D tx, Color c, int offset = 0) {
        int x0 = (int)p0.X;
        int y0 = (int)p0.Y;
        int x1 = (int)p1.X;
        int y1 = (int)p1.Y;
       
        int dx = Mathf.Abs(x1-x0);
        int dy = Mathf.Abs(y1-y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx-dy;
       
        while (true) {
            tx.SetPixel(x0+offset,y0+offset,c);
           
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2*err;
            if (e2 > -dy) {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx) {
                err += dx;
                y0 += sy;
            }
        }
    }
}      
 