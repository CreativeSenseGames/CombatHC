using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace VirtualIT.TouchInputs
{
    [Flags]
    public enum TouchCount
    {
        [Description("1")]
        One = 1 << 1,
        [Description("2")]
        Two = 1 << 2,
        [Description("3")]
        Three = 1 << 3,
        [Description("4")]
        Four = 1 << 4,
        [Description("5")]
        Five = 1 << 5,
        [Description("6")]
        Six = 1 << 6,
        [Description("7")]
        Seven = 1 << 7,
        [Description("8")]
        Eight = 1 << 8,
        [Description("9")]
        Nine = 1 << 9,
        [Description("10")]
        Ten = 1 << 10, 
        [Description(">10")]
        TenMore = 1 << 11,
    }
}
