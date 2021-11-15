using UnityEngine;

namespace DarknessMinion.Movement
{
    public interface IInputInterpreter
    {
        InputInfo GetInputInfo();
        Vector2 GetMovementDirection();
    }
}