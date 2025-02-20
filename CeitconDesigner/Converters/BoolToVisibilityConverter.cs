using System;
using System.Windows.Data;
using System.Windows;

namespace Ceitcon_Designer.Converters
{
    /// <summary>
    /// Converts <see cref="System.Boolean"/> value to <see cref="System.Windows.Visibility"/> value.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : ValueConverter<bool, Visibility>
    {
        /// <summary>
        /// An instance of <see cref="BoolToVisibilityConverter"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This  type is immutable.")]
        // todo: maybe do better naming

        public static readonly BoolToVisibilityConverter Collapsed = new BoolToVisibilityConverter(true, false);
        public static readonly BoolToVisibilityConverter Hidden = new BoolToVisibilityConverter(false, false);
        public static readonly BoolToVisibilityConverter InvertedCollapsed = new BoolToVisibilityConverter(true, true);
        public static readonly BoolToVisibilityConverter InvertedHidden = new BoolToVisibilityConverter(false, true);

        private readonly bool isCollapsed;
        private readonly bool isInverted;

        private BoolToVisibilityConverter(bool isCollapsed, bool isInverted)
        {
            this.isCollapsed = isCollapsed;
            this.isInverted = isInverted;
        }

        /// <summary>
        /// Converts <see cref="System.Boolean"/> value to <see cref="System.Windows.Visibility"/> value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public override Visibility Convert(bool value)
        {
            return (isInverted ? !value : value) ? Visibility.Visible : (this.isCollapsed ? Visibility.Collapsed : Visibility.Hidden);
        }

        /// <summary>
        /// Converts <see cref="System.Windows.Visibility"/> value to <see cref="System.Boolean"/> value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public override bool ConvertBack(Visibility value)
        {
            //todo: Am i using this at all? Maybe for now better to throw exception until we figure out where its used
            throw new InvalidOperationException();

            //return value == Visibility.Visible;
        }
    }
}
