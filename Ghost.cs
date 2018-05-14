using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains the code for AI of ghosts
/// </summary>
public class Ghost : MonoBehaviour {

	public float moveSpeed = 0f;
    
    /*public Transform ghostTransform = Transform.*/

    public int pinkyReleaseTimer = 5;
    public int inkyReleaseTimer = 14;
    public int clydeReleaseTimer = 21;
    public float ghostReleaseTimer = 0;

    public bool isInGhostHouse = false;

    public Node startingPosition;
    public Node homeNode;

    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;
    public int chaseModeTimer4 = 20;

    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0;

    // enumeration for Mode
    public enum Mode
    {
        Chase,
        Scatter,
        Frightened
    }
    
    Mode currentMode = Mode.Scatter;
    Mode previousMode;
    
    // enumeration for GhostType
    public enum GhostType
    {
        Red,
        Pink,
        Blue,
        Orange
    }

    
    public GhostType ghostType = GhostType.Red;

    private GameObject pacMan;
    private Node currentNode, targetNode, previousNode;
    private Vector3 direction, nextDirection;
    // Use this for initialization

    void Start () {
        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        Node node = GetNodeAtPosition(transform.position);

        if(node != null)
        {
            currentNode = node;
        }

        if(isInGhostHouse)
        {
            direction = Vector3.forward;
            targetNode = currentNode.neighbors[0];
        } else
        {
            direction = Vector3.left;
            targetNode = ChooseNextNode();
        }
        
        /*direction = Vector3.right;*/

        previousNode = currentNode;

        /*Vector3 pacmanPosition = pacMan.transform.position;
        Vector3 targetTile = new Vector3(Mathf.RoundToInt(pacmanPosition.x), Mathf.RoundToInt(pacmanPosition.y)
                                         , Mathf.RoundToInt(pacmanPosition.z));
        targetNode = GetNodeAtPosition(targetTile);*/

    }
	
	// Update is called once per frame
	void Update () {
        ModeUpdate();
        Move();
        ReleaseGhosts();
	}

    // method for movement of ghosts
    void Move()
    {
        /*moveSpeed = 19f;*/
        if(targetNode != currentNode && targetNode != null && !isInGhostHouse)
        {
            if(OverShotTarget())
            {
                currentNode = targetNode;
                transform.position = currentNode.transform.position;
                /*GameObject otherPortal = GetPortal(currentNode.transform.position);
                if(otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal.GetComponent<Node>();
                }*/
                targetNode = ChooseNextNode();
                previousNode = currentNode;
                currentNode = null;
            } else
            {
                /*Debug.Log("Running");*/
                transform.position += (direction * moveSpeed) * Time.deltaTime;
               /* Debug.Log("direction: " + direction);
                Debug.Log("move speed: " + moveSpeed);
                Debug.Log("time: " + Time.deltaTime);*/
            }
        }
    }

