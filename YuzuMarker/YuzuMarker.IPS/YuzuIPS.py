from fastapi import FastAPI
from typing import Optional
import uvicorn
from matplotlib import pyplot as plt
import cv2
import numpy as np
from utils import cv
from utils import peak
from utils import helper

host = "127.0.0.1"
port = 1029

app = FastAPI()


@app.get('/')
def read_root():
    return {"Hello": "World"}


@app.get('/detect_peak_color_bgr')
def detect_peak_color_bgr(src: str,
                          mask: str,
                          thres: Optional[float] = 0.1,
                          min_dist: Optional[int] = 1,
                          thres_abs: Optional[bool] = False,
                          preferred_color_bgr: Optional[list] = [255, 255, 255]):
    try:
        img = cv2.imread(src)
        mask = cv2.imread(mask, cv2.IMREAD_GRAYSCALE)
        mask = cv2.threshold(mask, 0, 255, cv2.THRESH_BINARY)[1]

        ret = []
        for i in range(3):
            hist = cv2.calcHist([img], [i], mask, [256], [0, 256])
            peaks = peak.indexes(hist.squeeze(), thres, min_dist, thres_abs)
            ret.append(int(peaks[np.argmin(np.abs(peaks - preferred_color_bgr[i]))]))
        return helper.get_server_success_message(ret)
    except Exception as e:
        return helper.get_server_exception_message(e)


@app.get('/detect_text_naive')
def detect_text_naive(src: str,
                      text_direction: Optional[str] = 'v',
                      text_color: Optional[str] = 'b',
                      rotated_output: Optional[bool] = True,
                      ed_iteration: Optional[int] = 3,
                      area_threshold: Optional[int] = 500,
                      kernel_ratio: Optional[float] = 0.005):
    img = cv2.imread(src)
    plt.imshow(img)
    plt.show()
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    ret, binary = cv2.threshold(gray, 0, 255, cv2.THRESH_OTSU + cv2.THRESH_BINARY)

    if text_color == 'b':
        binary = cv.reverse(binary)

    kernel_size_x = 1
    kernel_size_y = 1
    if text_direction == 'v':
        kernel_size_y = int(img.shape[1] * kernel_ratio)
    else:
        kernel_size_x = int(img.shape[1] * kernel_ratio)

    kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (kernel_size_x, kernel_size_y))

    dilation = cv2.dilate(binary, kernel, iterations=ed_iteration)
    erosion = cv2.erode(dilation, kernel, iterations=ed_iteration)

    plt.imshow(erosion, cmap=plt.cm.gray)

    contours, hierarchy = cv2.findContours(erosion, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    region = []

    for i in range(len(contours)):
        cnt = contours[i]
        # 计算该轮廓的面积
        area = cv2.contourArea(cnt)

        # 面积小的都筛选掉
        if (area < area_threshold):
            continue

        if rotated_output:
            # 找到最小的矩形，该矩形可能有方向
            rect = cv2.minAreaRect(cnt)
        else:
            rect = cv.minAreaNonRotatedRectangle(cnt)

        # box是四个点的坐标
        box = cv2.boxPoints(rect)
        box = np.int0(box)

        region.append(box)

    green_box = cv2.drawContours(img, region, -1, (0, 255, 0), 2)

    plt.imshow(green_box)
    plt.show()


if __name__ == '__main__':
    uvicorn.run(app, host=host, port=port)
