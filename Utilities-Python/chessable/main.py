import os
import shutil
import sys
import threading
from datetime import datetime
from multiprocessing import Pool

from bs4 import BeautifulSoup

from chessable import chessable
from pgn import pgn

profileIds = []

def main():
    if len(sys.argv) < 2:
        print("Please provide at least one courseID")
        return

    print(datetime.now())
    for courseId in sys.argv[1:]:
        #print("--- getting variation html ---")
        #print(chessable.getVariationHtml("4132323"))
        print("--- getting course html for course "+courseId+" ---")
        courseBS, chapters = chessable.getCourseDetail(courseId, "Default")
        print("----- found course '"+chessable.getCourseName(courseBS)+"'")
        print("----- read "+str(len(chapters))+" chapters")

        # this first pass loads/saves all of the chapter htmls
        # running single-threaded took about 8 min

        chapterResults = []
        variationResults = []

        chaptersRead=0
        varsPreviewed=0
        for c in chapters:
            thisResult = processChapter(courseId, str(c), "Default")
            varsPreviewed += len(thisResult[1])
            chapterResults.append( thisResult )
            chaptersRead += 1
            if chaptersRead > 500:
                break
        print(" - total of " + str(varsPreviewed) + " - variations previewed - ")

        # once we have the chapter details, we can load all of the variation htmls
        variationsRead=0
        chapterNbr = 0
        variationNbr = 0
        for b,v in chapterResults:
            chapterNbr += 1
            variationNbr = 0
            for variation in v:
                variationNbr += 1
                thisVarDet = chessable.getVariationDetail(courseId, variation, "Default")
                roundStr = str(chapterNbr) + "." + str(variationNbr)
                thisVarDet.append(roundStr)
                variationResults.append( thisVarDet )
                variationsRead += 1
                if variationsRead > 5000:
                    break
            if variationsRead > 5000:
                break
        print( " - total of "+str(variationsRead)+" - variations read - ")

        for [variation, variationId, roundStr] in variationResults:
            pgnOut = pgn.createPgnFromHtml(courseId, variationId, variation, roundStr)
            print(pgnOut)
            # actually want to save this to disk somewhere...


        print("--- complete ---")
        print(datetime.now())

TESTING_PROFILE_BASE_DIR = "C:/Users/john/AppData/Local/Google/Chrome for Testing/User Data"

def buildTestingProfiles(ref: str, prefix:str, pool_size:int):
    global profileIds
    for x in range(pool_size):
        srcDir = TESTING_PROFILE_BASE_DIR + "/" + "Default"
        destDir = TESTING_PROFILE_BASE_DIR + "/" + prefix + str(x)
        if not os.path.exists(destDir):
            shutil.copytree(srcDir, destDir,dirs_exist_ok=True)
        profileIds.append(prefix + str(x))


def destroyTestingProfiles(str, prefix:str, pool_size:int):
    for x in range(pool_size):
        thisDir = TESTING_PROFILE_BASE_DIR + "/" + prefix+str(x)
        shutil.rmtree(thisDir)

def processChapter( courseId, tagStr, profileName  ):
    #print("In pool fn '" + tagStr + "' ("+profileName+") ")
    chapter = BeautifulSoup(tagStr, "html.parser")
    print("Parsing '" + chessable.getChapterName(chapter) + "' ("+profileName+") ")
    chapterBS, variations = chessable.getChapterDetail(courseId, chapter, profileName)
    print(" returned  '" + chessable.getChapterName(chapter) + "' had ("+profileName+") " + str(len(variations)) + " variations")
    #print(" leaving pool fn '" + chessable.getChapterName(chapter) + "' had ("+profileName+") ")
    return [chapterBS, variations]

def processChapterFake( courseId, tagStr  ):
    chapter = BeautifulSoup(tagStr, "html.parser")
    print("----- reading chapter '" + chessable.getChapterName(chapter) + "'")


if __name__ == "__main__":
    main()
