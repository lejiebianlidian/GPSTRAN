using System.Collections.Generic;

namespace Common
{
   public class SpecialDBModel
    {
        private string name;

       
        private string tableName;

        private bool enabled;

       

        private int maxCount;

        private double lonOffset;

        private double latOffset;

        private List<int> entityID;

        public List<int> EntityID
        {
            get { return entityID; }
            set { entityID = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
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

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }


    }
}
