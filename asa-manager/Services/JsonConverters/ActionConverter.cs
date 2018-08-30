﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoTSolutions.AsaManager.Services.JsonConverters
{
    class ActionConverter : JsonConverter
    {
        // Maps Types of actions to their Service layer class.
        private static IDictionary<Services.Models.Type, Func<IActionApiModel>> actionMapping = new Dictionary<Services.Models.Type, Func<IActionApiModel>>()
            {
                { Models.Type.Email, () => { return new EmailActionApiModel(); } }
            };

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(IActionApiModel);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonArray = JArray.Load(reader);
            var actionItem = default(IActionApiModel);
            IList<IActionApiModel> actionItemList = new List<IActionApiModel>();
            foreach (var jsonObject in jsonArray)
            {
                Enum.TryParse(jsonObject["Type"].Value<string>(), true, out Models.Type action);
                actionItem = actionMapping[action]();
                serializer.Populate(jsonObject.CreateReader(), actionItem);
                actionItemList.Add(actionItem);
            }
            return actionItemList;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization");
        }
    }
}