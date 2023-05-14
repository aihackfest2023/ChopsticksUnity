# import the Chopsticks class from the ChopsticksPythonAI module
from ChopsticksPythonAI import ChopsticksGame

# create a new instance of the Chopsticks class -------------------------------
chopsticks = ChopsticksGame()

boardArr = chopsticks.getBoard()
print(boardArr)

currentTurn = chopsticks.getTurn()
print(currentTurn)

isGameOver = chopsticks.getGameOver()
print(isGameOver)

print(chopsticks.getWinner())


### Test play of game ---------------------------------------------------------
# MOVE_TYPES = {
#     0: "1L - 2L",
#     1: "1L - 2R",
#     2: "1R - 2L",
#     3: "1R - 2R",
#     4: "1 L-R",
#     5: "2 L-R",
#     6: "3 L-R",
#     7: "1 R-L",
#     8: "2 R-L",
#     9: "3 R-L",
#     10: "1L - 1R",
#     11: "2L - 2R"
# }


# Player 1 makes a move
print("Player 1 makes a move")
chopsticks.makeMove(0)

boardArr = chopsticks.getBoard()
print(boardArr)

# Player 2 AI makes a move
print("Player 2 AI makes a move")
chopsticks.p2AIMove()

boardArr = chopsticks.getBoard()
print(boardArr)

print("Turn " + str(chopsticks.getNumTurns()))

