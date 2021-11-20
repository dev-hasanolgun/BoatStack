using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public MeshFilter MeshFilter;
    
    public void GenerateMesh(Vector3[] positions, Vector3[] pathNormals, int Density, float Width, float Depth)
    {
        var mesh = MeshFilter.mesh;
        var posAmount = positions.Length;
        
        var vertices = new Vector3[posAmount*Density];
        for (int a = 0, i = 0; i < posAmount; i++)
        {
            var pos = positions[i];
            var vec = i == posAmount-1 ? pos - positions[i-1] : positions[i+1] - pos;
            var nor = Vector3.up;
            var cross = Vector3.Cross(vec.normalized, nor);
            for (int j = (Density - 1)/2; j > 0; j--)
            {
                var dis = Width/2f / ((Density - 1) / 2);
                var vel = Mathf.SmoothStep(0, 1, j / ((Density - 1) / 2f + Density/3f));
                var deltaPos = new Vector3(pathNormals[i].x,0,pathNormals[i].z);
                vertices[a] = pos + cross * (dis * j) + Vector3.up * (vel * Depth);
                a++;
            }
            vertices[a] = pos;
            a++;
            for (int j = 1; j <= (Density - 1)/2; j++)
            {
                var dis = Width/2f / ((Density - 1) / 2);
                var vel = Mathf.SmoothStep(0, 1, j / ((Density - 1) / 2f + Density/3f));
                var deltaPos = new Vector3(pathNormals[i].x,0,pathNormals[i].z);
                vertices[a] = pos - cross * (dis * j) + Vector3.up * (vel * Depth);
                a++;
            }
        }
        mesh.vertices = vertices;
        var tris = new int[(vertices.Length - Density) / Density * (Density - 1) * 6];
        
        var k = 0;
        var m = 0;
        for (int i = 0; i < (posAmount - 1)*6; i++)
        {
            tris[k] = 0+Density*m;
            tris[k+1] = Density+Density*m;
            tris[k+2] = 1+Density*m;
            tris[k+3] = Density+Density*m;
            tris[k+4] = Density+1+Density*m;
            tris[k+5] = 1+Density*m;
            i += 5;
            k += 6;
            for (int j = 0; j < (Density - 2) * 6; j++)
            {
                tris[k] = tris[k - 6] + 1;
                k++;
            }

            m++;
        }
        mesh.triangles = tris;
        
        mesh.RecalculateNormals();
    }

    public void GenerateObstacles(List<ObstacleData> obstacleDataList)
    {
        var data = obstacleDataList;
        for (int i = 0; i < data.Count; i++)
        {
            Instantiate(data[i].Obstacle, data[i].Position + Vector3.up * (data[i].Obstacle.transform.localScale.y / 2f), data[i].Rotation);
        }
    }
}
