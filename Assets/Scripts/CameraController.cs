using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputActionReference rightClickAction;
    [SerializeField] private CinemachineInputAxisController cameraInputAxesController;

    private void Awake()
    {
        cameraInputAxesController.ReadControlValueOverride = ReadControlValueOverride;
    }
    
    private float ReadControlValueOverride(InputAction action, IInputAxisOwner.AxisDescriptor.Hints hint, 
        Object context, CinemachineInputAxisController.Reader.ControlValueReader defaultReader)
    {
        // Pass LookAction value into Cinemachine camera only when the right mouse button is pressed
        // This only works if ActiveInputHandling is NewInputSystem, otherwise Cinemachine camera will use legacy input
        return rightClickAction.action.IsPressed() ? defaultReader.Invoke(action, hint, context, null) : 0.0f;
    }
}
