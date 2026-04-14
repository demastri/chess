import json
import urllib

from datetime import datetime
from urllib.request import urlopen

from lxml import html

# removed all of the screen scraping stuff
# two use cases
# 1 - event info: take in an event ID and build a tree of sections and players

BASE_AFFILIATE_ID = "A5008948" # waukesha
BASE_JSON_URL = 'https://ratings-api.uschess.org/api/v1/rated-events/'
BASE_AFFILIATE_URL = 'https://ratings-api.uschess.org/api/v1/affiliates/'
BASE_MEMBERS_URL = 'https://ratings-api.uschess.org/api/v1/members/'

class uscfRatingsApi:

    @classmethod
    def createEventFromUscfId(cls, uscfId: str):
        thisEvent = {}
        thisEvent["data"] = uscfRatingsApi.getEventJson(uscfId)
        for section in thisEvent["data"]["sections"]:
            section["data"] = uscfRatingsApi.getSectionJson(uscfId, str(section["number"]))
        return thisEvent

    @classmethod
    def getPlayersForEventFromUscfId(cls, uscfId: str):
        thisEvent = uscfRatingsApi.createEventFromUscfId(uscfId)
        players = []
        for section in thisEvent["data"]["sections"]:
            for player in section["data"]["items"]:
                players.append(player)
        return players

    @classmethod
    def getEventJson(cls, tournamentId: str):
        url = BASE_JSON_URL + tournamentId
        req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0', 'accept': 'text/plain'})
        connection = urlopen(req)
        raw = connection.read()
        return json.loads(raw.decode("utf-8"))

    @classmethod
    def getSectionJson(cls, tournamentId: str, section: str):
        url = BASE_JSON_URL + tournamentId + "/sections/" + section + "/standings"
        req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0', 'accept': 'text/plain'})
        connection = urlopen(req)
        raw = connection.read()
        return json.loads(raw.decode("utf-8"))

    @classmethod
    def getPlayerJson(cls, memberId: str):
        url = BASE_MEMBERS_URL + memberId
        req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0', 'accept': 'text/plain'})
        connection = urlopen(req)
        raw = connection.read()
        return json.loads(raw.decode("utf-8"))

    @classmethod
    def getEventsForAffiliate(cls, affiliateId: str):
        url = BASE_AFFILIATE_URL + (BASE_AFFILIATE_ID if (affiliateId is None or affiliateId == "") else affiliateId)  + "/events"
        req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0', 'accept': 'text/plain'})
        connection = urlopen(req)
        raw = connection.read()

        return json.loads(raw.decode("utf-8"))

