using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StockManagementSystem.DAL;
using StockManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.helper
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key,List<SalesItemVM> value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static List<SalesItemVM> GetObjectFromJson(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? new List<SalesItemVM>() : JsonConvert.DeserializeObject<List<SalesItemVM>>(value);
        }

        public static void SetObjectAsJson(this ISession session, string key, List<PurchaseItemVM> value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static List<PurchaseItemVM> GetObjectFromJson(this ISession session, string key,string val)
        {
            var value = session.GetString(key);
            return value == null ? new List<PurchaseItemVM>() : JsonConvert.DeserializeObject<List<PurchaseItemVM>>(value);
        }
    }
}
