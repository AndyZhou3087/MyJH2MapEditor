using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyJHMapEditor
{
    class CellData
    {
        private int[] m_eventsrnd = new int[7];//7个事件概率
        private int m_postype = -1;
        private string m_posnpcid = "";
        private int m_posnpcrnd = 0;

        private string[] m_monstersid = new string[6];
        private int[] m_monsterslv = new int[6];
        private int[] m_monstersqu = new int[6];
        private string[] m_monstergetres = new string[6];
        private int[] m_monstergetrescount = new int[6];
        private int[] m_monstergetresqu = new int[6];
        private float[] m_monstergetresrnd = new float[6]{100,100,100,100,100,100};
        public int col { get; set; }
        public int row { get; set; }
        public string boardblock { get; set; }
        public bool walkable { get; set; }
        public string buildblock { get; set; }
        public void setEventRnd(int eventindex, int rnd)
        {
            m_eventsrnd[eventindex] = rnd;
        }
        public int getEventRnd(int eventindex)
        {
            return m_eventsrnd[eventindex];
        }

        public int postype
        {
            get { return m_postype; }
            set { m_postype = value; }
        }

        public string posnpcid
        {
            get { return m_posnpcid; }
            set { m_posnpcid = value; }
        }
        public int posnpcrnd
        {
            get { return m_posnpcrnd; }
            set { m_posnpcrnd = value; }
        }

        public ChoicesData choiceDataA{ get; set; }
        public ChoicesData choiceDataB { get; set; }

        public void setMonstersId(int index, string id)
        {
            m_monstersid[index] = id;
        }
        public string getMonstersId(int index)
        {
            return m_monstersid[index];
        }

        public void setMonstersLv(int index, int lv)
        {
            m_monsterslv[index] = lv;
        }
        public int getMonstersLv(int index)
        {
            return m_monsterslv[index];
        }

        public void setMonstersQU(int index, int qu)
        {
            m_monstersqu[index] = qu;
        }
        public int getMonstersQU(int index)
        {
            return m_monstersqu[index];
        }

        public void setMonstersGetRes(int index, string id)
        {
            m_monstergetres[index] = id;
        }
        public string getMonstersGetRes(int index)
        {
            return m_monstergetres[index];
        }
        public void setMonstersGetResCount(int index, int count)
        {
            m_monstergetrescount[index] = count;
        }
        public int getMonstersGetResCount(int index)
        {
            return m_monstergetrescount[index];
        }
        public void setMonstersGetResQU(int index, int count)
        {
            m_monstergetresqu[index] = count;
        }
        public int getMonstersGetResQU(int index)
        {
            return m_monstergetresqu[index];
        }

        public void setMonstersGetResRnd(int index, float val)
        {
            m_monstergetresrnd[index] = val;
        }
        public float getMonstersGetResRnd(int index)
        {
            return m_monstergetresrnd[index];
        }
    }
}
