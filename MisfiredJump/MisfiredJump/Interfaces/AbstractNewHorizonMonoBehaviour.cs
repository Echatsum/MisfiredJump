using OWML.Common;
using UnityEngine;

namespace MisfiredJump.Interfaces
{
    public abstract class AbstractNewHorizonMonoBehaviour : MonoBehaviour
    {
        protected abstract string Nameofclass { get; }

        protected virtual void CheckUnityParameters() { }
        protected virtual void GetNHParameters() { }
        protected virtual void GetModParameters() { }

        protected virtual void Start()
        {
            CheckUnityParameters();
            GetNHParameters();
            GetModParameters();
        }
    }
}
