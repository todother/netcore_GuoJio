﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatsPrj.Model
{
    public class RobotChatModel
    {
        public int chatId { get; set; }
        public string robotName { get; set; }
        public string robotType { get; set; }
        public string chatContent { get; set; }
        public int chatIdx { get; set; }
        public int timeout { get; set; }
        public int waitUserResponse { get; set; }
    }
}
