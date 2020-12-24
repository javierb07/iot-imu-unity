import asyncio
import sys
import glob
import websockets
import serial
import socket


def serial_ports():
    """ Lists serial port names
        :raises EnvironmentError:
            On unsupported or unknown platforms
        :returns:
            A list of the serial ports available on the system
    """
    if sys.platform.startswith('win'):
        ports = ['COM%s' % (i + 1) for i in range(256)]
    elif sys.platform.startswith('linux') or sys.platform.startswith('cygwin'):
        # this excludes your current terminal "/dev/tty"
        ports = glob.glob('/dev/tty[A-Za-z]*')
    elif sys.platform.startswith('darwin'):
        ports = glob.glob('/dev/tty.*')
    else:
        raise EnvironmentError('Unsupported platform')
    result = []
    for port in ports:
        try:
            s = serial.Serial(port)
            s.close()
            result.append(port)
        except (OSError, serial.SerialException):
            pass
    return result


try:
    port = serial_ports()[0]
except IndexError:
    print("No ports available")
    sys.exit()

serialComm = serial.Serial(port, 115200, timeout=1)
imuData = []
port = 5678
ipAddress = socket.gethostbyname(socket.gethostname())
print(ipAddress)

async def imu(websocket, path):
    while True:
        imuData = serialComm.readline().decode('ascii')
        print(imuData)
        await websocket.send(imuData)
        await asyncio.sleep(0.05)

start_server = websockets.serve(imu, ipAddress, port)
asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()