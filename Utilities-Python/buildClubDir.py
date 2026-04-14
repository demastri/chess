import sys
import time
from datetime import datetime, timedelta
from threading import Thread

from USCF.uscfRatingsApi import uscfRatingsApi


def main():

    eventJson = uscfRatingsApi.getEventsForAffiliate( sys.argv[1] if len(sys.argv) > 1 else None )
    endDate = datetime.now() - timedelta(days=180)
    eventPlayers = set()
    for event in eventJson['items']:
        lastSeenDate = datetime.strptime(event['startDate'], "%Y-%m-%d")
        if lastSeenDate < endDate:
            break
        print(event['startDate']+" = "+event['name'])
        thisEventPlayers = uscfRatingsApi.getPlayersForEventFromUscfId(event['id'])
        for player in thisEventPlayers:
            eventPlayers.add(player['memberId'])

    outFile = "./tsv/ClubDirOut.tsv"
    fOut = open(outFile, "w+")

    counter = 1
    for player in eventPlayers:
        thisMember = uscfRatingsApi.getPlayerJson(player)
        time.sleep(0.900)
        outString = (str(counter)+"\t"+thisMember["id"]+"\t"+thisMember["lastName"].title()+", "+thisMember["firstName"].title()+"\t"+
           next((str(obj["rating"]) for obj in thisMember["ratings"] if obj["ratingSystem"] == "R" and "rating" in obj), "NA")+"\t"+
           next((str(obj["rating"]) for obj in thisMember["ratings"] if obj["ratingSystem"] == "Q" and "rating" in obj), "NA") + "\t" +
           next((str(obj["rating"]) for obj in thisMember["ratings"] if obj["ratingSystem"] == "B" and "rating" in obj), "NA") + "\t" +
           thisMember["expirationDate"])
        fOut.write(outString+"\n")
        print(outString)
        counter = counter+1
    fOut.close()

    return 0

if __name__ == "__main__":
    main()
