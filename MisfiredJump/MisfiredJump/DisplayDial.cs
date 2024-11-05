using UnityEngine;

namespace MisfiredJump
{
    // Heavily inspired from RotaryDial
    public class DisplayDial : MonoBehaviour
    {
        [SerializeField]
        private SymbolMeshGenerator[] _displaySlots; // Note: index 0 is the rightmost one
        [SerializeField]
        private int[] _targetSymbols;
        // Note: _displaySlots, _targetSymbols must be the same length

        private bool _isUpdating;
        public bool IsUpdating()
        {
            return _isUpdating;
        }

        private void Awake()
        {
            // starts enabled just to make sure update is right
            _isUpdating = true;
            base.enabled = true;
        }

        public void Update()
        {
            // if rotating element then this would be here, but for now it's all instant so nothing in Update()
            InstantUpdate();
            _isUpdating = false;

            // stop Update() calls if done
            if (!_isUpdating)
            {
                base.enabled = false;
            }
        }

        public void InstantUpdate()
        {
            for(int i=0; i<_displaySlots.Length; i++)
            {
                var slot = _displaySlots[i];
                var target = _targetSymbols[i];

                slot.SetSymbol(target);
            }
        }

        public bool Rotate(bool positive)
        {
            _isUpdating = true;
            base.enabled = true;

            // increment/decrement on self
            if (positive)
            {
                for(int i=0; i< _displaySlots.Length; i++)
                {
                    _targetSymbols[i]++;
                    if (_targetSymbols[i] < _displaySlots[i].GetSymbolOptionCount())
                    {
                        return false; // no rollover, we can stop immediately
                    }

                    _targetSymbols[i] = 0; // roll over, and continue forloop
                }
                return true; // rollover that goes on to side effect
            }
            else
            {
                for (int i = 0; i < _displaySlots.Length; i++)
                {
                    _targetSymbols[i]--;
                    if (_targetSymbols[i] >= 0)
                    {
                        return false; // no rollover, we can stop immediately
                    }

                    _targetSymbols[i] = _displaySlots[i].GetSymbolOptionCount() - 1; // roll over, and continue forloop
                }
                return true; // rollover that goes on to side effect
            }
        }

        public bool IsAtMaxValue()
        {
            for(int i=0; i<_displaySlots.Length; i++)
            {
                if(_targetSymbols[i] < _displaySlots[i].GetSymbolOptionCount() - 1)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsAtMinValue()
        {
            for (int i = 0; i <_displaySlots.Length; i++)
            {
                if (_targetSymbols[i] > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public int NumberOfPossibleValues()
        {
            int value = 1;
            foreach(var slot in _displaySlots)
            {
                value *= slot.GetSymbolOptionCount();
            }
            return value;
        }

        public int GetDisplayedValue()
        {
            int value = 0;
            for(int i=0; i<_displaySlots.Length; i++)
            {
                value += _targetSymbols[i] * ((int) Mathf.Pow(10, i)); // This assumes that less than 10 symbols per slot, which is dodgy but works for me
            }
            return value;
        }
        public int NumberOfSlots()
        {
            return _displaySlots.Length;
        }

        public void SetSymbols(int[] target)
        {
            for(int i=0; i<target.Length && i< _targetSymbols.Length; i++)
            {
                _targetSymbols[i] = target[i];
            }

            InstantUpdate();
        }
    }
}
