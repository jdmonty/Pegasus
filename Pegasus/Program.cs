﻿// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System;

    internal class Program
    {
        public static void Main(string[] args)
        {
            CompileManager.CompileFile(args[0], null, Console.WriteLine);
        }
    }
}