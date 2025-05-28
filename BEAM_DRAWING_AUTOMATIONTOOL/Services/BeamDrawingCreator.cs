using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using System.Windows.Forms;
using SK.Tekla.Drawing.Automation.Support;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SK.Tekla.Drawing.Automation.Drawing;
using SK.Tekla.Drawing.Automation.Models;
using System.Reflection;

namespace SK.Tekla.Drawing.Automation.Services
{
    public class BeamDrawingCreator
    {
        private readonly List<string> _messageStore = new List<string>();

        private readonly List<string> beamDrawings = new List<string>();

        public BeamDrawingCreator(List<string> messageStore) { 
            _messageStore = messageStore;
        }

        public string Execute(SheetSelector sheetSelector)
        {
            string statusMessage = String.Empty;


            TSM.Model mymodel = new TSM.Model();
            Type type_for_bolt = typeof(TSD.Bolt);
            Type type_for_part = typeof(TSD.Part);
            TSD.DrawingHandler my_handler = new TSD.DrawingHandler();
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            
            string skDefaultADFile = "";

            var drg_att_list = ConfigureDrawingData.PrepareDrawingAttributes(mymodel, 
                sheetSelector, ref skDefaultADFile);

            string defaultADFile = "";
            PickBeams();
            return statusMessage;
        }

        private bool PickBeams()
        {
            string msgCaption = string.Empty;
            TSM.UI.Picker picker = new TSM.UI.Picker();
            TSM.ModelObjectEnumerator beams = null;
            try { 
             beams = picker.PickObjects(TSM.UI.Picker.PickObjectsEnum.PICK_N_OBJECTS, "PICK ALL BEAMS");
            }
            catch (ApplicationException ae)
            {
                MessageBox.Show($"User Interupted during picking up objects. Please try again",
                msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                System.Environment.Exit(360);
            }
            if (beams == null || beams.GetSize() == 0)
            {
                MessageBox.Show("Beams are not yet picked");
                return false;
            }
            bool hasError = false;
            int cnt = 0;
            while (beams.MoveNext())
            {
                cnt++;
                TSM.ModelObject currentModelObject = beams.Current;
                string assemblyName = string.Empty;
                var numbered = AssemblyExtension.CheckNumbering(currentModelObject, ref assemblyName);
                if (!numbered)
                {
                    hasError = true;
                    _messageStore.Add($"Beam {numbered} is not numbered appropriately");
                    continue;
                }
                if (beamDrawings.Contains(assemblyName))
                {
                    //_messageStore.Add($"Beam {assemblyName} is already present");
                    continue;
                }
                beamDrawings.Add(assemblyName);
            }
            return hasError;
        }

        public async Task PerformOperationAsync(IProgress<string> progress)
        {
            progress.Report("Starting operation...");
            await Task.Delay(1000); // Simulate initial work
            progress.Report("Milestone 1 reached.");
            await Task.Delay(2000); // Simulate more work
            progress.Report("Milestone 2 reached.");
            await Task.Delay(1500); // Simulate final work
            progress.Report("Milestone 3 reached.");
            progress.Report("Operation completed.");
        }

        //add this in form
        //private async void button1_Click(object sender, EventArgs e)
        //{
        //    button1.Enabled = false; // Disable button to prevent multiple clicks
        //    textBox1.Clear(); // Clear previous progress
        //    try
        //    {
        //        var service = new LongRunningService();
        //        var progress = new Progress<string>(message => AppendProgress(message));
        //        await service.PerformOperationAsync(progress);
        //    }
        //    catch (Exception ex)
        //    {
        //        AppendProgress($"Error: {ex.Message}");
        //    }
        //    finally
        //    {
        //        button1.Enabled = true; // Re-enable button
        //    }
        //}

        //private void AppendProgress(string message)
        //{
        //    textBox1.AppendText(message + "\n");
        //    textBox1.SelectionStart = textBox1.Text.Length;
        //    textBox1.ScrollToCaret(); // Scroll to the latest message
        //}
    }


}
