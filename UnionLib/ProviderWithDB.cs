using DataService;
using StatsisLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnionLib
{
    public class ProviderWithDB : UnionLib.IProviderBase
    {
        QHXEntities DBContext = new QHXEntities();
        public NestDirectory GetNestDirectory(object paramsObj)
        {
            NestDirectory root = new NestDirectory() { Name = "Root" };
            var infos = DBContext.StatsicInfo.Where(x => x.IsDel != 1).ToList();
            foreach (var item in infos)
            {
                NestDirectory sLine = new NestDirectory() { Name = item.StatsicName };
                sLine.Children = DBContext.StatsicRelation.Where(x => x.StatsicName == item.StatsicName).Select(x => new NestDirectory()
                       {
                           Name = x.GroupName
                       }).ToList();
                root.Children.Add(sLine);
            }
            return root;
        }
        public List<StatsisLib.UserInfo> GetUserInfos(object paramsObj)
        {
            return DBContext.UserInfo.Where(x => x.IsDel != 1 && x.IsShield != 1).Select(x => new StatsisLib.UserInfo()
                {
                    Num = x.Id.ToString(),
                    Name = x.Name,
                    InTime = x.InTime,
                    GroupName = x.GroupName,
                    OrderIndex = x.OrderIndex ?? 0
                    ,
                    Remark = x.Remark,
                    IsShield = x.IsShield ?? 0
                }).ToList();
        }


        public int GetGroupIndex(string key)
        {
            var info = DBContext.GroupInfo.FirstOrDefault(x => x.Name == key);
            if (info == null)
            {
                return 0;
            }
            return info.OrderIndex ?? 0;
        }
    }
}
