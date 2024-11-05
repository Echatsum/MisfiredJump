using OWML.Common;
using UnityEngine;

namespace MisfiredJump.Interfaces
{
    // Heavily inspired from AbstractGhostDoorInterface
    public abstract class AbstractButton : AbstractNewHorizonMonoBehaviour
    {
        // Unity
        [SerializeField]
        protected InteractReceiver _interactReceiver; // the interactive object
        [SerializeField]
        protected RevealInteractID _revealInteractID = RevealInteractID.Unknown;

        // Mod config
        protected string[] _interactionFactIDs; // What facts get revealed upon first interaction

        // Other
        protected bool _isSingleuseButton; // Whether the interact receiver should diable interaction after one use

        public AbstractButton(bool isSingleuseButton)
        {
            _isSingleuseButton = isSingleuseButton;
        }

        protected override void CheckUnityParameters()
        {
            base.CheckUnityParameters();

            if (_interactReceiver == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with an interactive receiver [Unity issue]", MessageType.Error);
            }
            if (_revealInteractID == RevealInteractID.Unknown)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with an revealID [Unity issue]", MessageType.Error);
            }
        }
        protected override void GetModParameters()
        {
            base.GetModParameters();

            _interactionFactIDs = MisfiredJumpFacts.GetRevealInteractFacts(_revealInteractID);
        }

        protected virtual void Awake()
        {
            if (_interactReceiver != null)
            {
                _interactReceiver.OnPressInteract += OnPressInteract;
            }
        }
        protected virtual void OnDestroy()
        {
            if (_interactReceiver != null)
            {
                _interactReceiver.OnPressInteract -= OnPressInteract;
            }
        }

        protected virtual void OnPressInteract()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"Interacting with {Nameofclass}", MessageType.Info);
            MisfiredJumpFacts.RevealFacts(_interactionFactIDs);

            // Button = do the action then reset receiver (+ diable if single use)
            ButtonInteraction();
            _interactReceiver.ResetInteraction();
            if (_isSingleuseButton)
            {
                _interactReceiver.DisableInteraction();
            }
        }

        protected abstract void ButtonInteraction();
    }
}
