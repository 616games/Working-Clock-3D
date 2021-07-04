using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    #region --Fields / Properties--
    
    /// <summary>
    /// Prefab for the hours indicator.
    /// </summary>
    [SerializeField]
    private GameObject _hoursIndicator;

    /// <summary>
    /// Transform of the game object to place the instantiated hours indicators.
    /// </summary>
    [SerializeField]
    private Transform _hoursParent;

    /// <summary>
    /// Prefab of the hours arm.
    /// </summary>
    [SerializeField]
    private GameObject _hoursArm;

    /// <summary>
    /// Prefab for the minutes indicator.
    /// </summary>
    [SerializeField]
    private GameObject _minutesIndicator;
    
    /// <summary>
    /// Transform of the game object to place the instantiated minutes indicators.
    /// </summary>
    [SerializeField]
    private Transform _minutesParent;

    /// <summary>
    /// Prefab of the minutes arm.
    /// </summary>
    [SerializeField]
    private GameObject _minutesArm;

    /// <summary>
    /// Prefab of the seconds arm.
    /// </summary>
    [SerializeField]
    private GameObject _secondsArm;

    /// <summary>
    /// Cached Transform component.
    /// </summary>
    private Transform _transform;
    
    /// <summary>
    /// Reference to the seconds arm once instantiated.
    /// </summary>
    private GameObject _secondsArmInstantiated;
    
    /// <summary>
    /// Reference to the hours arm once instantiated.
    /// </summary>
    private GameObject _hoursArmInstantiated;
    
    /// <summary>
    /// Reference to the minutes arm once instantiated.
    /// </summary>
    private GameObject _minutesArmInstantiated;

    /// <summary>
    /// Hours indicators must be placed every 30 degrees of the clock face (circle) starting at 0 degrees.
    /// 360 degrees in a circle / 12 hours = 30 degrees per hour.
    /// </summary>
    private const float _hoursToDegrees = 30f;

    /// <summary>
    /// Minutes indicators must be placed every 6 degrees of the clock face (circle) starting at 0 degrees.
    /// 360 degrees in a circle / 60 minutes = 6 degrees per minute.
    /// </summary>
    private const float _minutesToDegrees = 6f;

    /// <summary>
    /// Used by the _secondsArm to properly rotate the entire 360 degrees of the clock face.
    /// 360 degrees in a circle / 60 seconds = 6 degrees per second.
    /// </summary>
    private const float _secondsToDegrees = 6f;
    
    #endregion
    
    #region --Unity Specifc Methods--

    private void Start()
    {
        Init();
    }
    
    private void Update()
    {
        Rotate();
    }
    
    #endregion
    
    #region --Custom Methods--

    /// <summary>
    /// Initializes variables and caches components.
    /// </summary>
    private void Init()
    {
        _transform = transform;
        SpawnClockComponents();
    }
    
    /// <summary>
    /// Consolidates all of the spawn functions into one method call.
    /// </summary>
    private void SpawnClockComponents()
    {
        SpawnHourIndicators();
        SpawnMinutesIndicators();
        SpawnSecondsArm();
        SpawnHoursArm();
        SpawnMinutesArm();
    }

    /// <summary>
    /// Spawns the hours indicators around the clock face.
    /// </summary>
    private void SpawnHourIndicators()
    {
        float _length = 4;
        float _angle = 0;
        for (int i = 0; i < 12; i++)
        {
            float _radians = _angle * Mathf.Deg2Rad;
            Vector3 _position = ConvertPolarToCartesian(_length, _radians);
            Vector3 _directionFromCenter = _transform.position - _position;
            Quaternion _rotation = Quaternion.LookRotation(_directionFromCenter, Vector3.forward);
            _rotation.x = 0;
            _rotation.y = 0;
            Instantiate(_hoursIndicator, _position, _rotation, _hoursParent);
            _angle += 30;
        }
    }

    /// <summary>
    /// Spawns the minute indicators around the clock face, except for where the hours indicators already exist.
    /// </summary>
    private void SpawnMinutesIndicators()
    {
        float _length = 4.5f;
        int _angle = 0;
        for (int i = 0; i < 60; i++)
        {
            //Exclude placing a minutes indicator where an hours indicator already exists.
            if (_angle % 10 == 0)
            {
                _angle += 6;
                continue;
            }
            
            float _radians = _angle * Mathf.Deg2Rad;
            Vector3 _position = ConvertPolarToCartesian(_length, _radians);
            Instantiate(_minutesIndicator, _position, Quaternion.identity, _minutesParent);
            _angle += 6;
        }
    }
    
    /// <summary>
    /// Spawns the hours arm in the appropriate rotation based on the current time.
    /// </summary>
    private void SpawnHoursArm()
    {
        Vector3 _position = _transform.position;
        
        //Position the hours arm .1f off the Z axis so it isn't inside the clock face.
        _position.z = .3f;
        _hoursArmInstantiated = Instantiate(_hoursArm, _position, Quaternion.identity);
        TimeSpan _time = DateTime.Now.TimeOfDay;
        _hoursArmInstantiated.transform.localRotation = Quaternion.Euler(0, 0, _hoursToDegrees * (float)_time.TotalHours);
    }
    
    /// <summary>
    /// Spawns the minutes arm in the appropriate rotation based on the current time.
    /// </summary>
    private void SpawnMinutesArm()
    {
        Vector3 _position = _transform.position;
        
        //Position the minutes arm .2 off the Z axis so it isn't inside the hours arm.
        _position.z = .2f;
        _minutesArmInstantiated = Instantiate(_minutesArm, _position, Quaternion.identity);
        TimeSpan _time = DateTime.Now.TimeOfDay;
        _minutesArmInstantiated.transform.localRotation = Quaternion.Euler(0, 0, _minutesToDegrees * (float)_time.TotalMinutes);
    }

    /// <summary>
    /// Spawns the seconds arm in the appropriate rotation based on the current time.
    /// </summary>
    private void SpawnSecondsArm()
    {
        Vector3 _position = _transform.position;
        
        //Position the seconds arm .1 off the Z axis so it isn't inside the minutes arm.
        _position.z = .1f;
        _secondsArmInstantiated = Instantiate(_secondsArm, _position, Quaternion.identity);
        TimeSpan _time = DateTime.Now.TimeOfDay;
        _secondsArmInstantiated.transform.localRotation = Quaternion.Euler(0, 0, _secondsToDegrees * (float)_time.TotalSeconds);
    }

    /// <summary>
    /// Rotates each of the clock's arms to the appropriate angle of the clock face based on the current time.
    /// </summary>
    private void Rotate()
    {
        TimeSpan _time = DateTime.Now.TimeOfDay;
        _secondsArmInstantiated.transform.localRotation = Quaternion.Euler(0, 0, _secondsToDegrees * (float)_time.TotalSeconds);
        _hoursArmInstantiated.transform.localRotation = Quaternion.Euler(0, 0, _hoursToDegrees * (float)_time.TotalHours);
        _minutesArmInstantiated.transform.localRotation = Quaternion.Euler(0, 0, _minutesToDegrees * (float)_time.TotalMinutes);
    }
    
    /// <summary>
    /// Since we are working with a circle (clock face) it is easier to work with polar coordinates first and then convert them to Cartesian.
    /// </summary>
    private Vector3 ConvertPolarToCartesian(float _r, float _angle)
    {
        Vector3 _position = new Vector3(_r * Mathf.Cos(_angle), _r * Mathf.Sin(_angle), 0);

        return _position;
    }
    
    #endregion
    
}
