using UnityEngine;

namespace Source.Level
{
    public interface IPositionModifier
    {
        public void AddToPosition(Vector3 deltaPosition);
    }
}