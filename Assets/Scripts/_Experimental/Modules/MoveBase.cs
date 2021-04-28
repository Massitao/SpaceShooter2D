using UnityEngine;

namespace GalaxyShooter.Entities.Modules
{
    public abstract class MoveBase : MonoBehaviour
    {
        public abstract void Move(Vector2 moveDir, float speed);
    }
}
