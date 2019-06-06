using System;
using System.Xml;

namespace Common
{
   public class LanguageHelper
    {
       private Language lang;
       public Language CurLanguage 
       {
           get { return lang; }
       }
       
       private string path;
       private XmlDocument Document = new XmlDocument();
       private XmlNode root;
       //init language information
       public LanguageHelper(Language desLanguage,string xmlPath) 
       {
           if (!xmlPath.Substring(xmlPath.LastIndexOf(".") + 1).Equals("xml"))
           {
               throw new Exception("invalid xml file path");
            
           }
           if (!System.IO.File.Exists(xmlPath)) 
           {
               throw new Exception("xml file path:"+xmlPath+" does not exists");
           }

           lang = desLanguage;
           path = xmlPath;
           Document.Load(path);
           root = Document.SelectSingleNode("Resource");
       }
       //init language information
       public LanguageHelper(Language desLanguage)
       {
          

           lang = desLanguage;
           path = "language//zh_cn.xml";
           Document.Load(path);
           root = Document.SelectSingleNode("Resource");
       }



       //init language information
       public LanguageHelper() 
       {
           this.lang = Language.zh_cn;
           path = "language//zh_cn.xml";
           Document.Load(path);
       }
       //根据传入的“key”值返回目标语言的描述
       public string Key(string key) 
       {
           
           try
           {
               XmlNode node = root.SelectSingleNode("//Add[@name='"+key+"']");
               if (node != null)
               {
                   return node.InnerText;
               }
               else 
               {
                   return key;
               }
           }
           catch
           {
               Logger.Error("error occur when change language,key name:"+key);
               return key;
           }

       }

    }

  public enum Language 
   {
        zh_cn,
        en_us,
   
   }


}
