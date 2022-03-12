using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class Main : MonoBehaviour
{
    private GameObject logger;
    private Queue<string> ReceivedMessages = null;
    private static string server = "ws://13.231.242.11:5001";

    [DllImport("__Internal")]
    private static extern void Initialize(string server);
    [DllImport("__Internal")]
    private static extern string GetMessage();
    [DllImport("__Internal")]
    private static extern void Send(string message);
    [DllImport("__Internal")]
    private static extern void Close();


    private void OnDestroy()
    {
        Close();
    }

    void Start()
    {
        logger = GameObject.Find("Logger");

        ReceivedMessages = new Queue<string>();

        Initialize(server);
    }


    private static string Formatter(string message) => $"message from server:\n{message}";

    void Update()
    {

        string message = GetMessage();

        if (message != "")
        {
            ReceivedMessages.Enqueue(message);

            Debug.Log($"Received -> {message}");
        }

        if (ReceivedMessages.Any())
        {

            string line = ReceivedMessages.Dequeue();

            logger.GetComponent<TextMesh>().text = Formatter(line);
        }


    }
}
