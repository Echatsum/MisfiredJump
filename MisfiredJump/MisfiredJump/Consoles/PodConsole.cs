using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using UnityEngine;

namespace MisfiredJump
{
    public enum Direction
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }
    public enum PodPositionContent
    {
        Empty = 0,
        Obstacle = 1,
        Nest = 2
    }
    public enum PodSfxType
    {
        AirlockOpen = 0,
        AirlockClose = 1,
        LavaSplash = 2,
        Warping = 3,
        PodGroaning = 4,
        PodFailure = 5,
        Stomping = 6,
        UsingArm = 7,
        Impacts = 8,
        NestFound = 9,
        EggGrabbed = 10
    }

    public class PodConsole : Interfaces.AbstractConsole
    {
        // Unity
        [SerializeField]
        private GameObject _sealedMode;
        [SerializeField]
        private GameObject _meltingMode;
        [SerializeField]
        private GameObject _deathMode;
        [SerializeField]
        private Transform _projectorsParent;
        [SerializeField]
        private MapCrossMeshGenerator _mapCross;
        [SerializeField]
        private MapPodMeshGenerator _mapPod;
        [SerializeField]
        private TemperatureMeshGenerator _temperatureDisplay;
        [SerializeField]
        private GameObject _eggSocketParent;
        [SerializeField]
        private GameObject _buildingMusic;
        [SerializeField]
        private GameObject _diveMusic;
        [SerializeField]
        private GameObject _diveDangerMusic;

        // New Horizons generated
        private Transform _emptyProjector;
        private Transform _leftObstacleProjector;
        private Transform _frontObstacleProjector;
        private Transform _rightObstacleProjector;
        private Transform _leftRightObstacleProjector;
        private Transform _nestProjector;
        private Transform _emptyWithArmProjector;
        private Transform _leftObstacleWithArmProjector;
        private Transform _frontObstacleWithArmProjector;
        private Transform _rightObstacleWithArmProjector;
        private Transform _leftRightObstacleWithArmProjector;
        private Transform _nestWithArmProjector;

        // Mod config
        private ScreenPrompt _dropDownPrompt;
        private ScreenPrompt _leftRightPrompt;
        private ScreenPrompt _forwardPrompt;
        private ScreenPrompt _useArmPrompt;
        private ScreenPrompt _leavePrompt;
        private Vector2Int _mapDimensions;
        private Vector2Int[] _obstaclePositions;
        private Vector2Int _nestPosition;
        private Vector2Int _startPosition;
        private Direction _startFacing;
        private float _dangerTemperature;
        private float _maxTemperature;
        private float _timeBetweenTemperatureUpdate;
        private Vector3 _temperatureChangePerUpdate;
        private string[] _divingFactIDs;
        private string[] _nestFoundFactIDs;

        // Other
        private Transform _currentlyActiveProjector;
        private Vector2Int _currentPosition;
        private Direction _currentFacing;
        private float _currentTemperature;
        private float _nextTemperatureUpdateTime;
        private float _nextImpactUpdateTime;
        private bool _podActive;
        private bool _isUpdatingStatus;
        private bool _isUsingArm;
        private bool _isArmHoldingEgg;
        public bool IsUpdatingStatus() => _isUpdatingStatus;

        protected override string Nameofclass => nameof(PodConsole);
        protected override void CheckUnityParameters()
        {
            base.CheckUnityParameters();

            if (_sealedMode == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with sealed mode [Unity issue]", MessageType.Error);
            }
            if (_meltingMode == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with melting mode [Unity issue]", MessageType.Error);
            }
            if (_deathMode == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with death mode [Unity issue]", MessageType.Error);
            }
            if (_projectorsParent == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with projectorParent [Unity issue]", MessageType.Error);
            }
            if (_mapCross == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with map cross [Unity issue]", MessageType.Error);
            }
            if (_mapPod == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with map pod [Unity issue]", MessageType.Error);
            }
            if (_temperatureDisplay == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with temperature display [Unity issue]", MessageType.Error);
            }
            if (_eggSocketParent == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with eggSocketParent [Unity issue]", MessageType.Error);
            }
            if (_buildingMusic == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with buildingMusic [Unity issue]", MessageType.Error);
            }
            if (_diveMusic == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with diveMusic [Unity issue]", MessageType.Error);
            }
            if (_diveDangerMusic == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass} was not set with diveDangerMusic [Unity issue]", MessageType.Error);
            }
        }
        protected override void GetNHParameters()
        {
            base.GetNHParameters();

            // Setup projectors
            _emptyProjector = _projectorsParent.Find("Empty");
            _emptyProjector?.gameObject.SetActive(false);
            _leftObstacleProjector = _projectorsParent.Find("LeftObstacle");
            _leftObstacleProjector?.gameObject.SetActive(false);
            _frontObstacleProjector = _projectorsParent.Find("FrontObstacle");
            _frontObstacleProjector?.gameObject.SetActive(false);
            _rightObstacleProjector = _projectorsParent.Find("RightObstacle");
            _rightObstacleProjector?.gameObject.SetActive(false);
            _leftRightObstacleProjector = _projectorsParent.Find("LeftRightObstacle");
            _leftRightObstacleProjector?.gameObject.SetActive(false);
            _nestProjector = _projectorsParent.Find("Nest");
            _nestProjector?.gameObject.SetActive(false);
            _emptyWithArmProjector = _projectorsParent.Find("EmptyArm");
            _emptyWithArmProjector?.gameObject.SetActive(false);
            _leftObstacleWithArmProjector = _projectorsParent.Find("LeftObstacleArm");
            _leftObstacleWithArmProjector?.gameObject.SetActive(false);
            _frontObstacleWithArmProjector = _projectorsParent.Find("FrontObstacleArm");
            _frontObstacleWithArmProjector?.gameObject.SetActive(false);
            _rightObstacleWithArmProjector = _projectorsParent.Find("RightObstacleArm");
            _rightObstacleWithArmProjector?.gameObject.SetActive(false);
            _leftRightObstacleWithArmProjector = _projectorsParent.Find("LeftRightObstacleArm");
            _leftRightObstacleWithArmProjector?.gameObject.SetActive(false);
            _nestWithArmProjector = _projectorsParent.Find("NestArm");
            _nestWithArmProjector?.gameObject.SetActive(false);

            // TODO: Add error logs if those are null but I'm too tired to do it now
            // TODO: There's probably a visually nicer way to do this (enum/dict?) but not going to bother for now
        }
        protected override void GetModParameters()
        {
            base.GetModParameters();

            // Facts
            _divingFactIDs = MisfiredJumpFacts.GetRevealInteractFacts(RevealInteractID.ScaldingAbyssLavaPodDive);
            _nestFoundFactIDs = MisfiredJumpFacts.GetRevealInteractFacts(RevealInteractID.ScaldingAbyssLavaPodNest);

            // Init prompt texts
            _interactReceiver?.SetPromptText(UITextType.UnknownInterfacePrompt);
            var dropDownPodConsolePromptText = MisfiredJump.Instance.NewHorizons.GetTranslationForUI("DropDownPodConsolePrompt"); // TODO: change key
            var leftRightPodConsolePromptText = MisfiredJump.Instance.NewHorizons.GetTranslationForUI("LeftRightPodConsolePrompt");
            var forwardPodConsolePromptText = MisfiredJump.Instance.NewHorizons.GetTranslationForUI("ForwardPodConsolePrompt");
            var useArmPodConsolePromptText = MisfiredJump.Instance.NewHorizons.GetTranslationForUI("UseArmPodConsolePrompt");
            _dropDownPrompt = new ScreenPrompt(InputLibrary.up, dropDownPodConsolePromptText);
            _leftRightPrompt = new ScreenPrompt(InputLibrary.left, InputLibrary.right, leftRightPodConsolePromptText, ScreenPrompt.MultiCommandType.POS_NEG);
            _forwardPrompt = new ScreenPrompt(InputLibrary.up, forwardPodConsolePromptText);
            _useArmPrompt = new ScreenPrompt(InputLibrary.jump, useArmPodConsolePromptText);
            _leavePrompt = new ScreenPrompt(InputLibrary.cancel, UITextLibrary.GetString(UITextType.LeavePrompt) + "   <CMD>");

            // Init map
            _mapDimensions = MisfiredJump.Instance.GetPodMapDimensions();
            _nestPosition = MisfiredJump.Instance.GetPodNestPosition();
            _obstaclePositions = MisfiredJump.Instance.GetPodObstaclePositions();
            _startPosition = MisfiredJump.Instance.GetPodStartPosition();
            _startFacing = MisfiredJump.Instance.GetPodStartFacing();

            // init temperature parameters
            _dangerTemperature = MisfiredJump.Instance.GetPodDangerTemperature();
            _maxTemperature = MisfiredJump.Instance.GetPodMaxTemperature();
            _timeBetweenTemperatureUpdate = MisfiredJump.Instance.GetPodTemperatureUpdateFrequency();
            _temperatureChangePerUpdate = MisfiredJump.Instance.GetPodTemperatureChange();
        }

        protected override void Start() {
            base.Start();

            // Setup
            _meltingMode.SetActive(false);
            _deathMode.SetActive(false);
            _eggSocketParent.SetActive(false);
            StartCoroutine(DeactivatePod(playSfx: false));
            _isPlayerInteracting = false;
            base.enabled = false;
        }

        protected override void AddScreenPrompts()
        {
            Locator.GetPromptManager().AddScreenPrompt(_dropDownPrompt, PromptPosition.UpperRight);
            Locator.GetPromptManager().AddScreenPrompt(_leftRightPrompt, PromptPosition.UpperRight);
            Locator.GetPromptManager().AddScreenPrompt(_forwardPrompt, PromptPosition.UpperRight);
            Locator.GetPromptManager().AddScreenPrompt(_useArmPrompt, PromptPosition.UpperRight);
            Locator.GetPromptManager().AddScreenPrompt(_leavePrompt, PromptPosition.UpperRight);
        }
        protected override void RemoveScreenPrompts()
        {
            Locator.GetPromptManager().RemoveScreenPrompt(_dropDownPrompt);
            Locator.GetPromptManager().RemoveScreenPrompt(_leftRightPrompt);
            Locator.GetPromptManager().RemoveScreenPrompt(_forwardPrompt);
            Locator.GetPromptManager().RemoveScreenPrompt(_useArmPrompt);
            Locator.GetPromptManager().RemoveScreenPrompt(_leavePrompt);
        }


        private void Update()
        {
            if (!_isPlayerInteracting && _currentTemperature == 0f)
            {
                base.enabled = false;
            }

            UpdateTemperature();
            UpdateImpacts();

            bool isLockedOn = OWInput.IsInputMode(InputMode.SatelliteCam);
            _dropDownPrompt.SetVisibility(isLockedOn && !_podActive && !_isUpdatingStatus);
            _leftRightPrompt.SetVisibility(isLockedOn && _podActive && !_isUpdatingStatus);
            _forwardPrompt.SetVisibility(isLockedOn && _podActive && !_isUpdatingStatus);
            _useArmPrompt.SetVisibility(isLockedOn && _podActive && !_isArmHoldingEgg && !_isUpdatingStatus);
            _leavePrompt.SetVisibility(isLockedOn);
            if (!isLockedOn) { return; }

            // Handle button presses
            if (OWInput.IsNewlyPressed(InputLibrary.cancel))
            {
                CancelInteraction();
                return;
            }

            // Only the cancel action is available while pod is doing something
            if (_isUpdatingStatus){ return; }

            if (OWInput.IsNewlyPressed(InputLibrary.right) || OWInput.IsNewlyPressed(InputLibrary.right2))
            {
                if (_podActive)
                {
                    StartCoroutine(ChangeDirection(leftTurn: false));
                }
            }
            else if (OWInput.IsNewlyPressed(InputLibrary.left) || OWInput.IsNewlyPressed(InputLibrary.left2))
            {
                if (_podActive)
                {
                    StartCoroutine(ChangeDirection(leftTurn: true));
                }
            }
            else if(OWInput.IsNewlyPressed(InputLibrary.up) || OWInput.IsNewlyPressed(InputLibrary.up2))
            {
                if (_podActive)
                {
                    if(GetFrontContent() == PodPositionContent.Empty)
                    {
                        StartCoroutine(GoForward());
                    }
                    else
                    {
                        // TODO: deny move
                    }
                }
            }
            else if(OWInput.IsNewlyPressed(InputLibrary.down) || OWInput.IsNewlyPressed(InputLibrary.down2))
            {
                if (!_podActive)
                {
                    StartCoroutine(ActivatePod());
                }
            }
            else if (OWInput.IsNewlyPressed(InputLibrary.jump))
            {
                if (_podActive && !_isArmHoldingEgg)
                {
                    StartCoroutine(UseArm());
                }
            }
        }

        private void UpdateImpacts()
        {
            if (_podActive && _isArmHoldingEgg)
            {
                var time = Time.time;
                if (time < _nextImpactUpdateTime) { return; }

                _nextImpactUpdateTime = time + 0.8f; // TODO: hardcoded bad

                PlaySfx(PodSfxType.Impacts);
            }
        }
        private void UpdateTemperature()
        {
            var time = Time.time;
            if(time < _nextTemperatureUpdateTime) { return; }
            _nextTemperatureUpdateTime = time + _timeBetweenTemperatureUpdate;

            if (_podActive)
            {
                _currentTemperature += _isArmHoldingEgg ? _temperatureChangePerUpdate[2] : _temperatureChangePerUpdate[0];

                if (_isArmHoldingEgg)
                {
                    PlaySfx(PodSfxType.Impacts);
                }

                if (_currentTemperature >= _dangerTemperature && !_meltingMode.activeSelf) // only on the frame going over
                {
                    PlaySfx(PodSfxType.PodGroaning);
                }
                if (_currentTemperature >= _maxTemperature && !_deathMode.activeSelf) // only on the frame going over
                {
                    PlaySfx(PodSfxType.PodFailure);
                }
            }
            else
            {
                _currentTemperature += _temperatureChangePerUpdate[1];
                if(_currentTemperature < 0f) { _currentTemperature = 0f; }
            }

            _temperatureDisplay.SetTemperature(_currentTemperature, _maxTemperature);

            _meltingMode.SetActive(_currentTemperature >= _dangerTemperature);
            _deathMode.SetActive(_currentTemperature >= _maxTemperature);
        }

        private float UpdateLinkedDisplays()
        {
            var waitTimeDefault = 1f; // TODO: bad hardcoded values
            var waitTimeLong = 2f;
            var waitTime = _isUsingArm ? waitTimeLong : waitTimeDefault;

            // projectors
            UpdateProjectors();
            // map
            _mapCross.SetCoordinates(_currentPosition, _mapDimensions);
            _mapPod.SetCoordinates(_currentPosition, _currentFacing, _mapDimensions);

            return waitTime;
        }

        private void UpdateProjectors()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Updating pod projectors", MessageType.Info);

            // Deactivate previous projector
            if (_currentlyActiveProjector != null)
            {
                _currentlyActiveProjector.gameObject.SetActive(false);
                _currentlyActiveProjector = null;
            }

            if (!_podActive) { return; }

            Transform nextProjector;

            // Check if anything in front
            var frontContent = GetFrontContent();
            if (frontContent == PodPositionContent.Nest)
            {
                nextProjector = _isUsingArm ? _nestWithArmProjector : _nestProjector;
            }
            else if (frontContent == PodPositionContent.Obstacle)
            {
                nextProjector = _isUsingArm ? _frontObstacleWithArmProjector : _frontObstacleProjector;
            }
            // Front is empty, so we need to check sides
            else
            {
                bool leftHasObstacle = GetLeftContent() == PodPositionContent.Obstacle;
                bool rightHasObstacle = GetRightContent() == PodPositionContent.Obstacle;
                if (leftHasObstacle)
                {
                    if (rightHasObstacle) // Both left and right obstacles
                    {
                        nextProjector = _isUsingArm ? _leftRightObstacleWithArmProjector : _leftRightObstacleProjector;
                    }
                    else // Only left obstacle
                    {
                        nextProjector = _isUsingArm ? _leftObstacleWithArmProjector : _leftObstacleProjector;
                    }
                }
                else
                {
                    if (rightHasObstacle) // Only right obstacle
                    {
                        nextProjector = _isUsingArm ? _rightObstacleWithArmProjector : _rightObstacleProjector;
                    }
                    else // No obstacle at all
                    {
                        nextProjector = _isUsingArm ? _emptyWithArmProjector : _emptyProjector;
                    }
                }
            }

            nextProjector.gameObject.SetActive(true);
            nextProjector.GetComponent<AutoSlideProjector>().Reset();
            _currentlyActiveProjector = nextProjector;
            return;
        }

        private IEnumerator ChangeDirection(bool leftTurn)
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Turning pod (leftTurn: {leftTurn})", MessageType.Info);
            _isUpdatingStatus = true;

            // Compute new direction
            _currentFacing = GetOtherDirection(leftOf: leftTurn, _currentFacing);

            // Process visuals and sound
            var waitTime = UpdateLinkedDisplays();
            StartCoroutine(PlayChangeDirectionSfx(waitTime));
            yield return new WaitForSeconds(waitTime);

            // Done
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Can interact with pod again", MessageType.Info);
            _isUpdatingStatus = false;
        }
        private IEnumerator GoForward()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Moving pod forward", MessageType.Info);
            _isUpdatingStatus = true;

            // Update position
            _currentPosition = GetAdjacentPosition(_currentFacing);

            // Process visuals and sound
            var waitTime = UpdateLinkedDisplays();
            StartCoroutine(PlayGoForwardSfx(waitTime));
            yield return new WaitForSeconds(waitTime);

            // Done
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Can interact with pod again", MessageType.Info);
            _isUpdatingStatus = false;
        }

        private IEnumerator ActivatePod()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Activating pod", MessageType.Info);
            _isUpdatingStatus = true;

            // Sealing
            _sealedMode.SetActive(true);

            // Dropping in lava
            PlaySfx(PodSfxType.AirlockClose);
            yield return new WaitForSeconds(1f); // TODO: hardcoded bad
            _podActive = true;
            MisfiredJumpFacts.RevealFacts(_divingFactIDs);

            // Process visuals and sound
            PlaySfx(PodSfxType.LavaSplash);
            UpdateMusic();
            var waitTime = UpdateLinkedDisplays();
            yield return new WaitForSeconds(waitTime);

            // Done
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Can interact with pod again", MessageType.Info);
            _isUpdatingStatus = false;
        }
        public IEnumerator DeactivatePod(bool playSfx = true)
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Deactivating pod", MessageType.Info);
            _isUpdatingStatus = true;

            // Warp to surface
            _podActive = false;
            _currentPosition = _startPosition;
            _currentFacing = _startFacing;
            _sealedMode.SetActive(false);

            // Process visuals and sound
            var waitTime = UpdateLinkedDisplays();
            if (playSfx)
            {
                PlaySfx(PodSfxType.Warping);
                PlaySfx(PodSfxType.AirlockOpen);
            }
            UpdateMusic();
            yield return new WaitForSeconds(waitTime);

            // Done
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Can interact with pod again", MessageType.Info);
            _isUpdatingStatus = false;
        }
        private IEnumerator UseArm()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Using pod arm", MessageType.Info);
            _isUpdatingStatus = true;

            // Use arm, process visuals and sounds
            _isUsingArm = true;
            PlaySfx(PodSfxType.UsingArm);
            var waitTime = UpdateLinkedDisplays();
            yield return new WaitForSeconds(waitTime);

            // Check if nest
            if(GetFrontContent() == PodPositionContent.Nest)
            {
                _nestPosition = new Vector2Int(-1,-1); // Essentially remove nest
                GrabEgg();
            }

            // Stop the arm, process visuals and sounds
            _isUsingArm = false;
            waitTime = UpdateLinkedDisplays();
            UpdateMusic();
            yield return new WaitForSeconds(waitTime);

            // Done
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Can interact with pod again", MessageType.Info);
            _isUpdatingStatus = false;
        }

        private void UpdateMusic()
        {
            // Normal
            _buildingMusic?.SetActive(!_podActive);
            // Dive
            _diveMusic.SetActive(_podActive && !_isArmHoldingEgg);
            // Dive + Egg acquired
            _diveDangerMusic?.SetActive(_podActive && _isArmHoldingEgg);
        }
        private void PlaySfx(PodSfxType sfxType)
        {
            switch (sfxType)
            {
                case PodSfxType.AirlockOpen:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.NomaiTimeLoopOpen, volume: 1f);
                    break;
                case PodSfxType.AirlockClose:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.NomaiTimeLoopClose, volume: 1f);
                    break;
                case PodSfxType.LavaSplash:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.Splash_Lava, volume: 1f);
                    break;
                case PodSfxType.Warping:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.BH_BlackHoleEmission, volume: 1f);
                    break;
                case PodSfxType.PodGroaning:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.ShipEatenGroan, volume: 1f); // ShipEatenGroan or DB_VineImpact
                    break;
                case PodSfxType.PodFailure:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.DamBreak_RW_Base, volume: 1f); // Unsure about that one
                    break;
                case PodSfxType.Stomping:
                    if(Random.Range(0f,1f) < .5f) { Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.ShipImpact_MediumDamage, volume: 1f); }
                    else { Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.ShipImpact_HeavyDamage, volume: 1f); }
                    break;
                case PodSfxType.UsingArm:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.ShipThrustRotationalUnderwater, volume: 1f); // ShipThrustRotationalUnderwater or SecretPassage_Stop
                    break;
                case PodSfxType.Impacts:
                    if (Random.Range(0f, 1f) < .5f) { Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.ShipImpact_NoDamage, volume: 1f); }
                    else { Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.ShipImpact_LightDamage, volume: 1f); }
                    break;
                case PodSfxType.NestFound:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.DBAnglerfishDetectDisturbance, volume: 1f);
                    break;
                case PodSfxType.EggGrabbed:
                    Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.DBAnglerfishOpeningMouth, volume: 1f);
                    break;

                default:
                    break;
            }
        }
        private IEnumerator PlayGoForwardSfx(float availableTime)
        {
            // TODO: something better timed
            PlaySfx(PodSfxType.Stomping);
            yield return new WaitForSeconds(availableTime);
            PlaySfx(PodSfxType.Stomping);
        }
        private IEnumerator PlayChangeDirectionSfx(float availableTime)
        {
            // TODO: something better timed
            PlaySfx(PodSfxType.Stomping);
            yield return new WaitForSeconds(availableTime);
            PlaySfx(PodSfxType.Stomping);
        }

        private void FoundNest()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Found nest", MessageType.Info);
            MisfiredJumpFacts.RevealFacts(_nestFoundFactIDs);

            PlaySfx(PodSfxType.NestFound);
        }
        private void GrabEgg()
        {
            MisfiredJump.Instance.ModHelper.Console.WriteLine($"{Nameofclass}: Grabbed the egg", MessageType.Info);

            PlaySfx(PodSfxType.EggGrabbed);
            _isArmHoldingEgg = true;
            _eggSocketParent.SetActive(true); // Grabbed the egg
        }

        private Direction GetOtherDirection(bool leftOf, Direction direction)
        {
            if(direction == Direction.North) { return leftOf ? Direction.West : Direction.East; }
            if (direction == Direction.East) { return leftOf ? Direction.North : Direction.South; }
            if (direction == Direction.South) { return leftOf ? Direction.East : Direction.West; }
            return leftOf ? Direction.South : Direction.North;
        }
        private Vector2Int GetAdjacentPosition(Direction facing)
        {
            int changeInX = facing == Direction.East ? 1 : (facing == Direction.West ? -1 : 0); // Ternaries? Nah, quaternaries!
            int changeInY = facing == Direction.North ? 1 : (facing == Direction.South ? -1 : 0);
            return _currentPosition + new Vector2Int(changeInX, changeInY);
        }
        private PodPositionContent GetPositionContent(Vector2Int position)
        {
            if (_nestPosition.Equals(position)) { return PodPositionContent.Nest; }

            foreach(var obstaclePosition in _obstaclePositions)
            {
                if(obstaclePosition.Equals(position)) { return PodPositionContent.Obstacle; }
            }

            return PodPositionContent.Empty;
        }

        private PodPositionContent GetLeftContent()
        {
            return GetPositionContent(GetAdjacentPosition(GetOtherDirection(leftOf: true, _currentFacing)));
        }
        private PodPositionContent GetFrontContent()
        {
            var frontContent = GetPositionContent(GetAdjacentPosition(_currentFacing));
            if(frontContent == PodPositionContent.Nest)
            {
                FoundNest();
            }

            return frontContent;
        }
        private PodPositionContent GetRightContent()
        {
            return GetPositionContent(GetAdjacentPosition(GetOtherDirection(leftOf: false, _currentFacing)));
        }
    }
}
