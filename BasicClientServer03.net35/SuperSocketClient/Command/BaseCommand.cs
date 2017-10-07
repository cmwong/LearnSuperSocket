﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.ClientEngine.Protocol;
using SuperSocket.ProtoBase;
namespace SuperSocketClient.Command
{
    abstract class BaseCommand : ICommand<StringClient, StringPackageInfo>
    {
        public abstract void ExecuteCommand(StringClient session, StringPackageInfo commandInfo);

        public virtual string Name
        {
            get { return GetType().Name; }
        }
    }
}
