using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

public interface IPidController<T>
{
    T Update(T setpoint, T measured_value, float dt);
    void Reset();
}

public interface IPidControllerSettings<T>
{
    public T Kp { get; }
    public T Ki { get; }
    public T Kd { get; }
}
public class PidControllerSettings : IPidControllerSettings<float>
{
    public PidControllerSettings(float kp, float ki, float kd)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
    }
    public float Kp { get; set; }
    public float Ki { get; set; }
    public float Kd { get; set; }
}
public class FunctionalPidControllerSettings : IPidControllerSettings<float>
{
    public Func<float> KpSelector { get; set; }= () => 0;
    public Func<float> KiSelector { get; set; }= () => 0;
    public Func<float> KdSelector { get; set; } = () => 0;

    public float Kp => KpSelector();
    public float Ki => KiSelector();
    public float Kd => KdSelector();
}
public class PidController : IPidController<float>
{
    public PidController(IPidControllerSettings<float> settings)
    {
        Settings = settings;
    }

    public IPidControllerSettings<float> Settings { get; }

    float integral;
    float previous_error;
    public float Update(float setpoint, float measured_value, float dt)
    {
        var error = setpoint - measured_value;
        var proportional = error;
        integral = integral + error * dt;
        var derivative = (error - previous_error) / dt;
        var output = Settings.Kp * proportional + Settings.Ki * integral + Settings.Kd * derivative;
        previous_error = error;
        return output;
    }
    public void Reset()
    {
        integral = 0;
        previous_error = 0;
    }
}
public class PidControllerRotation : IPidController<float>
{
    public PidControllerRotation(IPidControllerSettings<float> settings)
    {
        Settings = settings;
    }

    float integral;
    float previous_error;

    public IPidControllerSettings<float> Settings { get; }

    public float Update(float setpoint, float measured_value, float dt)
    {
        setpoint = Mathf.Repeat(setpoint, 360);
        measured_value = Mathf.Repeat(measured_value, 360);
        if(Mathf.Abs(measured_value - setpoint) > 180)
        {
            measured_value -= 360;
        }
        var error = setpoint - measured_value;
        var proportional = error;
        integral = integral + error * dt;
        var derivative = (error - previous_error) / dt;
        var output = Settings.Kp * proportional + Settings.Ki * integral + Settings.Kd * derivative;
        previous_error = error;
        return output;
    }
    public void Reset()
    {
        integral = 0;
        previous_error = 0;
    }
}
