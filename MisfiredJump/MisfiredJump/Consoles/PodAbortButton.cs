using OWML.Common;
using System;
using UnityEngine;

namespace MisfiredJump
{
    public class PodAbortButton : Interfaces.AbstractButton
    {
        // Unity
        [SerializeField]
        private PodConsole _podConsole;
        [SerializeField]
        private GameObject _socketOneParent;
        [SerializeField]
        private GameObject _socketTwoParent;

        // New Horizons generated
        private WarpCoreSocket _socketOne;
        private WarpCoreSocket _socketTwo;

        // Mod config
        private string[] _warpingFactIDs; // Note: not quite interactionIDs, as the button works on conditions

        // Other
        private bool _canWarp;

        public PodAbortButton() : base(isSingleuseButton: false) { }

        protected override string Nameofclass => nameof(PodAbortButton);

        protected override void CheckUnityParameters()
        {
            base.CheckUnityParameters();

            if (_podConsole == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with podConsole [Unity issue]", MessageType.Error);
            }
            if (_socketOneParent == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with socketParent one [Unity issue]", MessageType.Error);
            }
            if (_socketTwoParent == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with socketParent two [Unity issue]", MessageType.Error);
            }
        }
        protected override void GetNHParameters()
        {
            base.GetNHParameters();

            if (_socketOne == null) { _socketOne = _socketOneParent.GetComponentInChildren<WarpCoreSocket>(); }
            if (_socketTwo == null) { _socketTwo = _socketTwoParent.GetComponentInChildren<WarpCoreSocket>(); }

            if (_socketOne == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} could not find socket one [NH issue]", MessageType.Error);
            }
            if (_socketTwo == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} could not find socket two [NH issue]", MessageType.Error);
            }
        }
        protected override void GetModParameters()
        {
            base.GetModParameters();

            // Facts
            _warpingFactIDs = MisfiredJumpFacts.GetRevealInteractFacts(RevealInteractID.ScaldingAbyssLavaPodAbort);
        }

        protected override void Awake()
        {
            base.Awake();

            // TODO: I don't like this
            if (_socketOne == null) { _socketOne = _socketOneParent.GetComponentInChildren<WarpCoreSocket>(); }
            if (_socketTwo == null) { _socketTwo = _socketTwoParent.GetComponentInChildren<WarpCoreSocket>(); }
            WarpCoreSocket socketOne = _socketOne;
            socketOne.OnSocketablePlaced = (OWItemSocket.SocketEvent)Delegate.Combine(socketOne.OnSocketablePlaced, new OWItemSocket.SocketEvent(OnWarpCorePlaced));
            WarpCoreSocket socketOne2 = _socketOne;
            socketOne2.OnSocketableRemoved = (OWItemSocket.SocketEvent)Delegate.Combine(socketOne2.OnSocketableRemoved, new OWItemSocket.SocketEvent(OnWarpCoreRemoved));
            WarpCoreSocket socketTwo = _socketTwo;
            socketTwo.OnSocketablePlaced = (OWItemSocket.SocketEvent)Delegate.Combine(socketTwo.OnSocketablePlaced, new OWItemSocket.SocketEvent(OnWarpCorePlaced));
            WarpCoreSocket socketTwo2 = _socketTwo;
            socketTwo2.OnSocketableRemoved = (OWItemSocket.SocketEvent)Delegate.Combine(socketTwo2.OnSocketableRemoved, new OWItemSocket.SocketEvent(OnWarpCoreRemoved));
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            WarpCoreSocket socketOne = _socketOne;
            socketOne.OnSocketablePlaced = (OWItemSocket.SocketEvent)Delegate.Remove(socketOne.OnSocketablePlaced, new OWItemSocket.SocketEvent(OnWarpCorePlaced));
            WarpCoreSocket socketOne2 = _socketOne;
            socketOne2.OnSocketableRemoved = (OWItemSocket.SocketEvent)Delegate.Remove(socketOne2.OnSocketableRemoved, new OWItemSocket.SocketEvent(OnWarpCoreRemoved));
            WarpCoreSocket socketTwo = _socketTwo;
            socketTwo.OnSocketablePlaced = (OWItemSocket.SocketEvent)Delegate.Remove(socketTwo.OnSocketablePlaced, new OWItemSocket.SocketEvent(OnWarpCorePlaced));
            WarpCoreSocket socketTwo2 = _socketTwo;
            socketTwo2.OnSocketableRemoved = (OWItemSocket.SocketEvent)Delegate.Remove(socketTwo2.OnSocketableRemoved, new OWItemSocket.SocketEvent(OnWarpCoreRemoved));
        }

        protected override void Start()
        {
            base.Start();

            // Init prompt texts
            _interactReceiver?.SetPromptText(UITextType.UnknownInterfacePrompt);

            UpdateCanWarp();
        }

        protected override void ButtonInteraction()
        {
            if (_canWarp && !_podConsole.IsUpdatingStatus())
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"Warping pod with {Nameofclass}", MessageType.Info);
                MisfiredJumpFacts.RevealFacts(_warpingFactIDs);

                StartCoroutine(_podConsole.DeactivatePod());
            }
        }

        private void OnWarpCorePlaced(OWItem warpCore)
        {
            UpdateCanWarp();
        }
        private void OnWarpCoreRemoved(OWItem warpCore)
        {
            _canWarp = false;
        }
        private void UpdateCanWarp()
        {
            WarpCoreType typeOne = _socketOne.GetWarpCoreType();
            WarpCoreType typeTwo = _socketTwo.GetWarpCoreType();

            _canWarp = (typeOne == WarpCoreType.Black) && (typeTwo == WarpCoreType.White);
        }
    }
}
