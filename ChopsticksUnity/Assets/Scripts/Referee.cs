using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    // Current player turn
    [SerializeField]
    public int currentPlayerTurn = 1;

    [SerializeField]
    public string gameState = "idle";

    // Reference to the players
    public PlayerAgent player1;
    public PlayerAgent player2;

    // Start is called before the first frame update
    void Start()
    {
        // Find player 1 and 2
        PlayerAgent[] playersInGameArena = null;

        playersInGameArena = transform.GetComponentsInChildren<PlayerAgent>();

        // if len not 2 search again
        if (playersInGameArena.Length != 2)
        {
            playersInGameArena = null;
        }

        foreach (PlayerAgent player in playersInGameArena)
        {
            if (player.playerNum == 1)
            {
                player1 = player;
            }
            else if (player.playerNum == 2)
            {
                player2 = player;
            }
        }


        if (player1 == null || player2 == null)
        {
            playersInGameArena = null;
        }
        else
        {
            Debug.Log("Players found");
        }



        // Set the game state
        gameState = "playing";

        // Call player 1
        CallPlayer1();

    }

    private void CallPlayer1()
    {
        player1.RequestDecision();
    }

    private void CallPlayer2()
    {
        player2.RequestDecision();
    }

    public void PlayerResponse(int p1Left, int p1Right, int p2Left, int p2Right, int playerNum)
    {
        if (playerNum == 1)
        {
            Player1TurnResponse(p1Left, p1Right, p2Left, p2Right);
        }
        else if (playerNum == 2)
        {
            Player2TurnResponse(p2Left, p2Right, p1Left, p1Right);
        }
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
            return true;
        }
        else
        {
            return false;
        }
    }

}
