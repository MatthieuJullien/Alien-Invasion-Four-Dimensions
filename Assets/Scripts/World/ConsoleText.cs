using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private int nbLineOnScreen = 18;
    [SerializeField] private float refreshInterval = 2f;

    private List<string> lineBuffer = new List<string>();
    private int startLine = 0;
    private float lastRefreshTime = Mathf.NegativeInfinity;

    private void Start()
    {
        lineBuffer.AddRange(textMesh.text.Split('\n'));
    }

    //void OnEnable()
    //{
    //    Application.logMessageReceived += HandleLog;
    //}

    //void OnDisable()
    //{
    //    Application.logMessageReceived -= HandleLog;
    //}

    private void Update()
    {
        if (Time.time > lastRefreshTime + refreshInterval)
        {
            RefreshScreen();
            lastRefreshTime = Time.time;
        }
    }

    //void HandleLog(string logString, string stackTrace, LogType type)
    //{
    //    lineBuffer.AddRange(logString.Split('\n'));
    //    lineBuffer.AddRange(stackTrace.Split('\n'));
    //}

    private void RefreshScreen()
    {
        startLine++;
        if (startLine + nbLineOnScreen >= lineBuffer.Count)
        {
            startLine = 0;

            // Random shuffle
            for (int i = 0; i < lineBuffer.Count; i++)
            {
                string temp = lineBuffer[i];
                int randomIndex = UnityEngine.Random.Range(i, lineBuffer.Count);
                lineBuffer[i] = lineBuffer[randomIndex];
                lineBuffer[randomIndex] = temp;
            }

        }
        textMesh.text = String.Join("\n", lineBuffer.GetRange(startLine, nbLineOnScreen));
    }
}
