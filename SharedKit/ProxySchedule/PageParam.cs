using System;
using System.Collections.Generic;
using System.Text;

namespace ProxySchedule
{
    public class PageParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public PageParam(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }
    }
}
