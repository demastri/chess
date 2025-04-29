import json
import urllib
from datetime import datetime
from urllib.request import urlopen

import os.path
from pathlib import Path
import bs4
from selenium import webdriver
from bs4 import BeautifulSoup
import time
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from webdriver_manager.chrome import ChromeDriverManager

BASE_CHESSABLE_URL = 'https://www.chessable.com/'
TESTING_PROFILE_BASE_DIR = "C:/Users/john/AppData/Local/Google/Chrome for Testing/User Data"
TESTING_PROFILE = "Default"
HTML_CACHE_PATH = './html/'

class chessable:


    @classmethod
    def getCourseDetail(cls, courseId: str, profileName: str):
        # should return a map of chapter IDs and chapter names
        # courses are located at /course/<courseID>
        # chapters are located at /course/<courseID>/<chapterID>
        # individual variations are located at /variation/<variationID>
        # i don't need pgn for anything but variations.
        # getting course detail amounts to:
        #  getting the metadata about the course (name, author, ??)
        #  getting the list of chapters associated with this course
        #  for each chapter in the course
        #   getting the metadata about the chapter (name, priority, ??)
        #   getting the list of variations associated with this chapter
        #   for each variation in the chapter
        #    getting the detailed html for this variation
        #    generate the pgn for this detailed html
        #    do something with the pgn

        bs = chessable.getCourseHtml(courseId, profileName)
        chapters = chessable.getCourseChapters(bs)

        return bs, chapters

    @classmethod
    def getChapterDetail(cls, courseId: str, chapterBs: bs4.element.Tag, profileName: str):
        href = chapterBs.find('a', href=True)['href']
        tags = href.split('/')
        chapterID = tags[len(tags)-1]
        #print("In GetChapterDetail "+courseId+"-"+chapterID +" Getting HTML")
        bs = chessable.getChapterHtml(courseId, chapterID, profileName)
        #print("In GetChapterDetail "+courseId+"-"+chapterID +" Getting Vars")
        variations = chessable.getChapterVariations(bs)
        #print("In GetChapterDetail "+courseId+"-"+chapterID +" Returning")

        return bs, variations

    @classmethod
    def getCourseChapters(cls, bs: BeautifulSoup):
        chapters = bs.find_all("div", class_="chapter")
        return chapters

    @classmethod
    def getCourseName(cls, bs: BeautifulSoup):
        name = bs.find("title").text[:-12]  # strip off the trailing " - Chessable" from the title
        return name

    @classmethod
    def getChapterVariations(cls, bs: BeautifulSoup):
        variations = bs.find_all("div", class_="variation-card__row--main")
        return variations

    @classmethod
    def getChapterName(cls, tag: bs4.element.Tag):
        name = tag.find('div', class_="toBeClamped title").text
        return name

    @classmethod
    def getVariationDetail(cls, courseId: str, variationBs: bs4.element.Tag, profileName: str):
        name = variationBs.find('a', href=True).text
        href = variationBs.find('a', href=True)['href']
        tags = href.split('/')
        variationID = tags[len(tags)-2]
        print("In GetVariationDetail '"+courseId+"-"+variationID +"-"+name)
        bs = chessable.getVariationHtml(variationID, courseId, profileName)

        return [bs, variationID]

    @classmethod
    def getVariationHtml(cls, variationId: str, courseId:str, profileName:str):
        return chessable.getHtml("variation", variationId, profileName, "course/"+str(courseId) )

    @classmethod
    def getVariationParts(cls, variationBs: bs4.element.Tag):
        try:
            name = variationBs.find('div', id="theOpeningTitle").text
            chapter = variationBs.find('div', class_="allOpeningDetails").find_all("li")
            moves = variationBs.find('div', id="theOpeningMoves").findChildren("span", recursive=False)
            return name, chapter, moves
        except:
            print("problem parsing variation parts\n----------\n"+str(variationBs)+"\n----------\n")
            return "",None,None


    @classmethod
    def getChapterHtml(cls, courseId: str, chapterId: str, profileName):
        return chessable.getHtml("course", courseId+"/"+chapterId, profileName )

    @classmethod
    def getCourseHtml(cls, courseId: str, profileName: str):
        return chessable.getHtml("course", courseId, profileName )

    @classmethod
    def getHtml(cls, elementType, elementId: str, profileName: str, fileroot=""):
        location = elementType
        if elementId != "":
            location += "/" + elementId
        url = BASE_CHESSABLE_URL + location
        if fileroot != "":
            location = fileroot + "/" + location

        # if the file already exists, load it
        pageHtml = chessable.loadHtmlFromFile(location)
        # otherwise get it from the web
        if len(pageHtml) == 0:
            pageHtml = chessable.loadHtmlFromWeb(url, profileName)
            chessable.writeHtmlToFile(location, pageHtml)

        bs = BeautifulSoup(pageHtml, 'html.parser')
        return bs

    @classmethod
    def loadHtmlFromFile(self, location):
        if not os.path.exists(HTML_CACHE_PATH+location+".html"):
            return ""
        with open(HTML_CACHE_PATH+location+".html", "r", encoding='utf-8') as file:
            return file.read()

    @classmethod
    def writeHtmlToFile(self, location, content):
        path = Path(HTML_CACHE_PATH+location[:location.rfind("/")])
        path.mkdir(parents=True, exist_ok=True)
        with open(HTML_CACHE_PATH+location+".html", "w", encoding='utf-8') as file:
            return file.write(content)

    @classmethod
    def loadHtmlFromWeb(self, url, profileName):
        #print("Reading "+url+" using "+profileName)
        for retry in range(3):
            try:
                options = webdriver.ChromeOptions()
                options.add_argument('headless')
                options.binary_location = "C:/Users/john/chrome/win64-135.0.7049.114/chrome-win64/chrome.exe"
                options.add_argument('--user-data-dir='+TESTING_PROFILE_BASE_DIR)
                options.add_argument('--profile-directory='+profileName) # TESTING_PROFILE)
                #if chessable.svc == None:
                #    chessable.svc = Service(ChromeDriverManager().install())
                #svc = Service("C:/Users/john/chromedriver/win64-135.0.7049.114/chromedriver-win64/chromedriver.exe")
                #svc = Service("C:/Users/john/chromedriver/win64-137.0.7143.0/chromedriver-win64/chromedriver.exe")
                #browser = webdriver.Chrome(service=svc, options=options)
                browser = webdriver.Chrome(options=options)
                browser.get(url)
                time.sleep(5)
                outText = browser.page_source
                browser.quit()
                #print("Quitting Session for "+profileName)
                return outText
            except:
                print("error in loadHtmlFromWeb on attempt "+str(retry))
