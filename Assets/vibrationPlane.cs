using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class Cube : MonoBehaviour
{
  SerialPort data_stream = new SerialPort("COM5", 9600);
  public string receivedString;
  public Rigidbody rb;
  public float sens = 0.1f;
  public float previousG; // Store previous G value
  public float minimumGChange = 5.0f; // Minimum G change for movement

  void Start()
  {
    data_stream.Open();
    InvokeRepeating("Serial_Data_Reading", 0f, 0.01f); // Read data every 0.01 seconds
  }

  void Update()
  {
    // Directly extract X, Y, Z values within Serial_Data_Reading
    // and apply forces if successful
    Serial_Data_Reading();
  }

  int Serial_Data_Reading()
  {
    receivedString = data_stream.ReadLine();

    if (!string.IsNullOrEmpty(receivedString))
    {
      // Extract X, Y, Z values from received string
      float[] gValues = new float[3];  // Array to store X, Y, Z
      if (TryExtractXYZ(receivedString, gValues))
      {
        float currentG = CalculateG(gValues); // Calculate G from extracted values

        float gDifference = Mathf.Abs(currentG - previousG); // Absolute difference in G

        if (gDifference >= minimumGChange) // Check for significant change in G
        {
          // Apply force based on extracted values
          rb.AddForce(new Vector3(gValues[0] * sens, gValues[1] * sens, gValues[2] * sens), ForceMode.VelocityChange);
          previousG = currentG; // Update previous G
        }
      }
    }

    return 0; // Or return a value indicating successful data extraction
  }

  bool TryExtractXYZ(string dataString, float[] gValues)
  {
    string[] datas = dataString.Split(',');

    if (datas.Length < 3)
    {
      Debug.LogError("Incomplete data received from COM5");
      for (int i = 0; i < 3; i++)
      {
        gValues[i] = 0.0f;
      }
      return false;
    }

    try
    {
      for (int i = 0; i < 3; i++)
      {
        gValues[i] = float.Parse(datas[i]);
      }
      return true;
    }
    catch (System.FormatException)
    {
      Debug.LogError("Invalid data format received from COM5");
      for (int i = 0; i < 3; i++)
      {
        gValues[i] = 0.0f;
      }
      return false;
    }
  }

  float CalculateG(float[] gValues)
  {
    // Convert milliG/100 values to Gs (assuming x, y, z represent G-forces on each axis)
    for (int i = 0; i < 3; i++)
    {
      gValues[i] /= 100.0f;
    }

    // Calculate the total G-force (assuming Euclidean norm)
    return Mathf.Sqrt(gValues[0] * gValues[0] + gValues[1] * gValues[1] + gValues[2] * gValues[2]);
  }
}
