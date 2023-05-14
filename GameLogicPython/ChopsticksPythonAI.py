# Python implementation of the chopsticks game logic

# imports
import onnx
import onnxruntime as ort
import numpy as np

# CONSTANTS
# The maximum number of fingers a player can have on one hand
MAX_FINGERS = 5
MIN_FINGERS = 0

MOVE_TYPES = {
    0: "1L - 2L",
    1: "1L - 2R",
    2: "1R - 2L",
    3: "1R - 2R",
    4: "1 L-R",
    5: "2 L-R",
    6: "3 L-R",
    7: "1 R-L",
    8: "2 R-L",
    9: "3 R-L",
    10: "1L - 1R",
    11: "2L - 2R"
}


# The class which manages an instance of the game, with the AI and the player
class ChopsticksGame:
    def __init__(self):
        # The game board (player hands)
        self.p1left = 1
        self.p1right = 1

        self.p2left = 1
        self.p2right = 1

        # The game state
        self.gameOver = False

        # The player who's turn it is
        self.playerTurn = 1

        self.loadModel()

    def loadModel(self):
        self.model = ort.InferenceSession("/ChopstickPlayer.onnx") # TODO: Check path
        session = ort.InferenceSession("/ChopstickPlayer.onnx") # TODO: Check path


    # GETTERS AND SETTERS
    def getBoard(self):
        return [self.p1left, self.p1right, self.p2left, self.p2right]
    
    def getTurn(self):
        return self.playerTurn
    
    def getGameOver(self):
        return self.gameOver
    
    #TODO: check this logic
    def getWinner(self):
        if self.gameOver:
            if self.playerTurn == 1:
                return 2
            else:
                return 1
        else:
            return 0
    

    # Reset the game
    def newGame(self):
        self.p1left = 1
        self.p1right = 1

        self.p2left = 1
        self.p2right = 1

        self.gameOver = False

        self.playerTurn = 1

    def getMoveList(self):
        return MOVE_TYPES

    # Return list of legal moves
    def getLegalMoves(self):
        moves = []
        for _ in range(0, 12):
            moves.append(True)

        if (self.playerTurn == 1 and self.p1left == 0) or (self.playerTurn == 2 and self.p2left == 0):
            # Can't attack
            moves[0] = False
            moves[1] = False

            # Can't split L-R
            moves[4] = False
            moves[5] = False
            moves[6] = False

            # Can't self-add 1L - 1R
            moves[10] = False
        else:
            if (self.playerTurn == 1 and self.p2left == 0) or (self.playerTurn == 2 and self.p1left == 0):
                # Can't attack left hand
                moves[0] = False
            
            if (self.playerTurn == 1 and self.p2right == 0) or (self.playerTurn == 2 and self.p1right == 0):
                # Can't attack right hand
                moves[1] = False

            if (self.playerTurn == 1 and self.p1right == 0) or (self.playerTurn == 2 and self.p2right == 0):
                # Can't attack right self
                moves[10] = False

        if (self.playerTurn == 1 and self.p1right == 0) or (self.playerTurn == 2 and self.p2right == 0):
            # Can't attack
            moves[2] = False
            moves[3] = False

            # Can't split R-L
            moves[7] = False
            moves[8] = False
            moves[9] = False

            # Can't self-add 1R - 1L
            moves[11] = False
        else:
            if (self.playerTurn == 1 and self.p2left == 0) or (self.playerTurn == 2 and self.p1left == 0):
                # Can't attack left hand
                moves[2] = False
            
            if (self.playerTurn == 1 and self.p2right == 0) or (self.playerTurn == 2 and self.p1right == 0):
                # Can't attack right hand
                moves[3] = False

            if (self.playerTurn == 1 and self.p1left == 0) or (self.playerTurn == 2 and self.p2left == 0):
                # Can't attack left self
                moves[11] = False

        if (self.playerTurn == 1 and self.p1left - 1 == self.p1right) or (self.playerTurn == 2 and self.p2left - 1 == self.p2right):
            # Can't split L-R 1
            moves[4] = False 
        elif (self.playerTurn == 1 and (self.p1left - 1 <= MIN_FINGERS or self.p1right + 1 >= MAX_FINGERS)) or \
            (self.playerTurn == 2 and (self.p2left - 1 <= MIN_FINGERS or self.p2right + 1 >= MAX_FINGERS)):
            # Can't split L-R 1
            moves[4] = False 
        
        if (self.playerTurn == 1 and self.p1left - 2 == self.p1right) or (self.playerTurn == 2 and self.p2left - 2 == self.p2right):
            # Can't split L-R 2
            moves[5] = False
        elif (self.playerTurn == 1 and (self.p1left - 2 <= MIN_FINGERS or self.p1right + 2 >= MAX_FINGERS)) or \
            (self.playerTurn == 2 and (self.p2left - 2 <= MIN_FINGERS or self.p2right + 2 >= MAX_FINGERS)):
            # Can't split L-R 2
            moves[5] = False

        if (self.playerTurn == 1 and self.p1left - 3 == self.p1right) or (self.playerTurn == 2 and self.p2left - 3 == self.p2right):
            # Can't split L-R 3
            moves[6] = False
        elif (self.playerTurn == 1 and (self.p1left - 3 <= MIN_FINGERS or self.p1right + 3 >= MAX_FINGERS)) or \
            (self.playerTurn == 2 and (self.p2left - 3 <= MIN_FINGERS or self.p2right + 3 >= MAX_FINGERS)):
            # Can't split L-R 3
            moves[6] = False

        if (self.playerTurn == 1 and self.p1right - 1 == self.p1left) or (self.playerTurn == 2 and self.p2right - 1 == self.p2left):
            # Can't split R-L 1
            moves[7] = False
        elif (self.playerTurn == 1 and (self.p1right - 1 <= MIN_FINGERS or self.p1left + 1 >= MAX_FINGERS)) or \
            (self.playerTurn == 2 and (self.p2right - 1 <= MIN_FINGERS or self.p2left + 1 >= MAX_FINGERS)):
            # Can't split R-L 1
            moves[7] = False

        if (self.playerTurn == 1 and self.p1right - 2 == self.p1left) or (self.playerTurn == 2 and self.p2right - 2 == self.p2left):
            # Can't split R-L 2
            moves[8] = False
        elif (self.playerTurn == 1 and (self.p1right - 2 <= MIN_FINGERS or self.p1left + 2 >= MAX_FINGERS)) or \
            (self.playerTurn == 2 and (self.p2right - 2 <= MIN_FINGERS or self.p2left + 2 >= MAX_FINGERS)):
            # Can't split R-L 2
            moves[8] = False

        if (self.playerTurn == 1 and self.p1right - 3 == self.p1left) or (self.playerTurn == 2 and self.p2right - 3 == self.p2left):
            # Can't split R-L 3
            moves[9] = False
        elif (self.playerTurn == 1 and (self.p1right - 3 <= MIN_FINGERS or self.p1left + 3 >= MAX_FINGERS)) or \
            (self.playerTurn == 2 and (self.p2right - 3 <= MIN_FINGERS or self.p2left + 3 >= MAX_FINGERS)):
            # Can't split R-L 3
            moves[9] = False

        # Update player turn
        if self.playerTurn == 1:
            self.playerTurn = 2
        else:
            self.playerTurn = 1

        return moves

    
    def makeMove(self, moveNum):
        """
        Makes a move based on the move number
        """
        if moveNum == 0:
            # Attack (1L - 2L)
            if self.playerTurn == 1:
                updatedP2Left = self.p2left + self.p1left
                if updatedP2Left >= MAX_FINGERS:
                    updatedP2Left = MIN_FINGERS
                self.p2left = updatedP2Left
            else:
                updatedP1Left = self.p1left + self.p2left
                if updatedP1Left >= MAX_FINGERS:
                    updatedP1Left = MIN_FINGERS
                self.p1left = updatedP1Left
                
        elif moveNum == 1:
           # Attack (1L - 2R)
            if self.playerTurn == 1:
                updatedP2Right = self.p2right + self.p1left
                if updatedP2Right >= MAX_FINGERS:
                    updatedP2Right = MIN_FINGERS
                self.p2right = updatedP2Right
            else:
                updatedP1Right = self.p1right + self.p2left
                if updatedP1Right >= MAX_FINGERS:
                    updatedP1Right = MIN_FINGERS
                self.p1right = updatedP1Right

        elif moveNum == 2:
            # Attack (1R - 2L)
            if self.playerTurn == 1:
                updatedP2Left = self.p2left + self.p1right
                if updatedP2Left >= MAX_FINGERS:
                    updatedP2Left = MIN_FINGERS
                self.p2left = updatedP2Left
            else:
                updatedP1Left = self.p1left + self.p2right
                if updatedP1Left >= MAX_FINGERS:
                    updatedP1Left = MIN_FINGERS
                self.p1left = updatedP1Left
        
        elif moveNum == 3:
            # Attack (1R - 2R)
            if self.playerTurn == 1:
                updatedP2Right = self.p2right + self.p1right
                if updatedP2Right >= MAX_FINGERS:
                    updatedP2Right = MIN_FINGERS
                self.p2right = updatedP2Right
            else:
                updatedP1Right = self.p1right + self.p2right
                if updatedP1Right >= MAX_FINGERS:
                    updatedP1Right = MIN_FINGERS
                self.p1right = updatedP1Right

        elif moveNum == 4:
            # Split the hand (1 L-R)
            if self.playerTurn == 1:
                self.p1left -= 1
                self.p1right += 1
            else:
                self.p2left -= 1
                self.p2right += 1

        elif moveNum == 5:
            # Split the hand (2 L-R)
            if self.playerTurn == 1:
                self.p1left -= 2
                self.p1right += 2
            else:
                self.p2left -= 2
                self.p2right += 2
        
        elif moveNum == 6:
            # Split the hand (3 L-R)
            if self.playerTurn == 1:
                self.p1left -= 3
                self.p1right += 3
            else:
                self.p2left -= 3
                self.p2right += 3

        elif moveNum == 7:
            # Split the hand (1 R-L)
            if self.playerTurn == 1:
                self.p1left += 1
                self.p1right -= 1
            else:
                self.p2left += 1
                self.p2right -= 1

        elif moveNum == 8:
            # Split the hand (2 R-L)
            if self.playerTurn == 1:
                self.p1left += 2
                self.p1right -= 2
            else:
                self.p2left += 2
                self.p2right -= 2

        elif moveNum == 9:
            # Split the hand (3 R-L)
            if self.playerTurn == 1:
                self.p1left += 3
                self.p1right -= 3
            else:
                self.p2left += 3
                self.p2right -= 3

        elif moveNum == 10:
            # Self add (1L - 1R)
            if self.playerTurn == 1:
                updatedP1Right = self.p1right + self.p1left
                if updatedP1Right >= MAX_FINGERS:
                    updatedP1Right = MIN_FINGERS
                self.p1right = updatedP1Right
            else:
                updatedP2Right = self.p2right + self.p2left
                if updatedP2Right >= MAX_FINGERS:
                    updatedP2Right = MIN_FINGERS
                self.p2right = updatedP2Right

        elif moveNum == 11:
            # Self add (1R - 1L)
            if self.playerTurn == 1:
                updatedP1Left = self.p1left + self.p1right
                if updatedP1Left >= MAX_FINGERS:
                    updatedP1Left = MIN_FINGERS
                self.p1left = updatedP1Left
            else:
                updatedP2Left = self.p2left + self.p2right
                if updatedP2Left >= MAX_FINGERS:
                    updatedP2Left = MIN_FINGERS
                self.p2left = updatedP2Left

    # AI functions
    def getP2Move(self):
        # Not P2 turn
        if self.playerTurn == 1:
            return 12, "Not P2 turn"

        # Define the input data
        input_data = np.array([self.p1left / 4, self.p1right / 4, self.p2left / 4, self.p2right / 4])

        # Define the action mask
        movesMask = self.getMovesMask()
        for i in range(len(movesMask)):
            if not movesMask[i]:
                action_mask = np.append(input_data, 0)
            else:
                action_mask = np.append(input_data, 1)

        # Make action_mask np.float32
        action_mask = action_mask.astype(np.float32)

        # Run the model
        output = self.session.run(None, {'obs_0': input_data, 'action_masks': action_mask})

        # Get the output data
        output_data = output[2]

        return output_data, MOVE_TYPES[output_data]

    def p2AIMove(self):
        # Get the move
        moveNum, moveName = self.getP2Move()

        if moveNum == 12:
            return moveName
        
        # Make the move
        self.makeMove(moveNum)

        # Return the move
        return moveName
