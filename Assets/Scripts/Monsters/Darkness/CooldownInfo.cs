using System;

namespace Darkness
{
    public class CooldownInfo
    {
        public enum CooldownStatus { Attacking, Patrolling, Idling, Moving, Spawn}
        public CooldownStatus acType { get; private set; }
        private float remainingTime;
        public Action<DarknessController> Callback;

        public CooldownInfo(float cdTime, CooldownStatus acT, Action<DarknessController> cback)
        {
            remainingTime = cdTime;
            acType = acT;
            Callback = cback;
        }

        public bool TimeRemaining(float time)
        {
            remainingTime = UnityEngine.Mathf.Max(remainingTime - time, 0);
            if(remainingTime == 0)
                return false;
            else return true;
        }

        public bool CheckTimerComplete()
        {
            if (remainingTime == 0)
                return true;
            else return false;
        }
    }
}