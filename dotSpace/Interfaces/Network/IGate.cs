﻿using System;

namespace dotSpace.Interfaces.Network
{
    /// <summary>
    /// Defines a mechanism for controlling the flow of Gate events.
    /// </summary>
    public interface IGate
    {
        /// <summary>
        /// Starts the gate, and the callback function is executed when an incoming connection is established
        /// </summary>
        void Start(Action<IConnectionMode> callback);
        /// <summary>
        /// Stops the gate from reacting on incoming connections.
        /// </summary>
        void Stop();
    }
}
