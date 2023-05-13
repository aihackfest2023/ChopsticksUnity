using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using TMPro;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// A player agent for chopsticks
/// </summary>
public class PlayerAgent : Agent
{
    [Tooltip("Player number")]
    public int playerNum = 1;

    [Tooltip("Num fingers left hand")]
    public int leftNum = 1;

    [Tooltip("Num fingers right hand")]
    public int rightNum = 1;

    [Tooltip("Label for the left hand")]
    public TextMeshProUGUI leftLabel;

    [Tooltip("Label for the right hand")]
    public TextMeshProUGUI rightLabel;

    // Refernce to the other player, shown in the inspector
    [SerializeField]
    private PlayerAgent otherPlayer;

    [SerializeField]
    private Referee referee;

    private bool foundOtherObjects = false;

    /// <summary>
    /// Initialize the agent
    /// </summary>
    public override void Initialize()
    {
        // base.Initialize();

        // Set the initial values 
        leftNum = 1;
        rightNum = 1;

        if (leftLabel == null || rightLabel == null)
        {
            Debug.LogError("Could not find left or right label");
        }
        else
        {
            // Set the labels to the initial values
            leftLabel.text = leftNum.ToString();
            rightLabel.text = rightNum.ToString();
        }

        // TODO: Remove this later
        // Test getting the other player's numbers
        Debug.Log("Other player left num: " + otherPlayer.GetLeftNum());
        Debug.Log("Other player right num: " + otherPlayer.GetRightNum());

        if (otherPlayer != null && leftLabel != null && rightLabel != null && referee != null)
        {
            foundOtherObjects = true;
        }
        else
        {
            Debug.LogError("Could not find other player, left label, right label, or referee");
        }

    }

    /// <summary>
    /// An agent is reset when an episode begins
    /// </summary>
    public override void OnEpisodeBegin()
    {
        // base.OnEpisodeBegin();

        // Reset the left and right hand numbers to 1
        leftNum = 1;
        rightNum = 1;

        // Set the labels to the initial values
        leftLabel.text = leftNum.ToString();
        rightLabel.text = rightNum.ToString();

        // Call the referee to reset the game
        if (playerNum == 1)
        {
            referee.player1Ready = true;
        } else
        {
            referee.player2Ready = true;
        }
    }

    /// <summary>
    /// Called when an action is recieved from either the player or the neural network
    /// </summary>
    /// <param name="actions">The actions to take</param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        // base.OnActionReceived(actions);

        // ActionSegment<float> vectorAction = actions.ContinuousActions;
        ActionSegment<int> vectorAction = actions.DiscreteActions;

        // Possible actions: attack (4 options), split (6 options, some not feasible) or self-add (2 options)
        // Attack: 0, 1, 2, 3
        // Split: 4, 5, 6, 7, 8, 9
        // Self-add: 10, 11

        // Attack
        if (vectorAction[0] < 4)
        {
            // Attack the other player's hand
            // 0: 1L - 2L
            // 1: 1L - 2R
            // 2: 1R - 2L
            // 3: 1R = 2R

            if (vectorAction[0] == 0)
            {
                int updatedOtherLeft = otherPlayer.GetLeftNum() + leftNum;
                if (updatedOtherLeft >= 5)
                {
                    updatedOtherLeft = 0;
                }

                referee.PlayerResponse(leftNum, rightNum, updatedOtherLeft, otherPlayer.GetRightNum(), playerNum, 0);
            }
            else if (vectorAction[0] == 1)
            {
                int updatedOtherRight = otherPlayer.GetRightNum() + leftNum;
                if (updatedOtherRight >= 5)
                {
                    updatedOtherRight = 0;
                }
                referee.PlayerResponse(leftNum, rightNum, otherPlayer.GetLeftNum(), updatedOtherRight, playerNum, 1);
            }
            else if (vectorAction[0] == 2)
            {
                int updatedOtherLeft = otherPlayer.GetLeftNum() + rightNum;
                if (updatedOtherLeft >= 5)
                {
                    updatedOtherLeft = 0;
                }
                referee.PlayerResponse(leftNum, rightNum, updatedOtherLeft, otherPlayer.GetRightNum(), playerNum, 2);
            }
            else if (vectorAction[0] == 3)
            {
                int updatedOtherRight = otherPlayer.GetRightNum() + rightNum;
                if (updatedOtherRight >= 5)
                {
                    updatedOtherRight = 0;
                }
                referee.PlayerResponse(leftNum, rightNum, otherPlayer.GetLeftNum(), updatedOtherRight, playerNum, 3);
            }

        }

