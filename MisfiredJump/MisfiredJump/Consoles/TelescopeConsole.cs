using OWML.Common;
using UnityEngine;

namespace MisfiredJump
{
    public enum TelescopeSfxType
    {
        RotateFail = 0,
        ConsoleRotate = 1,
        BigTelescopeRotate = 2,
        ChangeZoom = 3
    }

    public class TelescopeConsole : Interfaces.AbstractConsole
    {
        // Unity
        [SerializeField]
        private Transform[] _rotatingTelescopes;
        [SerializeField]
        private float _altitudeUpperLimit; // note: limit included
        [SerializeField]
        private float _altitudeLowerLimit;
        [SerializeField]
        private DisplayDial[] _azimuthDials;
        [SerializeField]
        private DisplayDial[] _altitudeDials;

        // Mod config
        private ScreenPrompt _leftRightPrompt;
        private ScreenPrompt _upDownPrompt;
        private ScreenPrompt _changeZoomPrompt;
        private ScreenPrompt _leavePrompt;
        private int _azimuthCode;
        private int _altitudeCode;
        private int _azimuthSecondaryCode;
        private int _altitudeSecondaryCode;
        private string[] _noLensFactIDs;
        private string[] _lensFactIDs;
        private string[] _probeFactIDs;
        private string[] _eastereggFactIDs;

        // Other
        private int _selectedZoom = 0; // Note: 0 is max zoom (units dial)
        private float[] _computedAzimuthChange;
        private float[] _computedAltitudeChange;
        private bool _checkCodeOnceStable;
        private bool _isPowerOn;
        public void SetPower(bool turnOn)
        {
            _isPowerOn = turnOn;
        }

        protected override string Nameofclass => nameof(TelescopeConsole);
        protected override void CheckUnityParameters()
        {
            base.CheckUnityParameters();

            if (_rotatingTelescopes == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with rotating telescopes [Unity issue]", MessageType.Error);
            }
            if (_azimuthDials == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with azimuth dials [Unity issue]", MessageType.Error);
            }
            if (_altitudeDials == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with altitude dials [Unity issue]", MessageType.Error);
            }
        }
        protected override void GetModParameters()
        {
            base.GetModParameters();

            // Get codes
            _azimuthCode = MisfiredJump.Instance.GetAzimuthCode();
            _altitudeCode = MisfiredJump.Instance.GetAltitudeCode();
            _azimuthSecondaryCode = MisfiredJump.Instance.GetAzimuthSecondaryCode();
            _altitudeSecondaryCode = MisfiredJump.Instance.GetAltitudeSecondaryCode();

            // Facts
            _noLensFactIDs = MisfiredJumpFacts.GetRevealInteractFacts(RevealInteractID.TelescopeConsoleNoLens);
            _lensFactIDs = MisfiredJumpFacts.GetRevealInteractFacts(RevealInteractID.TelescopeConsoleLens);
            _probeFactIDs = MisfiredJumpFacts.GetRevealInteractFacts(RevealInteractID.TelescopeProbeFound);
            _eastereggFactIDs = MisfiredJumpFacts.GetRevealInteractFacts(RevealInteractID.TelescopeErnestoFound);

            // Init prompt texts
            _interactReceiver?.SetPromptText(UITextType.UnknownInterfacePrompt);
            var leftRightTelescopeConsolePromptText = MisfiredJump.Instance.NewHorizons.GetTranslationForUI("LeftRightTelescopeConsolePrompt");
            var upDownTelescopeConsolePromptText = MisfiredJump.Instance.NewHorizons.GetTranslationForUI("UpDownTelescopeConsolePrompt");
            var changeZoomTelescopeConsolePromptText = MisfiredJump.Instance.NewHorizons.GetTranslationForUI("ChangeZoomTelescopeConsolePrompt");
            _leftRightPrompt = new ScreenPrompt(InputLibrary.left, InputLibrary.right, leftRightTelescopeConsolePromptText, ScreenPrompt.MultiCommandType.POS_NEG);
            _upDownPrompt = new ScreenPrompt(InputLibrary.up, InputLibrary.down, upDownTelescopeConsolePromptText, ScreenPrompt.MultiCommandType.POS_NEG);
            _changeZoomPrompt = new ScreenPrompt(InputLibrary.jump, changeZoomTelescopeConsolePromptText);
            _leavePrompt = new ScreenPrompt(InputLibrary.cancel, UITextLibrary.GetString(UITextType.LeavePrompt) + "   <CMD>");
        }

        protected override void Start()
        {
            base.Start();

            // Compute azimuth and altitude changes on the telescopes when interacting (once here to avoid doing it every time)
            _computedAzimuthChange = ComputeRotationChange(_azimuthDials, 0f, 360f);
            _computedAltitudeChange = ComputeRotationChange(_altitudeDials, _altitudeLowerLimit, _altitudeUpperLimit);

            // Start as disabled (will update only when interacted with)
            base.enabled = false;
            _selectedZoom = Mathf.Max(_azimuthDials.Length, _altitudeDials.Length)-1; // Starting at max unzoom
        }

