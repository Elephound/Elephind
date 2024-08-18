using UnityEngine;
using TMPro;

public class ConsoleToText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consoleOutput;
    private static ConsoleToText instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Application.logMessageReceived += HandleLog;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            Application.logMessageReceived -= HandleLog;
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string formattedLog = $"{logString}\n";
        consoleOutput.text += formattedLog;
    }

    public static void Log(string message)
    {
        Debug.Log(message);
    }
}
