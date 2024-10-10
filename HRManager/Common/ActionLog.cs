using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManager.Common
{
    public class ActionLog
    {
        private HRManagerEntities db = new HRManagerEntities();
        public void InsertActionLog(string AccountName , int FunctionID , int ActionID , string ObjectID )
        {
            tblSysLog syslog = new tblSysLog();
            syslog.AccountLogin = AccountName;
            syslog.DateLog = DateTime.Now;
            syslog.FunctionLog = FunctionID;
            syslog.ActionLog = ActionID;
            syslog.ObjectLog = ObjectID;
            db.tblSysLog.Add( syslog );
            db.SaveChanges();
        }
    }
}