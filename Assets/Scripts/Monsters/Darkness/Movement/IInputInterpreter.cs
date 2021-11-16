using UnityEngine;

namespace Darkness.Movement
{
    public interface IInputInterpreter
    {
        InputInfo GetInputInfo();
        Vector2 GetMovementDirection();
    }
}