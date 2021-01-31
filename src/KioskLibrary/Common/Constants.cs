﻿/*
 * Copyright 2021
 * City of Stanton
 * Stanton, Kentucky
 * www.stantonky.gov
 * github.com/CityOfStanton
 */

namespace KioskLibrary.Common
{
    /// <summary>
    /// Constant values that are reused throughout the application
    /// </summary>
    public class Constants
    {
        public static string ValidURIMessage = "URL is valid!";

        /// <summary>
        /// Keys used for storing objects in local application storage
        /// </summary>
        public class ApplicationStorage
        {
            public static string CurrentOrchestration = "#CurrentOrchestration";
            public static string CurrentOrchestrationSource = "#CurrentOrchestrationSource";
            public static string CurrentOrchestrationURI = "#CurrentOrchestrationURI";
            public static string NextOrchestration = "#NextOrchestration";
            public static string SettingsViewModel = "#SettingsViewModel";
            public static string PollingInterval = "#PollingInterval";
            public static string EndOrchestration = "#EndOrchestration";
        }
    }
}
