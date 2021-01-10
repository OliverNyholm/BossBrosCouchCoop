using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class DebugPrintTask : Action
{
    public string myDebugText;
    public SharedVariable myVariable1;
    public SharedVariable myVariable2;
    public SharedVariable myVariable3;

    public override TaskStatus OnUpdate()
    {
        if (myVariable1 != null && myVariable2 != null && myVariable3 != null)
            Debug.Log(myDebugText + ", " + myVariable1.GetValue() + ", " + myVariable2.GetValue() + ", " + myVariable3.GetValue());
        else if(myVariable1 != null && myVariable2 != null)
            Debug.Log(myDebugText + ", " + myVariable1.GetValue() + ", " + myVariable2.GetValue());
        else if (myVariable1 != null)
            Debug.Log(myDebugText + ", " + myVariable1.GetValue());
        else
            Debug.Log(myDebugText);

        return TaskStatus.Success;
    }
}