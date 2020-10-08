using UnityEngine;
using UnityEditor;

namespace DarknessMinion
{ 
    [CreateAssetMenu (menuName = "AI/Darkness/State/RemainState")]
    public class RemainState: DarkState
    {
        protected override void FirstTimeSetup()
        {
            stateType = StateType.REMAIN;
        }
        public override void InitializeState(Darkness controller) {
            Debug.LogWarning(string.Format("Darkness {0} has entered {1} State at {2}", controller.creationID, this.name, Time.deltaTime));
        }
        public override void UpdateState(Darkness controller) {}
        public override void ExitState(Darkness controller) {}
        public override void MovementUpdate(Darkness controller) {}
        protected override void CooldownCallback(Darkness controller) {}
    }
}