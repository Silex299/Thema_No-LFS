using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Player_Scripts;

public class ConditionalInputTrigger : MonoBehaviour
{

    [SerializeField, EnumToggleButtons, PropertySpace(10f, 10f)] private PlayerMovementState m_conditionState;

    [SerializeField, BoxGroup, EnumToggleButtons] private InputType m_inputType;

    [HideIf("@this.m_inputType == InputType.ButtonInput"), SerializeField, BoxGroup] private float m_axisThresold;
    [HideIf("@this.m_inputType == InputType.ButtonInput"), SerializeField, BoxGroup] private bool m_isGreater = true;

    [SerializeField, BoxGroup] private string m_input;
    [SerializeField, BoxGroup] private string m_triggerTag;
    [SerializeField, BoxGroup] private float m_secondActionDelay;


    [BoxGroup, SerializeField] private UnityEvent actions;

    private bool _m_isInTrigger;
    private float _m_lastActionTime;
    private bool _m_called;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(m_triggerTag))
        {
            _m_isInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(m_triggerTag))
        {
            _m_isInTrigger = false;
        }
    }


    private void Update()
    {

        if (!_m_isInTrigger) return;

        if (Time.time < _m_lastActionTime + m_secondActionDelay)
        {
            return;
        }

        switch (m_inputType)
        {
            case InputType.AxisInput:
                AxisTrigger();
                break;
            case InputType.ButtonInput:
                ButtonTrigger();
                break;
        }
    }

    private void AxisTrigger()
    {
        var input = Input.GetAxis(m_input);

        var result = m_isGreater ? (input > m_axisThresold) : (input < m_axisThresold);

        if (result)
        {

            if (!PlayerMovementController.Instance.VerifyState(m_conditionState)) return;

            actions.Invoke();
            _m_lastActionTime = Time.time;
            print("Unity is shit");
        }
    }


    private void ButtonTrigger()
    {
        if (Input.GetButtonDown(m_input))
        {
            if (!PlayerMovementController.Instance.VerifyState(m_conditionState)) return;
            _m_lastActionTime = Time.time;
            actions.Invoke();
        }
    }



    [System.Serializable]
    public enum InputType
    {
        ButtonInput,
        AxisInput
    }
}
