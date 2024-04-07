﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Common.Network
{
    public enum Commands
    {
        SET_USER = 1,
        SET_USER_OK,
        SET_USER_FAIL,
        MESSAGE,
        DISCONNECT,
        SERVER_STOPPED,
        ERROR
    }
}
