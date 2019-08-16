﻿using System;

namespace dotNET.Core
{
    public class DisposableAction : IDisposable
    {
        readonly Action _action;

        public DisposableAction(Action action)
        {
            _action = action ?? throw new ArgumentNullException("action");
        }

        public void Dispose()
        {
            _action();
        }
    }
}