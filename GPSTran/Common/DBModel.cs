using System.Collections.Generic;

namespace Common
{
   public class DBModel
    {
        //instance name
        private string name;

        
        //instance enabled
        private bool enabled;

       
        //table name
        private string tableName;

       
        // longitude offset
        private double lonOffset;

       
        //latitude offset
        private double latOffset;

        
        
        //if issi belongs to entityID , then GPS data will insert into the table
        private List<int> entityID;

        private int citycode;

        public int Citycode
        {
            get { return citycode; }
            set { citycode = value; }
        }

        private int maxCount;

        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        public double LonOffset
        {
            get { return lonOffset; }
            set { lonOffset = value; }
        }

        public double LatOffset
        {
            get { return latOffset; }
            set { latOffset = value; }
        }

        public List<int> EntityID
        {
            get { return entityID; }
            set { entityID = value; }
        }

    }
}
