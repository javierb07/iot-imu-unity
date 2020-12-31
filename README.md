# IoT Inertial Measurement Unit Unity Controlled

An IoT inertial measurement unit that exposes data through WebSocket running on a server and a Unity program that connects to it uses data to rotate a game object in real time.

Tested with 9DoF Razor IMU M0. For a hookup guide look at: https://learn.sparkfun.com/tutorials/9dof-razor-imu-m0-hookup-guide

## Instructions:

1) Connect IMU to a server and run the websocket program. Take note of the IP address of the server. Optionally, port forward the websocket service to allow connection from any network. However, you will need a domain and a SSL certificate. Additionally, make sure that the IMU is streaming time, accelX, accelY, accelZ, quatW, quatX, quatY, quatZ (in that order), through the serial channel.

2) Open the index.html file and copy the websocket IP. Check that the measurements appear in the table.

3) Open the Unity program by clicking the Open Unity button. There, you will see a model of the device rotating accordingly to the real hardware.

### Example panel of a connected IMU through WebSocket:

![dashboard](https://github.com/javierb07/iot-imu-unity/blob/master/images/panel_example.JPG)

### Example Unity interface of a connected IMU through WebSocket:

![dashboard](https://github.com/javierb07/iot-imu-unity/blob/master/images/unity_example.JPG)
