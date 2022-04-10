using Sue3.SUM.Model.Components.Descriptive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sue3.Web.Blazor.Data
{
    //this is used as a session storage, and general coordinator for the user.
    public class ModelHandlerService
    {
        public bool IsSuperUser { get; set; }
        public List<Model> Models { get; set; } = new List<Model>();// List of models used in models list
        public int ModelsPageIndex { get; set; }// remembers page of models list
        public Model CurrentModel { get; set; }//used between all pages as the model.
        public string CurrentModelOwner { get; set; }//used to remember who the current model belongs to, since this is only visible in the models list to superusers.
        public bool CalibrationSaved { get; set; }//bool to keep track of menu 5, if the user modifed anything.
        public int CalibrationPageIndex { get; set; }// remembers page of calibration log
        public bool NewModel { get; set; }//when user creates a fresh model, this makes so that in menu 1, the menus flow automatically.
        private Panes _currentPane;
        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();
        public Panes CurrentPane { //this is used to keep track of the current menu, and update the navmenu when it changes.
            get { return _currentPane; }
            set {
                _currentPane = value;
                NotifyStateChanged();
            } 
        }
        public enum Panes // menus
        {
            ModelList,
            Manage,
            Train,
            Visualise,
            Use,
            Calibrate
        }
        public ManagePanes CurrentManagePane { get; set; }//if in menu 1, keeps track of the submenu
        public enum ManagePanes// menu 1 submenus
        {
            Model,
            Outcome,
            Variables
        }
    }
}
