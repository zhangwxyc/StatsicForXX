using System;
namespace UnionLib
{
    public interface IProviderBase
    {
        StatsisLib.NestDirectory GetNestDirectory(object paramsObj);
        System.Collections.Generic.List<StatsisLib.UserInfo> GetUserInfos(object paramsObj);

        int GetGroupIndex(string key);
    }
}
