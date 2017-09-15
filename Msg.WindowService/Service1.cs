using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lelog.Data;
using Msg.Entities;



namespace Msg.WindowService
{
    /// <summary>
    /// 取消和修改已收货订单服务
    /// </summary>
    public partial class Service1 : ServiceBase
    {
        private System.Threading.Timer _timer1;
        public Service1()
        {
            InitializeComponent();
        }


        public void StartService(string[] args)
        {
            _timer1 = new Timer(CancelOrDoneOrder, null, 0, 1800000);

        }

        public void CancelOrDoneOrder(object state)
        {
            //已收货订单修改为已完成
            var sql = string.Format("UPDATE dbo.OrdersEntity SET Status={0} WHERE Status={1}   ",
                (int)OrderStatusEnum.Done, (int)OrderStatusEnum.Received);
            //过期取消订单
            sql +=
                string.Format(
                    "  UPDATE dbo.OrdersEntity SET Status ={0} WHERE Status = {1} AND DATEDIFF(DAY,CreateTime,getdate()) >= 7",
                    (int)OrderStatusEnum.Expired, (int)OrderStatusEnum.UnPay);

            DbHelper db=new DbHelper();
            var cmd = db.GetSqlStringCommond(sql);
            db.ExecuteNonQuery(cmd);
        }

        protected override void OnStart(string[] args)
        {
            StartService(args);
        }

        protected override void OnStop()
        {
        }
    }
}
