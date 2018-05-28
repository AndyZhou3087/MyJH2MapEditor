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
        private string m_posnpcid;
        private int m_posnpcrnd = 0;

        public int col { get; set; }
        public int row { get; set; }
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
    }
}
