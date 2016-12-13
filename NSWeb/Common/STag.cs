using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSWeb.Common
{
    public class STag
    {
        private static readonly string[] Tags = { "北京", "西安", "电话", "全媒", "VIP", "一般" };
        public static readonly string[] DomainTags = { "北京", "西安" };
        public static readonly string[] TypeTags = { "一般", "VIP", "全媒" };
        public static List<string> GetTags(string data)
        {
            List<string> tags = new List<string>();
            if (string.IsNullOrWhiteSpace(data))
            {
                return tags;
            }
            data = data.ToUpper();
            foreach (var item in Tags)
            {
                if (data.Contains(item))
                {
                    tags.Add(item);
                }
            }
            if (!data.Contains("全媒"))
            {
                tags.Add("电话");
            }
            if (!data.Contains("VIP") && !data.Contains("全媒"))
            {
                tags.Add("一般");
            }
            return tags;
        }
    }
}