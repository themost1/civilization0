﻿using UnityEngine;
using System.Collections;

/// <summary>
/// The individual hexagon; lives only in the heap
/// </summary>
public class HexInfo
{
	#region fields
	private Vector3 gridPosition;//cube cordinates stored(x,y == axial)
	public Vector3 localPosition;
	public Vector3 worldPosition;
	
	public Vector3 hexExt;
	public Vector3 hexCenter;
	
	public HexChunk parentChunk;
	
	public Mesh localMesh;
	
	public bool clicked, previouslyGenerated, full = false;
	public float randy;
	public float randx;
	public float prevx, prevy;
	
	public bool conifer = false;
	public bool broadleaf = false;
	public bool brimstone = false;
	
	//basic hexagon mesh making
	public Vector3[] vertices;
	public Vector2[] uv;
	public int[] triangles;
	
	//	public HexInfo (bool asdf){
	//		if(asdf){
	//			randy = 2f;
	//			randx = 3f;
	//		}
	//	}
	//
	//	public HexInfo(){
	//		clicked = false;
	//	}
	
	//get axial grid position
	public Vector2 AxialGridPosition
	{
		get { return new Vector2(CubeGridPosition.x, CubeGridPosition.y); }
	}
	//get/set cube grid position
	public Vector3 CubeGridPosition
	{
		get { return gridPosition; }
		set { gridPosition = value; }
	}
	#endregion
	
	/// <summary>
	/// This is the setup called from HexChunk when it's ready for us to generate our meshes
	/// </summary>
	public void Start()
	{
		MeshSetup();
	}
	
	/// <summary>
	/// This pulls the cached hexagon mesh data from the WorldManager down into our localMesh
	/// </summary>
	public void MeshSetup()
	{
		
		//create new mesh to start fresh
		localMesh = new Mesh();
		
		//pull mesh data from WorldManager
		localMesh.vertices = parentChunk.worldManager.flatHexagonSharedMesh.vertices;
		
		//		new Vector2(0,0.25f),
		//		new Vector2(0,0.75f),
		//		new Vector2(0.5f,1),
		//		new Vector2(1,0.75f),
		//		new Vector2(1,0.25f),
		//		new Vector2(0.5f,0),
		
		//Assign terrain randomly
		if(!this.clicked){
			randx = 0f;
			randy = 0f;
			do{
				randx = Random.Range(0,4)*1f;
				randy = Random.Range(0,4)*1f;
			}while(randx == 3f && randy == 1f);
			if (this.previouslyGenerated) {
				randx=prevx;
				randy=prevy;
			}
			else {
				prevx=randx;
				prevy=randy;
			}
			if (!previouslyGenerated)
				previouslyGenerated=true;
			
			if((randx == 3f && randy == 3f) || (randx == 2f && randy == 0f))
				conifer = true;
			else if(randx == 2f && randy == 3f)
				broadleaf = true;
			else if(randx == 0 && randy == 3f)
				brimstone = true;
		}
		else{
			randx = 3f;
			randy = 1f;
		}
		
		float tUnit = 0.25f;
		Vector2[] temp = {
			new Vector2(randx*tUnit, tUnit*(randy + 0.25f)),
			new Vector2(randx*tUnit, tUnit*(randy + 0.75f)),
			new Vector2(tUnit*(randx + 0.5f),tUnit*(randy + 1f)),
			new Vector2(tUnit*(randx + 1f),tUnit*(randy + 0.75f)),
			new Vector2(tUnit*(randx + 1f),tUnit*(randy + 0.25f)),
			new Vector2(tUnit*(randx + 0.5f),tUnit*randy),
		};
		localMesh.uv = temp;
		
		localMesh.triangles = parentChunk.worldManager.flatHexagonSharedMesh.triangles;
		
		
		
		//recalculate normals to play nicely with lighting
		localMesh.RecalculateNormals();
	}
}