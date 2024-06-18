using System.Collections.Generic;

namespace NOOD.ModifiableStats
{
    [System.Serializable]
    public class ModifiableStats<T> where T : struct
    {
        private T initValue;
        public T InitValue { get => initValue; }
        public T Value
        {
            get
            {
                T tempValue = initValue;
                foreach(Modifier<T> modifier in modifiers)
                {
                    tempValue = modifier.ApplyModify(tempValue);
                }
                return tempValue;
            }
        }
        List<Modifier<T>> modifiers = new();

        #region Constructor
        // Constructor
        public ModifiableStats(T value)
        {
            this.initValue = value;
        }
        public ModifiableStats()
        {
            this.initValue = default;
        }
        #endregion

        #region Setup
        public void SetInitValue(T value)
        {
            this.initValue = value;
        }
        #endregion

        public void AddModifier(ModifyType modify, T value)
        {
            modifiers.Add(new Modifier<T>(modify, value));
        }
        public void RemoveModifier(ModifyType modify, T value)
        {
            foreach(Modifier<T> modifier in modifiers)
            {
                if(modifier.Compare(modify, value)) modifiers.Remove(modifier);
                return;
            }
        }
        public void ClearAllModifiers()
        {
            modifiers.Clear();
        }
    }
}