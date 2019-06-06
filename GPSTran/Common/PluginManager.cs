using System;

namespace Common
{
   public class PluginManager:IPlugin
    {
  
       private PluginModel pModel;

       private PluginHelper helper;
      
       private readonly Object lckHelper = new Object();




       public PluginManager(PluginModel pModel, bool threadStart) 
       {
          
           this.pModel = pModel;
           try
           {
               helper = new PluginHelper(pModel,threadStart);
           }
           catch (Exception ex)
           {
               //throw ex;
               Logger.Error(ex.ToString());

           }
       
       }

       public void SetPluginModel(PluginModel pModel) 
       {
           lock (lckHelper)
           {
               try
               {
                   helper.SetPluginModel(pModel);
                   this.pModel = pModel;
               }
               catch (Exception ex) 
               {
                   //throw ex;
                   Logger.Error(ex.ToString());
               
               }
           }
        
       }
       public PluginModel GetPluginModel() 
       {
           return pModel;
       }
      public int GetStatus() 
       {
           try
           {
               return helper.GetStatus();
           }
           catch (Exception ex)
           {
               //throw ex;
               Logger.Error(ex.ToString());
               return 0;
           }
       }
       public long GetStatistics() 
       {
           try
           {
               return helper.GetStatistics();
           }
           catch (Exception ex)
           {
               //throw ex;
               Logger.Error(ex.ToString());
               return 0;
           }
       }
      public  PluginType GetPluginType() 
       {
           try
           {
               return helper.GetPluginType();
           }
           catch (Exception ex)
           {
               //throw ex;
               Logger.Error(ex.ToString());
               return 0;
           }
       }
      public void SetEnabled(bool enabled) 
       {
           try
           {
                helper.SetEnabled(enabled);
           }
           catch (Exception ex)
           {
               //throw ex;
               Logger.Error(ex.ToString());

           }
       }
      public  void SetDebugEnabled(bool enabled) 
       {
           try
           {
                helper.SetDebugEnabled(enabled);
           }
           catch (Exception ex)
           {
               //throw ex;
               Logger.Error(ex.ToString());
           }
       }
       public void Execute(byte[] data)
       {
           try
           {
               helper.Execute(data);
           }
           catch (Exception ex)
           {
               //throw ex;
               Logger.Error(ex.ToString());
           }
       
       }
       public bool IsStop() 
       {
           try
           {
                return helper.IsStop();
           }
           catch (Exception ex)
           {
               //throw ex;
               Logger.Error(ex.ToString());
               return false;
           }
       
       
       }


    }
}
