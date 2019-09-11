using System.ComponentModel.DataAnnotations.Schema;

namespace InterestRateSwapPricing
{
    [Table(name: "SwapCurve")]
    public class SwapCurve
    {
        private string _Maturity;
        [Column(name: "Maturity")]
        public string Maturity
        {
            get
            {
                return this._Maturity;
            }
            set
            {
                this._Maturity = value;
            }
        }

        private string _SwapRate;
        [Column(name: "SwapRate")]
        public string SwapRate
        {
            get
            {
                return this._SwapRate;
            }
            set
            {
                this._SwapRate = value;
            }
        }
    }
}
