using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineThirdPersonFollow))]
public class DriveTrainCamera : MonoBehaviour
{
    [SerializeField] private float reverseSholderOffsetZ;
    [SerializeField] private float camFlipSpeed;
    [SerializeField] private GearSystemScript GearBox;

    private CinemachineThirdPersonFollow thirdPersonFollow;
    private float defalutSholderOffsetZ;

    private void Start()
    {
        thirdPersonFollow = GetComponent<CinemachineThirdPersonFollow>();
        defalutSholderOffsetZ = thirdPersonFollow.ShoulderOffset.z;
    }

    private void Update()
    {
        if (GearBox.InReverse)
        {
            thirdPersonFollow.ShoulderOffset.z = Mathf.Lerp
                (
                    thirdPersonFollow.ShoulderOffset.z,
                    reverseSholderOffsetZ,
                    Time.deltaTime * camFlipSpeed
                );
        }
        else
        {
            thirdPersonFollow.ShoulderOffset.z = Mathf.Lerp
                (
                    thirdPersonFollow.ShoulderOffset.z,
                    defalutSholderOffsetZ,
                    Time.deltaTime * camFlipSpeed
                );
        }
    }
}
