using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCube : MonoBehaviour
{
	public Shader shader;
	protected Material material;

	void Start()
	{
		material = new Material(shader);
	}

	void OnRenderObject()
	{
		material.SetPass(0);
		Graphics.DrawProceduralNow(MeshTopology.Triangles, 36 * 100000, 1);
	}
}
