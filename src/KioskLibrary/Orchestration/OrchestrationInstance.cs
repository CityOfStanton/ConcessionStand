﻿/*
 * Copyright 2021
 * City of Stanton
 * Stanton, Kentucky
 * www.stantonky.gov
 * github.com/CityOfStanton
 */

using KioskLibrary.Common;
using KioskLibrary.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Web.Http;
using Action = KioskLibrary.Actions.Action;

namespace KioskLibrary.Orchestration
{
    /// <summary>
    /// An instance of an orchestration
    /// </summary>
    public class OrchestrationInstance
    {
        /// <summary>
        /// The interval used to check for updated versions of this <see cref="OrchestrationInstance" />
        /// </summary>
        public int PollingIntervalMinutes { get; set; }

        /// <summary>
        /// The lifecycle behavior of an <see cref="OrchestrationInstance" />
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LifecycleBehavior Lifecycle { get; set; } = LifecycleBehavior.SingleRun;

        /// <summary>
        /// The order to iterate through the set of <see cref="OrchestrationInstance.Actions" />
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Ordering Order { get; set; } = Ordering.Sequential;

        /// <summary>
        /// The source of this <see cref="OrchestrationInstance" />
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public OrchestrationSource OrchestrationSource { get; set; }

        /// <summary>
        /// A list of <see cref="Action" />s to process
        /// </summary>
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
        public List<Action> Actions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public OrchestrationInstance() { Actions = new List<Action>(); }

        /// <summary>
        /// The HTTP helper
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public IHttpHelper HttpHelper { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actions">A list of <see cref="Action" />s to process</param>
        /// <param name="pollingInterval">The interval used to check for updated versions of this <see cref="OrchestrationInstance" /></param>
        /// <param name="orchestrationSource">The source of this <see cref="OrchestrationInstance" /></param>
        /// <param name="lifecycle">The lifecycle behavior of an <see cref="OrchestrationInstance" /></param>
        /// <param name="order">The order to iterate through the set of <see cref="Actions" /></param>
        public OrchestrationInstance(List<Action> actions, int pollingInterval, OrchestrationSource orchestrationSource, LifecycleBehavior lifecycle = LifecycleBehavior.SingleRun, Ordering order = Ordering.Sequential, IHttpHelper httpHelper = null)
        {
            PollingIntervalMinutes = pollingInterval;
            Actions = actions;
            Lifecycle = lifecycle;
            Order = order;
            OrchestrationSource = orchestrationSource;
            HttpHelper = httpHelper ?? new HttpHelper();
        }

        /// <summary>
        /// Gets the <see cref="OrchestrationInstance" /> from the specified <paramref name="uri" />
        /// </summary>
        /// <param name="uri">The URI where the <see cref="OrchestrationInstance" /> is stored</param>
        /// <param name="httpHelper">The <see cref="IHttpHelper"/> to use for HTTP requests</param>
        /// <returns>An <see cref="OrchestrationInstance" /> if the it could be retrieved, else <see cref="null"/></returns>
        public async static Task<OrchestrationInstance> GetOrchestrationInstance(Uri uri, IHttpHelper httpHelper)
        {
            try
            {
                var result = await httpHelper.GetAsync(uri);
                if (result.StatusCode == HttpStatusCode.Ok)
                    return ConvertStringToOrchestrationInstance(await result.Content.ReadAsStringAsync());
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Converts a string to an instance of <see cref="OrchestrationInstance" />
        /// </summary>
        /// <param name="orchestrationInstanceAsString">The <see cref="OrchestrationInstance" /> as a <see cref="string" /></param>
        /// <returns>An <see cref="OrchestrationInstance" /> if the it could be parsed, else <see cref="null"/></returns>
        public static OrchestrationInstance ConvertStringToOrchestrationInstance(string orchestrationInstanceAsString)
        {
            try
            {
                // Try to parse the text as JSON
                return SerializationHelper.JSONDeserialize<OrchestrationInstance>(orchestrationInstanceAsString);
            }
            catch (JsonException)
            {
                // Try to parse the text as XML
                using var sr = new StringReader(orchestrationInstanceAsString);
                try
                {
                    return SerializationHelper.XMLDeserialize<OrchestrationInstance>(sr);
                }
                catch { }
                finally
                {
                    sr.Close();
                }
            }
            catch (ArgumentNullException) { return null; }

            return null;
        }

        /// <summary>
        /// Validates this <see cref="OrchestrationInstance" />
        /// </summary>
        /// <returns>A boolean indicating whether or not this <see cref="OrchestrationInstance" /> is valid as well as a list of errors (if validation fails)</returns>
        public async Task<(bool IsValid, List<string> Errors)> ValidateAsync()
        {
            var errors = new List<string>();

            if (PollingIntervalMinutes < 15)
                errors.Add(Constants.ValidationMessages.InvalidPollingMessage);

            if (Actions != null)
                foreach (var a in Actions)
                {
                    (bool status, string name, List<string> actionErrors) = await a.ValidateAsync(HttpHelper);
                    if (!status)
                        foreach (var actionError in actionErrors)
                            errors.Add($"{name}: {actionError}");
                }

            return (!errors.Any(), errors);
        }
    }
}
