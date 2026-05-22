using UnityEngine;

public class MessageUIManager : MonoBehaviour
{
    private string message = "Ready";

    public void ShowMessage(string newMessage)
    {
        message = newMessage;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();

        style.fontSize = 50;
        style.normal.textColor = Color.red;

        GUI.Label(new Rect(50, 50, 1000, 100), message, style);
    }
}