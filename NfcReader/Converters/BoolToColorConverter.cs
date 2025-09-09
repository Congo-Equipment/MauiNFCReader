using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace NfcReader.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // If value is null, return Gray (neutral)
            if (value == null)
                return Colors.Gray;

            if (value is bool isSuccessful)
                return isSuccessful ? Colors.Green : Colors.Red;

            return Colors.Gray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}