using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Common.Base
{
    /// <summary>
    /// Helper class for mathematical calculations like percentage, average,...
    /// </summary>

    public static class Calculations
    {
        /// <summary>
        /// Calculates the percentage rounded up to an integer.
        /// <remarks>
        /// Formula: (divided / divisor) * 100
        /// The result is rounded up from .5 (MidpointRounding.AwayFromZero).
        /// 
        /// If divisor is 0 - 0 is returned.
        /// </remarks>
        /// <example>
        /// divided ... 50
        /// divisor ... 150
        /// result: 33%
        /// </example>
        /// </summary>
        /// <param name="dividend">Fraction of divisor (e.g. points achieved by the student)</param>
        /// <param name="divisor">Divisor (e.g. possible points)</param>
        /// <returns>Return percentage rounded to an integer.</returns>

        public static int GetPercentage(int dividend, int divisor)
        {
            return (int)Calculations.GetPercentage((double)dividend, (double)divisor, 0);
        }

        /// <summary>
        /// Calculate the percentage rounded to <paramref name="precision"/> fractional decimals.
        /// <remarks>
        /// If divisor is 0 - 0 is returned.
        /// </remarks>
        /// </summary>
        /// <param name="dividend">Fraction of divisor (e.g. points achieved by the student)</param>
        /// <param name="divisor">Total points</param>
        /// <param name="precision">Number of fraction decimals to round the result to.</param>
        /// <returns>Return the percentage rounded to <paramref name="precision"/> fractional decimals.</returns>

        public static double GetPercentage(double dividend, double divisor, int precision)
        {
            if ((dividend == 0) || (divisor == 0)) { return 0; }

            double percent = dividend / divisor;
            return FractionalPercentageToWholePercentage(percent, precision);
        }

        /// <summary>
        /// Converts a fractional percentage (e.g. 0.15) to a whole percentage (15) as a double,
        /// rounded to <paramref name="precision" />.
        /// </summary>
        /// <param name="percent">The percent.</param>
        /// <param name="precision">The precision.</param>

        public static double FractionalPercentageToWholePercentage(double percent, int precision = 0)
        {
            percent = percent * 100;
            return Math.Round(percent, precision, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Converts a fractional percentage (e.g. 0.15) to a whole percentage (15) as an int,
        /// rounded to <paramref name="precision" />.
        /// </summary>
        /// <param name="percent">The percent.</param>
        /// <param name="precision">The precision.</param>

        public static int FractionalPercentageToIntegerWholePercentage(double percent, int precision = 0)
        {
            return (int)FractionalPercentageToWholePercentage(percent, precision);
        }

        /// <summary>
        /// Calculate average of a data set - rounded to <paramref name="precision"/> fractional decimals.
        /// </summary>
        /// <param name="dataset">List of double numbers to calculate the average from.</param>
        /// <param name="precision">Number of fraction decimals to round the result to.</param>
        /// <returns>
        /// Return the average rounded to <paramref name="precision"/> fractional decimals. 
        /// Return 0 if no <paramref name="dataset"/> is set (either <c>null</c> or empty)
        /// </returns>

        public static double GetAverage(IList<double> dataset, int precision)
        {
            if ((dataset != null) && (dataset.Count > 0))
            {
                return Math.Round(dataset.Sum(d => d) / dataset.Count(), 4, MidpointRounding.AwayFromZero);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Round a number to <paramref name="digits"/>.
        /// </summary>
        /// <param name="value">Value to round.</param>
        /// <param name="digits">The number of fractional digits in the return value.</param>
        /// <returns>Rounded value.</returns>
        
        public static double Round(double value, int digits)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Round a number to an int.
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <returns>Rounded integer.</returns>

        public static int Round(double value)
        {
            return (int)Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }
    }
}