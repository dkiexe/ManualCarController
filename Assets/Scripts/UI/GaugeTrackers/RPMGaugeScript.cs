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
        string gearIndexString;

        if (currentGearVal == 0) gearIndexString = "N";
        else if (currentGearVal == -1) gearIndexString = "R";
        else gearIndexString = currentGearVal.ToString();

        gearDisplayText.text = gearIndexString.ToString();
    }
}