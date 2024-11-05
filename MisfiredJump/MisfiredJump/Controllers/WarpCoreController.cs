using OWML.Common;
using System;
using UnityEngine;

namespace MisfiredJump
{
    public class WarpCoreController : Interfaces.AbstractNewHorizonMonoBehaviour
    {
        // Unity
        [SerializeField]
        private GameObject _socketOneParent;
        [SerializeField]
        private GameObject _socketTwoParent;
        [SerializeField]
        private GameObject _blackHoleOne;
        [SerializeField]
        private GameObject _blackHoleTwo;
        [SerializeField]
        private GameObject _whiteHoleOne;
        [SerializeField]
        private GameObject _whiteHoleTwo;

        // New Horizons generated
        private WarpCoreSocket _socketOne;
        private WarpCoreSocket _socketTwo;

        // Mod config
        private float _interactRange;

        protected override string Nameofclass => nameof(WarpCoreController);
        protected override void CheckUnityParameters()
        {
            base.CheckUnityParameters();

            if (_socketOneParent == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with socketParent one [Unity issue]", MessageType.Error);
            }
            if (_socketTwoParent == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with socketParent two [Unity issue]", MessageType.Error);
            }
            if (_blackHoleOne == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with blackHole one [Unity issue]", MessageType.Error);
            }
            if (_blackHoleTwo == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with blackHole two [Unity issue]", MessageType.Error);
            }
            if (_whiteHoleOne == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with whiteHole one [Unity issue]", MessageType.Error);
            }
            if (_whiteHoleTwo == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with whiteHole two [Unity issue]", MessageType.Error);
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

            _interactRange = MisfiredJump.Instance.GetTelescopeWarpRanges();
        }

        private void Awake()
        {
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
        private void Destroy()
        {
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

            _socketOne._interactRange = this._interactRange;
            _socketTwo._interactRange = this._interactRange;

            CheckActivation();
        }

        private void OnWarpCorePlaced(OWItem warpCore)
        {
            CheckActivation();
        }
        private void OnWarpCoreRemoved(OWItem warpCore)
        {
            Deactivate();
        }

        private void CheckActivation()
        {
            WarpCoreType typeOne = _socketOne.GetWarpCoreType();
            WarpCoreType typeTwo = _socketTwo.GetWarpCoreType();

            if ((typeOne == WarpCoreType.Black) && (typeTwo == WarpCoreType.White))
            {
                Activate(oneTowardsTwo: true);
            } 
            else if ((typeOne == WarpCoreType.White) && (typeTwo == WarpCoreType.Black))
            {
                Activate(oneTowardsTwo: false);
            }
            else
            {
                Deactivate();
            }
        }

        private void Activate(bool oneTowardsTwo)
        {
            // one towards two
            _blackHoleOne.SetActive(oneTowardsTwo);
            _whiteHoleTwo.SetActive(oneTowardsTwo);

            // two towards one
            _blackHoleTwo.SetActive(!oneTowardsTwo);
            _whiteHoleOne.SetActive(!oneTowardsTwo);
        }
        private void Deactivate()
        {
            _blackHoleOne.SetActive(false);
            _blackHoleTwo.SetActive(false);
            _whiteHoleOne.SetActive(false);
            _whiteHoleTwo.SetActive(false);
        }
    }
}