        // Split
        else if (vectorAction[0] < 10)
        {
            // Split the hand
            // 4-6: move 1-3 from L-R (3*1 = 3 moves)
            // 7-9: move 1-3 from R-L (3*1 = 3 moves)

            if (vectorAction[0] == 4)
            {
                referee.PlayerResponse(leftNum - 1, rightNum + 1, otherPlayer.GetLeftNum(), otherPlayer.GetRightNum(), playerNum, 4);
            }
            else if (vectorAction[0] == 5)
            {
                referee.PlayerResponse(leftNum - 2, rightNum + 2, otherPlayer.GetLeftNum(), otherPlayer.GetRightNum(), playerNum, 5);
            }
            else if (vectorAction[0] == 6)
            {
                referee.PlayerResponse(leftNum - 3, rightNum + 3, otherPlayer.GetLeftNum(), otherPlayer.GetRightNum(), playerNum, 6);
            }
            else if (vectorAction[0] == 7)
            {
                referee.PlayerResponse(leftNum + 1, rightNum - 1, otherPlayer.GetLeftNum(), otherPlayer.GetRightNum(), playerNum, 7);
            }
            else if (vectorAction[0] == 8)
            {
                referee.PlayerResponse(leftNum + 2, rightNum - 2, otherPlayer.GetLeftNum(), otherPlayer.GetRightNum(), playerNum, 8);
            }
            else if (vectorAction[0] == 9)
            {
                referee.PlayerResponse(leftNum + 3, rightNum - 3, otherPlayer.GetLeftNum(), otherPlayer.GetRightNum(), playerNum, 9);
            }
        }

