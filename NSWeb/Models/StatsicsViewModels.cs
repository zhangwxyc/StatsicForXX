using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSWeb.Models
{
    public class StatsicsViewModels
    {
        public string StatsicLineName { get; set; }
        public List<GroupItemModel> Datas { get; set; }

        public List<string> GroupsData { get; set; }

        public StatsicsViewModels()
        {
            Datas = new List<GroupItemModel>();
        }
    }
    public class GroupItemModel
    {
        public GroupInfo Info { get; set; }
        public int IsSelected { get; set; }

        public List<string> Tags { get; set; }
    }
    public class GroupSubItemModel
    {

        public List<GroupSubItemModel> SubList { get; set; }

        
    }
}