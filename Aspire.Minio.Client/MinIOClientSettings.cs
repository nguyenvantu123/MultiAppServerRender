// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


/// <summary>
/// Provides the client configuration settings for connecting to a RabbitMQ message broker.
/// </summary>
public sealed class MinIOClientSettings
{
    /// <summary>
    /// Gets or sets the connection string of the RabbitMQ server to connect to.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// <para>Gets or sets the maximum number of connection retry attempts.</para>
    /// <para>Default value is 5, set it to 0 to disable the retry mechanism.</para>
    /// </summary>
    public string? AccessKey { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the RabbitMQ health check is enabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="true"/>.
    /// </value>
    public string SecretKey { get; set; }

    /// </value>
    public bool HealthChecks { get; set; } = true;

}
