using UnityEngine;
using Pinball.Gameplay;

namespace Pinball.Interfaces
{
    public interface IBallContactHandler
    {
        void OnBallContact(Ball ball, Collider2D ballCollider);
    }
}