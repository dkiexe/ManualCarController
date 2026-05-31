using UnityEngine;


[RequireComponent(typeof(PedalScript))]
public class BreakLightsRegulator : MonoBehaviour
{
    [SerializeField] private GameObject BreakLightGameObjectParent;

    private PedalScript _BreakPedal;

    private void Start()
    {
        BreakLightGameObjectParent.SetActive(false);
        _BreakPedal = GetComponent<PedalScript>();
    }
    private void Update()
    {
        if (_BreakPedal.PedalPressure > 0 && !BreakLightGameObjectParent.activeSelf)
        {
            BreakLightGameObjectParent.SetActive(true);
        }
        else if (_BreakPedal.PedalPressure == 0 && BreakLightGameObjectParent.activeSelf)
        {
            BreakLightGameObjectParent.SetActive(false);
        }
    }
}
