using System;
using System.Windows.Data;
using System.Globalization;
using Ceitcon_Designer.Utilities;

namespace Ceitcon_Designer.Converters
{
    /// <summary>
    /// Base generic class for converters that have a parameter.
    /// </summary>
    /// <typeparam name="TFrom">Type of object from which to convert.</typeparam>
    /// <typeparam name="TTo">Type of object to which to convert.</typeparam>
    /// <typeparam name="TParameter">Type of parameter.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "We need all three parameters.")]
    public abstract class ValueConverter<TFrom, TTo, TParameter> : IValueConverter
    {
        /// <summary>
        /// Function that describes the conversion that needs to be overriden in child classes.
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="parameter">parameter</param>
        /// <returns>Converted item of type TTo.</returns>
        public abstract TTo Convert(TFrom value, TParameter parameter);

        /// <summary>
        /// Function that describes the back conversion that needs to be overriden in child classes.
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="parameter">parameter</param>
        /// <returns>Back converted item of type TFrom.</returns>
        public virtual TFrom ConvertBack(TTo value, TParameter parameter)
        {
            throw new NotSupportedException("Converter " + this.GetType().Name + " doesn't support convert back operation.");
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = TypeUtilities.Convert<TFrom>(value, "value");
            var param = TypeUtilities.Convert<TParameter>(parameter, "parameter");

            //todo: return checks
            //if (!TypeUtilities.IsAssignableFrom<TTo>(targetType))
            //    throw new ArgumentException("Argument 'targetType' can't be assigned to output type.");

            return this.Convert(val, param);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = TypeUtilities.Convert<TTo>(value, "value");
            var param = TypeUtilities.Convert<TParameter>(parameter, "parameter");

            //todo: return checks
            //if (!TypeUtilities.IsAssignableFrom<TFrom>(targetType))
            //    throw new ArgumentException("Argument 'targetType' can't be assigned to output type.");

            return this.ConvertBack(val, param);
        }
    }

    /// <summary>
    /// Base generic class for converters that don't have a parameter.
    /// </summary>
    /// <typeparam name="TFrom">Type of object from which to convert.</typeparam>
    /// <typeparam name="TTo">Type of object to which to convert.</typeparam>
    public abstract class ValueConverter<TFrom, TTo> : IValueConverter
    {
        /// <summary>
        /// Function that describes the conversion that needs to be overriden in child classes.
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>Converted item of type TTo.</returns>
        public abstract TTo Convert(TFrom value);

        /// <summary>
        /// Function that describes the back conversion that needs to be overriden in child classes.
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>Converted item of type TTo.</returns>
        public virtual TFrom ConvertBack(TTo value)
        {
            throw new NotSupportedException("Converter " + this.GetType().Name + " doesn't support convert back operation.");
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = TypeUtilities.Convert<TFrom>(value, "value");

            //todo: return checks
            //if (!TypeUtilities.IsAssignableFrom<TTo>(targetType))
            //    throw new ArgumentException("Argument 'targetType' can't be assigned to output type.");

            return this.Convert(val);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = TypeUtilities.Convert<TTo>(value, "value");

            //todo: return checks
            //if (!TypeUtilities.IsAssignableFrom<TFrom>(targetType))
            //    throw new ArgumentException("Argument 'targetType' can't be assigned to output type.");

            return this.ConvertBack(val);
        }
    }
}
