"""
board_loader
~~~~~~~~~~~~

based on minst_loader.  For now I have 14k tests, so we;ll use 8k train, 3k test 3k validation
instead of 50k 10k 10k

A library to load the MNIST image data.  For details of the data
structures that are returned, see the doc strings for ``load_data``
and ``load_data_wrapper``.  In practice, ``load_data_wrapper`` is the
function usually called by our neural network code.
"""

#### Libraries
# Standard library
import pickle as cPickle
import gzip

# Third-party libraries
import numpy as np
import os
import cv2
import numpy as np


def load_data():
    """Return the MNIST data as a tuple containing the training data,
    the validation data, and the test data.

    The ``training_data`` is returned as a tuple with two entries.
    The first entry contains the actual training images.  This is a
    numpy ndarray with 50,000 entries.  Each entry is, in turn, a
    numpy ndarray with 784 values, representing the 28 * 28 = 784
    pixels in a single MNIST image.

    The second entry in the ``training_data`` tuple is a numpy ndarray
    containing 50,000 entries.  Those entries are just the digit
    values (0...9) for the corresponding images contained in the first
    entry of the tuple.

    The ``validation_data`` and ``test_data`` are similar, except
    each contains only 10,000 images.

    This is a nice data format, but for use in neural networks it's
    helpful to modify the format of the ``training_data`` a little.
    That's done in the wrapper function ``load_data_wrapper()``, see
    below.
    """

    image_data, label_data = load_all_square_data("./images/CTFS Boards/Square Images")

    training_values = image_data[0:8000]
    training_labels = label_data[0:8000]
    training_data = [training_values, training_labels]

    validation_values =image_data[8000:11000]
    validation_labels =label_data[8000:11000]
    validation_data = [validation_values, validation_labels]

    test_values = image_data[11000:14000]
    test_labels = label_data[11000:14000]
    test_data = [test_values, test_labels]

    return (training_data, validation_data, test_data)


def load_data_wrapper():
    """Return a tuple containing ``(training_data, validation_data,
    test_data)``. Based on ``load_data``, but the format is more
    convenient for use in our implementation of neural networks.

    In particular, ``training_data`` is a list containing 50,000
    2-tuples ``(x, y)``.  ``x`` is a 784-dimensional numpy.ndarray
    containing the input image.  ``y`` is a 10-dimensional
    numpy.ndarray representing the unit vector corresponding to the
    correct digit for ``x``.

    ``validation_data`` and ``test_data`` are lists containing 10,000
    2-tuples ``(x, y)``.  In each case, ``x`` is a 784-dimensional
    numpy.ndarry containing the input image, and ``y`` is the
    corresponding classification, i.e., the digit values (integers)
    corresponding to ``x``.

    Obviously, this means we're using slightly different formats for
    the training data and the validation / test data.  These formats
    turn out to be the most convenient for use in our neural network
    code."""
    tr_d, va_d, te_d = load_data()
    training_inputs = [np.reshape(x, (784, 1)) for x in tr_d[0]]
    training_results = [vectorized_result(y, 26) for y in tr_d[1]]
    training_data = zip(training_inputs, training_results)
    validation_inputs = [np.reshape(x, (784, 1)) for x in va_d[0]]
    validation_data = zip(validation_inputs, va_d[1])
    test_inputs = [np.reshape(x, (784, 1)) for x in te_d[0]]
    test_data = zip(test_inputs, te_d[1])
    return (list(training_data), list(validation_data), list(test_data))


def vectorized_result(j, tSize):
    """Return a 10-dimensional unit vector with a 1.0 in the jth
    position and zeroes elsewhere.  This is used to convert a digit
    (0...9) into a corresponding desired output from the neural
    network."""
    e = np.zeros((tSize, 1))
    e[j] = 1.0
    return e


def load_all_square_data(sqDir):
    lf = open(sqDir+"/labels.txt", "rt")
    lines = lf.readlines()

    image_holder = np.ndarray(shape=(15000), dtype=np.ndarray)
    label_holder = np.ndarray(shape=(15000), dtype=np.ndarray)

    thisElt = 0
    for line in lines:
        tags = line.split(", ")
        sf = tags[0]
        label = tags[2]
        gray = readImage(os.path.join(sqDir, sf))
        flat = gray.reshape(-1)
        image_holder[thisElt] = flat / 256.0
        label_holder[thisElt] = int(label[:-1])
        thisElt += 1

    return image_holder, label_holder

def readImage(bdFilename):
    gray = cv2.imread(bdFilename, cv2.IMREAD_GRAYSCALE)
    return gray
