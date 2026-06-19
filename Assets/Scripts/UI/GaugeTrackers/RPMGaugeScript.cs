using TMPro;
using UnityEngine;

public class RPMGaugeScript : GaugeScript
{
    [SerializeField] private EngineScript engine;
    [SerializeField] private GearSystemScript GearBox;
    [SerializeField] private TextMeshProUGUI gearDisplayText;

    // Privete fields
    private int currentGearVal = 0; 

    private void Update()
    {
        if (currentGearVal != GearBox.currentGearIndex)
        {
            currentGearVal = GearBox.currentGearIndex;
            ChangeGearDisplay();
        }

        if (engine != null)
        {
            MoveNeedle(engine.EngineRPM);
        }
    }

    private void ChangeGearDisplay()
    {
        string gearIndexString = currentGearVal == 0? "N" : currentGearVal.ToString();
        gearDisplayText.text = gearIndexString.ToString();
    }
}