using System;
using System.Web;
using System.Web.Caching;
using DevExpress.Web;

namespace ClinicalStudy.Site {
    public class LoginPhotoImageStorageStrategy : RuntimeStorageStrategy {
        public override bool CanStoreData(ASPxWebControlBase control) {
            return HttpContext.Current.Cache != null;
        }
        public override string GetResourceKey(ASPxWebControlBase control, byte[] content, string mimeType) {
            int pos = control.ID.LastIndexOf("_");
            return control.ID.Substring(pos + 1);
        }
        public override void StoreResourceData(ASPxWebControlBase control, string key, BinaryStorageData data) {
            HttpContext.Current.Cache.Add(key, data, null, Cache.NoAbsoluteExpiration,
                TimeSpan.FromMinutes(1), CacheItemPriority.NotRemovable, null);
        }
        public override BinaryStorageData GetResourceData(string key) {
            if(HttpContext.Current.Cache != null) {
                BinaryStorageData data = (BinaryStorageData)HttpContext.Current.Cache[key];
                if(data != null) HttpContext.Current.Cache.Remove(key);
                return data;
            }
            return null;
        }
    }
}
