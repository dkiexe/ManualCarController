using TMPro;
using UnityEngine;

public class BasicTextTrackerSpeed : MonoBehaviour
{
    [SerializeField] private EngineScript engine;
    [SerializeField] private TextMeshProUGUI speedDisplayText;


    private const string strSurfix = " KM/H";

    private void FixedUpdate()
    {
        speedDisplayText.text = Mathf.Round(engine.KMH).ToString() + strSurfix;
    }
}