using OWML.Common;
using UnityEngine;

namespace MisfiredJump
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(IgnoreCollision))]
    public class TelescopeWaterController : Interfaces.AbstractNewHorizonMonoBehaviour
    {
        // Unity
        [SerializeField]
        private bool _turnOnWater;

        // New Horizons generated
        private GameObject _planetWater;

        protected override string Nameofclass => nameof(TelescopeWaterController);
        protected override void GetNHParameters()
        {
            base.GetNHParameters();

            _planetWater = this.GetAttachedOWRigidbody()?.gameObject.transform.Find("Sector/Water")?.gameObject;
            if (_planetWater == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} could not find water component [NH issue]", MessageType.Warning);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _planetWater?.SetActive(_turnOnWater);
            }

            MisfiredJump.Instance.ModHelper.Console.WriteLine($"Tag: {other.tag}", MessageType.Info);
        }
    }
}
