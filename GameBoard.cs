using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for populating gameboard
///	A Gameboard array is used to mantain information of all the pellets, super pelllets and intersection nodes 
/// </summary>
public class GameBoard : MonoBehaviour
{

	//Data initialization
	private static int boardWidthX = 200;
	private static int boardHeightZ = 200;
	private static int boardSpaceY = 200;

	public int counter = 0;

	public int totalPellets = 0;
	public int score = 0;
	public GameObject light;
	
	//Gameboard array
	public GameObject[,,] board = new GameObject[boardWidthX,boardSpaceY,boardHeightZ];
	
	// Use this for initialization
	void Start () {
		//Adding light to the sccene
		//When scene in started by script: lights has to be initialized  
		Instantiate(light, light.transform.position, light.transform.rotation);
		
		//Finding all the gameobjects in the scene 
		Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

		//Looping through the gameboard array
		foreach (GameObject o in objects)
		{
			//getting position
			Vector3 pos = o.transform.position;
			/*Debug.Log(o.name);*/
			
			//Getting all the gameobjects that has tags of "nodepellets", "ghosts" and "ghosehome
			if (o.tag == "NodePellet" && o.tag != "Ghost" && o.tag != "ghostHome")
			{
				if (o.GetComponent<Tile>() != null)
				{
					//Getting pellets and superpellets from the gameboard array
					if (o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
					{
						totalPellets++;
					}
				}
				counter++;
				//populating the array
				board[(int) pos.x, (int) pos.y, (int) pos.z] = o;
			}
			/*else
			{
				Debug.Log("Found Pacman at: " + pos);
				
			}*/			
		}
		/*Debug.Log("pellet count : " + counter);*/
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
