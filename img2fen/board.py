import os
import cv2
import numpy as np
import matplotlib.pyplot as plt

from zipfile import ZipFile
from urllib.request import urlretrieve

# Read image to be aligned
#bdFilename = "images\Screenshot 2025-04-03 152735.png" #empty
#bdFilename = "images\Screenshot 2025-04-02 222146.png"
#bdFilename = "images\WIN_20250403_17_56_07_Pro.jpg"  #2x3
bdFilename = "images\silman-152.png" #kindle

print("Reading image to align:", bdFilename)
imbd = cv2.imread(bdFilename, cv2.IMREAD_COLOR)
imbd_gray = cv2.cvtColor(imbd, cv2.COLOR_BGR2GRAY)
imbd_gray = imbd_gray.astype(np.uint8)
retval, img_thresh_adp = cv2.threshold(imbd_gray, 140, 255, cv2.THRESH_BINARY)
#img_thresh_adp = cv2.adaptiveThreshold(imbd_gray, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY, 33, 9)
edges = cv2.Canny(img_thresh_adp, 120, 150)

MAX_NUM_FEATURES = 500
orb = cv2.ORB_create(MAX_NUM_FEATURES)
keypoints1, descriptors1 = orb.detectAndCompute(edges, None)

# Display
im1_display = cv2.drawKeypoints(imbd, keypoints1, outImage=np.array([]),
                                color=(255, 0, 0), flags=cv2.DRAW_MATCHES_FLAGS_DRAW_RICH_KEYPOINTS)

contours, _ = cv2.findContours(edges, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# Iterate through contours
for contour in contours:
    # Approximate the contour
    approx = cv2.approxPolyDP(contour, 0.04 * cv2.arcLength(contour, True), True)

    # Check if the contour has four sides
    if len(approx) == 4:
        # Draw a rectangle around the contour
        x, y, w, h = cv2.boundingRect(contour)
        #if w > 50 and abs( w-h < 25):
        cv2.rectangle(imbd, (x, y), (x + w, y + h), (0, 255, 0), 2)

        sq_off = 6.0
        sq = (w-2*sq_off)/8.0
        for r in range(8):
            for c in range(8):
                cv2.rectangle(imbd, (int(x+sq_off+r*sq), int(y+sq_off+c*sq)), (int(x+sq_off+r*sq+sq), int(y+sq_off+c*sq+sq)), (0, 0, 255), 2)

plt.figure(figsize=[20, 10]);
plt.subplot(131); plt.axis('off'); plt.imshow(imbd, cmap="gray"); plt.title("diagram")
plt.subplot(132); plt.axis('off'); plt.imshow(img_thresh_adp, cmap="gray"); plt.title("to binary")
plt.subplot(133); plt.axis('off'); plt.imshow(edges, cmap="gray"); plt.title("edges")
plt.show()

