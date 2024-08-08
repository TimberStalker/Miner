using UnityEngine;

public class PidVisualizer : MonoBehaviour
{
    [SerializeField] float kp;
    [SerializeField] float ki;
    [SerializeField] float kd;
    [SerializeField] float targetValue;
    [SerializeField] float initialValue;
    [SerializeField] int iterationCount = 200;

    [SerializeField] AnimationCurve displayCurve;
    private void OnValidate()
    {
        var pid = new PidController(new PidControllerSettings(kp, ki, kd));
        displayCurve.ClearKeys();

        float pidValue = initialValue;
        for(int i = 0; i < iterationCount; i++)
        {
            displayCurve.AddKey(i / (float)iterationCount, pidValue);
            pidValue = pid.Update(targetValue, pidValue, Time.fixedDeltaTime);
        }
        displayCurve.AddKey(1, pidValue);
    }
}
