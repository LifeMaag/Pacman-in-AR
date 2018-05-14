using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for pacman's gameplay 
/// </summary>
public class movement : MonoBehaviour
{
	//Data initialization
	
	public Vector3 orientation;
	public AudioClip chomp1;
	public AudioClip chomp2;  
	
	private float speed = 30f;

	public Transform pacmanTransform;
	public Vector3 direction = Vector3.zero;
	public Vector3 nextDirection;

	private int pelletsConsumed = 0;
	private bool soundChomp1 = false;
	private AudioSource audio;

	private Node currentNode, targetNode, previousNode;
	public Text scoreText;
	private GameObject[] ghostss;
	
	//Initialization routine
	void Start ()
	{
		//Adding sounds
		audio = transform.GetComponent<AudioSource>();
		ghostss = GameObject.FindGameObjectsWithTag("Ghost");
		pacmanTransform = transform;

		// getting pacman's 0 minute position
		Node node = GetNodeAtPosition(transform.position);
		/*Debug.Log(transform.position);*/
		
		
		if (node!= null) {
			currentNode = node;
			/*Debug.Log("Current Node: " + currentNode);*/
			
		}
		
		// setting pacman's 0 minute movement (to the left)
		direction = Vector3.left;
		orientation = Vector3.left;
		
		//setting pacman's 0 minute rotation
		updateOrientation();
		
		//making move
		pacmanTransform.Translate(0,0,-speed * Time.deltaTime);
		ChangePosition(direction);
		Debug.Log("total pellets: " + GameObject.Find("Game").GetComponent<GameBoard>().totalPellets);
	}
	
	//Routine for setting position for pacman with every frame
	void ChangePosition(Vector3 d)
	{
		if (d != direction)
		{
			nextDirection = d;
		}

		if (currentNode != null)
		{
			Node moveToNode = CanMove(d);
			
			if (moveToNode != null)
			{
				direction = d;
				targetNode = moveToNode;
				previousNode = currentNode;
				currentNode = null;
			}
		}	
	}
	
	//Routine for setting rotation of pacman with every frame on based on input
	void updateOrientation() {
		//Case: Pacman moving to left  
		if (direction == Vector3.left) {
			orientation = Vector3.left;
			transform.localRotation = Quaternion.Euler(0, 270, 0);
			/*Debug.Log("left");*/
		}
		//Case: Pacman moving to right
		if (direction == Vector3.right) {
			orientation = Vector3.right;
			transform.localRotation = Quaternion.Euler(0, 90, 0);
			/*Debug.Log("right");*/
		}
		//Case: Pacman moving to top
		if (direction == Vector3.forward) {
			orientation = Vector3.forward;
			transform.localRotation = Quaternion.Euler(0, 0, 0);
			/*Debug.Log("forward");*/
		}
		//Case: Pacman moving to bottom
		if (direction == Vector3.back) {
			orientation = Vector3.back;
			transform.localRotation = Quaternion.Euler(0, 180, 0);
			/*Debug.Log("backword");*/
		}
	}

	//Toggeling animation on and off based of movement state of pacman 
	void UpdateAnimationState()
	{
		//When pacman isn't moving: incase of headfirst collision with wall
		if (direction == Vector3.zero) {
			GetComponent<Animator>().enabled = false;
		}
		//When pacman starts moving again
		else {
			GetComponent<Animator>().enabled = true;
		}
	}

	//Routine for consuming pellets, calculating scores and displaying score
	void ConsumePellet() {
		GameObject o = GetTilePosition(transform.position);

		if (o != null) {
			Tile tile = o.GetComponent<Tile>();

			if (tile != null) {
				if (!tile.didConsume && (tile.isPellet || tile.isSuperPellet)) {
					o.GetComponent<Renderer>().enabled = false;
					tile.didConsume = true;
					
					//Displaying current score
					GameObject.Find("Game").GetComponent<GameBoard>().score += 1;
					pelletsConsumed++;
					PlayChompSound();
				}
			}
		}
	}
	
	//Making move routine
	void MovePacman() {
		if (targetNode != currentNode && targetNode != null) {
			if (nextDirection == (direction * -1)) {
				direction *= -1;

				Node temp = targetNode;
				targetNode = previousNode;
				previousNode = temp;
			}
			if (OverShotTarget()) {
				currentNode = targetNode;
				transform.position = currentNode.transform.position;
				Node moveToNode = CanMove(nextDirection);

				if (moveToNode != null) {
					direction = nextDirection;
					updateOrientation();
				}

				if (moveToNode == null) {
					moveToNode = CanMove(direction);
				}

				if (moveToNode != null) {
					targetNode = moveToNode;
					previousNode = currentNode;
					currentNode = null;
					updateOrientation();
				}
				else {
					direction = Vector3.zero;
					/*Debug.Log("target node : " + targetNode);
					Debug.Log("current node : " + currentNode);
					Debug.Log("previous node : " + previousNode);
					Debug.Log("direction : " + direction);
					Debug.Log("next direction : " + nextDirection);*/
				}
			}
			else {
				updateOrientation();
				transform.position += (direction * speed) * Time.deltaTime;
			}
		}
	}
	
