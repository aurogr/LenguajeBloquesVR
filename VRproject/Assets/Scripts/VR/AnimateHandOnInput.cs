using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    [SerializeField] private InputActionProperty _controllerTriggerAction; // for the pinch animation
    [SerializeField] private InputActionProperty _controllerGripAction; // for the grip animation

    private Animator _handAnimator;

    private void Awake()
    {
        _handAnimator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        float triggerValue = _controllerTriggerAction.action.ReadValue<float>(); // a float because the chosen action tells us how much
                                                                              // is the trigger pressed (instead of just a boolean stating if its pressed or not)

        _handAnimator.SetFloat("Trigger", triggerValue); // using the oculus hand models pinch animation

        float gripValue = _controllerGripAction.action.ReadValue<float>();

        _handAnimator.SetFloat("Grip", gripValue); // using the oculus hand models grip animation
    }
}
