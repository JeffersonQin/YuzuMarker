import numpy as np


def reverse(src):
    return np.zeros(src.shape, dtype=src.dtype) + 255 - src


def minAreaNonRotatedRectangle(points):
    y_1 = points[:, 0, 0].min()
    y_2 = points[:, 0, 0].max()
    x_1 = points[:, 0, 1].min()
    x_2 = points[:, 0, 1].max()
    return ((int((y_1 + y_2) / 2), int((x_1 + x_2) / 2)),
            (int(x_2 - x_1), int(y_2 - y_1)), -90)
