using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ceitcon_Designer.Utilities
{
    public static class TypeUtilities
    {
        /// <summary>
        /// Casts value of type <see cref="System.Object"/> to type T.
        /// </summary>
        /// <typeparam name="T">Destination type.</typeparam>
        /// <param name="value">Value to cast.</param>
        /// <returns>Returns cast value.</returns>
        /// <exception cref="ArgumentNullException">If value can't be cast to destination type.</exception>
        public static T Convert<T>(object value)
        {
            return Convert<T>(value, null);
        }

        /// <summary>
        /// Casts value of type <see cref="System.Object"/> to type T.
        /// </summary>
        /// <typeparam name="T">Destination type.</typeparam>
        /// <param name="value">Value to cast.</param>
        /// <param name="parameterName">Name of the value argument (it is used to make better exception message if such occurs).</param>
        /// <returns>Returns cast value.</returns>
        /// <exception cref="ArgumentNullException">If value can't be cast to destination type.</exception>
        public static T Convert<T>(object value, string parameterName)
        {
            if (value is T || (default(T) == null && value == null))
                return (T)value;
            else
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Can't cast parameter of type {0} to type {1}. Parameter name is {2} and value is {3}.",
                    value == null ? "Unknown" : value.GetType().ToString(),
                    typeof(T).ToString(),
                    parameterName ?? "Unknown",
                    value == null ? "Null" : value.ToString()));
            }
        }

        /// <summary>
        /// Checks if a type is nullable.
        /// </summary>
        /// <param name="type">Type which will be checked.</param>
        /// <returns>True if it is nullable, false otherwise.</returns>
        public static bool IsNullable(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Returns underlying type of specified type.
        /// </summary>
        /// <param name="type">Type from which an underlying type will be returned.</param>
        /// <returns>Underlying type.</returns>
        /// <exception cref="ArgumentNullException">If type is null.</exception>
        public static Type GetUnderlyingType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return IsNullable(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Checks if target type is assignable from source type.
        /// </summary>
        /// <typeparam name="TTargetType">Target type.</typeparam>
        /// <param name="sourceType">Source type.</param>
        /// <returns><b>True</b> if target type is assignable from source type, <b>False</b> otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "We don't need additional method without generic type.")]
        public static bool IsAssignableFrom<TTargetType>(Type sourceType)
        {
            // This only deals with implicit conversion of nullable<T> to T, not other types of implicit conversions
            return typeof(TTargetType).IsAssignableFrom(GetUnderlyingType(sourceType));
        }

        public static T[] CreateSingleArrayFromElement<T>(T item)
        {
            return new T[] { item };
        }

        public static IEnumerable<T> GetValuesFromEnum<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        //todo: add caching
        // todo: recheck all suppressmessages
        //todo: maybe there is a better way of doing it instead of having two overloads
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This is ok.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is ok.")]
        public static string GetFieldNameFromExpression<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            // todo: replace this with nameof
            if (expression == null)
                throw new ArgumentNullException("expression");

            return ((MemberExpression)expression.Body).Member.Name;
        }

        public static string GetFieldNameFromExpression<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return ((MemberExpression)expression.Body).Member.Name;
        }
    }
}