    // method for updating the mode of ghost movement 
    void ModeUpdate()
    {
        if(currentMode != Mode.Frightened)
        {
            modeChangeTimer += Time.deltaTime;
            // comparison for first iteration of mode update
            if(modeChangeIteration == 1)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                {
                    // change mode to Chase mode
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)
                {
                    modeChangeIteration = 2;
                    // change mode to Scatter mode
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            // comparison for second iteration of mode update
            else if(modeChangeIteration == 2)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    // change mode to Chase mode
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    // change mode to Scatter mode
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            // comparison for third iteration of mode update
            else if (modeChangeIteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    // change mode to Chase mode
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    modeChangeTimer = 0;
                } 

            }else if (modeChangeIteration == 4)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                
            }
        }
        else if(currentMode == Mode.Frightened)
        {
            // implementation for frightened mode 

        }
    }
    // method for changing the mode
    void ChangeMode (Mode m)
    {
        currentMode = m;
    }

    // method for target tile of Red ghost
    Vector3 GetRedGhostTargetTile()
    {
        Vector3 pacManPosition = pacMan.transform.position;
        Vector3 targetTile = new Vector3(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y)
        ,Mathf.RoundToInt(pacManPosition.z));
        return targetTile;

    }
    // method for target tile of Pink ghost
    Vector3 GetPinkGhostTargetTile()
    {
        //- four tiles ahead Pac-Man
        //- taking account Position and Orientation
        Vector3 pacManPosition = pacMan.transform.position;
        Vector3 pacManOrientation = pacMan.GetComponent<movement>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);
        int pacManPositionZ = Mathf.RoundToInt(pacManPosition.z);

        Vector3 pacManTile = new Vector3(pacManPositionX, pacManPositionY, pacManPositionZ);
        Vector3 targetTile = pacManTile + (4 * pacManOrientation);

        return targetTile;

    }

    // method for target tile of Blue ghost
    /*Vector3 GetBlueGhostTargetTile()
    {
        Vector3 pacManPosition = pacMan.transform.position;
        Vector3 pacManOrientation = pacMan.GetComponent<movement>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);
        int pacManPositionZ = Mathf.RoundToInt(pacManPosition.z);

        Vector3 pacManTile = new Vector3(pacManPositionX, pacManPositionY, pacManPositionZ);
        Vector3 targetTile = pacManTile + (2 * pacManOrientation);
        
        Vector3 tempBlinkyPosition = GameObject.Find("Ghost").transform.position;
        int blinkyPositionX = Mathf.RoundToInt(tempBlinkyPosition.x);
        int blinkyPositionY = Mathf.RoundToInt(tempBlinkyPosition.y);
        int blinkyPositionZ = Mathf.RoundToInt(tempBlinkyPosition.z);
        
        tempBlinkyPosition = new Vector3(blinkyPositionX, blinkyPositionY, blinkyPositionZ);

        float distance = GetDistance(tempBlinkyPosition, targetTile);
        distance *= 2;

        targetTile = new Vector3(tempBlinkyPosition.x + distance, tempBlinkyPosition.y + distance, tempBlinkyPosition.z + distance);

        return targetTile;
        return Vector3.zero;
    }*/
    // method for target tile of Orange ghost
    /*Vector3 GetOrangeGhostTargetTile()
    {
        Vector3 pacManPosition = pacMan.transform.position;

        float distance = GetDistance(transform.position, pacManPosition);
        Vector3 targetTile = Vector3.zero;

        if(distance>8)
        {
            targetTile = new Vector3(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y)
                                     ,Mathf.RoundToInt(pacManPosition.z));
        } else if(distance<8)
        {
            targetTile = homeNode.transform.position;
        }
        return targetTile;
        /*return Vector3.zero;#1#
    }*/
    // method for getting the target tile 
    Vector3 GetTargetTile()
    {
        Vector3 targetTile = Vector3.zero;
        if(ghostType == GhostType.Red)
        {
            targetTile = GetRedGhostTargetTile();
        } else if(ghostType == GhostType.Pink)
        {
            targetTile = GetPinkGhostTargetTile();
        }
        /*else if (ghostType == GhostType.Blue)
        {
            targetTile = GetBlueGhostTargetTile();
        }*/
        /*else if (ghostType == GhostType.Orange)
        {
            targetTile = GetOrangeGhostTargetTile();
        }*/
        return targetTile;
    }

    // method for the release of Pink ghost from ghost house
    void ReleasePinkGhost()
    {
        if(ghostType == GhostType.Pink && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }
    // method for the release of Blue ghost from ghost house
    void ReleaseBlueGhost()
    {
        if (ghostType == GhostType.Blue && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }
    // method for the release of Orange ghost from ghost house
    void ReleaseOrangeGhost()
    {
        if (ghostType == GhostType.Orange && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }
    // method for release of ghosts from ghost house
    void ReleaseGhosts()
    {
        ghostReleaseTimer += Time.deltaTime;

        if(ghostReleaseTimer > pinkyReleaseTimer)
        {
            ReleasePinkGhost();
        }
        if (ghostReleaseTimer > inkyReleaseTimer)
        {
            ReleaseBlueGhost();
        }
        if (ghostReleaseTimer > clydeReleaseTimer)
        {
            ReleaseOrangeGhost();
        }
    }


    // method for choosing the next node to move the ghost
    Node ChooseNextNode()
    {
        Vector3 targetTile = Vector3.zero;

        /*Vector3 pacmanPosition = pacMan.transform.position;
        targetTile = new Vector3(Mathf.RoundToInt(pacmanPosition.x), Mathf.RoundToInt(pacmanPosition.y)
                                 ,Mathf.RoundToInt(pacmanPosition.z));*/

        targetTile = GetTargetTile();
        
        if(currentMode == Mode.Chase)
        {
            targetTile = GetTargetTile();
        } else if(currentMode == Mode.Scatter)
        {
            targetTile = homeNode.transform.position;
        }

        Node movetoNode = null;

        // array to hold the nodes that can be four
        // left, right, up, down
        Node[] foundNodes = new Node[4];
        // array to hold the directions that can be four
        Vector3[] foundNodesDirection = new Vector3[4];

        int nodeCounter = 0;
        for(int i=0; i<currentNode.neighbors.Length; i++)
        {
            if(currentNode.validDirections[i] != direction * -1)
            {
                foundNodes[nodeCounter] = currentNode.neighbors[i];
                foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                nodeCounter++;
            }
        }

        if(foundNodes.Length == 1)
        {
            movetoNode = foundNodes[0];
            direction = foundNodesDirection[0];
        }
        if(foundNodes.Length > 1)
        {
            float leastDistance = 100000f;
            for(int i=0; i<foundNodes.Length; i++)
            {
                if(foundNodesDirection[i] != Vector3.zero)
                {
                    float distance = GetDistance(foundNodes[i].transform.position, targetTile);
                    if(distance < leastDistance)
                    {
                        leastDistance = distance;
                        movetoNode = foundNodes[i];
                        direction = foundNodesDirection[i];
                    }
                }
            }
        }
        return movetoNode;
    }


    // methd to get the node at specific position, passed as argument
    Node GetNodeAtPosition (Vector3 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y, (int)pos.z];
        if(tile != null)
        {
            if (tile.GetComponent<Node>() != null)
            {
                return tile.GetComponent<Node>();
            }
        }
        return null;
    }
    /*GameObject GetPortal(Vector3 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        if(tile != null)
        {
            if(tile.GetComponent<Tile>().isPortal)
            {
                GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                return otherPortal;
            }
        }
        return null;
    }*/

    // method to return the length from a node to target
    float LengthFromNode(Vector3 targetPosition)
    {
        Vector3 vec = targetPosition - (Vector3)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.position);

        return nodeToSelf > nodeToTarget;
    }

    // method to return the distance between two points, passed as arguments
    float GetDistance(Vector3 posA, Vector3 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;
        float dz = posA.z - posB.z;

        float distance = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);

        return distance;
    }
}
