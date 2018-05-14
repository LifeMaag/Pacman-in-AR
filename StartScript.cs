sing System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StartScript : MonoBehaviour {

	//Routine for starting the game
	//In case of start button gets clicked this routine gets called
	public void startGame() {
		//Load game scene when start button is clicked
		Application.LoadLevel("Level_1_5_final");
	}
	
}
