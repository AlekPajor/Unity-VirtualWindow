using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

public class FaceTracking : MonoBehaviour
{
    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];

    void Start()
    {
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
        server.Start();

        client = server.AcceptTcpClient();
        stream = client.GetStream();

        StartCoroutine(ReceiveMessages());
    }

    IEnumerator ReceiveMessages()
    {
        while (true)
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            UpdateCameraAngle(message);

            yield return null;
        }
    }

    void UpdateCameraAngle(string message)
    {
        try
        {
            Debug.Log($"Received message: {message}");

            message = message.Trim();
            string[] values = message.Split(',');

            if (values.Length == 2)
            {
                Debug.Log($"Split values - [X]: {values[0]}, [Y]: {values[1]}");

                if (float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float xValue) &&
                    float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float yValue))
                {
                    float xMapped = Remap(xValue, 0f, 1f, -10f, 10f) + 76;
                    float yMapped = Remap(1 - yValue, 0f, 1f, -10f, 10f);
                    transform.rotation = Quaternion.Euler(yMapped, xMapped, 0);
                }
                else
                {
                    Debug.LogError("Error parsing values: Invalid float format");
                }
            }
            else
            {
                Debug.LogError($"Error parsing values: Expected 2 values, but found {values.Length}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing values [Message]: {e.Message}");
        }
    }

    float Remap(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return toLow + (value - fromLow) / (fromHigh - fromLow) * (toHigh - toLow);
    }

    void OnDestroy()
    {
        server.Stop();
        client.Close();
    }
}
