import os
import cv2
import numpy
import numpy as np
import matplotlib.pyplot as plt

from zipfile import ZipFile
from urllib.request import urlretrieve

from cv2 import drawKeypoints
from cv2.typing import Size

targetSquareSize = 20

def processDirectory(dir):
    for f in os.listdir(dir):
        print(os.path.join(dir, f))
        img, gray = readImage(os.path.join(dir, f))
        unskewed, keypoints, rectangles, skewPoints = processImage(img, gray)
        plotImage(img, unskewed, keypoints, rectangles, skewPoints)

def checkTargetSquareSizes(dir):
    global targetSquareSize
    # ok - take the first image in the directory and iterate over various square sizes
    for targetSquareSize in range(15,31,5):
        for fn in range(2):
            f = os.listdir(dir)[fn]
            print(os.path.join(dir, f))
            img, gray = readImage(os.path.join(dir, f))
            unskewed, keypoints, rectangles, skewPoints = processImage(img, gray)
            plotImage(img, unskewed, keypoints, rectangles, skewPoints)

def findEdges(img):
    # retval, img_thresh_adp = cv2.threshold(imbd_gray, 140, 255, cv2.THRESH_BINARY)
    img_thresh = cv2.adaptiveThreshold(img.astype(np.uint8), 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY, 33, 9)
    edges = cv2.Canny(img_thresh, 120, 150)
    return img_thresh, edges


def findFeatures(edges):
    MAX_NUM_FEATURES = 500
    orb = cv2.ORB_create(MAX_NUM_FEATURES)
    return orb.detectAndCompute(edges, None)


def findBoardRectangle(contours):
    outSet = []
    for contour in contours:
        # Approximate the contour
        approx = cv2.approxPolyDP(contour, 0.04 * cv2.arcLength(contour, True), True)

        # Check if the contour has four sides
        if len(approx) >= 4:
            # Draw a rectangle around the contour
            sq_x, sq_y, sq_w, sq_h = cv2.boundingRect(contour)
            if sq_w > 600 and sq_h > 600:
                outSet.append([sq_x, sq_y, sq_w, sq_h])
    return outSet

def findSkewRectangle(contours):
    outSet = []
    for contour in contours:
        # Approximate the contour
        approx = cv2.approxPolyDP(contour, 0.04 * cv2.arcLength(contour, True), True)

        # Check if the contour has four sides
        if len(approx) == 4:
            # Draw a rectangle around the contour
            sq_x, sq_y, sq_w, sq_h = cv2.boundingRect(contour)
            if sq_w > 600 and sq_h > 600:
                outSet.append(approx)
    return outSet


def warpImage(image, corners, target, width, height):
    transform = cv2.getPerspectiveTransform(np.float32(corners), np.float32(target))
    out = cv2.warpPerspective(image, transform, (width, height))
    return out

def unSkewRectangle(src, corners):
    # the corners move around the square - have to be revisited...
    # top left has smallest x,y
    # top right has largest x with smallest y (max x-x0)
    # bottom right has smallest x with largest y (max x-x0)
    # center point is sum(x)/4, sum(y/4), tl is in Q2, tr in Q1, br in Q4, bl in Q3
    centerx = (corners[0][0][0]+corners[1][0][0]+corners[3][0][0]+corners[2][0][0]) / 4
    centery = (corners[0][0][1]+corners[1][0][1]+corners[3][0][1]+corners[2][0][1]) / 4
    tlIndex= -1
    trIndex= -1
    blIndex= -1
    brIndex= -1
    for index in range(4):
        if corners[index][0][0] < centerx and corners[index][0][1] > centery:
            blIndex = index
        if corners[index][0][0] > centerx and corners[index][0][1] > centery:
            brIndex = index
        if corners[index][0][0] > centerx and corners[index][0][1] < centery:
            trIndex = index
        if corners[index][0][0] < centerx and corners[index][0][1] < centery:
            tlIndex = index

    if tlIndex <0 or trIndex<0 or blIndex<0 or brIndex<0:
        return None

    # now we can ensure the right orientation
    source = [corners[tlIndex][0], corners[trIndex][0], corners[blIndex][0], corners[brIndex][0]]
    imgWidth = targetSquareSize*8.0+2.0
    target = [(0.0, 0.0), (imgWidth, 0.0), (0.0, imgWidth), (imgWidth, imgWidth)]
    return warpImage(src, source, target, int(imgWidth), int(imgWidth))[1:int(imgWidth-1), 1:int(imgWidth-1)]

def readImage(bdFilename):
    print("Reading image to align:", bdFilename)
    imbd = cv2.imread(bdFilename, cv2.IMREAD_COLOR)
    gray = cv2.imread(bdFilename, cv2.IMREAD_GRAYSCALE)
    return imbd, gray

def processImage(imbd, img_gray):    # process image
    thresh, edges = findEdges(img_gray)
    keypoints, descriptors = findFeatures(edges)
    contours, _ = cv2.findContours(edges, cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)
    rectangles = findBoardRectangle(contours)
    skewPoints = findSkewRectangle(contours)
    for sp in skewPoints:
        distorted = unSkewRectangle(img_gray, sp)
        if distorted is not None:
            break

    return distorted, keypoints, rectangles, skewPoints

