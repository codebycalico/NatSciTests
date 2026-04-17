import numpy as np
import cv2
import pyautogui

kinect_cam = cv2.VideoCapture(0, cv2.CAP_DSHOW)

# roughly define the upper right and lower left
# bounds of the sandbox
upperBound_x, upperBound_y, lowerBound_x, lowerBound_y = 400, 100, 1500, 1400

# hold the circle from the previous frame
prevCircle = None

# function to calculate distance between two circles in the list
dist = lambda x1, y1, x2, y2: (x1 - x2) ** 2 + (y1 - y2) ** 2

while True:
    ret, frame = kinect_cam.read()
    pyautogui.FAILSAFE = False

    # flip kinect live video horizontally
    frame = cv2.flip(frame, 0)

    # rotate kinect cam feed to match the Unity projection
    #frame = cv2.rotate(frame, cv2.ROTATE_90_CLOCKWISE)

    # gray scale the kinect video
    gray_kinect_cam = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    # get rid of some noise
    blurFrame = cv2.GaussianBlur(gray_kinect_cam, (15, 15), 0)

    # 1.2 = dp, the precision and accuracy
    # 700 = the minimum distance between the center of two circles,
    # if only detecting one circle, have a high number, if detecting 
    # multiple circles, lower the number
    # param1 = sensitivity - too high: won't detect enough circles,
    # too low = will detect too many circles 
    # param2 = accuracy - number of points / edges needed to declare a 
    # circle is present - too low: detect too many circles,
    # too high: detect too few circles
    # minRadius = minimum size of circle to be detected (if farther from webcam)
    # maxRadius = maximum size of circle to be detected (if closer to webcam)
    # circles returns list of circles found
    circles = cv2.HoughCircles(blurFrame, cv2.HOUGH_GRADIENT, 1.2, 700,
                               param1=100, param2=50, minRadius=25, maxRadius=75)
    
    # if we have found a circle
    if circles is not None:
        circles = np.uint16(np.around(circles))
        # chosen holds the circle to compare other circles to
        chosen = None
        for i in circles[0, :]:
            if ( upperBound_x < i[0] < lowerBound_x ) and ( upperBound_y < i[1] < lowerBound_y ):
                if chosen is None:
                    chosen = i
                if prevCircle is not None:
                    # if we find a circle that is closer in distance, set as new center
                    #if dist(chosen[0], chosen[1], prevCircle[0], prevCircle[1]) <= dist(i[0], i[1], prevCircle[0], prevCircle[1]):
                    if ( upperBound_x <= chosen[0] < (upperBound_x + lowerBound_x) ) and ( upperBound_y <= chosen[1] < ( upperBound_y + lowerBound_y) ):
                        chosen = i
            # check if circle is within the sand"box" rectangle
            if chosen is not None:
                # draw the chosen circle's center
                cv2.circle(frame, (chosen[0], chosen[1]), 1, (0, 100, 100), 3)
                # draw around the circumference of the chosen circle
                cv2.circle(frame, (chosen[0], chosen[1]), chosen[2], (255, 0, 255), 3)
                pyautogui.moveTo(chosen[0] + chosen[2], chosen[1] + chosen[2], 0)
                pyautogui.mouseDown(button='left')
                prevCircle = chosen

    pyautogui.mouseUp(button='left')
    # show the output in the cam
    cv2.rectangle(frame, (upperBound_x, upperBound_y), (lowerBound_x, lowerBound_y), (255, 0, 0), 5)
    cv2.imshow("circles", gray_kinect_cam)
        
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

kinect_cam.release()
cv2.destroyAllWindows()