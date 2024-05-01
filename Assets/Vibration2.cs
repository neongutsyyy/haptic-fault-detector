using UnityEngine;
using System.IO.Ports;

public class VibrationSimulator : MonoBehaviour
{
    public Rigidbody rb;
    public float threshold = 10; // Adjust as needed
    public float sensitivity = 0.1f; // Adjust as needed

    SerialPort stream;
    string receivedData;
    string[] dataArray;

    Vector3 previousValues = Vector3.zero;

    void Start()
    {
        stream = new SerialPort("COM5", 31250); // Adjust COM port and baud rate if necessary
        stream.Open();

        // Calibration routine (optional, depending on your sensor)
        // ... (previous calibration code, if needed) ... 
    }

    void Update()
    {
        receivedData = stream.ReadLine();
        dataArray = receivedData.Split(',');

        if (dataArray.Length == 3)
        {
            float x = float.Parse(dataArray[0]);
            float y = float.Parse(dataArray[1]);
            float z = float.Parse(dataArray[2]);

            // Subtract 1g offset from y-axis (adjust based on your sensor)
            y -= 1000.0f / 10.0f;

            Debug.Log("COM Port Values (milliG): X: " + x + ", Y: " + y + ", Z: " + z);

            Vector3 currentValues = new Vector3(x, y, z);

            // Calculate individual axis differences
            float deltaX = Mathf.Abs(currentValues.x - previousValues.x);
            float deltaY = Mathf.Abs(currentValues.y - previousValues.y);
            float deltaZ = Mathf.Abs(currentValues.z - previousValues.z);

            // Check if ALL deltas are below the threshold
            if (deltaX < threshold && deltaY < threshold && deltaZ < threshold)
            {
                rb.velocity = Vector3.zero; // Stop the Rigidbody
            }
            else 
            {
                // Calculate acceleration
                Vector3 acceleration = (currentValues - previousValues) * sensitivity;

                // Apply forces individually based on threshold
                if (deltaX > threshold)
                {
                    rb.AddForce(acceleration.x * sensitivity, 0, 0, ForceMode.Acceleration);
                }
                if (deltaY > threshold)
                {
                    rb.AddForce(0, acceleration.y * sensitivity, 0, ForceMode.Acceleration);
                }
                if (deltaZ > threshold)
                {
                    rb.AddForce(0, 0, acceleration.z * sensitivity, ForceMode.Acceleration);
                }
            }

            rb.position = Vector3.zero;

            previousValues = currentValues;
        }
    }

    void OnApplicationQuit()
    {
        stream.Close();
    }
}