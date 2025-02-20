using System;
using System.Windows.Data;
using System.Windows;

namespace Ceitcon_Designer.Converters
{
    /// <summary>
    /// Converts <see cref="System.Boolean"/> value to <see cref="System.Windows.Visibility"/> value.
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibilityConverter : ValueConverter<string, Visibility>
    {
        /// <summary>
        /// An instance of <see cref="BoolToVisibilityConverter"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This  type is immutable.")]
        // todo: maybe do better naming

        public static readonly StringToVisibilityConverter Collapsed = new StringToVisibilityConverter(true, false);
        public static readonly StringToVisibilityConverter Hidden = new StringToVisibilityConverter(false, false);
        public static readonly StringToVisibilityConverter InvertedCollapsed = new StringToVisibilityConverter(true, true);
        public static readonly StringToVisibilityConverter InvertedHidden = new StringToVisibilityConverter(false, true);

        private readonly bool isCollapsed;
        private readonly bool isInverted;

        private StringToVisibilityConverter(bool isCollapsed, bool isInverted)
        {
            this.isCollapsed = isCollapsed;
            this.isInverted = isInverted;
        }

        /// <summary>
        /// Converts <see cref="System.Boolean"/> value to <see cref="System.Windows.Visibility"/> value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public override Visibility Convert(string value)
        {
            return (isInverted ? String.IsNullOrEmpty(value) : !String.IsNullOrEmpty(value)) ? Visibility.Visible : (this.isCollapsed ? Visibility.Collapsed : Visibility.Hidden);
        }

        /// <summary>
        /// Converts <see cref="System.Windows.Visibility"/> value to <see cref="System.Boolean"/> value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public override string ConvertBack(Visibility value)
        {
            //todo: Am i using this at all? Maybe for now better to throw exception until we figure out where its used
            throw new InvalidOperationException();
        }
    }
}

