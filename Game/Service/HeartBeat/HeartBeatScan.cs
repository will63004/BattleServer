using System;
using System.Collections.Generic;
using System.Threading;

namespace Game.Service.HeartBeat
{
    public class HeartBeatScan
    {
        private IUsers m_users;

        private HeartBeatScanContext m_contex;

        private Dictionary<ulong, DateTime> m_tmpTimes = new Dictionary<ulong, DateTime>();

        public HeartBeatScan(IUsers users, HeartBeatScanContext context)
        {
            m_users = users;

            m_contex = context;
        }

        public void Start()
        {
            // 开启扫描离线线程
            Thread t = new Thread(new ThreadStart(ScanOffline));
            t.IsBackground = true;
            t.Start();
        }

        private void ScanOffline()
        {
            while (true)
            {
                // 一秒掃描一次
                Thread.Sleep(1000);

                m_users.GetUsersActiveDateTime(ref m_tmpTimes);

                var e = m_tmpTimes.GetEnumerator();
                while (e.MoveNext())
                {
                    // 判斷最後心跳時間是否大於間隔
                    TimeSpan sp = DateTime.Now - e.Current.Value;
                    if (sp.Seconds > m_contex.HeartBeatInterval)
                        m_users.ShutdownUser(e.Current.Key);
                }
            }
        }
    }
}
