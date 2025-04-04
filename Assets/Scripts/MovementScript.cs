﻿using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {
	// Use this for initialization
	float totalMoved;
	Vector3 direction;
	bool moving, initialCheck;
	static float xDist = Mathf.Sqrt(3)/2f,zDist = 1.5f;
	
	Vector3 UPLEFT = new Vector3(-xDist,0,zDist),
	UPRIGHT = new Vector3(xDist,0,zDist),
	LEFT = new Vector3(-2f*xDist,0,0),
	RIGHT = new Vector3(2f*xDist,0,0),
	DOWNLEFT = new Vector3(-xDist,0,-zDist),
	DOWNRIGHT = new Vector3(xDist,0,-zDist);

	public bool test=false;
	GameObject theTank; //The currently selected gameobject

	void Start () {
		totalMoved = 0;
		direction = new Vector3(0,0,0);
		moving = false;
	}

	//Read key press to move selected tank
	void Update () {
		if (!moving){
			theTank = Gameplay.selected;
			
			if (Input.GetKey (KeyCode.Q))
				beginMoving(UPLEFT);	//Move to the adjacent hex up and to the left
			else if (Input.GetKey (KeyCode.E))
				beginMoving(UPRIGHT);	//Move up and right
			else if(Input.GetKey(KeyCode.A))
				beginMoving(LEFT);	//Move left
			else if(Input.GetKey(KeyCode.D))
				beginMoving(RIGHT);	//Move right
			else if(Input.GetKey(KeyCode.Z))
				beginMoving(DOWNLEFT);	//Move down and left
			else if(Input.GetKey(KeyCode.C))
				beginMoving(DOWNRIGHT);	//Move down and right
			else if(Input.GetKey(KeyCode.J) && theTank!=null)			
				theTank.transform.Rotate(Vector3.down*Time.deltaTime*69);
			else if(Input.GetKey(KeyCode.L) && theTank!=null)
				theTank.transform.Rotate(Vector3.up*Time.deltaTime*69);
			else if(Input.GetKey(KeyCode.I))
				rotateBarrel(Vector3.left); //Rotate barrel up
			else if(Input.GetKey(KeyCode.K))
				rotateBarrel(Vector3.right); //Rotate barrel down
		}
		else{
			beginMoving(direction);
		}
	}

	void beginMoving(Vector3 dir) {
		direction = dir;
		bool canMove = true, hexFound = false;
		int movementPoints=0;
		
		//hexFound is the hex that it will land on (so it doesn't go off the map)
		if (!initialCheck&&theTank!=null) {
			initialCheck = true;
			foreach (GameObject tnak in Gameplay.vehicles)
				if (theTank != null && tnak != null && !tnak.Equals (theTank) && 
					Vector3.Distance (tnak.transform.position, theTank.transform.position + direction) < 0.1f)
					canMove = false;
			foreach (HexChunk chunk in WorldManager.hexChunks) {
				foreach (HexInfo hex in chunk.hexArray) {
					if (Vector3.Distance (theTank.transform.position + direction, hex.worldPosition) < 0.5f && hex.full)
						canMove = false;
					else if (Vector3.Distance (theTank.transform.position + direction, hex.worldPosition) < 1f){
						hexFound = true;
						movementPoints=1;
						if(hex.conifer||hex.broadleaf)
							movementPoints++;
						else if(hex.brimstone)
							canMove=false;
						if (canMove && Gameplay.powerPoints>=movementPoints)
							Gameplay.powerPoints -= movementPoints;
						else {
							canMove=false;
						}
							
					}
				}
			}
		} 
		else {
			canMove=true;
			hexFound=true;
		}
		if (Gameplay.powerPoints < movementPoints)
			Gameplay.warningText = "Not enough powerpoints!";
		else if (SelectUnit.clickedHex.brimstone)	
			Gameplay.warningText = "Can't move on brimstone!";
		moving = true;
		movementPoints = 0;
		if(theTank!=null && canMove && hexFound){
			float dt = Time.deltaTime;
			theTank.transform.Translate(direction.x*dt,0,direction.z*dt,Space.World);
			totalMoved += (Mathf.Abs(direction.x) + Mathf.Abs(direction.z))*dt;

			if(totalMoved >= ((Mathf.Abs(direction.x) + Mathf.Abs(direction.z))*(1-dt))){
				moving = false;
			}
		}
		else
			moving = false;

		//right and left must move total of Mathf.Sqrt(3)
		//other directions must move total of 1.5+Mathf.Sqrt(3)

		if(!moving && theTank != null){
			initialCheck=false;
			totalMoved = 0;
			
			SelectUnit.clickedHex.full = false;
			SelectUnit.clickedHex.clicked = false;
			SelectUnit.clickedHex.Start();
			SelectUnit.clickedHex.parentChunk.Combine();
			
			HexInfo theHex = null;
			foreach(HexChunk chunk in WorldManager.hexChunks)
				foreach(HexInfo hex in chunk.hexArray)
					if(Vector3.Distance(theTank.transform.position,hex.worldPosition)<1f)
						theHex = hex;
			
			Vector3 XactPos = theHex.worldPosition;
			XactPos.y += 0.4f;
			
			theTank.transform.position = XactPos;
												
			SelectUnit.clickedHex = theHex;
			SelectUnit.clickedHex.full = true;
			SelectUnit.clickedHex.clicked = true;
			SelectUnit.clickedHex.Start();
			SelectUnit.clickedHex.parentChunk.Combine();
		}
	}
	
	void rotateBarrel(Vector3 dir){
		if (theTank != null) {
			Transform barrel = theTank.transform.Find ("MainGun");
			float dt = Time.deltaTime * 69f;
			bool canRotate = false;
			
			if (dir == Vector3.right) {
				theTank.GetComponent<Vehicle> ().barrelRotation -= dt;
				if (theTank.GetComponent<Vehicle> ().barrelRotation >= -23f)
					canRotate = true;
				else
					theTank.GetComponent<Vehicle> ().barrelRotation = -23f;
			} else {
				theTank.GetComponent<Vehicle> ().barrelRotation += dt;
				if (theTank.GetComponent<Vehicle> ().barrelRotation <= 110f)
					canRotate = true;
				else
					theTank.GetComponent<Vehicle> ().barrelRotation = 110f;
			}
			
			if (canRotate)
				barrel.Rotate (dir * dt);
		}
	}
}