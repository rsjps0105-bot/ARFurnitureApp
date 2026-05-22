using UnityEngine;
using TMPro;

public class UIButtonTester : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI modeText;

    private void Start()
    {
        modeText.text = "Mode : Ready";
    }

    public void OnAdd()
    {
        modeText.text = "Mode : Add";
    }

    public void OnDelete()
    {
        modeText.text = "Mode : Delete";
    }

    public void OnMove()
    {
        modeText.text = "Mode : Move";
    }

    public void OnRotate()
    {
        modeText.text = "Mode : Rotate";
    }

    public void OnScale()
    {
        modeText.text = "Mode : Scale";
    }

    public void OnApply()
    {
        modeText.text = "Mode : Apply";
    }

    public void OnCancel()
    {
        modeText.text = "Mode : Cancel";
    }
}