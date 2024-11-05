using OWML.Common;
using System.Collections.Generic;
using UnityEngine;

namespace MisfiredJump
{
    public class TelescopeCodeController : Interfaces.AbstractNewHorizonMonoBehaviour
    {
        // Unity
        [SerializeField]
        private DisplayDial[] _azimuthDials;
        [SerializeField]
        private DisplayDial[] _altitudeDials;
        [SerializeField]
        private bool _isSecondaryCode;

        // Mod config
        private int _azimuthCodeRaw;
        private int _altitudeCodeRaw;

        protected override string Nameofclass => nameof(TelescopeCodeController);
        protected override void CheckUnityParameters()
        {
            base.CheckUnityParameters();

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

            // Get code
            if (!_isSecondaryCode)
            {
                _azimuthCodeRaw = MisfiredJump.Instance.GetAzimuthCode();
                _altitudeCodeRaw = MisfiredJump.Instance.GetAltitudeCode();
            }
            else
            {
                _azimuthCodeRaw = MisfiredJump.Instance.GetAzimuthSecondaryCode();
                _altitudeCodeRaw = MisfiredJump.Instance.GetAltitudeSecondaryCode();
            }
        }

        protected override void Start()
        {
            base.Start();

            // Parse and set code (azimuth)
            var azimuthLen = 0;
            foreach(var dial in _azimuthDials)
            {
                azimuthLen += dial.NumberOfSlots();
            }
            SetSymbols(_azimuthDials, ParseCode(_azimuthCodeRaw, azimuthLen));

            // Parse and set code (altitude)
            var altitudeLen = 0;
            foreach (var dial in _altitudeDials)
            {
                altitudeLen += dial.NumberOfSlots();
            }
            SetSymbols(_altitudeDials, ParseCode(_altitudeCodeRaw, altitudeLen));
        }

        private int[] ParseCode(int codeRaw, int len)
        {
            var array = new int[len];
            int tmp = codeRaw;
            for(int i=0; i<len; i++)
            {
                var value = tmp % 10;
                tmp = (tmp - value) / 10;
                array[i] = value;
            }
            return array;
        }

        private void SetSymbols(DisplayDial[] dials, int[] symbols)
        {
            var list = new List<int>();
            list.AddRange(symbols);

            int startIndex = 0;
            for (int i=0; i<dials.Length; i++)
            {
                var subList = list.GetRange(startIndex, dials[i].NumberOfSlots());
                dials[i].SetSymbols(subList.ToArray());
                startIndex += subList.Count;
            }
        }

    }
}
