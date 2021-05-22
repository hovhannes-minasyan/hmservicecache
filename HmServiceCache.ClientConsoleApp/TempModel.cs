﻿using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace HmServiceCache.ClientConsoleApp
{
    [MessagePackObject]
    public class TempModel
    {
        [Key(0)]
        public int A { get; set; }
        [Key(1)]
        public string B { get; set; }

        public override string ToString()
        {
            return "{" + $"A = {A} B = {B}" + "}";
        }
    }
}