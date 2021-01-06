using UnityEngine;
using System.Threading;
using System.Net.WebSockets;
using System.Text;
using System;
using UnityEngine.UI;

public class IMU : MonoBehaviour
{
    // Websocket variables declaration
    ClientWebSocket RPi4Socket = null;
    ArraySegment<byte> buf = new ArraySegment<byte>(new byte[2048]);

    // Movement update variables declaration
    string data;
    Quaternion imu_gyro;
    Vector3 imu_accel;
    Vector3 velocity = Vector3.zero;
    Vector3 position_update;
    float timeF;
    float timeI;
    bool timeObtained = false;
    float g = 9.81f;

    // UI Display Variables
    [SerializeField] Text timeK;
    [SerializeField] Text accelX;
    [SerializeField] Text accelY;
    [SerializeField] Text accelZ;
    [SerializeField] Text quatW;
    [SerializeField] Text quatX;
    [SerializeField] Text quatY;
    [SerializeField] Text quatZ;
    [SerializeField] GameObject websocketInput;
    string websocketURL;
    public void OnPressButton()
    {
        InputField input = websocketInput.GetComponent<InputField>();
        websocketURL = input.text;
        if (websocketURL != "")
        {
            Connect();
        }
    }
    async void Connect()
    {
        Uri RPiURL = new Uri(websocketURL);
        RPi4Socket = new ClientWebSocket();
        try
        {
            await RPi4Socket.ConnectAsync(RPiURL, CancellationToken.None);
            if (RPi4Socket.State == WebSocketState.Open)
            {
                Debug.Log("IMU Websocket Connection Established");
            }
            GetMeasurements();
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
    }
    async void GetMeasurements()
    {
        while (Application.isPlaying)
        {
            // Recieve Data
            WebSocketReceiveResult result = await RPi4Socket.ReceiveAsync(buf, CancellationToken.None);
            Debug.Log("received");
            data = Encoding.UTF8.GetString(buf.Array, 0, result.Count);
            string[] streamData = data.Split(','); // Splits the data
            string time = streamData[0];
            timeF = float.Parse(time);
            string[] accelPosition = new string[3] { streamData[1], streamData[2], streamData[3] };
            string[] gyroPosition = new string[4] { streamData[4], streamData[5], streamData[6], streamData[7] };
            timeK.text = time;
            accelX.text = streamData[1];
            accelY.text = streamData[2];
            accelZ.text = streamData[3];
            quatW.text = streamData[4];
            quatX.text = streamData[5];
            quatY.text = streamData[6];
            quatZ.text = streamData[7];
            imu_gyro = new Quaternion(float.Parse(gyroPosition[0]), float.Parse(gyroPosition[1]), float.Parse(gyroPosition[2]), float.Parse(gyroPosition[3]));
            // Update game object
            Update_Rotation(imu_gyro);
            // Update_Position(imu_gyro, accelPosition);
        }
    }
    void Update_Rotation(Quaternion gyroPosition)
    {
        transform.rotation = new Quaternion(imu_gyro[1], imu_gyro[0], imu_gyro[2], imu_gyro[3]); // adjust IMU rotatation to Unity axis
    }
    void Update_Position(Quaternion imu_gyro, string[] accelPosition)
    {
        imu_accel = new Vector3(float.Parse(accelPosition[0]) * g, float.Parse(accelPosition[1]) * g, float.Parse(accelPosition[2]) * g);
        imu_accel = Compensate_Gravity(imu_gyro, imu_accel);
        velocity += imu_accel * (timeF - timeI) * 0.001f;
        position_update += velocity * (timeF - timeI) * 0.001f;
        //Debug.Log(Compensate_Gravity(imu_gyro, imu_accel));
        //Debug.Log((timeF - timeI) * 0.001f);
        timeI = timeF;
        if (timeF - timeI < 1)
        {
            transform.position = position_update;
        }
    }
    public Vector3 Compensate_Gravity(Quaternion q, Vector3 acc)
    {
        /* Compensate the accelerometer readings from gravity. 
           @param q the quaternion representing the orientation of a 9DOM MARG sensor array
           @param acc the readings coming from an accelerometer expressed in g
           @return a 3d vector representing dinamic acceleration expressed in g
        */
        Vector3 g = new Vector3(0.0f, 0.0f, 0.0f);
        // Get expected direction of gravity
        g[0] = 2 * (q[1] * q[3] - q[0] * q[2]);
        g[1] = 2 * (q[0] * q[1] + q[2] * q[3]);
        g[2] = q[0] * q[0] - q[1] * q[1] - q[2] * q[2] + q[3] * q[3];
        // Compensate accelerometer readings with the expected direction of gravity
        Vector3 a = new Vector3(acc[0] - g[0], acc[1] - g[1], acc[2] - g[2]);
        return a;
    }
    public void doExitGame()
    {
        Application.Quit();
    }
}
