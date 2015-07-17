using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ad.Fw
{
   public static class AppSetting
    {
       // 强插
       private static string forcedInsert;
       // 强拆
       private static string forcedHangUp;
       // 分机代答
       private static string extensionAnswer;
       // 监听
       private static string monitor;
       // 呼叫转移
       private static string callTransfer;
       public static string AppTitle
       {
           get
           {
               return ConfigurationManager.AppSettings["AppTitle"];
           }
       }

       public static int PageSize
       {
           get
           {
               return int.Parse(ConfigurationManager.AppSettings["PageSize"]);
           }
       }

       public static string ForcedInsert
       {
           get
           {
               if (!string.IsNullOrWhiteSpace(forcedInsert))
               {
                   return forcedInsert;
               }
               forcedInsert = ConfigurationManager.AppSettings["ForcedInsert"];
               return forcedInsert;
           }
       }

       public static string ForcedHangUp
       {
           get
           {
               if (!string.IsNullOrWhiteSpace(forcedHangUp))
               {
                   return forcedHangUp;
               }
               forcedHangUp = ConfigurationManager.AppSettings["ForcedHangUp"];
               return forcedHangUp;
           }
       }

       public static string ExtensionAnswer
       {
           get
           {
               if (!string.IsNullOrWhiteSpace(extensionAnswer))
               {
                   return extensionAnswer;
               }
               extensionAnswer = ConfigurationManager.AppSettings["ExtensionAnswer"];
               return extensionAnswer;
           }
       }

       public static string Monitor
       {
           get
           {
               if (!string.IsNullOrWhiteSpace(monitor))
               {
                   return monitor;
               }
               monitor = ConfigurationManager.AppSettings["Monitor"];
               return monitor;
           }
       }

       public static string CallTransfer
       {
           get
           {
               if (!string.IsNullOrWhiteSpace(callTransfer))
               {
                   return callTransfer;
               }
               callTransfer = ConfigurationManager.AppSettings["CallTransfer"];
               return callTransfer;
           }
       }
    }
}
