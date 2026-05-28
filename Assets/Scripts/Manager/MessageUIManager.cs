using TMPro;
using UnityEngine;

public class MessageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;

    private float timer;
    private bool isShowing;

    private void Update()
    {
        if (!isShowing) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            HideMessage();
        }
    }

    public void ShowMessage(string message, float duration = 2f)
    {
        messagePanel.SetActive(true);

        messageText.text = message;

        timer = duration;
        isShowing = true;
    }

    private void HideMessage()
    {
        messagePanel.SetActive(false);
        isShowing = false;
    }
}