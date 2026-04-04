from USCF.uscfRatingsApi import *

def printWcaCrosstable(event):
    print("No.|Sect|Name|ID|Rate|Pts|Rnd1-Rndn")
    playerCount = 0
    for section in event["data"]["sections"]:
        sectionName = section["name"]
        for player in section["data"]["items"]:
            playerCount = playerCount+1
            print(str(playerCount)+"|"+sectionName+"|"+player["lastName"].title()+", "+player["firstName"].title()+"|"+player["memberId"]+"|", end="")
            print(str(list(filter(lambda x: x["ratingSystem"] == "R",player["ratings"]))[0]["postRating"])+"|", end="")
            print(str(player["score"])+"|", end="")
            for outcome in player["roundOutcomes"]:
                match outcome["outcome"]:
                    case "Unpaired":
                        print("U", end="")
                    case "ByeZero":
                        print("X", end="")
                    case "ByeHalf":
                        print("H", end="")
                    case "ByeFull":
                        print("B", end="")
                    case "Win":
                        print("W"+str(outcome["opponentOrdinal"]), end="")
                    case "Loss":
                        print("L"+str(outcome["opponentOrdinal"]), end="")
                    case "Draw":
                        print("D"+str(outcome["opponentOrdinal"]), end="")
                print(" ", end="")
            print()


if __name__ == "__main__":
    def main():
        eventId = "202602280803"
        event = uscfRatingsApi.createEventFromUscfId(eventId)
        printWcaCrosstable(event)

    main();