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
    inCourse = False
    inChapter = False
    inVariation = False
    for arg in sys.argv[1:] :
        if arg == 'c':
            inCourse = True
            inChapter = inVariation = False
            continue
        elif arg == 'p':
            inChapter = True
            inCourse = inVariation = False
            continue
        elif arg == 'v':
            inVariation = True
            inCourse = inChapter = False
            continue

        if inCourse:
            courseId = arg
            print("Full process of course "+courseId)
            #print("--- getting variation html ---")
            courseBS, chapters = loadCourseInfo( courseId )
            # this first pass loads/saves all of the chapter htmls
            # running single-threaded - an hour trying to get threading and processing failed (selenium issues)

            chapterResults = loadChapterInfo( courseId, chapters )
            # once we have the chapter details, we can load all of the variation htmls

            variationResults = loadVariationInfo(courseId, chapterResults)
            # now all of the variation htmls are available locally

            pgnOut = generateCoursePGNs(courseId, variationResults)
            print(pgnOut)
            # prints...actually want to save this to disk somewhere...
            pgn.writeCoursePgnFile(courseId, pgnOut)

        if inVariation:
            variationID = arg
            courseId = "one-off"
            print("Full process of variation "+variationID)
            # print("--- getting variation html ---")
            thisVarResult = chessable.getVariationDetailFromId(courseId, variationID, "Default")
            thisVarResult.append("x.x")

            pgnOut = generateCoursePGNs(courseId, [thisVarResult])
            # prints...actually want to save this to disk somewhere...
            print(pgnOut)
            pgn.writeVariationPgnFile(variationID, pgnOut)


        print("--- complete ---")
        print(datetime.now())

def loadCourseInfo(courseId):
    print("--- getting course html for course " + courseId + " ---")
    courseBS, chapters = chessable.getCourseDetail(courseId, "Default")
    print("----- found course '" + chessable.getCourseName(courseBS) + "'")
    print("----- read " + str(len(chapters)) + " chapters")
    return courseBS, chapters

def loadChapterInfo( courseId, chapters ):
    chapterResults = []

    chaptersRead = 0
    varsPreviewed = 0
    for c in chapters:
        thisResult = processChapter(courseId, str(c), "Default")
        varsPreviewed += len(thisResult[1])
        chapterResults.append(thisResult)
        chaptersRead += 1
        if chaptersRead > 500:
            break
    print(" - total of " + str(varsPreviewed) + " - variations previewed - ")
    return chapterResults

def loadVariationInfo( courseId, chapterResults ):
    variationResults = []

    variationsRead = 0
    chapterNbr = 0
    variationNbr = 0
    for b, v in chapterResults:
        chapterNbr += 1
        variationNbr = 0
        for variation in v:
            variationNbr += 1
            thisVarDet = chessable.getVariationDetailFromTag(courseId, variation, "Default")
            roundStr = str(chapterNbr) + "." + str(variationNbr)
            thisVarDet.append(roundStr)
            variationResults.append(thisVarDet)
            variationsRead += 1
            if variationsRead > 5000:
                break
        if variationsRead > 5000:
            break
    print(" - total of " + str(variationsRead) + " - variations read - ")
    return variationResults


def generateCoursePGNs(courseId, variationResults):
    aggregatePgn = ""
    for [variation, variationId, roundStr] in variationResults:
        pgnOut = pgn.createPgnFromHtml(courseId, variationId, variation, roundStr)
        aggregatePgn += pgnOut
    return aggregatePgn

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
