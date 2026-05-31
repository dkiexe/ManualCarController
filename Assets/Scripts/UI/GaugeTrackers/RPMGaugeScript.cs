using TMPro;
using UnityEngine;

public class RPMGaugeScript : GaugeScript
{
    [SerializeField] private IntChannel GearChannel;
    [SerializeField] private TextMeshProUGUI gearDisplayText;

    // Privete fields
    private int currentGearVal = 0; 

    private void Update()
    {
        if (currentGearVal != GearChannel.Value)
        {
            currentGearVal = GearChannel.Value;
            ChangeGearDisplay();
        }
    }

    private void ChangeGearDisplay()
    {
        string gearIndexString = currentGearVal == 0? "N" : currentGearVal.ToString();
        gearDisplayText.text = gearIndexString.ToString();
    }
}