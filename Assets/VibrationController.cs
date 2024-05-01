using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;


public class Accelerometer : MonoBehaviour {

  public string COM;
  public float speed;
  public int arrLen;

  public string receivedString; // To store received data string
  public float vibrationSens; // Sensitivity for vibration data (optional)

  private SerialPort serialPort;
  private bool serialOK = false;

  private Vector3[] vibrationBuffer;
  private int bufIndex = 0;

  void Start()
  {
    serialPort = new SerialPort(COM, 9600, Parity.None, 8, StopBits.One);

    try
    {
      serialPort.Open();
      serialOK = true;
      Debug.Log("Serial OK");
    }
    catch (System.IO.IOException ioe)
    {
      Debug.LogError("IOException: " + ioe.Message);
    }

    vibrationBuffer = new Vector3[arrLen];
  }

  void ReadSerial()
  {
    receivedString = serialPort.ReadLine();
    var dataBlocks = receivedString.Split(',');

    if (dataBlocks.Length < 3)
    {
      Debug.LogWarning("Invalid data received");
      return;
    }

    int vibrationX, vibrationY, vibrationZ;

    // Assuming vibration data starts from the first element (modify if needed)
    if (!int.TryParse(dataBlocks[0], out vibrationX))
    {
      Debug.LogWarning("Failed to parse vibrationX. RawData: " + dataBlocks[0]);
      return;
    }
    if (!int.TryParse(dataBlocks[1], out vibrationY))
    {
      Debug.LogWarning("Failed to parse vibrationY. RawData: " + dataBlocks[1]);
      return;
    }
    if (!int.TryParse(dataBlocks[2], out vibrationZ))
    {
      Debug.LogWarning("Failed to parse vibrationZ. RawData: " + dataBlocks[2]);
      return;
    }

    // Call ProcessVibration to handle the parsed vibration data (add your logic here)
    ProcessVibration(vibrationX, vibrationY, vibrationZ);
  }


  void ProcessVibration(int vibrationX, int vibrationY, int vibrationZ)
  {
    Vector3 newVibration = new Vector3((float)vibrationX, (float)vibrationY, (float)vibrationZ) * vibrationSens;

    if (bufIndex < arrLen - 1)
    {
      vibrationBuffer[bufIndex] = newVibration;
      bufIndex++;
    }
    else
    {
      var newVibrationArray = new Vector3[vibrationBuffer.Length];
      Array.Copy(vibrationBuffer, 1, newVibrationArray, 0, vibrationBuffer.Length - 1);
      newVibrationArray[vibrationBuffer.Length - 1] = newVibration;
      vibrationBuffer = newVibrationArray;

      // You can access the latest vibration data here (e.g., for triggering events):
      // Vector3 latestVibration = vibrationBuffer[bufIndex - 1];
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (serialOK)
    {
      try
      {
        ReadSerial();
      }
      catch (Exception)
      {
        Debug.LogError("Error reading serial data.");
      }
    }
  }
}
