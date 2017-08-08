using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class LocatorModel
    {
        public int LocatorId { get; set; }
        public string LocatorValue { get; set; }
        public bool CreateNew { get; set; }
        public string WarehouseInfo { get; set; }
        public int WarehoseId { get; set; }
        public string Warehouse { get; set; }
        public string Xaxsis { get; set; }
        public string Yaxsis { get; set; }
        public string Zaxsis { get; set; }
        public string Value { get; set; }

        /// <summary>
        /// savelocator value into database
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="warehouseId"></param>
        /// <param name="tValue"></param>
        /// <param name="tX"></param>
        /// <param name="tY"></param>
        /// <param name="tZ"></param>
        /// <returns></returns>
        public int LocatorSave(Ctx ctx, string warehouseId, string tValue, string tX, string tY, string tZ)
        {
            var loc = MLocator.Get(ctx, Convert.ToInt32(warehouseId), tValue, tX, tY, tZ);
            return loc.GetM_Locator_ID();
        }
    }
}