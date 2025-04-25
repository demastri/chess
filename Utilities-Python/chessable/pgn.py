from chessable import chessable

HTML_CACHE_PATH = './html/'

class pgn:
    @classmethod
    def createPgnFromHtml(cls, courseId:str, variationId, variation, roundStr):
        name, chapter, moves = chessable.getVariationParts(variation)
        result = pgn.getGameResult(moves)
        outPgn = pgn.buildHeader(name, chapter, result, roundStr)
        outPgn += pgn.buildMoveBody(moves)
        outPgn += pgn.buildGameResult(result)
        return outPgn

    @classmethod
    def buildHeader(cls, name:str, chapter, result, roundStr):
        header = """[Event \"Chessable->PGN\"]
[Site \"?\"]
[Date \"????.??.??\"]
[Round \""""+roundStr+"""\"]
[White \"\"]
[Black \"\"]
[Result \""""+result+"""\"]
[Title \""""+name.strip()+"\"]\n"
        return header

    @classmethod
    def buildMoveBody(cls, moves):
        return "yyy\n"

    @classmethod
    def buildGameResult(cls, result):
        return "\n "+result+" \n"

    @classmethod
    def getGameResult( cls, result ):
        return "*"
