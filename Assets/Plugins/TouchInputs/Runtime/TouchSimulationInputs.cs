//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.0
//     from Assets/Plugins/TouchInputs/Runtime/TouchSimulationInputs.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @TouchSimulationInputs: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @TouchSimulationInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TouchSimulationInputs"",
    ""maps"": [
        {
            ""name"": ""TouchSimulation"",
            ""id"": ""305dd8ed-ac94-45d6-9024-ba247009df22"",
            ""actions"": [
                {
                    ""name"": ""SimpleClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""543d6bf5-dae8-4f46-b16b-36d391268e75"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TwoFingerClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0f2cafca-46f2-4bec-b804-320989b02f37"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ThreeFingerClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2961bd88-b02d-4a79-99c6-8c5fe92df0d4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""PassThrough"",
                    ""id"": ""07504d82-f6af-4869-956c-03b5223dba94"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AlternativeBehaviour"",
                    ""type"": ""PassThrough"",
                    ""id"": ""12f9fbbd-26dc-456e-a5b6-e05546dcafab"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PointerPosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e489395d-27e1-42dc-a2a0-6916e20daf39"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PointerDeltaPosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""14c4d8f3-e565-41b7-b4fd-080c7e558188"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8e14f771-609e-48a9-890e-c70409f43123"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SimpleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e0eb8527-91c9-470a-bb00-278cba15463d"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThreeFingerClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9aa061e4-be55-49a6-9d81-784b0bbe3424"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TwoFingerClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3cbdad6e-0a66-49ba-a1ac-0c7c163bc5e5"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""04151b9d-a168-4da2-a3bf-7400abf346ed"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AlternativeBehaviour"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee37d554-5158-4d04-b5d9-36f3bea9ee9a"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""26def8b9-cc51-4426-b99c-8460664cf48e"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerDeltaPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // TouchSimulation
        m_TouchSimulation = asset.FindActionMap("TouchSimulation", throwIfNotFound: true);
        m_TouchSimulation_SimpleClick = m_TouchSimulation.FindAction("SimpleClick", throwIfNotFound: true);
        m_TouchSimulation_TwoFingerClick = m_TouchSimulation.FindAction("TwoFingerClick", throwIfNotFound: true);
        m_TouchSimulation_ThreeFingerClick = m_TouchSimulation.FindAction("ThreeFingerClick", throwIfNotFound: true);
        m_TouchSimulation_Zoom = m_TouchSimulation.FindAction("Zoom", throwIfNotFound: true);
        m_TouchSimulation_AlternativeBehaviour = m_TouchSimulation.FindAction("AlternativeBehaviour", throwIfNotFound: true);
        m_TouchSimulation_PointerPosition = m_TouchSimulation.FindAction("PointerPosition", throwIfNotFound: true);
        m_TouchSimulation_PointerDeltaPosition = m_TouchSimulation.FindAction("PointerDeltaPosition", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // TouchSimulation
    private readonly InputActionMap m_TouchSimulation;
    private List<ITouchSimulationActions> m_TouchSimulationActionsCallbackInterfaces = new List<ITouchSimulationActions>();
    private readonly InputAction m_TouchSimulation_SimpleClick;
    private readonly InputAction m_TouchSimulation_TwoFingerClick;
    private readonly InputAction m_TouchSimulation_ThreeFingerClick;
    private readonly InputAction m_TouchSimulation_Zoom;
    private readonly InputAction m_TouchSimulation_AlternativeBehaviour;
    private readonly InputAction m_TouchSimulation_PointerPosition;
    private readonly InputAction m_TouchSimulation_PointerDeltaPosition;
    public struct TouchSimulationActions
    {
        private @TouchSimulationInputs m_Wrapper;
        public TouchSimulationActions(@TouchSimulationInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @SimpleClick => m_Wrapper.m_TouchSimulation_SimpleClick;
        public InputAction @TwoFingerClick => m_Wrapper.m_TouchSimulation_TwoFingerClick;
        public InputAction @ThreeFingerClick => m_Wrapper.m_TouchSimulation_ThreeFingerClick;
        public InputAction @Zoom => m_Wrapper.m_TouchSimulation_Zoom;
        public InputAction @AlternativeBehaviour => m_Wrapper.m_TouchSimulation_AlternativeBehaviour;
        public InputAction @PointerPosition => m_Wrapper.m_TouchSimulation_PointerPosition;
        public InputAction @PointerDeltaPosition => m_Wrapper.m_TouchSimulation_PointerDeltaPosition;
        public InputActionMap Get() { return m_Wrapper.m_TouchSimulation; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TouchSimulationActions set) { return set.Get(); }
        public void AddCallbacks(ITouchSimulationActions instance)
        {
            if (instance == null || m_Wrapper.m_TouchSimulationActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_TouchSimulationActionsCallbackInterfaces.Add(instance);
            @SimpleClick.started += instance.OnSimpleClick;
            @SimpleClick.performed += instance.OnSimpleClick;
            @SimpleClick.canceled += instance.OnSimpleClick;
            @TwoFingerClick.started += instance.OnTwoFingerClick;
            @TwoFingerClick.performed += instance.OnTwoFingerClick;
            @TwoFingerClick.canceled += instance.OnTwoFingerClick;
            @ThreeFingerClick.started += instance.OnThreeFingerClick;
            @ThreeFingerClick.performed += instance.OnThreeFingerClick;
            @ThreeFingerClick.canceled += instance.OnThreeFingerClick;
            @Zoom.started += instance.OnZoom;
            @Zoom.performed += instance.OnZoom;
            @Zoom.canceled += instance.OnZoom;
            @AlternativeBehaviour.started += instance.OnAlternativeBehaviour;
            @AlternativeBehaviour.performed += instance.OnAlternativeBehaviour;
            @AlternativeBehaviour.canceled += instance.OnAlternativeBehaviour;
            @PointerPosition.started += instance.OnPointerPosition;
            @PointerPosition.performed += instance.OnPointerPosition;
            @PointerPosition.canceled += instance.OnPointerPosition;
            @PointerDeltaPosition.started += instance.OnPointerDeltaPosition;
            @PointerDeltaPosition.performed += instance.OnPointerDeltaPosition;
            @PointerDeltaPosition.canceled += instance.OnPointerDeltaPosition;
        }

        private void UnregisterCallbacks(ITouchSimulationActions instance)
        {
            @SimpleClick.started -= instance.OnSimpleClick;
            @SimpleClick.performed -= instance.OnSimpleClick;
            @SimpleClick.canceled -= instance.OnSimpleClick;
            @TwoFingerClick.started -= instance.OnTwoFingerClick;
            @TwoFingerClick.performed -= instance.OnTwoFingerClick;
            @TwoFingerClick.canceled -= instance.OnTwoFingerClick;
            @ThreeFingerClick.started -= instance.OnThreeFingerClick;
            @ThreeFingerClick.performed -= instance.OnThreeFingerClick;
            @ThreeFingerClick.canceled -= instance.OnThreeFingerClick;
            @Zoom.started -= instance.OnZoom;
            @Zoom.performed -= instance.OnZoom;
            @Zoom.canceled -= instance.OnZoom;
            @AlternativeBehaviour.started -= instance.OnAlternativeBehaviour;
            @AlternativeBehaviour.performed -= instance.OnAlternativeBehaviour;
            @AlternativeBehaviour.canceled -= instance.OnAlternativeBehaviour;
            @PointerPosition.started -= instance.OnPointerPosition;
            @PointerPosition.performed -= instance.OnPointerPosition;
            @PointerPosition.canceled -= instance.OnPointerPosition;
            @PointerDeltaPosition.started -= instance.OnPointerDeltaPosition;
            @PointerDeltaPosition.performed -= instance.OnPointerDeltaPosition;
            @PointerDeltaPosition.canceled -= instance.OnPointerDeltaPosition;
        }

        public void RemoveCallbacks(ITouchSimulationActions instance)
        {
            if (m_Wrapper.m_TouchSimulationActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ITouchSimulationActions instance)
        {
            foreach (var item in m_Wrapper.m_TouchSimulationActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_TouchSimulationActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public TouchSimulationActions @TouchSimulation => new TouchSimulationActions(this);
    public interface ITouchSimulationActions
    {
        void OnSimpleClick(InputAction.CallbackContext context);
        void OnTwoFingerClick(InputAction.CallbackContext context);
        void OnThreeFingerClick(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
        void OnAlternativeBehaviour(InputAction.CallbackContext context);
        void OnPointerPosition(InputAction.CallbackContext context);
        void OnPointerDeltaPosition(InputAction.CallbackContext context);
    }
}
