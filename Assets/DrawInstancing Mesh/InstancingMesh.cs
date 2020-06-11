using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancingMesh : MonoBehaviour
{

    List<Matrix4x4> transformList = new List<Matrix4x4>();

    public Mesh cubeMesh;
    public Material cubeMaterial;
    // Start is called before the first frame update
    void Start()
    {
        //These for loops create offsets from the position at which you want to draw your cube built from cubes.
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                for (int z = -1; x < 1; z++)
                {
                    //We will assume you want to create your cube of cubes at 0,0,0
                    Vector3 position = new Vector3(0, 0, 0);

                    //Take the origin position, and apply the offsets
                    position.x += x;
                    position.y += y;
                    position.z += z;

                    //Create a matrix for the position created from this iteration of the loop
                    Matrix4x4 matrix = new Matrix4x4();

                    //Set the position/rotation/scale for this matrix
                    matrix.SetTRS(position, Quaternion.Euler(Vector3.zero), Vector3.one);

                    //Add the matrix to the list, which will be used when we use DrawMeshInstanced.
                    transformList.Add(matrix);
                }
            }
        }
        //After the for loops are finished, and transformList has several matrices in it, simply pass DrawMeshInstanced the mesh, a material, and the list of matrices containing all positional info
        Graphics.DrawMeshInstanced(cubeMesh, 0, cubeMaterial, transformList);
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
