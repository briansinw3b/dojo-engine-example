
namespace NOOD.ModifiableStats
{

    public enum ModifyType
    {
        Plus,
        Subtract,
        Multiply,
        Divide
    }

    [System.Serializable]
    public class Modifier<T> where T : struct
    {
        ModifyType modify;
        T value;

        public Modifier(){}

        public Modifier(ModifyType modify, T value)
        {
            this.modify = modify;
            this.value = value;
        }

        public bool Compare(ModifyType type, T compareValue)
        {
            if(typeof(T) == typeof(float))
            {
                return (modify == type && (float)(object)value == (float)(object)compareValue);
            }
            else if(typeof(T) == typeof(int))
            {
                return (modify == type && (int)(object)value == (int)(object)compareValue);
            }
            else
            {
                return (modify == type && (double)(object)value == (double)(object)compareValue);
            }
        }

        public T ApplyModify(T inputValue) 
        {
            object modifierValue = (object)value;
            object result = (object)inputValue;
            if(typeof(T) == typeof(float))
            {
                float floatModifierValue = (float)modifierValue;
                float floatResult = (float)result;
                switch (modify)
                {
                    case ModifyType.Plus:
                        floatResult += floatModifierValue;
                        break;
                    case ModifyType.Subtract:
                        floatResult -= floatModifierValue;
                        break;
                    case ModifyType.Multiply:
                        floatResult *= floatModifierValue;
                        break;
                    case ModifyType.Divide:
                        floatResult /= floatModifierValue;
                        break;
                    default:
                        break;
                }
                result = floatResult;
            }
            if (typeof(T) == typeof(double))
            {
                double doubleModifierValue = (double)modifierValue;
                double doubleResult = (double)result;
                switch (modify)
                {
                    case ModifyType.Plus:
                        doubleResult += doubleModifierValue;
                        break;
                    case ModifyType.Subtract:
                        doubleResult -= doubleModifierValue;
                        break;
                    case ModifyType.Multiply:
                        doubleResult *= doubleModifierValue;
                        break;
                    case ModifyType.Divide:
                        doubleResult /= doubleModifierValue;
                        break;
                    default:
                        break;
                }
                result = doubleResult;
            }
            if (typeof(T) == typeof(int)) 
            {
                int intModifierValue = (int)modifierValue;
                int intResult = (int)result;
                switch (modify)
                {
                    case ModifyType.Plus:
                        intResult += intModifierValue;
                        break;
                    case ModifyType.Subtract:
                        intResult -= intModifierValue;
                        break;
                    case ModifyType.Multiply:
                        intResult += intModifierValue;
                        break;
                    case ModifyType.Divide:
                        intResult /= intModifierValue;
                        break;
                    default:
                        break;
                }
                result = intResult;
            }

            return (T)(object)result;
        }
    }
}
