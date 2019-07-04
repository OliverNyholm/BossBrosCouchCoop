using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CompareInt : Conditional
{
    public enum IntComparison
    {
        Less,
        Equal,
        Greater
    }

    public IntComparison myOperator;

    public SharedInt myFirstValue;
    public int mySecondValue;

    public override TaskStatus OnUpdate()
    {
        bool success = false;
        switch (myOperator)
        {
            case IntComparison.Less:
                success = myFirstValue.Value < mySecondValue;
                break;
            case IntComparison.Equal:
                success = myFirstValue.Value == mySecondValue;
                break;
            case IntComparison.Greater:
                success = myFirstValue.Value > mySecondValue;
                break;
        }

        return success ? TaskStatus.Success : TaskStatus.Failure;
    }
}