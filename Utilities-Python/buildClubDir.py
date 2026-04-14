import json
import sys
import time
from datetime import datetime, timedelta
from threading import Thread

from USCF.uscfRatingsApi import uscfRatingsApi

global_counter = 1
def main():
    global global_counter

    eventJson = uscfRatingsApi.getEventsForAffiliate( sys.argv[1] if len(sys.argv) > 1 else None )
    endDate = datetime.now() - timedelta(days=180)
    recentEvents = [event for event in eventJson["items"] if datetime.strptime(event['startDate'], "%Y-%m-%d") >= endDate]
    playerSets = [uscfRatingsApi.getPlayersForEventFromUscfId(event['id']) for event in recentEvents]
    eventPlayers = {player["memberId"] for playerList in playerSets for player in playerList}

    outFile = "./tsv/ClubDirOut.tsv"
    fOut = open(outFile, "w+")

    global_counter = 1
    [writePlayer(fOut, plr) for plr in eventPlayers]
    fOut.close()

    return 0

def writePlayer(fOut, plr):
    global global_counter

    thisMember = uscfRatingsApi.getPlayerJson(plr)
    time.sleep(0.900)
    outString = (str(global_counter) + "\t" + thisMember["id"] + "\t" + thisMember["lastName"].title() + ", " + thisMember[
        "firstName"].title() + "\t" +
         next((str(obj["rating"]) for obj in thisMember["ratings"] if obj["ratingSystem"] == "R" and "rating" in obj), "NA") + "\t" +
         next((str(obj["rating"]) for obj in thisMember["ratings"] if obj["ratingSystem"] == "Q" and "rating" in obj), "NA") + "\t" +
         next((str(obj["rating"]) for obj in thisMember["ratings"] if obj["ratingSystem"] == "B" and "rating" in obj), "NA") + "\t" +
         thisMember["expirationDate"])
    fOut.write(outString + "\n")
    print(outString)
    global_counter = global_counter +1


if __name__ == "__main__":
    main()
