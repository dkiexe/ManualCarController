using UnityEngine;

public class SpeedGaugeScript : GaugeScript
{
    [SerializeField] private EngineScript engine;

    private void Update()
    {
        if (engine != null)
        {
            MoveNeedle(engine.KMH);
        }
    }
}
