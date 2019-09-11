using System.Collections.Generic;

namespace InterestRateSwapPricing
{
    public class SwapModel
    {
        public double Notional { get; set; }
        public int NPV { get; set; }
        public float FixedRate { get; set; }
        public double TotalFixedPV { get; set; }
        public double TotalFloatPV { get; set; }
        public Dictionary<int, double> ZeroCouponRates { get; set; }
        public Dictionary<int, double> ForwardRates { get; set; }
        public Dictionary<int, double> FixedCashflows { get; set; }
        public Dictionary<int, double> FloatingCashflows { get; set; }
        public Dictionary<int, double> FixedCashflowsPV { get; set; }
        public Dictionary<int, double> FloatingCashflowsPV { get; set; }

        public SwapModel(Dictionary<int, double> ZeroCouponRates, Dictionary<int, double> ForwardRates)
        {
            this.ZeroCouponRates = ZeroCouponRates;
            this.ForwardRates = ForwardRates;
        }

        public SwapModel(Dictionary<int, double> ZeroCouponRates, Dictionary<int, double> ForwardRates, double Notional, float FixedRate)
        {
            this.ZeroCouponRates = ZeroCouponRates;
            this.ForwardRates = ForwardRates;
            this.Notional = Notional;
            this.FixedRate = FixedRate;

            CalculateCashflows(true);
            CalculateCashflows(false);

            DiscountCashflows(true);
            DiscountCashflows(false);
        }

        public void CalculateCashflows(bool isFixed)
        {
            foreach (var maturity in ZeroCouponRates)
            {
                if (isFixed)
                {
                    FixedCashflows.Add(maturity.Key, FixedRate * Notional);
                }
                else
                {
                    FloatingCashflows.Add(maturity.Key, ForwardRates[maturity.Key] * Notional);
                }
            }
        }
        public void DiscountCashflows(bool isFixed)
        {
            if (isFixed)
            {
                foreach (var fixedCashflow in FixedCashflows)
                {
                    FixedCashflowsPV.Add(fixedCashflow.Key, fixedCashflow.Value * ZeroCouponRates[fixedCashflow.Key]);
                }
            }
            else
            {
                foreach (var floatingCashflow in FloatingCashflows)
                {
                    FloatingCashflowsPV.Add(floatingCashflow.Key, floatingCashflow.Value * ZeroCouponRates[floatingCashflow.Key]);
                }
            }
        }
        public void TotalDiscountedCashflows(bool isFixed)
        {
            if (isFixed)
            {
                foreach (var cashflow in FixedCashflowsPV)
                {
                    TotalFixedPV += cashflow.Value;
                }
            }
            else
            {
                foreach (var cashflow in FloatingCashflowsPV)
                {
                    TotalFloatPV += cashflow.Value;
                }
            }
        }
    }
}