def brightenImage(gray):
    clip_hist_percent = 1

    # Calculate grayscale histogram
    hist = cv2.calcHist([gray], [0], None, [256], [0, 256])
    hist_size = len(hist)

    # Calculate cumulative distribution from the histogram
    accumulator = []
    accumulator.append(float(hist[0]))
    for index in range(1, hist_size):
        accumulator.append(accumulator[index - 1] + float(hist[index]))

    # Locate points to clip
    maximum = accumulator[-1]
    clip_hist_percent *= (maximum / 100.0)
    clip_hist_percent /= 2.0

    # Locate left cut
    minimum_gray = 0
    while accumulator[minimum_gray] < clip_hist_percent:
        minimum_gray += 1

    # Locate right cut
    maximum_gray = hist_size - 1
    while accumulator[maximum_gray] >= (maximum - clip_hist_percent):
        maximum_gray -= 1

    # Calculate alpha and beta values
    alpha = 255 / (maximum_gray - minimum_gray)
    beta = -minimum_gray * alpha

    auto_result = cv2.convertScaleAbs(gray, alpha=alpha, beta=beta)
    return (auto_result, alpha, beta)


    # Display
def plotImage(img, unskewed, keypoints, rectangles, skewPoints):
    drawKeypointsOnImage = True
    drawSquaresOnImage = True
    drawSkewSquaresOnImage = True
    drawGridOnImage = False
    drawGridOnDistortedImage = True
    drawThresholdOnUnskewed = True


    outDisplay = img
    if drawKeypointsOnImage:
        cv2.drawKeypoints(outDisplay, keypoints, outImage=np.array([]),
                          color=(255, 0, 0), flags=cv2.DRAW_MATCHES_FLAGS_DRAW_RICH_KEYPOINTS)
    if drawSquaresOnImage:
        for [x,y,w,h] in rectangles:
            cv2.rectangle(outDisplay, (x, y), (x + w, y + h), (0, 255, 0), 2)

            if drawGridOnImage:
                dx = w / 8.0
                dy = h / 8.0
                for r in range(8):
                    for c in range(8):
                        cv2.rectangle(outDisplay, (int(x + c * dx), int(y + r * dy)),
                                      (int(x + c * dx + dx), int(y + r * dy + dy)), (0, 0, 255), 2)

    if drawSkewSquaresOnImage:
        for point in skewPoints[0]:
            x, y = point[0]
            cv2.circle(outDisplay, (x, y), 3, (255, 0, 0), -1)
        cv2.drawContours(outDisplay, [skewPoints[0]], -1, (255, 0, 0))

    bright = unskewed
    a = 10
    while a> 1.005:
        (bright,a,b) = brightenImage(bright)

    if drawGridOnDistortedImage:
        s = 8.0*targetSquareSize
        dx = s / 8.0
        dy = s / 8.0
        for r in range(8):
            for c in range(8):
                cv2.rectangle(unskewed, (int(c * dx), int(r * dy)),
                              (int(c * dx + dx), int(r * dy + dy)), (0, 0, 255), 1)

    plt.figure(figsize=[20, 10])
    plt.subplot(141)
    plt.axis('off')
    plt.imshow(outDisplay, cmap="gray")
    plt.title("diagram")
    plt.subplot(143)
    plt.axis('off')
    plt.imshow(bright, cmap="gray")
    plt.title("brightened")
    plt.subplot(142)
    plt.axis('off')
    plt.imshow(unskewed, cmap="gray")
    plt.title("unskewed - "+str(targetSquareSize))
    if drawThresholdOnUnskewed:
        img_thresh = cv2.adaptiveThreshold(unskewed.astype(np.uint8), 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY, 23, 9) # int(targetSquareSize/2)*2+1), 9)
        plt.subplot(144)
        plt.axis('off')
        plt.imshow(img_thresh, cmap="gray")
        plt.title("threshold - "+str(targetSquareSize))

    plt.show()

    return


def main():
    processDirectory("./images/CTFS Full Boards")
    #checkTargetSquareSizes("./images/CTFS Full Boards")

    processDirectory("./images/RYAC Full Boards")
    #checkTargetSquareSizes("./images/RYAC Full Boards")

    # phase 0 is complete, I can find boards in images and reasonably normalize them
    # subjectively 20 looks like the winner, and brightened by 1% until a < 1.005
    # "clean" images cut from a digital source (web or book) are much crisper than camera images
    # I think a model trained on camera images will do ok on web images, but not v/v - (prove?)

    # phase 1 is data collection
    # take the images for at least 50-100 diagrams from camera, break them into squares and label them
    # Can probably get thousands from lichess, that I can automatically generate and labeled...
    # 100 boards is about 100 samples for each piece type, maybe 1000 for Ps and 4000 empty squares
    # actually should be enough to get started

    # phase 2 is model building
    # then build a GPU aware regular model (20x20 inputs, 26 outputs - 2 background, (empty + 2 piece color * 6 piece types))
    # then build a GPU aware CNN (20x20 inputs, 24 outputs - 2 background, 2 color piece, 6 piece type)
    # compare the results (write this up...)

    # phase 3 is model serving and FEN creation

if __name__ == "__main__":
    main()