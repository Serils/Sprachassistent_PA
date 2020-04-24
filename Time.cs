using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SliraAssistentFramework
{
    class Time
    {
        //Attribute
        private int hour;
        private int minute;

        //Construktor
       

        public int getHour()
        {
            return hour;
        }
        public int getMinute()
        {
            return minute;
        }

        public void setHour(int pHour)
        {
            hour = pHour;
        }
        public void setMinute(int pMinute)
        {
            minute = pMinute;
        }

        public Boolean timeIsEqual(Time pTime2)
        {
            if (this.getHour() == pTime2.getHour() && this.getMinute() == pTime2.getMinute())
            {
                return true;
            }
            else {return false;}
        }
    }
}