        protected override void AddScreenPrompts()
        {
            Locator.GetPromptManager().AddScreenPrompt(_leftRightPrompt, PromptPosition.UpperRight);
            Locator.GetPromptManager().AddScreenPrompt(_upDownPrompt, PromptPosition.UpperRight);
            Locator.GetPromptManager().AddScreenPrompt(_changeZoomPrompt, PromptPosition.UpperRight);
            Locator.GetPromptManager().AddScreenPrompt(_leavePrompt, PromptPosition.UpperRight);
        }
        protected override void RemoveScreenPrompts()
        {
            Locator.GetPromptManager().RemoveScreenPrompt(_leftRightPrompt);
            Locator.GetPromptManager().RemoveScreenPrompt(_upDownPrompt);
            Locator.GetPromptManager().RemoveScreenPrompt(_changeZoomPrompt);
            Locator.GetPromptManager().RemoveScreenPrompt(_leavePrompt);
        }

        private void Update()
        {
            // End updates if player has stopped interacting
            if (!_isPlayerInteracting)
            {
                base.enabled = false;
            }

            // Prompt visibility
            bool isLockedOn = OWInput.IsInputMode(InputMode.SatelliteCam);
            _leftRightPrompt.SetVisibility(isLockedOn);
            _upDownPrompt.SetVisibility(isLockedOn);
            _changeZoomPrompt.SetVisibility(isLockedOn);
            _leavePrompt.SetVisibility(isLockedOn);
            if(!isLockedOn) { return; }

            // Handle button presses
            if (OWInput.IsNewlyPressed(InputLibrary.right) || OWInput.IsNewlyPressed(InputLibrary.right2))
            {
                TryRotate(positive: true);
            }
            else if (OWInput.IsNewlyPressed(InputLibrary.left) || OWInput.IsNewlyPressed(InputLibrary.left2))
            {
                TryRotate(positive: false);
            }
            else if (OWInput.IsNewlyPressed(InputLibrary.up) || OWInput.IsNewlyPressed(InputLibrary.up2))
            {
                TryElevate(positive: true);
            }
            else if (OWInput.IsNewlyPressed(InputLibrary.down) || OWInput.IsNewlyPressed(InputLibrary.down2))
            {
                TryElevate(positive: false);
            }
            else if (OWInput.IsNewlyPressed(InputLibrary.jump))
            {
                if (!IsPowerOn()) { return; }
                if (!IsLensOn()) { return; }

                _selectedZoom--;
                if(_selectedZoom < 0)
                {
                    _selectedZoom = Mathf.Max(_azimuthDials.Length-1, _altitudeDials.Length-1);
                }

                PlaySfx(TelescopeSfxType.ChangeZoom);
            }
            else if (OWInput.IsNewlyPressed(InputLibrary.cancel))
            {
                CancelInteraction();
            }

            // check for code if requested and if dials are settled
            if(!_checkCodeOnceStable) { return; }
            foreach(var dial in _azimuthDials)
            {
                if(dial.IsUpdating()) { return; }
            }
            foreach(var dial in _altitudeDials)
            {
                if(dial.IsUpdating()) { return; }
            }
            CheckForCode();
            CheckForSecondaryCode();
            _checkCodeOnceStable = false; // after checking once, setting flag to false again
        }

        private void TryRotate(bool positive)
        {
            if (!IsPowerOn()) { return; }

            if (_selectedZoom < _azimuthDials.Length)
            {
                var zoomIndex = _selectedZoom;
                bool sideEffect = true;
                while (sideEffect && (zoomIndex < _azimuthDials.Length))
                {
                    sideEffect = _azimuthDials[zoomIndex].Rotate(positive); // rotate and check if dial on the left gets impacted (think 99 going to 100)
                    if (sideEffect)
                    {
                        zoomIndex++;
                    }
                }

                foreach (var telescopeY in _rotatingTelescopes)
                {
                    float y = _computedAzimuthChange[_selectedZoom];
                    telescopeY.Rotate(new Vector3(0f, y * (positive ? 1f : -1f), 0f));
                }

                PlaySfx(TelescopeSfxType.ConsoleRotate);
                PlaySfx(TelescopeSfxType.BigTelescopeRotate);
                _checkCodeOnceStable = true; // updated dials, so check if code okay once dials have settled
            }
        }
        private void TryElevate(bool positive)
        {
            if (!IsPowerOn()) { return; }

            if (_selectedZoom < _altitudeDials.Length && CanChangeAltitude(positive))
            {
                var zoomIndex = _selectedZoom;
                bool sideEffect = true;
                while (sideEffect && (zoomIndex < _altitudeDials.Length))
                {
                    sideEffect = _altitudeDials[zoomIndex].Rotate(positive);
                    if (sideEffect)
                    {
                        zoomIndex++;
                    }
                }

                foreach (var telescopeY in _rotatingTelescopes)
                {
                    float x = _computedAltitudeChange[_selectedZoom];
                    var telescopeX = telescopeY.GetChild(0);
                    telescopeX.Rotate(new Vector3(-x * (positive ? 1f : -1f), 0f, 0f));
                }

                PlaySfx(TelescopeSfxType.ConsoleRotate);
                PlaySfx(TelescopeSfxType.BigTelescopeRotate);
                _checkCodeOnceStable = true;
            }
        }

