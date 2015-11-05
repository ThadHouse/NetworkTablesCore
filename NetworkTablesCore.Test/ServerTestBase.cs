﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    public class ServerTestBase
    {
        [SetUp]
        public void Init()
        {
            if (!GlobalTestConstants.Started)
            {
                NetworkTable.SetIPAddress("127.0.0.1");
                NetworkTable.SetPort(10000);
                NetworkTable.SetServerMode();
                NetworkTable.Initialize();
                GlobalTestConstants.Started = true;
                GlobalTestConstants.Server = true;
            }
            else
            {
                if (GlobalTestConstants.Server == false)
                {
                    throw new InvalidOperationException("Test can only run either client or Server. Not both.");
                }
            }
        }
    }
}
