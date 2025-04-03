using UnityEngine;

namespace LOBS
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // GlobalStaticValue
    // 전역변수 한곳에 몰아서 정의
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class GlobalStaticValue
    {
        public const int NoMeaningNumber = 970910;

        public static class SortedLayer
        {
            public static readonly int MapLayer = 2;
        }

        #region [static] Protocol 
        //Device
        public static readonly string RESET = "r";
        public static readonly string NAME_CHANGE = "n";
        // Main Sensor
        public static readonly string SENSOR_SCAN_SPEED = "v";
        public static readonly string SENSOR_SCAN_MOVE_DISTANCE = "p";
        public static readonly string SENSOR_SCAN_MOVE_CYCLE = "dd";
        public static readonly string SENSOR_SCAN_START = "S";
        public static readonly string SENSOR_SCAN_DATA_RESPONSE_PULSE = "Xxx";
        public static readonly string SENSOR_SCAN_DATA_RESPONSE_SENSOR = "xxx";
        public static readonly string SENSOR_SCAN_RESPONSE_END_RETURN = "e";
        public static readonly string SENSOR_SCAN_RESPONSE_COMPLETE = "h";
        public static readonly string SENSOR_SCAN_RESPONSE_ERROR = "f";
        // Move Motor
        public static readonly string MOTOR_SET_DISTANCE = "mp";
        public static readonly string MOTOR_SET_DIRECTION = "md";
        public static readonly string MOTOR_MOVE_START = "ms";
        public static readonly string MOTOR_MOVE_CANCLE = "mc";
        public static readonly string MOTOR_MOVE_PAUSE = "mt";
        public static readonly string MOTOR_MOVE_RESTART = "mr";
        public static readonly string MOTOR_NOW_MOVE_DISTANCE = "mm";
        public static readonly string MOTOR_MOVE_COMPLETE = "mh";
        #endregion

        #region [static] SceneName
        public static readonly string RL_CLIENT = "RL_Client";
        public static readonly string RL_LOGIN = "RL_Login";
        public static readonly string RL_CONTROLPANEL = "RL_ControlPanel";
        #endregion
    }

}
