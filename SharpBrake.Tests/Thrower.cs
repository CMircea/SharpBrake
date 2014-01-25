using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBrake.Tests
{
    internal static class Thrower
    {
        public static void Throw(Exception exception)
        {
            throw exception;
        }
    }
}
