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
        private int stunde;
        private int minute;

        //Construktor
       

        public int getStunde()
        {
            return stunde;
        }
        public int getMinute()
        {
            return minute;
        }

        public void setStunde(int pStunde)
        {
            stunde = pStunde;
        }
        public void setMinute(int pMinute)
        {
            minute = pMinute;
        }

        public Boolean timeIsEqual(Time pTime2)
        {
            if (this.getStunde() == pTime2.getStunde() && this.getMinute() == pTime2.getMinute())
            {
                return true;
            }
            else {return false;}
        }
    }
}
