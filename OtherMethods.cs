using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shrek_2_team_action_tools
{
    public class OtherMethods
    {
        public static bool hasSpecificChars(string str)
        {
            for(int i = 0; i < str.Length; i++)
            {
                if (str[i] < ' ') return true;
            }

            return false;
        }
    }
}