        private void PlaySfx(TelescopeSfxType sfxType)
        {
            switch (sfxType)
            {
                case TelescopeSfxType.RotateFail:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.GearRotate_Fail, volume: 1f);
                    break;
                case TelescopeSfxType.ConsoleRotate:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.ShipCockpitScopeActivate, volume: 1f);
                    break;
                case TelescopeSfxType.BigTelescopeRotate:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.NomaiPillarRotate, volume: 1f);
                    break;
                case TelescopeSfxType.ChangeZoom:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.ShipCockpitScopeDeactivate, volume: 1f);
                    break;

                default:
                    break;
            }
        }

        private bool IsPowerOn()
        {
            if (!_isPowerOn)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: RotateFail because power is off", MessageType.Info);
                PlaySfx(TelescopeSfxType.RotateFail);
            }

            return _isPowerOn;
        }
        private bool IsLensOn()
        {
            var isLensOn = DialogueConditionManager.SharedInstance.ConditionExists("COND_LENS_ON_TELESCOPE")
                && DialogueConditionManager.SharedInstance.GetConditionState("COND_LENS_ON_TELESCOPE");

            if (!isLensOn)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: RotateFail because lens is off", MessageType.Info);
                MisfiredJumpFacts.RevealFacts(_noLensFactIDs);

                PlaySfx(TelescopeSfxType.RotateFail);
            }
            else
            {
                MisfiredJumpFacts.RevealFacts(_lensFactIDs);
            }

            return isLensOn;
        }

        private bool CanChangeAltitude(bool positive)
        {
            for (int i = _altitudeDials.Length-1; i >= _selectedZoom; i--)
            {
                if (positive && !_altitudeDials[i].IsAtMaxValue()) // If we are not at leftmost rollover point, then altitude can change
                {
                    return true;
                }
                if(!positive && !_altitudeDials[i].IsAtMinValue())
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckForCode()
        {
            // check azimuth
            bool isValidAzimuth = IsValidPartialCode(_azimuthDials, _azimuthCode);
            if(!isValidAzimuth) { return; } // Wrong code, no need to check altitude

            // check altitude
            bool isValidAltitude = IsValidPartialCode(_altitudeDials, _altitudeCode);
            if (!isValidAltitude) { return; }

            RevealProbeFacts(); // Both codes valid, we activate
        }

        private void CheckForSecondaryCode()
        {
            // check azimuth
            bool isValidAzimuth = IsValidPartialCode(_azimuthDials, _azimuthSecondaryCode);
            if (!isValidAzimuth) { return; } // Wrong code, no need to check altitude

            // check altitude
            bool isValidAltitude = IsValidPartialCode(_altitudeDials, _altitudeSecondaryCode);
            if (!isValidAltitude) { return; }

            RevealEasterEggFacts(); // Both codes valid, we activate
        }

        private void RevealProbeFacts()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Aligned with probe code", MessageType.Info);
            MisfiredJumpFacts.RevealFacts(_probeFactIDs);
        }
        private void RevealEasterEggFacts()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Aligned with Ernesto code", MessageType.Info);
            MisfiredJumpFacts.RevealFacts(_eastereggFactIDs);
        }

        private bool IsValidPartialCode(DisplayDial[] dials, int code)
        {
            int currentValue = 0;
            for (int i = 0; i < dials.Length; i++)
            {
                // TODO: having hardcoded 100 is bad, but for now will do (aka for me 2 symbols per dial, so value fits into two digits)
                currentValue += dials[i].GetDisplayedValue() * ((int)Mathf.Pow(100,i)); // value will look like ..zzyyxx where xx is the first dial value, yy is second, etc
            }

            return (currentValue == code);
        }

        private float[] ComputeRotationChange(DisplayDial[] dials, float min, float max)
        {
            float[] computedValues = new float[dials.Length];
            float section = max - min;
            for (int i = dials.Length - 1; i >= 0; i--)
            {
                var possibleValues = dials[i].NumberOfPossibleValues();
                computedValues[i] = section / possibleValues; // every time a change occurs at that zoom, rotate by this much
                section = computedValues[i]; // this section becomes the next one to subdivide
            }

            return computedValues;
        }
    }
}
