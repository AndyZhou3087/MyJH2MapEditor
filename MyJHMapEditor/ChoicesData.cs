using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJHMapEditor
{
    class ChoicesData
    {
        private string[] res = new string[3];
        private int[] rescount = new int[3];
        private int m_RetType = -1;
        public string Content{ get; set; }
        public void setGetRes(int index, string resid)
        {
            res[index] = resid;
        }
        public string getGetRes(int index)
        {
            return res[index];
        }
        public void setGetResCount(int index, int count)
        {
            rescount[index] = count;
        }
        public int getGetResCount(int index)
        {
            return rescount[index];
        }
        public string LossRes { get; set; }
        public int LossResCount { get; set; }
        
        public string EffectBoss { get; set; }

        public int RetType
        {
            get { return m_RetType; }
            set { m_RetType = value; }
        }
    }
}
