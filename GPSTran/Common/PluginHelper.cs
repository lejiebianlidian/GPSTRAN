using System;
using System.Reflection;

/**
 *插件辅助类：
 *提供插件（dll文件）的加载方法
 *限制1：dll名称一定要与命名空间名称一致，例如:GPS.dll,那么命名空间名称为GPS ，
 *限制2：插件中必须实现IPlugin接口，必须在内部以异步的方式处理GPS数据，也就是说
 *限制3：实现IPlugin接口的类名称必须为Plugin
 *必须在插件中另建缓存，另外启动一个处理GPS消息的线程。具体例子参考TaiZhouPGIS2项目
**/


namespace Common
{
   public class PluginHelper
    {
       private MethodInfo mInfo;
       private Assembly ass;
       private Type classType;
       private Object obj;
       private string namespaceName;
       public PluginHelper(string path) 
       {
           if (string.IsNullOrEmpty(path)) 
           {
               throw new Exception("empty string value");
           }

           if (!path.Substring(path.LastIndexOf(".") + 1).Equals("dll")) 
           {
               throw new Exception("invalid dll path");
           }
           //获取命名空间名称
           if (path.IndexOf("\\") >= 0)
           {
               string temp = path.Substring(path.LastIndexOf("\\")+1);
               temp = temp.Remove(temp.LastIndexOf("."));
               namespaceName=temp;
           }
           else
           {
               string temp = path.Remove(path.LastIndexOf("."));

               namespaceName = temp;
           }
           try
           {

               ass = Assembly.LoadFrom(path);
                
               classType = ass.GetType(namespaceName+".Plugin");
              //实例化插件类的对象
               obj = Activator.CreateInstance(classType, null);
           }
           catch (Exception ex) 
           {
               throw ex; 
           }
       
       }
       public PluginHelper(PluginModel pModel,bool threadStart) 
       {
           if (string.IsNullOrEmpty(pModel.DllModel.MainDllPath))
           {
               throw new Exception("empty string value");
           }

           if (!pModel.DllModel.MainDllPath.Substring(pModel.DllModel.MainDllPath.LastIndexOf(".") + 1).Equals("dll"))
           {
               throw new Exception("invalid dll path");
           }
           //获取命名空间名称
           if (pModel.DllModel.MainDllPath.IndexOf("\\") >= 0)
           {
               string temp = pModel.DllModel.MainDllPath.Substring(pModel.DllModel.MainDllPath.LastIndexOf("\\") + 1);
               temp = temp.Remove(temp.LastIndexOf("."));
               namespaceName = temp;
           }
           else
           {
               string temp = pModel.DllModel.MainDllPath.Remove(pModel.DllModel.MainDllPath.LastIndexOf("."));

               namespaceName = temp;
           }

           try
           {

               ass = Assembly.LoadFrom(pModel.DllModel.MainDllPath);
               //实例化插件类的对象
               classType = ass.GetType(namespaceName + ".Plugin");
               obj = Activator.CreateInstance(classType, new object[]{pModel,threadStart});
               mInfo = classType.GetMethod("Execute");
           }
           catch (Exception ex)
           {
               throw ex;
           }
       
       }
       public void SetPluginModel(PluginModel pModel) 
       {
           try
           {
               MethodInfo Info = classType.GetMethod("SetPluginModel");
               Info.Invoke(obj, new object[]{pModel});
           }
           catch (Exception ex)
           {
               throw ex;
           }
       
       }

       public void Execute(byte[] data) 
       {
           try
           {
               mInfo.Invoke(obj, new object[] { data });

           }
           catch (Exception ex) 
           {
               throw ex;
           
           }
       
       }

       public PluginType GetPluginType() 
       {
           try
           {
             MethodInfo  Info = classType.GetMethod("GetPluginType");
             return (PluginType)Info.Invoke(obj, null); 
           }
           catch (Exception ex) 
           {
               throw ex;
           }

       }
       public string GetNamespaceName() 
       {
           return namespaceName;
       }

       public int GetStatus() 
       {

           try
           {
               MethodInfo Info = classType.GetMethod("GetStatus");
               return (int)Info.Invoke(obj, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       
       }
       public bool IsStop()
       {
           try
           {
               MethodInfo Info = classType.GetMethod("IsStop");
               return (bool)Info.Invoke(obj, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       
       }


       public long GetStatistics() 
       {
           try
           {
               MethodInfo Info = classType.GetMethod("GetStatistics");
               return (long)Info.Invoke(obj, null);
           }
           catch (Exception ex)
           {
               throw ex;
           } 
        
       }
       public void SetEnabled(bool enabled) 
       {
           try
           {
               MethodInfo Info = classType.GetMethod("SetEnabled");
               Info.Invoke(obj, new object[]{enabled});
           }
           catch (Exception ex)
           {
               throw ex;
           } 
       
       }
       public void SetDebugEnabled(bool enabled)
       {
           try
           {
               MethodInfo Info = classType.GetMethod("SetDebugEnabled");
               Info.Invoke(obj, new object[] { enabled });
           }
           catch (Exception ex)
           {
               throw ex;
           }

       }


    }
}
