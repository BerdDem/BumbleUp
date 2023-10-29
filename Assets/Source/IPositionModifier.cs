using UnityEngine;

namespace Source
{
    public interface IPositionModifier
    {
        public void AddToPosition(Vector3 deltaPosition);
    }
}