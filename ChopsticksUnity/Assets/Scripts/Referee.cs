using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using TMPro;

using UnityEngine;

public class Referee : MonoBehaviour
{
    // Current player turn
    [SerializeField]
    public int currentPlayerTurn = 1;

    [SerializeField]
    public string gameState = "idle";

    [Tooltip("Game state label")]
    public TextMeshProUGUI gameStateLabel;

    
    [Tooltip("Game state label")]
    public TextMeshProUGUI prevStateLabel;

    
    [Tooltip("Game state label")]
    public TextMeshProUGUI currentStateLabel;

    [Tooltip("Turn number label")]
    public TextMeshProUGUI currentTurnNumLabel;
    

    private int numTurnsTaken = 0;


    private int[] prevState = new int[4];
    private int[] currentState = new int[4];


    // Reference to the players
    public PlayerAgent player1;
    public PlayerAgent player2;

    private bool waitingforPlayerResponse = false;

    // Start is called before the first frame update
    void Start()
    {

        prevState[0] = 1;
        prevState[1] = 1;
        prevState[2] = 1;
        prevState[3] = 1;

        if (!(player1 == null || player2 == null))
        {
            Debug.Log("Players found");
        }

        // Set the game state
        gameState = "playing";
        gameStateLabel.text = "Playing";
    }

    private void CallPlayer1()
    {
        gameStateLabel.text = "Player 1 turn";
        waitingforPlayerResponse = true;
        player1.RequestDecision();
    }

    private void CallPlayer2()
    {
        gameStateLabel.text = "Player 2 turn";
        waitingforPlayerResponse = true;
        player2.RequestDecision();
    }

    public void PlayerResponse(int p1Left, int p1Right, int p2Left, int p2Right, int playerNum, int moveID)
    {
        Debug.Log("Player" + playerNum + " response: " + p1Left + " " + p1Right + " " + p2Left + " " + p2Right);

        // Update the previous state
        for (int i = 0; i < 4; i++)
        {
            prevState[i] = currentState[i];
        }

        // Update the current state
        currentState[0] = p1Left;
        currentState[1] = p1Right;
        currentState[2] = p2Left;
        currentState[3] = p2Right;

        if (playerNum == 1)
        {
            Player1TurnResponse(p1Left, p1Right, p2Left, p2Right);
        }
        else if (playerNum == 2)
        {
            Player2TurnResponse(p2Left, p2Right, p1Left, p1Right);
        }
        else
        {
            Debug.Log("Invalid player number");
        }

        // Update the previous state label
        prevStateLabel.text = "Previous state: \n" + prevState[0] + " " + prevState[1] + " " + prevState[2] + " " + prevState[3];

        // Update the current state label
        currentStateLabel.text = "Current state: \n" + currentState[0] + " " + currentState[1] + " " + currentState[2] + " " + currentState[3];

        waitingforPlayerResponse = false;
    }

    // Player 1 returns a new game state as an action
    public void Player1TurnResponse(int p1Left, int p1Right, int p2Left, int p2Right)
    {
        // Update the game state
        if (IsGameOver(p1Left, p1Right, p2Left, p2Right))
        {
            Debug.Log("Game over");
            return;
        }

        // Check if a hand was killed
        if (p1Left == 0)
        {
            player1.AddReward(-0.01f);
        }
        if (p1Right == 0)
        {
            player1.AddReward(-0.01f);
        }

        // Check if a opponent hand was killed
        if (p2Left == 0)
        {
            player1.AddReward(0.03f);
        }
        if (p2Right == 0)
        {
            player1.AddReward(0.03f);
        }

        // Update the game state for player 1
        player1.leftNum = p1Left;
        player1.rightNum = p1Right;

        // Update the game state for player 2
        player2.leftNum = p2Left;
        player2.rightNum = p2Right;

        // Change player turn
        currentPlayerTurn = 2;

        // Call player 2
        CallPlayer2();

    }

    // Player 2 returns a new game state as an action
    public void Player2TurnResponse(int p2Left, int p2Right, int p1Left, int p1Right)
    {
        // Update the game state
        if (IsGameOver(p2Left, p2Right, p1Left, p1Right))
        {
            Debug.Log("Game over");
            return;
        }

        // Check if a hand was killed
        if (p2Left == 0)
        {
            player2.AddReward(-0.01f);
        }
        if (p2Right == 0)
        {
            player2.AddReward(-0.01f);
        }

        // Check if a opponent hand was killed
        if (p1Left == 0)
        {
            player2.AddReward(0.03f);
        }
        if (p1Right == 0)
        {
            player2.AddReward(0.03f);
        }

        // Update the game state for player 1
        player1.leftNum = p1Left;
        player1.rightNum = p1Right;

        // Update the game state for player 2
        player2.leftNum = p2Left;
        player2.rightNum = p2Right;

        // Change player turn
        currentPlayerTurn = 1;

        // Call player 2
        CallPlayer2();

    }

    public bool IsGameOver(int p1Left, int p1Right, int p2Left, int p2Right)
    {
        if (p1Left == 0 && p1Right == 0)
        {
            gameState = "player2Win";
            player1.AddReward(-1f);
            player2.AddReward(1f);
        }
        else if (p2Left == 0 && p2Right == 0)
        {
            gameState = "player1Win";
            player2.AddReward(-1f);
            player1.AddReward(1f);
        }

        if (gameState == "player1Win" || gameState == "player2Win")
        {
            player1.EndEpisode();
            player2.EndEpisode();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Update()
    {
        if (gameState == "playing" && !waitingforPlayerResponse)
        {
            numTurnsTaken++;

            currentTurnNumLabel.text = "Turn number: " + numTurnsTaken;

            if (currentPlayerTurn == 1)
            {
                CallPlayer1();
            }
            else 
            {
                CallPlayer2();
            }
        }
    }

}
