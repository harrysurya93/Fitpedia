using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Models
{
    public class Function
    {
        public static Structure.Message Authorization(string Username, string Password)
        {
            if (Connection.ExecuteSelectQuery(String.Format("SELECT * FROM users WHERE username='{0}' AND password='{1}'", Username, Password)).Count > 0)
            {
                Structure.LoginInformation oLoginInformation = new Structure.LoginInformation();
                oLoginInformation.username = Username;
                HttpContext.Current.Session["LoginInformation"] = oLoginInformation;
                return new Structure.Message { MessageText = "Authorization Passed", MessageStatus = true };
            }
            else
            {
                return new Structure.Message { MessageText = "Wrong username / password, please retry again", MessageStatus = false };
            }
        }

        public static Structure.Message Authentication(ControllerBase controller)
        {
            if (HttpContext.Current.Session["LoginInformation"] == null) { return new Structure.Message { MessageStatus = false, MessageText = "Illegal access, Please login to continue" }; }
            controller.ViewBag.LoginInformation = (Structure.LoginInformation)HttpContext.Current.Session["LoginInformation"];

            return new Structure.Message { MessageStatus = true, MessageText = "Authentication Passed" };
        }

        public static List<double> RemoveOutliers(List<double> allNumbers)
        {
            List<double> normalNumbers = new List<double>();
            List<double> outLierNumbers = new List<double>();
            double avg = allNumbers.Average();
            double standardDeviation = Math.Sqrt(allNumbers.Average(v => Math.Pow(v - avg, 2)));
            foreach (double number in allNumbers)
            {
                if ((Math.Abs(number - avg)) > (2 * standardDeviation))
                    outLierNumbers.Add(number);
                else
                    normalNumbers.Add(number);
            }

            return normalNumbers;
        }

        public static double[] EMA(double[] x, int N)
        {
            // x is the input series            
            // N is the notional age of the data used
            // k is the smoothing constant

            double k = 2.0 / (N + 1);
            double[] y = new double[x.Length];
            y[0] = x[0];
            for (int i = 1; i < x.Length; i++) y[i] = k * x[i] + (1 - k) * y[i - 1];

            return y;
        }

        public class MachineLearning
        {
            public static Structure.MachineLearningOutput TimeSeriesForecast(string username)
            {
                List<dynamic> SourceData = Connection.ExecuteSelectQuery("SELECT datetime, ROUND(weight / POWER(height / 100, 2), 3) AS BMI FROM fitpedia.activity INNER JOIN fitpedia.users ON fitpedia.users.username = fitpedia.activity.username WHERE fitpedia.activity.username = '" + username + "'");
                List<Models.Structure.BMI_Data> oInput = new List<Structure.BMI_Data>();
                foreach(dynamic value in SourceData)
                {
                    oInput.Add(new Structure.BMI_Data { datetime = value.datetime, BMI = float.Parse(value.BMI.ToString()) });
                }
                var context = new MLContext();
                var data = context.Data.LoadFromEnumerable(oInput);
                var pipeline = context.Forecasting.ForecastBySsa(
                        "Forecast",
                        nameof(Fitpedia.Models.Structure.BMI_Data.BMI),
                        windowSize: 5,
                        seriesLength: 10,
                        trainSize: 20,
                        horizon: 7

                    );
                var model = pipeline.Fit(data);
                var ForecastEngine = model.CreateTimeSeriesEngine<Structure.BMI_Data, Structure.BMI_Forecast>(context);
                Structure.BMI_Forecast oResult = ForecastEngine.Predict();
                Structure.ForecastEvaluation oEvaluation = Evaluate(data, model, context);
                
                return new Structure.MachineLearningOutput { Evaluation = oEvaluation, Result = oResult };
            }

            static Structure.ForecastEvaluation Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
            {
                // Make predictions
                IDataView predictions = model.Transform(testData);

                // Actual values
                IEnumerable<float> actual =
                    mlContext.Data.CreateEnumerable<Models.Structure.BMI_Data>(testData, true)
                        .Select(observed => observed.BMI);

                // Predicted values
                IEnumerable<float> forecast =
                    mlContext.Data.CreateEnumerable<Models.Structure.BMI_Forecast>(predictions, true)
                        .Select(prediction => prediction.Forecast[0]);

                // Calculate error (actual - forecast)
                var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

                // Get metric averages
                var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
                var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error

                return new Structure.ForecastEvaluation { MAE = MAE, RMSE = RMSE };
            }
        }
        
    }
}