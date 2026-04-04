import sys
from player import Player
from root.utils import String

inputFilename = '../resources/EarlySummer-Reserve.txt'

if len(sys.argv) > 1:
    inputFilename = sys.argv[1]

# main script starts here
with open(inputFilename) as fin:
    players = []
    numRounds = 0
    index = 0
    line1 = ""
    line2 = ""

    for line in fin:
        if index == 0:
            # nothing to do here
            # clean up
            line1 = ""
            line2 = ""
        elif index == 1:
            line1 = line
        elif index == 2:
            line2 = line
            player = Player.createPlayer(line1.strip(), line2.strip())
            players.append(player)
            if numRounds == 0:
                numRounds = len(player.rounds)

            # clean up
            index = -1
        index = index + 1

    # now let's print out the crosstable
    String.printTableHeader(numRounds)
    for player in players:
        player.printHtml()
    String.printPageClose()
