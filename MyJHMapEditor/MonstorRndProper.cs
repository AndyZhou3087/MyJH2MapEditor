using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJHMapEditor
{
    class MonstorRndProper
    {
        public string mid { get; set; }
        public int minlv { get; set; }
        public int maxlv { get; set; }
        public int minqu { get; set; }
        public int maxqu { get; set; }
        public int rnd { get; set; }
    }

    class MonstorAwardRes
    {
        private string[] m_res = new string[6];
        private int[] m_rescount = new int[6];
        private int[] m_resqu = new int[6];
        private float[] m_resrnd = new float[6] { 100, 100, 100, 100, 100, 100 };

        public void setRes(int index, string id)
        {
            m_res[index] = id;
        }
        public string getRes(int index)
        {
            return m_res[index];
        }
        public void setResCount(int index, int count)
        {
            m_rescount[index] = count;
        }
        public int getResCount(int index)
        {
            return m_rescount[index];
        }
        public void setResQU(int index, int count)
        {
            m_resqu[index] = count;
        }
        public int getResQU(int index)
        {
            return m_resqu[index];
        }

        public void setResRnd(int index, float val)
        {
            m_resrnd[index] = val;
        }
        public float getResRnd(int index)
        {
            return m_resrnd[index];
        }
    }
}