	//Routines for Handleing inputs
	//Input: Up
	public void inputUp() {
		nextDirection = Vector3.forward;
		ChangePosition(nextDirection);
		/*inputDown();*/
		MovePacman();
	}
	//Input: Down
	public void inputDown() {
		nextDirection = Vector3.back;
		ChangePosition(nextDirection);
		/*inputDown();*/
		MovePacman();
	}
	//Input: Left
	public void inputLeft() {
		nextDirection = Vector3.left;
		ChangePosition(nextDirection);
		/*inputDown();*/
		MovePacman();
	}
	//Input: Right
	public void inputRight() {
		nextDirection = Vector3.right;
		ChangePosition(nextDirection);
		/*inputDown();*/
		MovePacman();
	}
	
	// Update is called once per frame
	void Update () {
		
		//Debug cases: for keyboard inputs
		if(Input.GetKeyDown("up"))
		{
			inputUp();
		}
		if(Input.GetKeyDown("down"))
		{
			inputDown();
		}
		if(Input.GetKeyDown("left"))
		{
			inputLeft();
		}
		if(Input.GetKeyDown("right"))
		{
			inputRight();
		}
		
		//Calling makeing move routine
		MovePacman();
		
		//Calling toggeling animations routine
		UpdateAnimationState();
		
		//Calling consuming pellets routine
		ConsumePellet();
		
		//Updating score by every frame
		scoreText.text = "Scores: " + pelletsConsumed;

		//Win condition for the game
		if (pelletsConsumed == 150) {
			Debug.Log("win");
			//Changing scene to win scene 
			Application.LoadLevel("end_win");
		}
		
		/*if (ghostss[0].transform.position == transform.position || ghostss[1].transform.position == transform.position)*/
		
		//Lose condition for the game 
		//If pacman comes in contact with any of ghosts
		if(Vector3.Distance(ghostss[0].transform.position, transform.position) < 5 || Vector3.Distance(ghostss[1].transform.position, transform.position) < 5)
		{
			Debug.Log("caught");
			//Changing scene to lose scene 
			Application.LoadLevel("end_lose");
		}

	}

	//Controlling sounds routine
	void PlayChompSound()
	{
		if (soundChomp1)
		{
			audio.PlayOneShot(chomp2);
			soundChomp1 = false;
		}
		else
		{
			audio.PlayOneShot(chomp1);
			soundChomp1 = true;
		}
		
	}

	//Test routine (not important) 
	void MoveToNode(Vector3 d)
	{
		/*Debug.Log(d);*/
		Node moveToNode = CanMove(d);
		
		if (moveToNode != null)
		{
			transform.position = moveToNode.transform.position;
			currentNode = moveToNode;
			/*Debug.Log(d);*/
		}
	}

	//Checking if move is possible (if there is no wall between pacman's current position and targeted position
	Node CanMove(Vector3 d)
	{
		/*Debug.Log(d);*/
		Node moveToNode = null;

		//Finding neighbouring nodes of pacman for calibrating possible moves from current node 
		for (int i = 0; i < currentNode.neighbors.Length; i++)
		{
			//Storing neighbours
			if (currentNode.validDirections[i] == d)
			{
				moveToNode = currentNode.neighbors[i];
				break;
			}
		}
		// tergeted node
		return moveToNode;
	}

	//Locating current node in backend array
	GameObject GetTilePosition(Vector3 pos)
	{
		int tileX = Mathf.RoundToInt(pos.x);
		int tileY = Mathf.RoundToInt(pos.y);
		int tileZ = Mathf.RoundToInt(pos.z);
		
		GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[tileX,tileY,tileZ];

		if (tile != null)
		{
			return tile;
		}

		return null;
	}
	//Locating node on specific position
	Node GetNodeAtPosition(Vector3 pos)
	{
		
		GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y, (int)pos.z];
		
		if (tile != null)
		{
			/*Debug.Log(tile.name);*/
			return tile.GetComponent<Node>();
		}

		return null;
	}

	//Overshoting movement
	//Continueing movement to the next node in same direction in case no direction change input  
	bool OverShotTarget()
	{
		int nodeToTarget = (int)LengthFromNode(targetNode.transform.position);
		int nodeToSelf = (int)LengthFromNode(transform.position);
		/*Debug.Log("node to target : " + nodeToTarget);
		Debug.Log("node to self : " + nodeToSelf);*/
		return nodeToSelf > nodeToTarget;
	}
	
	float LengthFromNode(Vector3 targetPosition)
	{
		Vector3 vec = targetPosition - previousNode.transform.position;
		return vec.sqrMagnitude;
	}
}
