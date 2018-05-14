using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

	public Node[] neighbors;
	public Vector3[] validDirections;
	 
	// Use this for initialization
	void Start () {
		
		validDirections = new Vector3[neighbors.Length];

		for (int i = 0; i < neighbors.Length; i++)
		{
			Node neighbor = neighbors[i];
			Vector3 tempVector = neighbor.transform.position - transform.position;

			validDirections[i] = tempVector.normalized;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
