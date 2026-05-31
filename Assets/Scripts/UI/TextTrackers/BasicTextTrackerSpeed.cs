using TMPro;
using UnityEngine;

public class BasicTextTrackerSpeed : MonoBehaviour
{
    [SerializeField] private FloatChannel Speedchannel;
    [SerializeField] private TextMeshProUGUI speedDisplayText;


    private const string strSurfix = " KM/H";

    private void FixedUpdate()
    {
        speedDisplayText.text = Mathf.Round(Speedchannel.Value).ToString() + strSurfix;
    }
}