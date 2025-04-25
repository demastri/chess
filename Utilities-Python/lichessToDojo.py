import re
import sys


def main():
    gameType = "lichess"
    if len(sys.argv) > 1:
        gameType = sys.argv[1]

    gameInFile = "test.pgn"
    gameOutFile = "somefileOut.pgn"

    fIn = open(gameInFile, "r")
    fOut = open(gameOutFile, "w")

    curTC = findTimeControl(fIn, fOut, gameType)
    curClocks = [curTC[0], curTC[0]]

    for line in fIn:
        if gameType == "lichess":
            processLichessClockTimes(line, fOut, curTC, curClocks)
        if gameType == "OTB":
            processOTBClockTimes(line, fOut, curTC, curClocks)

def processOTBClockTimes(line, fOut, tc, clocks):
    # so - games entered into cb have the emt tag as well, but are actually clock times...
    global replacedTimes
    curIndex = 0 # next char to write out.
    matches = [(m.start(), m.end()) for m in re.finditer("\[%emt \d:\d\d:\d\d\]", line)]
    print("Found "+str(len(matches))+" matches:")


    if len(matches) == 0:
        fOut.write(line)
    for match in matches:
        fOut.write(line[curIndex:match[0]])
        curIndex = match[1]
        hr = int(line[match[0]+6:match[0]+7])
        min = int(line[match[0]+8:match[0]+10])
        sec = int(line[match[0]+11:match[0]+13])

        moveTime = 3600*hr + 60*min + sec
        wbIndex = (replacedTimes % 2)

        # need to generate the current actual clock time based on the previous clock time, the emt and any increment
        # note that lichess does not charge time for the 1st move and doesn't add increment.
        # CB puts 1:0:0 for each first move.
        # to correct this, initial time should be start - inc so that adding inc views as 0

        finalClock = moveTime

        clocks[wbIndex] = finalClock

        fcHr = int(finalClock/3600)
        fcMin = int(finalClock/60) % 60
        fcSec = int(finalClock) % 60
        fOut.write("[%clk "+str(fcHr)+":"+str(fcMin).zfill(2)+":"+str(fcSec).zfill(2)+"]")
        replacedTimes += 1
    if curIndex > 0:
        fOut.write(line[curIndex:])

replacedTimes = 0

def processLichessClockTimes(line, fOut, tc, clocks):
    global replacedTimes
    curIndex = 0 # next char to write out.
    matches = [(m.start(), m.end()) for m in re.finditer("\[%emt \d:\d\d:\d\d\]", line)]
    print("Found "+str(len(matches))+" matches:")


    if len(matches) == 0:
        fOut.write(line)
    for match in matches:
        fOut.write(line[curIndex:match[0]])
        curIndex = match[1]
        hr = int(line[match[0]+6:match[0]+7])
        min = int(line[match[0]+8:match[0]+10])
        sec = int(line[match[0]+11:match[0]+13])

        moveTime = 3600*hr + 60*min + sec
        wbIndex = (replacedTimes % 2)

        # need to generate the current actual clock time based on the previous clock time, the emt and any increment
        # note that lichess does not charge time for the 1st move and doesn't add increment.
        # CB puts 1:0:0 for each first move.
        # to correct this, initial time should be start - inc so that adding inc views as 0

        if replacedTimes < 2:
            moveTime = 0
        finalClock = clocks[wbIndex] - moveTime + tc[1]

        clocks[wbIndex] = finalClock

        fcHr = int(finalClock/3600)
        fcMin = int(finalClock/60) % 60
        fcSec = int(finalClock) % 60
        fOut.write("[%clk "+str(fcHr)+":"+str(fcMin).zfill(2)+":"+str(fcSec).zfill(2)+"]")
        replacedTimes += 1
    if curIndex > 0:
        fOut.write(line[curIndex:])

def findTimeControl(fIn, fOut, gameType):
    for line in fIn:
        if line.startswith("[TimeControl \""):
            sub = line[14:]
            if "+" in sub:
                sub = sub.replace("+", " ")
                sub = sub.replace("\"]\n", "")
                tc = [int(s) for s in sub.split() if s.isdigit()]
                tc.append(0)
                if gameType == "lichess":
                    tc[0] = tc[0] - tc[1]
                # to allow the Dojo board to display this properly, we should subtract 1 inc from the tc
                fOut.write("[TimeControl \""+str(tc[0])+"+"+str(tc[1])+"\"]\n")
                return tc

            if "d" in sub:
                sub = sub.replace("d", " ")
                sub = sub.replace("\"]\n", "")
                tc = [int(s) for s in sub.split() if s.isdigit()]
                tc.append(tc[1])
                tc[1] = 0
                if gameType == "lichess":
                    tc[0] = tc[0] - tc[1]
                # to allow the Dojo board to display this properly, we should subtract 1 inc from the tc
                fOut.write("[TimeControl \""+str(tc[0])+"d"+str(tc[2])+"\"]\n")
                return tc

        fOut.write(line)
    return [0, 0, 0]



if __name__ == "__main__":
    main()
