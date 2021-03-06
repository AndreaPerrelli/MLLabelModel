using System;
using Microsoft.ML;
using Microsoft.ML.Data;

class Program
{
    public class HouseData
    {
        public float Size { get; set; }
        public float Price { get; set; }
    }

    public class Prediction
    {
        [ColumnName("Score")]
        public float Price { get; set; }
    }

    static void Main(string[] args)
    {
        MLContext mlContext = new MLContext();

        // 1. Load Data
        HouseData[] houseData = {
           new HouseData() { Size = 1.1F, Price = 1.2F },
           new HouseData() { Size = 1.9F, Price = 2.3F },
           new HouseData() { Size = 2.8F, Price = 3.0F },
           new HouseData() { Size = 3.4F, Price = 3.7F } };
        IDataView trainingData = mlContext.Data.LoadFromEnumerable(houseData);

        // 2. Create pipeline
        var pipeline = mlContext.Transforms.Concatenate("Features", new[] { "Size" })
            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Price", maximumNumberOfIterations: 100));

        // 3. Train model
        var model = pipeline.Fit(trainingData);

        while(true)
        {
            // 4. Make a prediction and evaluate
            var size = new HouseData() { Size = readInput() };
            var price = mlContext.Model.CreatePredictionEngine<HouseData, Prediction>(model).Predict(size);

            Console.WriteLine($"Predicted price for size: {size.Size * 1000} sq ft= {price.Price * 100:C}k");
        }

    }

    static private float readInput()
    {
        float risultato;
        const string description = "Enter size of house for prediction";
        const string error = "You didn't enter a number, retry";
        Console.WriteLine(description);
        while(!(float.TryParse(Console.ReadLine(), out risultato)))
        {
            Console.WriteLine(error);
        }
        return risultato;
    }
}
