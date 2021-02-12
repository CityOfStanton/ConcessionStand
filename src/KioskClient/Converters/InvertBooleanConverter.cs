﻿/*
 * Copyright 2021
 * City of Stanton
 * Stanton, Kentucky
 * www.stantonky.gov
 * github.com/CityOfStanton
 */

using System;
using Windows.UI.Xaml.Data;

namespace KioskLibrary.Converters
{
    /// <summary>
    /// Inverts the boolean value for data bound elements
    /// </summary>
    public class InvertBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Invert the value
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language) => !(bool)value;

        /// <summary>
        /// Restore a converted value to its original value
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language) => !(bool)value;
    }
}
