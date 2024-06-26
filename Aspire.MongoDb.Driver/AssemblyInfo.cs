// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire;
using Aspire.MongoDb.Driver;
using Aspire.MongoDB.Driver;

[assembly: ConfigurationSchema("Aspire:MongoDB:Driver", typeof(MongoDbSettings))]

[assembly: LoggingCategories(
    "MongoDB",
    "MongoDB.Command",
    "MongoDB.SDAM",
    "MongoDB.ServerSelection",
    "MongoDB.Connection",
    "MongoDB.Internal")]
