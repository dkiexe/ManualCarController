using UnityEngine;
using UnityEngine.UI;

public class HandBrakeScript : PedalScript
{
    [Header("References")]
    [SerializeField] private Image HandBrakeIcon;
    [field: SerializeField] public WheelCollider[] HandBrakeWheels { get; private set; }

    [Header("Slide Settings")]
    [SerializeField, Range(0, 1)] private float sideStiffnessReduction;
    [SerializeField] private float LiftHandBrakeSpeed = 1f;
    [SerializeField] private float LowerHandBrakeSpeed = 5f;


    private float _currentStiffness;
    private float _defaultStiffness;
    private float _minStiffness;
    private bool isPressed = false;

    protected override void Start()
    {
        base.Start();

        _defaultStiffness = HandBrakeWheels[0].sidewaysFriction.stiffness;
        _currentStiffness = _defaultStiffness;
        _minStiffness = _defaultStiffness * sideStiffnessReduction;
    }

    protected override void Update()
    {
        base.Update();

        if (playerPress.IsPressed())
        {
            isPressed = true;
        }
        else
        {
            isPressed = false;
        }
        UpdateIconOpacity();
    }

    private void FixedUpdate()
    {
        float _newStiffness;

        if (isPressed) 
        {
            _newStiffness = Mathf.Lerp(
                    _currentStiffness,
                    _minStiffness * PedalPressure,
                    Time.fixedDeltaTime * LiftHandBrakeSpeed
                );
        }
        else
        {
            _newStiffness = Mathf.Lerp(
                    _currentStiffness,
                    _defaultStiffness * (1 + PedalPressure),
                    Time.fixedDeltaTime * LowerHandBrakeSpeed
                );
        }
        
        _newStiffness = Mathf.Clamp(_newStiffness, _minStiffness, _defaultStiffness);
        UpdateWheelStiffness(_newStiffness);
        _currentStiffness = _newStiffness;
    }

    private void UpdateWheelStiffness(float value)
    {
        foreach (WheelCollider Wc in HandBrakeWheels)
        {
            WheelFrictionCurve friction = Wc.sidewaysFriction;
            
            friction.stiffness = value;
            
            Wc.sidewaysFriction = friction;
        }
    }

    private void UpdateIconOpacity()
    {
        float opacity_t = (_currentStiffness - _defaultStiffness) / (_minStiffness - _defaultStiffness);

        if (opacity_t < 0) opacity_t = 0;
        else if (opacity_t > 1) opacity_t = 1;

        Color new_color = HandBrakeIcon.color;

        new_color.a = opacity_t;
        HandBrakeIcon.color = new_color;
    }
}
