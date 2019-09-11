using System;
using System.Collections.Generic;
using InterestRateSwapPricing.Enums;
using InterestRateSwapPricing.Constants;

namespace InterestRateSwapPricing
{
    class Program
    {
        static void Main(string[] args)
        {
            var swapCurve = DataLoader.LoadSwapCurve(SwapCurveTableNames.Table3_5);

            var zeroCouponRates = GetZeroCouponRates(swapCurve);

            PrintRates(zeroCouponRates, Rates.ZeroCouponRates);

            var forwards = GetForwardRates(zeroCouponRates);

            PrintRates(forwards, Rates.ForwardRates);

            var swapModel = new SwapModel(zeroCouponRates, forwards);

            var swapRate = CalculateSwapRate(swapModel, 3);

            Console.WriteLine(swapRate);

            //var NPV = CalculateMarkToMarket(swapModel, 100000000, 7.0, 3);

        }

        private static double CalculateMarkToMarket(SwapModel swapModel, double notional, double fixedRate, int currentYear)
        {
            var yearSwap = swapModel.ForwardRates.Count - currentYear;


            throw new NotImplementedException();
        }

        private static double CalculateSwapRate(SwapModel swapModel, int year)
        {
            var swapRate = 0.0;
            var discountFactors = CalculateDiscountFactors(swapModel);
            var discountFactorsSum = SumDiscountFactors(discountFactors, year);

            for (int i=0; i<year; i++)
            {
                var weight = (discountFactors[i] / discountFactorsSum);
                swapRate += weight * swapModel.ForwardRates[i+1];
            }

            return swapRate * 100;
        }

        private static double[] CalculateDiscountFactors(SwapModel swapModel)
        {
            var discountFactors = new double[swapModel.ZeroCouponRates.Count];

            foreach (var zeroCouponRate in swapModel.ZeroCouponRates)
            {
                discountFactors[zeroCouponRate.Key - 1] = 1 / Math.Pow(1 + zeroCouponRate.Value, zeroCouponRate.Key);
            }

            return discountFactors;
        }

        private static double SumDiscountFactors(double[] discountFactors, int year)
        {
            var discountFactorsSum = 0.0;

            for (int i=0; i<year; i++)
            {
                discountFactorsSum += discountFactors[i];
            }

            return discountFactorsSum;
        }

        private static Dictionary<int, double> GetForwardRates(Dictionary<int, double> zeroCouponRates)
        {
            var forwards = new Dictionary<int, double>();

            foreach (var zeroCouponRate in zeroCouponRates)
            {
                forwards.Add(zeroCouponRate.Key, CalculateForwardRate(zeroCouponRate.Key, zeroCouponRates));
            }

            return forwards;
        }

        private static Dictionary<int, double> GetZeroCouponRates(Dictionary<int, double> swapCurve)
        {
            var zeroCouponRates = new Dictionary<int, double>();

            foreach (var swapRate in swapCurve)
            {
                zeroCouponRates.Add(swapRate.Key, CalculateZeroCouponRate(swapRate.Key, swapCurve, zeroCouponRates));
            }

            return zeroCouponRates;
        }

        private static double CalculateZeroCouponRate(int time, Dictionary<int, double> swapCurve, Dictionary<int, double> zeroCouponRates)
        {
            if (time == 1)
            {
                return swapCurve[time] / 100;
            }

            var cashFlows = new double[time];
            var totalDiscountedCashflows = 0.0;

            for (int i=0; i<cashFlows.Length; i++)
            {
                if (i == cashFlows.Length - 1)
                {
                    cashFlows[i] = 100 + swapCurve[time];
                }
                else
                {
                    cashFlows[i] = swapCurve[time] / Math.Pow(1 + (zeroCouponRates[i+1]), i + 1);
                    totalDiscountedCashflows += cashFlows[i];
                }
            }

            var zeroCouponRate = Math.Pow((cashFlows[cashFlows.Length - 1])/(100 - totalDiscountedCashflows), 1.0 / time) - 1;
            return zeroCouponRate;
        }

        private static double CalculateForwardRate(int time, Dictionary<int, double> zeroCouponRates)
        {
            if (time == 1)
            {
                return zeroCouponRates[time];
            }

            return ((Math.Pow(1 + zeroCouponRates[time], time)) / (Math.Pow(1 + zeroCouponRates[time-1], time-1)) - 1.0);
        }

        private static void PrintRates(Dictionary<int, double> rates, Rates rateName)
        {
            switch (rateName)
            {
                case Rates.ForwardRates:
                    Console.WriteLine("Forward Rates");
                    break;

                case Rates.ZeroCouponRates:
                    Console.WriteLine("Zero Coupon Rates");
                    break;
            }

            foreach (var rate in rates)
            {
                Console.WriteLine(rate.Value * 100);
            }

            Console.WriteLine();
        }
    }
}