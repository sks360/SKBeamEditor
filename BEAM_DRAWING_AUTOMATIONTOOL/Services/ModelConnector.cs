using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures;

namespace SK.Tekla.Drawing.Automation.Services
{
    /// <summary>
    /// Establish connection with Tekla Model
    /// </summary>
    public class ModelConnector
    {
        public bool ValidateConnectivity()
        {
            var connected = false;
            
            var currentModel = skTSLib.ModelAccess.ConnectToModel();
            if (currentModel != null)
            {
                //try establishing conneciton to Tekla
                connected = TeklaStructures.Connect();
            }
            return connected;
        }

    }
}
