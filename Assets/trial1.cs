using UnityEngine;
using System;
using System.IO.Ports;

public class VibrationController : MonoBehaviour
{
    public string COM = "COM5"; // Change this to the appropriate COM port
    public float vibrationThreshold = 1.0f; // Adjust this threshold to control vibration sensitivity
    public float g = 0.001f; // 1 milliG equals 0.001 G
    public string receivedData; // Variable to store received data

    private SerialPort serialPort;
    private float prevX = 0f;
    private float prevY = 0f;
    private float prevZ = 0f;

    void Start()
    {
        serialPort = new SerialPort(COM, 9600);
        OpenSerialPort();
    }

    void OpenSerialPort()
    {
        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                string dataString = serialPort.ReadLine();
                receivedData = dataString; // Store received data in public variable

                string[] values = dataString.Split(',');

                if (values.Length != 3)
                {
                    Debug.LogWarning("Invalid data format received from serial port: " + dataString);
                    return;
                }

                float x, y, z;
                if (float.TryParse(values[0], out x) && float.TryParse(values[1], out y) && float.TryParse(values[2], out z))
                {
                    // Convert the values from milliG to G
                    x *= g;
                    y *= g;
                    z *= g;

                    // Check if there's a significant change in G-force values
                    if (Mathf.Abs(x - prevX) > 0.01f || Mathf.Abs(y - prevY) > 0.01f || Mathf.Abs(z - prevZ) > 0.01f)
                    {
                        VibrateObject();
                        prevX = x;
                        prevY = y;
                        prevZ = z;
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to parse G-force values from serial data: " + dataString);
                }
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("Timeout while reading from serial port.");
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading from serial port: " + e.Message);
            }
        }
    }

    void VibrateObject()
    {
        // Add your vibration logic here
        // For example, you can apply a force to the object to simulate vibration
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 vibrationForce = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            rb.AddForce(vibrationForce, ForceMode.Impulse);
        }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }
}
