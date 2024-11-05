using OWML.Common;
using UnityEngine;

namespace MisfiredJump.Interfaces
{
    // Heavily inspired from AbstractGhostDoorInterface
    public abstract class AbstractConsole : AbstractNewHorizonMonoBehaviour
    {
        // Unity
        [SerializeField]
        protected InteractReceiver _interactReceiver; // the interactive object
        [SerializeField]
        protected RevealInteractID _revealInteractID = RevealInteractID.Unknown;
        [SerializeField]
        protected Transform _lockOnTransform; // where the camera will lock on while interacting

        // Mod config
        protected string[] _interactionFactIDs; // What facts get revealed upon first interaction

        // Other
        protected bool _isPlayerInteracting;

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
            if (_lockOnTransform == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with a lockon point [Unity issue]", MessageType.Error);
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

            // unequip tool and lock camera on
            Locator.GetToolModeSwapper().UnequipTool();
            Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().LockOn(_lockOnTransform, Vector3.zero);
            GlobalMessenger.FireEvent("EnterSatelliteCameraMode");

            // Add prompts on screen
            AddScreenPrompts();

            // allow updating
            base.enabled = true;
            _isPlayerInteracting = true;
        }
        protected virtual void CancelInteraction()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"Stopping interaction with {Nameofclass}", MessageType.Info);

            // remove prompts from screen
            RemoveScreenPrompts();

            // unlock camera and reset receiver. TODO: unsure what ResetInteraction really does, check later.
            Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().BreakLock();
            _interactReceiver.ResetInteraction();
            GlobalMessenger.FireEvent("ExitSatelliteCameraMode");

            // first step in stopping updates
            _isPlayerInteracting = false;
        }

        protected abstract void AddScreenPrompts();
        protected abstract void RemoveScreenPrompts();
    }
}
