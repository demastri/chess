from typing import List
import re
import sys


def main():
    gameType = "lichess"
    if len(sys.argv) > 1:
        gameType = sys.argv[1]

    gameInFile = "./pgn/Source File In.pgn"
    gameOutFile = "./pgn/Dojo File Out.pgn"

    fIn = open(gameInFile, "r")
    fOut = open(gameOutFile, "w")

    # this writes header lines and assumes TC is last line in header block
    curTCS = findTimeControl(fIn, fOut, gameType)

    curClocks = [curTCS[0].time, curTCS[0].time]

    for line in fIn:
        if gameType == "lichess":
            processLichessClockTimes(line, fOut, curTCS, curClocks)
        if gameType == "OTB":
            processOTBClockTimes(line, fOut, curTCS, curClocks)


def processOTBClockTimes(line, fOut, tcs, clocks):
    # so - games entered into cb have the emt tag as well, but are actually clock times...
    global replacedTimes
    curIndex = 0  # next char to write out.
    matches = [(m.start(), m.end()) for m in re.finditer(r"\[%emt \d:\d\d:\d\d]", line)]
    print("Found " + str(len(matches)) + " matches:")

    if len(matches) == 0:
        fOut.write(line)
    for match in matches:
        fOut.write(line[curIndex:match[0]])
        curIndex = match[1]

        ckStr = line[match[0]:match[1]].replace("emt", "clk")
        fOut.write(ckStr)
        replacedTimes += 1

    if curIndex > 0:
        fOut.write(line[curIndex:])


replacedTimes = 0


def processLichessClockTimes(line, fOut, tcs, clocks):
    global replacedTimes
    curIndex = 0  # next char to write out.
    matches = [(m.start(), m.end()) for m in re.finditer(r"\[%emt \d:\d\d:\d\d]", line)]
    print("Found " + str(len(matches)) + " matches:")

    if len(matches) == 0:
        fOut.write(line)
    for match in matches:
        fOut.write(line[curIndex:match[0]])
        curIndex = match[1]
        hr = int(line[match[0] + 6:match[0] + 7])
        min = int(line[match[0] + 8:match[0] + 10])
        sec = int(line[match[0] + 11:match[0] + 13])

        moveTime = 3600 * hr + 60 * min + sec
        wbIndex = (replacedTimes % 2)

        # need to generate the current actual clock time based on the previous clock time, the emt and any increment
        # note that lichess does not charge time for the 1st move and doesn't add increment.
        # CB puts 1:0:0 for each first move.
        # to correct this, initial time should be start - inc so that adding inc views as 0


        if replacedTimes < 2:
            moveTime = 0

        thisTC, thisTCAdder = TimeControl.getCurrentTimeControl(tcs, int(replacedTimes/2)+1)
        clocks[wbIndex] += thisTCAdder

        # if you took 3 sec with a 5 sec delay (0 incr) clock doesn't change
        # if you took 8 sec with a 5 sec delay (0 incr) clock decrements by 3 sec
        if moveTime <= thisTC.delay:
            moveTime = 0
        else:
            moveTime -= thisTC.delay    # ok if delay is 0

        # otherwise be sure to add any increment to the clock
        finalClock = clocks[wbIndex] - moveTime + thisTC.incr

        clocks[wbIndex] = finalClock

        fcHr = int(finalClock / 3600)
        fcMin = int(finalClock / 60) % 60
        fcSec = int(finalClock) % 60
        fOut.write("[%clk " + str(fcHr) + ":" + str(fcMin).zfill(2) + ":" + str(fcSec).zfill(2) + "]")
        replacedTimes += 1
    if curIndex > 0:
        fOut.write(line[curIndex:])


def findTimeControl(fIn, fOut, gameType):
    # [TimeControl "6000d5"]
    # [TimeControl "6000+5"]
    # [TimeControl "40/4800d30:1800d30"]

    for line in fIn:
        if line.startswith("[TimeControl \""):
            sub = line.strip()[14:-2]
            tcs = TimeControl.parseTimeControlString(sub, gameType)
            fOut.write("[TimeControl \"" + TimeControl.generateCompleteTimeControlString(tcs) + "\"]\n")
            return tcs
        fOut.write(line)
    return []


class TimeControl:

    def __init__(self, initStr: str, gameType):
        # [TimeControl "6000d5"]
        # [TimeControl "6000+5"]
        # [TimeControl "6000i5"]
        # [TimeControl "40/4800d30:1800d30"]

        self.moves = -1
        self.time = 0
        self.delay = 0
        self.incr = 0

        moveDelim = initStr.find("/")
        delayDelim = initStr.find("d")
        incrDelim = initStr.find("+")
        if incrDelim == -1:
            incrDelim = initStr.find("i")

        timeEnd = delayDelim if delayDelim > 0 else incrDelim

        if timeEnd < 0:
            self.time = int(initStr[moveDelim + 1:])
        else:
            self.time = int(initStr[moveDelim + 1:timeEnd])

        if moveDelim > 0:
            self.moves = int(initStr[:moveDelim])

        if delayDelim > 0:
            self.delay = int(initStr[delayDelim + 1:])

        if incrDelim > 0:
            self.incr = int(initStr[incrDelim + 1:])

        # to allow board to display correctly
        if gameType == "lichess":
            self.time -= (self.delay + self.incr)

    def __str__(self):
        outStr = ""
        if self.moves > 0:
            outStr += str(self.moves) + "/"
        outStr += str(self.time)
        if self.delay > 0:
            outStr += "d" + str(self.delay)
        elif self.incr > 0:
            outStr += "+" + str(self.incr)
        return outStr

    @classmethod
    def parseTimeControlString(cls, tcString: str, gameType: str):
        outTCs = []
        tcs = tcString.split(":")
        for tc in tcs:
            outTCs.append(TimeControl(tc, gameType))
        return outTCs

    @classmethod
    def generateCompleteTimeControlString(cls, tcs):
        if len(tcs) == 0:
            return ""
        outString = str(tcs[0])
        for tc in tcs[1:]:
            outString += ":" + str(tc)
        return outString

    @classmethod
    def getCurrentTimeControl(cls, tcs, moveNbr):
        curMoveCount = 0
        for tc in tcs:
            curAdder = 0
            if tc.moves > 0:
                curMoveCount += tc.moves
            if moveNbr == curMoveCount:
                curAdder = tc.time
            if tc.moves < 0 or moveNbr <= curMoveCount:
                return tc, curAdder
        return None, 0

def testTimeControls():
    thisStr = "[TC \"234\"]"
    print(thisStr[5:-4])

    print(TimeControl.generateCompleteTimeControlString(TimeControl.parseTimeControlString("3600", "OTB")))
    print(TimeControl.generateCompleteTimeControlString(TimeControl.parseTimeControlString("3600d5", "OTB")))
    print(TimeControl.generateCompleteTimeControlString(TimeControl.parseTimeControlString("3600i5", "OTB")))
    print(TimeControl.generateCompleteTimeControlString(TimeControl.parseTimeControlString("3600+5", "OTB")))
    print(TimeControl.generateCompleteTimeControlString(TimeControl.parseTimeControlString("40/3600d5:1800+30", "OTB")))

if __name__ == "__main__":
    # testTimeControls()
    main()
