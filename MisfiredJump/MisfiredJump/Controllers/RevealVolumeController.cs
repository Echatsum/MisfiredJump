using OWML.Common;
using UnityEngine;

namespace MisfiredJump
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(IgnoreCollision))]
    public class RevealVolumeController : Interfaces.AbstractNewHorizonMonoBehaviour
    {
        // Unity
        [SerializeField]
        private RevealVolumeID _revealVolumeID = RevealVolumeID.Unknown;

        // Other
        private string[] _revealVolumeFacts;

        protected override string Nameofclass => nameof(RevealVolumeController);
        protected override void CheckUnityParameters()
        {
            base.CheckUnityParameters();

            if (_revealVolumeID == RevealVolumeID.Unknown)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with an revealID [Unity issue]", MessageType.Error);
            }
        }
        protected override void GetModParameters()
        {
            base.GetModParameters();
            _revealVolumeFacts = MisfiredJumpFacts.GetRevealVolumeFacts(_revealVolumeID);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Probe"))
            {
                MisfiredJumpFacts.RevealFacts(_revealVolumeFacts);
            }
        }
    }
}
