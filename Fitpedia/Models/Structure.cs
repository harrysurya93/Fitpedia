using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fitpedia.Models
{
    public class Structure
    {
        public class LoginInformation
        {
            public string username { get; set; }
        }

        public class Message
        {
            public bool MessageStatus { get; set; }
            public string MessageText { get; set; }
        }

        public class IoT_Message
        {
            public string DeviceId { get; set; }
            public double DeviceValue { get; set; }
        }
        public class BMI_Graphic
        {
            public string datetime { get; set; }
            public float BMI { get; set; }
        }
        public class BMI_Data
        {
            public DateTime datetime { get; set; }
            public float BMI { get; set; }
        }

        public class BMI_Forecast
        {
            public float[] Forecast { get; set; }
        }

        public class MachineLearningOutput
        {
            public BMI_Forecast Result { get; set; }
            public ForecastEvaluation Evaluation { get; set; }
        }

        public class ForecastEvaluation
        {
            public float MAE { get; set; }
            public double RMSE { get; set; }

        }
    }
}