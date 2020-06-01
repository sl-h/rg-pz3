using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;

public class ToolTipHelper
{
    public static string Serialize(List<PowerEntity> entities)
    {
        string val = string.Empty;
        foreach (var item in entities)
        {
            val += "Type: " + GetType(item) + "\n";
            val += "Name: " + item.Name + "\n";
            val += "Id: " + item.Id + "\n";
            val += "X: " + item.X + "\n";
            val += "Y: " + item.Y + "\n";
            if (item is SwitchEntity) val += "Status: " + (item as SwitchEntity).Status + "\n";
            val += "___________" + "\n";
        }

        return val;
    }

    static string GetType(PowerEntity entity)
    {
        if (entity is NodeEntity) return "Node";
        else if (entity is SubstationEntity) return "Substation";
        else if (entity is SwitchEntity) return "Switch";
        else return "";
    }
}
