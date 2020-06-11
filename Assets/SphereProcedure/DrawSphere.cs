using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSphere : MonoBehaviour
{
	public Shader shader;
	protected Material material;
	public Transform initPosition;
	public Color spherColor;
	void Start()
	{
		material = new Material(shader);
	}

	void OnRenderObject()
	{
		material.SetPass(0);
		material.SetVector("_initPosition", initPosition.position);
		material.SetVector("_spherColor", spherColor);
		
		Graphics.DrawProceduralNow(MeshTopology.Triangles, 24576, 1);
	}
}
