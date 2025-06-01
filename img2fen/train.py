import mnist_loader
import board_loader
import network

if False:
    training_data, validation_data, test_data = mnist_loader.load_data_wrapper()
    net = network.Network([784, 20, 10]) # [784, 100, 10]
    net.SGD(training_data, 30, 40, 3.0, test_data=test_data)
elif False:
    training_data, validation_data, test_data = board_loader.load_data_wrapper()
    net = network.Network([784, 50, 26]) # [784, 100, 10]
    net.SGD(training_data, 40, 40, 0.95, test_data=test_data)
    print("Validation: {0} / {1}".format(
        net.evaluate(validation_data), len(validation_data) ))
else:
    training_data, validation_data, test_data = board_loader.load_data_wrapper()
    net = network.Network([784, 100, 26]) # [784, 100, 10]
    net.SGD(training_data, 60, 40, 0.95, test_data=test_data)
    print("Validation: {0} / {1}".format(
        net.evaluate(validation_data), len(validation_data) ))




