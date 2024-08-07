using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player.Conditions
{

    public class InputCondition : TriggerCondition
    {

        [SerializeField, Space(10), BoxGroup, EnumToggleButtons] private ButtonType triggerButtonType;
        [SerializeField, BoxGroup, HideIf("@this.triggerButtonType == ButtonType.Button")] private bool isGreater;
        [SerializeField, BoxGroup, HideIf("@this.triggerButtonType == ButtonType.Button")] private float axisThresold;
        [SerializeField, BoxGroup, PropertySpace(0, 10)] private string triggerName;

        public override bool Condition(Object other)
        {

            switch (triggerButtonType)
            {
                case ButtonType.Button:
                    return ButtonTrigger();
                case ButtonType.Axis:
                    return AxisTrigger();
            }
            return false;
        }

        private bool ButtonTrigger()
        {
            if (Input.GetButton(triggerName))
            {
                return true;
            }

            return false;
        }

        private bool AxisTrigger()
        {
            var input = Input.GetAxis(triggerName);
 
            if (isGreater ? (input > axisThresold) : (input < axisThresold))
            {
                return true;
            }
            return false;
        }

        [System.Serializable]
        public enum ButtonType
        {
            Button, 
            Axis
        }
    }
}
