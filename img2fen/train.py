import mnist_loader
import network

training_data, validation_data, test_data = mnist_loader.load_data_wrapper()

net = network.Network([784, 20, 10]) # [784, 100, 10]

net.SGD(training_data, 30, 40, 3.0, test_data=test_data)

