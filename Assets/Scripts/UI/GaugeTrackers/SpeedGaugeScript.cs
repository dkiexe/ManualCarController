using UnityEngine;

public class SpeedGaugeScript : GaugeScript
{
    [SerializeField] private EngineScript engine;

    private void Update()
    {
        if (engine != null)
        {
            UpdateNeedle(0, engine.MaxSpeedKMH, engine.KMH);
        }
    }
}