        // Self-add
        else
        {
            // Add to the hand
            // 10: 1L - 1R
            // 11: 1R - 1L

            if (vectorAction[0] == 10)
            {
                int updatedRight = leftNum + rightNum;
                if (updatedRight >= 5)
                {
                    updatedRight = 0;
                }
                referee.PlayerResponse(leftNum, updatedRight, otherPlayer.GetLeftNum(), otherPlayer.GetRightNum(), playerNum, 10);
            }
            else if (vectorAction[0] == 11)
            {
                int updatedLeft = leftNum + rightNum;
                if (updatedLeft >= 5)
                {
                    updatedLeft = 0;
                }
                referee.PlayerResponse(updatedLeft, rightNum, otherPlayer.GetLeftNum(), otherPlayer.GetRightNum(), playerNum, 11);
            }

        }

    }

    /// <summary>
    /// Collect vector observations from the environment
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // base.CollectObservations(sensor);

        if (!foundOtherObjects)
        {
            Debug.LogError("Could not find other objects");
            sensor.AddObservation(new float[4]);
            return;
        }


        // Add the left and right hand numbers to the observations
        sensor.AddObservation(leftNum / 4);
        sensor.AddObservation(rightNum / 4);

        // Add the other player's left and right hand numbers to the observations
        sensor.AddObservation(otherPlayer.GetLeftNum() / 4);
        sensor.AddObservation(otherPlayer.GetRightNum() / 4);

        // 4 total observations
    }

    // Masking the actions
    // https://unity-technologies.github.io/ml-agents/Learning-Environment-Design-Agents/#decisions:~:text=300f%2C%20directionZ%20*%2040f))%3B-,Masking%20Discrete%20Actions,-When%20using%20Discrete

    /// <summary>
    /// Mask illegal actions
    /// </summary>
    /// <param name="actionMask">The action mask</param>
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {

        Debug.Log("Writing action mask for player " + playerNum);
        // base.WriteDiscreteActionMask(actionMask);

        // Format: actionMask.SetActionEnabled(branch, actionIndex, isEnabled);

        bool[] actionsEnabledArr = new bool[12];

        for (int i = 0; i < 12; i++)
        {
            actionsEnabledArr[i] = true;
        }

        // set all actions to enabled first
        for (int i = 0; i < 12; i++)
        {
            // NOTE: if cannot, replace with temp array
            actionMask.SetActionEnabled(0, i, true);
        }

        // Get the other player's numbers
        int otherLeftNum = otherPlayer.GetLeftNum();
        int otherRightNum = otherPlayer.GetRightNum();

        // Disable actions that are not possible
        if (leftNum == 0)
        {
            // Can't attack
            actionsEnabledArr[0] = false;
            actionsEnabledArr[1] = false;
            // actionMask.SetActionEnabled(0, 0, false);
            // actionMask.SetActionEnabled(0, 1, false);

            // Can't split L-R
            actionsEnabledArr[4] = false;
            actionsEnabledArr[5] = false;
            actionsEnabledArr[6] = false;
            // actionMask.SetActionEnabled(0, 4, false);
            // actionMask.SetActionEnabled(0, 5, false);
            // actionMask.SetActionEnabled(0, 6, false);

            // Can't self-add 1L - 1R
            actionsEnabledArr[10] = false;
            // actionMask.SetActionEnabled(0, 10, false);
        }
        else
        {
            if (otherPlayer.GetLeftNum() == 0)
            {
                // Can't attack left hand
                actionsEnabledArr[0] = false;
                // actionMask.SetActionEnabled(0, 0, false);
            }
            if (otherPlayer.GetRightNum() == 0)
            {
                // Can't attack right hand
                actionsEnabledArr[1] = false;
                // actionMask.SetActionEnabled(0, 1, false);
            }
            if (rightNum == 0) {
                // Can't attack right self
                actionsEnabledArr[10] = false;
            }
        }

        if (rightNum == 0)
        {
            // Can't attack
            actionsEnabledArr[2] = false;
            actionsEnabledArr[3] = false;
            // actionMask.SetActionEnabled(0, 2, false);
            // actionMask.SetActionEnabled(0, 3, false);

            // Can't split R-L
            actionsEnabledArr[7] = false;
            actionsEnabledArr[8] = false;
            actionsEnabledArr[9] = false;
            // actionMask.SetActionEnabled(0, 7, false);
            // actionMask.SetActionEnabled(0, 8, false);
            // actionMask.SetActionEnabled(0, 9, false);

            // Can't self-add 1R - 1L
            actionsEnabledArr[11] = false;
            // actionMask.SetActionEnabled(0, 11, false);
        }
        else
        {
            if (otherPlayer.GetLeftNum() == 0)
            {
                // Can't attack left hand
                actionsEnabledArr[2] = false;
                // actionMask.SetActionEnabled(0, 2, false);
            }
            if (otherPlayer.GetRightNum() == 0)
            {
                // Can't attack right hand
                actionsEnabledArr[3] = false;
                // actionMask.SetActionEnabled(0, 3, false);
            }
            if (leftNum == 0) {
                // Can't attack left self
                actionsEnabledArr[11] = false;
            }
        }

        // If the result of a split has either side as 5 or 0, it is not allowed

        // If the result of a split is flipped L-R values, disable it
        if (leftNum - 1 == rightNum && rightNum + 1 == leftNum)
        {
            // Can't split L-R 1
            actionsEnabledArr[4] = false;
            // actionMask.SetActionEnabled(0, 4, false);
        }
        else if (leftNum - 1 <= 0 || rightNum + 1 >= 5)
        {
            // Can't split L-R 1
            actionsEnabledArr[4] = false;
            // actionMask.SetActionEnabled(0, 4, false);
        }

        if (leftNum - 2 == rightNum && rightNum + 2 == leftNum)
        {
            // Can't split L-R 2
            actionsEnabledArr[5] = false;
            // actionMask.SetActionEnabled(0, 5, false);
        }
        else if (leftNum - 2 <= 0 || rightNum + 2 >= 5)
        {
            // Can't split L-R 2
            actionsEnabledArr[5] = false;
            // actionMask.SetActionEnabled(0, 5, false);
        }

        if (leftNum - 3 == rightNum && rightNum + 3 == leftNum)
        {
            // Can't split L-R 3
            actionsEnabledArr[6] = false;
            // actionMask.SetActionEnabled(0, 6, false);
        }
        else if (leftNum - 3 <= 0 || rightNum + 3 >= 5)
        {
            // Can't split L-R 3
            actionsEnabledArr[6] = false;
            // actionMask.SetActionEnabled(0, 6, false);
        }

        // If the result of a split is flipped R-L values, disable it
        if (rightNum - 1 == leftNum && leftNum + 1 == rightNum)
        {
            // Can't split R-L 1
            actionsEnabledArr[7] = false;
            // actionMask.SetActionEnabled(0, 7, false);
        }
        else if (rightNum - 1 <= 0 || leftNum + 1 >= 5)
        {
            // Can't split R-L 1
            actionsEnabledArr[7] = false;
            // actionMask.SetActionEnabled(0, 7, false);
        }

        if (rightNum - 2 == leftNum && leftNum + 2 == rightNum)
        {
            // Can't split R-L 2
            actionsEnabledArr[8] = false;
            // actionMask.SetActionEnabled(0, 8, false);
        }
        else if (rightNum - 2 <= 0 || leftNum + 2 >= 5)
        {
            // Can't split R-L 2
            actionsEnabledArr[8] = false;
            // actionMask.SetActionEnabled(0, 8, false);
        }

        if (rightNum - 3 == leftNum && leftNum + 3 == rightNum)
        {
            // Can't split R-L 3
            actionsEnabledArr[9] = false;
            // actionMask.SetActionEnabled(0, 9, false);
        }
        else if (rightNum - 3 <= 0 || leftNum + 3 >= 5)
        {
            // Can't split R-L 3
            actionsEnabledArr[9] = false;
            // actionMask.SetActionEnabled(0, 9, false);
        }

        for (int i = 0; i < 12; i++)
        {
            if (actionsEnabledArr[i] == false)
            {
                actionMask.SetActionEnabled(0, i, false);
            }
            else if (actionsEnabledArr[i] == true)
            {
                actionMask.SetActionEnabled(0, i, true);
            }

        }

    }

    /// <summary>
    /// When behaviour type is set to "Heuristic Only" on the agent's Behaviour Parameters,
    /// this function will be called. Its return values will be fed into
    /// <see cref="Heuristic(in ActionBuffers)"/> instead of using the neural network 
    /// </summary>
    /// <param name = "actionsOut">An output action buffer</param>
    public override void Heuristic(in ActionBuffers actionsOutBuffer)
    {
        // base.Heuristic(actionsOut);

        // Get the action buffer
        ActionSegment<int> actionsOut = actionsOutBuffer.DiscreteActions;

        // user press enter first
        while (true)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                break;
            }
        }

        // Wait for input in a while loop
        while (true)
        {
            if (Input.GetKey(KeyCode.Alpha0))
            {
                actionsOut[0] = 0;
                break;
            }
            else if (Input.GetKey(KeyCode.Alpha1))
            {
                actionsOut[0] = 1;
                break;
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                actionsOut[0] = 2;
                break;
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                actionsOut[0] = 3;
                break;
            }
            else if (Input.GetKey(KeyCode.Alpha4))
            {
                actionsOut[0] = 4;
                break;
            }
            else if (Input.GetKey(KeyCode.Alpha5))
            {
                actionsOut[0] = 5;
                break;
            }
            else if (Input.GetKey(KeyCode.Alpha6))
            {
                actionsOut[0] = 6;
                break;
            }
            else if (Input.GetKey(KeyCode.Alpha7))
            {
                actionsOut[0] = 7;
                break;
            }
            else if (Input.GetKey(KeyCode.Alpha8))
            {
                actionsOut[0] = 8;
                break;
            }
            else if (Input.GetKey(KeyCode.Alpha9))
            {
                actionsOut[0] = 9;
                break;
            }
            else if (Input.GetKey(KeyCode.Plus))
            {
                actionsOut[0] = 10;
                break;
            }
            else if (Input.GetKey(KeyCode.Minus))
            {
                actionsOut[0] = 11;
                break;
            }
        }



    }

    /// <summary>
    /// Function which returns the Agent current left hand num
    /// </summary>
    /// <returns>Left hand num</returns>
    public int GetLeftNum()
    {
        return leftNum;
    }

    /// <summary>
    /// Function which returns the Agent current right hand num
    /// </summary>
    /// <returns>Right hand num</returns>
    public int GetRightNum()
    {
        return rightNum;
    }



}
