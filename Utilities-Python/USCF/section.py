from root.chess.player import Player


class Section:
    def __init__(self, name, uscfName, href, jsonData=None, eventData=None):
        self.players = list()
        if jsonData is None:
            self.name = name
            self.uscfName = uscfName
            self.href = href
        else:
            self.jsonData = jsonData
            for sectionJson in eventData["sections"]:
                if sectionJson["number"] == jsonData["sectionIndex"]:
                    self.name = sectionJson["name"]
                    self.uscfName = sectionJson["name"]
                    self.href = ""
                    for item in jsonData["items"]:
                        self.players.append(Player.createPlayerFromJson(item))

    @classmethod
    def createSection(cls, name, uscfName, href):
        aSection = Section(name, uscfName, href)
        return aSection

    def getName(self):
        return self.name

    def setName(self, aName: str):
        self.name = aName

    def getUscfName(self):
        return self.uscfName

    def setUscfName(self, aName: str):
        self.uscfName = aName

    def addPlayer(self, player: Player):
        self.players.append(player)

    def getPlayers(self):
        return self.players

    def getPlayerCount(self):
        return len(self.players)

    def getRoundCount(self):
        if len(self.players) > 0:
            player1: Player = self.players.__getitem__(0)
            return len(player1.rounds)
        return 0

    def toHtml(self):
        buffer = ""  # "<span class='wccSection'>" + self.name + "</span>\n"
        for player in self.players:
            buffer += player.toHtml()
        return buffer

    def getNameHtml(self):
        buffer = "<span class='wccSection'>" + self.name + "</span>\n"
        return buffer

    def getSectionNameFromJson(self, eventInfo):
        buffer = "<span class='wccSection'>" + self.name + "</span>\n"
        return buffer
