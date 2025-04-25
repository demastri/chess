from pandas.core.computation.ops import isnumeric
from prompt_toolkit.utils import to_int

def load_raw_pgn(path, fn):
    games = {}
    inGame = False
    thisGame = []
    index = -1

    seen72 = False  # made a mistake tagging image 71, and it's out-of-order.  First time we see game 72, make it 71

    # '[' tag starts game
    # "*" ends game
    # Game is a list of strings

    file = open(path+fn, "r")
    for line in file:
        if not inGame and len(line.strip()) > 0 and line.strip()[0] == "[":
            inGame = True

        if inGame:
            if len(line.strip()) > 0 and line.strip()[0] == "{":
                index = int(line.split()[2])
                if not seen72 and index == 72:
                    index = 71
                    seen72 = True
            thisGame.append(line)

        if inGame and len(line.strip()) > 0 and line.strip()[0] == "*":
            if index == -1:
                print("No index for completed game...")
            else:
                games[index] = thisGame
            thisGame = []
            index = -1
            inGame = False

        # print(line.strip())  # The comma to suppress the extra new line char

    print(str(len(games))+" Games loaded from "+fn)
    # print(games)
    return games


def load_supplemental(path, fn):
    stars = []
    sections = {}

    currentPosition = 0
    file = open(path+fn, "r")
    for line in file:
        # if this line starts with a character, it's a tag, mark the next position as the start for this tag
        if line[0].isalpha():
            # print( "tag - ", end="")
            sections[line.strip()] = currentPosition
        # otherwise, validate the index, update the current indexed position difficulty
        if line[0].isnumeric():
            # print( "difficulties - ", end="")
            words = line.split()
            thisPos = int(words[0][:-1])
            if thisPos != currentPosition + 1:
                print("line mismatch - ", thisPos)
            else:
                for word in words[1:]:
                    stars.append(int(word))
                    currentPosition += 1

        # print(line.strip())  # The comma to suppress the extra new line char

    # since 3.7 preserves insertion order
    #for tag in sections.keys():
    #    print(tag, sections[tag])
    #print(len(sections))
    #print(sections)
    print(str(len(stars)) + " difficulties read")
    #print(difficulty)
    return stars, sections


def write_updated_pgn(path, fn, rawGames, processedGames, stars, sections):
    # we have all of the difficulties - not all images/pgns, write the ones we have in order
    # Round = section index, subround = element in this section
    # White = CTFS - Overall Position
    # Black = Section Text - Section Position index
    # none of these tags exist, ok to write them before we write what was in the file (just fen and result}

    # the understanding is that the images go from high-low, but always go to 1 at the end
    # this has to be reworked if that's not the case

    file = open(path+fn, "a")
    read = 0
    written = 0
    for i in range(1, len(rawGames) + 1):
        #print(games[i])

        # ok - we have an index, which is the overall position number
        # need the index offset and name of the tag for this game
        thisTag = ""
        thisOffset = -1
        tagIndex = 0
        for tag in sections.keys():
            if sections[tag] < i:
                tagIndex += 1
                thisTag = tag
                thisOffset = i-sections[tag]
        read += 1

        if i not in processedGames:
            file.write("[Event \"Chess Tactics From Scratch\"]\n") # should be first by standard
            file.write("[White \"Test Position "+str(i)+" - Difficulty: "+str(stars[i-1])+" Stars\"]\n")
            file.write("[Black \""+thisTag+" - Position "+str(thisOffset)+"\"]\n")
            file.write("[Round \"" + str(tagIndex)+ "."+str(thisOffset)+"\"]\n")
            file.writelines(rawGames[i]) # includes only Result and FEN
            file.write("\n")
            written += 1

    print(str(read) + " games read")
    print( str(written) + " games written")
    return


def main():
    pgnPath = "M:/Chess/Data/PGN/"
    rawFn = "CTFS Diagrams Raw.pgn"
    processedFn = "CTFS Diagrams Processed.pgn"
    difficultyFn = "CTFS Diagrams Supplement.txt"

    rawGames = load_raw_pgn(pgnPath, rawFn)
    processedGames = load_raw_pgn(pgnPath, processedFn)
    difficulties, sections = load_supplemental(pgnPath, difficultyFn)
    write_updated_pgn(pgnPath, processedFn, rawGames, processedGames, difficulties, sections)


if __name__ == "__main__":
    main()
