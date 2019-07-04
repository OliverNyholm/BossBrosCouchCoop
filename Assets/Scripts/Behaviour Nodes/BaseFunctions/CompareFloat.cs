using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CompareFloat : Conditional
{
    public enum FloatComparison
    {
        Less,
        Equal,
        Greater
    }

    public FloatComparison myOperator;

    public SharedFloat myFirstValue;
    public float mySecondValue;

    public override TaskStatus OnUpdate()
    {
        bool success = false;
        switch (myOperator)
        {
            case FloatComparison.Less:
                success = myFirstValue.Value < mySecondValue;
                break;
            case FloatComparison.Equal:
                success = myFirstValue.Value == mySecondValue;
                break;
            case FloatComparison.Greater:
                success = myFirstValue.Value > mySecondValue;
                break;
        }

        return success ? TaskStatus.Success : TaskStatus.Failure;
    }
}