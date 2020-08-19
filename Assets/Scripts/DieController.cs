﻿using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DieController : MonoBehaviour
{
	public float upForce;
	public float spinForce;

	Color[] faceColors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan };
	bool rolled = false;
	bool stopped = false;
	bool held = false;
	Light light;
	int tilesheetW = 48;
	int tilesheetH = 22;
	Mesh mesh;
	Rigidbody body;
	int count = 0;
	int tile = 0;
	Renderer renderer;
	Material material;

	float x;
	float z;

	void SetFace(int tileX, int tileY, int face)
	{
		Vector2[] tileCoords = {
			new Vector2((float)tileX / tilesheetW, (float)tileY / tilesheetH),
			new Vector2((float)(tileX + 1) / tilesheetW, (float)tileY / tilesheetH),
			new Vector2((float)tileX / tilesheetW, (float)(tileY + 1) / tilesheetH),
			new Vector2((float)(tileX + 1) / tilesheetW, (float)(tileY + 1) / tilesheetH),
		};

		Vector2[] uvs = mesh.uv;


		if (face == 4)
		{
			// Front
			uvs[0] = tileCoords[0];
			uvs[1] = tileCoords[1];
			uvs[2] = tileCoords[2];
			uvs[3] = tileCoords[3];
		}
		else if (face == 2)
		{
			// Top
			uvs[8] = tileCoords[0];
			uvs[9] = tileCoords[1];
			uvs[4] = tileCoords[2];
			uvs[5] = tileCoords[3];
		}
		else if (face == 5)
		{
			// Back
			uvs[7] = tileCoords[0];
			uvs[6] = tileCoords[1];
			uvs[11] = tileCoords[2];
			uvs[10] = tileCoords[3];
		}
		else if (face == 3)
		{
			// Bottom
			uvs[13] = tileCoords[0];
			uvs[12] = tileCoords[1];
			uvs[14] = tileCoords[2];
			uvs[15] = tileCoords[3];
		}
		else if (face == 1)
		{
			// Left
			uvs[17] = tileCoords[0];
			uvs[16] = tileCoords[1];
			uvs[18] = tileCoords[2];
			uvs[19] = tileCoords[3];
		}
		else if (face == 0)
		{
			// Right        
			uvs[21] = tileCoords[0];
			uvs[20] = tileCoords[1];
			uvs[22] = tileCoords[2];
			uvs[23] = tileCoords[3];
		}

		mesh.uv = uvs;
	}

	// Start is called before the first frame update
	void Start()
	{
		light = GetComponentInChildren<Light>();
		renderer = GetComponentInChildren<Renderer>();
		material = renderer.material;
		body = GetComponent<Rigidbody>();
		var mf = GetComponent<MeshFilter>();
		mesh = mf.mesh;

		/*SetFace(32, 15, 0);
		SetFace(32, 15, 1);
		SetFace(32, 15, 2);
		SetFace(41, 10, 3);
		SetFace(37, 18, 4);
		SetFace(37, 18, 5);*/

		SetFace(35, 4, 0);
		SetFace(36, 4, 1);
		SetFace(37, 4, 2);
		SetFace(38, 4, 3);
		SetFace(39, 4, 4);
		SetFace(40, 4, 5);

	}

	private void Update()
	{
		if (rolled && !stopped && body.velocity == Vector3.zero && body.angularVelocity == Vector3.zero)
		{
			Color c = faceColors[GetUpwardFace()];
			material.SetColor("FaceColor", c);
			renderer.material = material;
			light.color = c;
			stopped = true;
		}
	}

	public void Roll(Vector3 pos)
	{
		material.SetColor("FaceColor", Color.white);
		renderer.material = material;
		light.color = Color.white;
		transform.rotation = Quaternion.Euler(UnityEngine.Random.onUnitSphere * 360);
		transform.position = pos;
		rolled = true;
		stopped = false;
		held = false;
	}

	public bool Hold(Vector3 pos)
	{
		if (held || !stopped || !rolled)
		{
			return false;
		}
		transform.position = new Vector3(pos.x, transform.position.y, pos.z);
		Debug.Log("hold");
		held = true;
		return true;
	}

	private void OnDrawGizmos()
	{
		Vector3[] directions = { transform.right, -transform.right, transform.up, -transform.up, transform.forward, -transform.forward };
		Gizmos.DrawLine(transform.position, transform.position + directions[GetUpwardFace()]);
	}

	private int GetUpwardFace()
	{
		Vector3[] directions = { transform.right, -transform.right, transform.up, -transform.up, transform.forward, -transform.forward};
		var angles = directions.Select(x => Mathf.Abs(Vector3.Angle(Vector3.up, x))).ToArray();
		return Array.IndexOf(angles, angles.Min());

	}
}
