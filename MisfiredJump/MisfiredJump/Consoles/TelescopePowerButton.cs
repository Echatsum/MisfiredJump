using OWML.Common;
using UnityEngine;

namespace MisfiredJump
{
    public class TelescopePowerButton : Interfaces.AbstractButton
    {
        // Unity
        [SerializeField]
        private TelescopeConsole _telescopeConsole;
        [SerializeField]
        private GameObject _water;
        [SerializeField]
        private GameObject _electricity;

        // Mod config
        private float _timeToFlood; // Probably out of place and flooding should be handled elsewhere, but ehhh

        // Other
        private bool _isPowerOn = false;
        private bool _hasFlooded = false;
        private bool _hasElectrifiedWater = false;

        public TelescopePowerButton() : base(isSingleuseButton: true) { }

        protected override string Nameofclass => nameof(TelescopePowerButton);

        protected override void CheckUnityParameters()
        {
            base.CheckUnityParameters();

            if (_telescopeConsole == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with telescopeConsole [Unity issue]", MessageType.Error);
            }
            if (_water == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with water [Unity issue]", MessageType.Error);
            }
            if (_electricity == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with electricity [Unity issue]", MessageType.Error);
            }
        }
        protected override void GetModParameters()
        {
            base.GetModParameters();

            _timeToFlood = MisfiredJump.Instance.GetTelescopeFloodingTime();
        }

        protected override void Start()
        {
            base.Start();

            // Init prompt texts
            _interactReceiver?.SetPromptText(UITextType.UnknownInterfacePrompt);

            // Initial state
            _water?.SetActive(false);
            _electricity?.SetActive(false);
        }

        protected override void ButtonInteraction()
        {
            _telescopeConsole.SetPower(true);
            _isPowerOn = true;
        }

        public void Update()
        {
            if (_hasElectrifiedWater)
            {
                base.enabled = false; // Cannot get out of this state so we can stop updating
                return;
            }

            var time = TimeLoop.GetMinutesElapsed();
            if (time >= _timeToFlood)
            {
                if (!_hasFlooded)
                {
                    FloodArea();
                }
                if (_isPowerOn)
                {
                    ElectrifyWater();
                }
            }
        }
        private void FloodArea()
        {
            _water.SetActive(true);
            _hasFlooded = true;
        }
        private void ElectrifyWater()
        {
            _electricity.SetActive(true);
            _hasElectrifiedWater = true;
        }

    }
}
