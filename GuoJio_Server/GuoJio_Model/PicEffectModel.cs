﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatsPrj.Model
{
    public class PicEffectModel
    {
        public string effectId { get; set; }
        public string effectName { get; set; }
        public int G { get; set; }
        public int R { get; set; }
        public int B { get; set; }
        public int Rrate { get; set; }
        public int Grate { get; set; }
        public int Brate { get; set; }
        public int effectIndex { get; set; }
        public string effectsThumbNail { get; set; }
    }
}
