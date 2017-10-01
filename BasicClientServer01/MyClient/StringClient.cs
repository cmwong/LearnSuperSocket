using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using SuperSocket.ClientEngine.Protocol;

namespace MyClient
{
    public class StringClient : EasyClient<StringPackageInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, ICommand<StringClient, StringPackageInfo>> m_CommandDict
            = new Dictionary<string, ICommand<StringClient, StringPackageInfo>>(StringComparer.OrdinalIgnoreCase);

        public StringClient() : base()
        {
            Init();
        }

        protected void Init()
        {
            Command.ResponseEcho echoCmd = new Command.ResponseEcho();
            m_CommandDict.Add(echoCmd.Name, echoCmd);
            Command.ResponseAdd addCmd = new Command.ResponseAdd();
            m_CommandDict.Add(addCmd.Name, addCmd);


            this.Error += new EventHandler<ErrorEventArgs>(Client_Error);
            this.Connected += new EventHandler((s, e) =>
            {
                log4j.Info("connected");
            });
            this.Closed += new EventHandler((s, e) =>
            {
                log4j.Info("closed");
            });

            this.Initialize(new StringReceiveFilter());
        }

        protected void Client_Error(object sender, ErrorEventArgs e)
        {
            log4j.Error(string.Format("sender: {0}", sender), e.Exception);
        }




        protected override void HandlePackage(IPackageInfo package)
        {
            StringPackageInfo cmdInfo = (StringPackageInfo)package;
            if (cmdInfo.Key.Contains("Response"))
            {
                ExceuteCommand(cmdInfo);
            }
            else
            {
                log4j.Info("unknow command: " + cmdInfo.Key + ", body: " + cmdInfo.Body);
            }

        }

        protected void ExceuteCommand(StringPackageInfo cmdInfo)
        {
            if (m_CommandDict.TryGetValue(cmdInfo.Key, out ICommand<StringClient, StringPackageInfo> command))
            {
                command.ExecuteCommand(this, cmdInfo);
            }
            else
            {
                log4j.Info("unknow command: " + cmdInfo.Key + ", body: " + cmdInfo.Body);
            }
        }

    }
}
