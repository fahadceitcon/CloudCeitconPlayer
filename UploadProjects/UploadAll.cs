using Ceitcon_Data.Utilities;
using System.Security.Cryptography;
using System.Windows.Forms;
using UploadProjects.Model;

namespace UploadProjects
{
    public partial class UploadAll : Form
    {
        public UploadAll()
        {
            InitializeComponent();
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            string sResult = string.Empty;
            var file = LoadFile.ShowDialog();

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            

            if (file == DialogResult.OK)
            {
                var schedules = ((List<Schedule>)GV_Schedules.DataSource).Where(s => s.Selected).ToList();
                foreach (var schedule in schedules)
                {
                    if (LoadFile.FileNames.Count() > 0)
                    {
                        bool bAdded = false;
                        var Project = Ceitcon_Data.Utilities.IOManagerProject.LoadProjectFromString(schedule.CDPFileContent, false);

                        string projectPath = Path.Combine(path, Project.Id);
                        string projectFile = Path.Combine(projectPath, String.Format("{0}{1}", Project.Information.ProjectName, IOManagerProject.ProjectFileExtension));

                        var flies = LoadFile.FileNames;
                        schedule.Files = new List<string>();
                        foreach (var f in flies)
                        {
                            if (f.ToLower().EndsWith(".jpg") || f.ToLower().EndsWith(".png") || f.ToLower().EndsWith(".bmp") || f.ToLower().EndsWith(".jpeg"))
                            {
                                Ceitcon_Data.Model.SlideModel slide = new Ceitcon_Data.Model.SlideModel(Project.Regions[0]);
                                slide.Id = Guid.NewGuid().ToString();
                                slide.Name = "Slide_" + slide.Id.Substring(5);
                                slide.Duration = new TimeSpan(0, 0, ((int)NumDuration.Value));
                                slide.Forever = false;
                                slide.EnableSchedule = true;
                                slide.StartDate = DTStartDate.Value;
                                slide.EndDate = DTEndDate.Value;
                                slide.StartTime = DTStartTime.Value.ToShortTimeString();
                                slide.EndTime = DTEndTime.Value.ToShortTimeString();
                                Ceitcon_Data.Model.LayerModel layer = new Ceitcon_Data.Model.LayerModel(slide);
                                layer.Id = Guid.NewGuid().ToString();
                                layer.Name = "Layer_" + layer.Id.Substring(5);
                                layer.ZIndex = 1;
                                layer.IsVisible = true;
                                layer.IsLocked = false;
                                Ceitcon_Data.Model.ControlModel control = new Ceitcon_Data.Model.ControlModel(Ceitcon_Data.Model.ControlType.Image, 0, 0, Project.Regions[0].Width, Project.Regions[0].Height, layer)
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = "Image",
                                    Opacity = 1,
                                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                                    Stretch = System.Windows.Media.Stretch.Fill,
                                    HorizontalFlip = false,
                                    VerticalFlip = false,
                                    Rotate = 0,
                                    FontSize = 0,
                                    //FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(20),
                                    //FontStyle = new System.Windows.FontStyle();
                                    //TextDecoration = TextDecoration
                                    InvertDirection = false,
                                    Duration = new TimeSpan(0),
                                    FlowDirection = false,
                                    DateTimeFormat = 0,
                                    ItemCount = 0,
                                    Type = Ceitcon_Data.Model.ControlType.Image,
                                    ZIndex = 1,
                                    IsVisible = true,
                                    IsLocked = false,
                                    Playlist = new System.Collections.ObjectModel.ObservableCollection<Ceitcon_Data.Model.Playlist.PlaylistModel>()
                                };
                                Ceitcon_Data.Model.Playlist.SetContentModel playlist = new Ceitcon_Data.Model.Playlist.SetContentModel(control)
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = "Playlist_" + Guid.NewGuid().ToString().Substring(0, 6),
                                    StartTime = new TimeSpan(0L),
                                    Duration = new TimeSpan(0L),
                                    Forever = true,
                                    Type = Ceitcon_Data.Model.Playlist.PlaylistType.SetContent,
                                    Content = System.IO.Path.GetFileName(f),
                                    ContentSize = new System.IO.FileInfo(f).Length
                                };

                                control.Playlist.Add(playlist);
                                control.SelectedPlaylist = control.Playlist.FirstOrDefault();
                                layer.Controls = new System.Collections.ObjectModel.ObservableCollection<Ceitcon_Data.Model.ControlModel>() { control };
                                layer.SelectedControl = layer.Controls.FirstOrDefault();
                                slide.Layers = new System.Collections.ObjectModel.ObservableCollection<Ceitcon_Data.Model.LayerModel>() { layer };
                                slide.SelectedLayer = slide.Layers.FirstOrDefault();
                                var collection = Project.Regions[0].SlidesDownload;
                                collection.Add(slide);
                                Project.Regions[0].SlidesDownload = collection;
                                schedule.Files.Add(f);
                                bAdded = true;
                            }
                            else if (f.ToLower().EndsWith(".mp4"))
                            {
                                Ceitcon_Data.Model.SlideModel slide = new Ceitcon_Data.Model.SlideModel(Project.Regions[0]);
                                slide.Id = Guid.NewGuid().ToString();
                                slide.Name = "Slide_" + slide.Id.Substring(5);
                                slide.Duration = new TimeSpan(0, 0, ((int)NumDuration.Value));
                                slide.Forever = false;
                                slide.EnableSchedule = true;
                                slide.StartDate = DTStartDate.Value;
                                slide.EndDate = DTEndDate.Value;
                                slide.StartTime = DTStartTime.Value.ToShortTimeString();
                                slide.EndTime = DTEndTime.Value.ToShortTimeString();
                                Ceitcon_Data.Model.LayerModel layer = new Ceitcon_Data.Model.LayerModel(slide);
                                layer.Id = Guid.NewGuid().ToString();
                                layer.Name = "Layer_" + layer.Id.Substring(5);
                                layer.ZIndex = 1;
                                layer.IsVisible = true;
                                layer.IsLocked = false;
                                Ceitcon_Data.Model.ControlModel control = new Ceitcon_Data.Model.ControlModel(Ceitcon_Data.Model.ControlType.Image, 0, 0, Project.Regions[0].Width, Project.Regions[0].Height, layer)
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = "Video",
                                    Opacity = 1,
                                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                                    Stretch = System.Windows.Media.Stretch.Fill,
                                    HorizontalFlip = false,
                                    VerticalFlip = false,
                                    Rotate = 0,
                                    FontSize = 0,
                                    //FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(20),
                                    //FontStyle = new System.Windows.FontStyle();
                                    //TextDecoration = TextDecoration
                                    InvertDirection = false,
                                    Duration = new TimeSpan(0),
                                    FlowDirection = false,
                                    DateTimeFormat = 0,
                                    ItemCount = 0,
                                    Type = Ceitcon_Data.Model.ControlType.Video,
                                    ZIndex = 1,
                                    IsVisible = true,
                                    IsLocked = false,
                                    Playlist = new System.Collections.ObjectModel.ObservableCollection<Ceitcon_Data.Model.Playlist.PlaylistModel>()
                                };
                                Ceitcon_Data.Model.Playlist.SetContentModel playlist = new Ceitcon_Data.Model.Playlist.SetContentModel(control)
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = "Playlist_" + Guid.NewGuid().ToString().Substring(0, 6),
                                    StartTime = new TimeSpan(0L),
                                    Duration = new TimeSpan(0L),
                                    Forever = true,
                                    Type = Ceitcon_Data.Model.Playlist.PlaylistType.SetContent,
                                    Content = System.IO.Path.GetFileName(f),
                                    ContentSize = new System.IO.FileInfo(f).Length
                                };

                                control.Playlist.Add(playlist);
                                control.SelectedPlaylist = control.Playlist.FirstOrDefault();
                                layer.Controls = new System.Collections.ObjectModel.ObservableCollection<Ceitcon_Data.Model.ControlModel>() { control };
                                layer.SelectedControl = layer.Controls.FirstOrDefault();
                                slide.Layers = new System.Collections.ObjectModel.ObservableCollection<Ceitcon_Data.Model.LayerModel>() { layer };
                                slide.SelectedLayer = slide.Layers.FirstOrDefault();
                                var collection = Project.Regions[0].SlidesDownload;
                                collection.Add(slide);
                                Project.Regions[0].SlidesDownload = collection;
                                schedule.Files.Add(f);
                                bAdded = true;
                            }
                            else
                            {
                                sResult += f + " is invalid type";
                            }
                        }
                        if (bAdded)
                        {

                            string cdpfile = Ceitcon_Data.Utilities.IOManagerProject.ProjectToXML(Project, false, projectFile);
                            if (cdpfile != null && cdpfile.Trim() != string.Empty)
                            {
                                schedule.CDPFileContent = cdpfile;
                                HttpHelper.UploadScheduler(schedule);
                            }
                        }
                    }
                }
            }
        }

        private void UploadAll_Load(object sender, EventArgs e)
        {
            try
            {
                //string xmlPath = @"C:\CeitconDirectory\Project_37ddf7.cdp";
                //var Project = Ceitcon_Data.Utilities.IOManagerProject.LoadProject(xmlPath);
                //Ceitcon_Data.Model.SlideModel slide = new Ceitcon_Data.Model.SlideModel(Project.Regions[0]);
                //slide.Id = Guid.NewGuid().ToString();
                //slide.Name = "Slide_" + slide.Id.Substring(5);
                //slide.Duration = new TimeSpan(0, 0, 10);
                //slide.Forever = false;
                //slide.EnableSchedule = true;
                //slide.StartDate = DateTime.Now.Date;
                //slide.EndDate = DateTime.Now.Date.AddDays(7);
                //slide.StartTime = "07:00 AM";
                //slide.EndTime = "11:00 AM";
                //Ceitcon_Data.Model.LayerModel layer = new Ceitcon_Data.Model.LayerModel(slide);
                //layer.Id = Guid.NewGuid().ToString();
                //layer.Name = "Layer_" + layer.Id.Substring(5);
                //layer.ZIndex = 1;
                //layer.IsVisible = true;
                //layer.IsLocked = false;
                //Ceitcon_Data.Model.ControlModel control = new Ceitcon_Data.Model.ControlModel(Ceitcon_Data.Model.ControlType.Image, 0, 0, Project.Regions[0].Width, Project.Regions[0].Height, layer);
                //control.Id = Guid.NewGuid().ToString();
                //control.Name = "Image";
                //control.Opacity = 1;
                //control.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                //control.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                //control.Stretch = System.Windows.Media.Stretch.Fill;
                //control.HorizontalFlip = false;
                //control.VerticalFlip = false;
                //control.Rotate = 0;
                //control.FontSize = 0;
                ////control.FontWeight = 1;
                ////control.FontStyle = Fo
                ////control.TextDecoration = TextDecoration
                //control.InvertDirection = false;
                //control.Duration = new TimeSpan(0);
                //control.FlowDirection = false;
                //control.DateTimeFormat = 0;
                //control.ItemCount = 0;
                //control.Type = Ceitcon_Data.Model.ControlType.Image;
                //control.ZIndex = 1;
                //control.IsVisible = true;
                //control.IsLocked = false;
                //control.Playlist = new System.Collections.ObjectModel.ObservableCollection<Ceitcon_Data.Model.Playlist.PlaylistModel>();
                //Ceitcon_Data.Model.Playlist.SetContentModel playlist = new Ceitcon_Data.Model.Playlist.SetContentModel(control);
                //playlist.Id = Guid.NewGuid().ToString();
                //playlist.Name = "Playlist_" + playlist.Id.Substring(0, 6);
                //playlist.StartTime = new TimeSpan(0L);
                //playlist.Duration = new TimeSpan(0L);
                //playlist.Forever = true;
                //playlist.Type = Ceitcon_Data.Model.Playlist.PlaylistType.SetContent;
                //playlist.Content = "Fahad.JPG";
                //playlist.ContentSize = 1024;

                //control.Playlist.Add(playlist);
                //control.SelectedPlaylist = control.Playlist.FirstOrDefault();
                //layer.Controls = new System.Collections.ObjectModel.ObservableCollection<Ceitcon_Data.Model.ControlModel>() { control };
                //layer.SelectedControl = layer.Controls.FirstOrDefault();
                //slide.Layers = new System.Collections.ObjectModel.ObservableCollection<Ceitcon_Data.Model.LayerModel>() { layer };
                //slide.SelectedLayer = slide.Layers.FirstOrDefault();
                //var collection = Project.Regions[0].SlidesDownload;
                //collection.Add(slide);

                //Project.Regions[0].SlidesDownload = collection;

                //bool bSaved = Ceitcon_Data.Utilities.IOManagerProject.SaveProject(Project, xmlPath);


                GV_Schedules.DataSource = HttpHelper.GetAllSchedules();
            }
            catch (Exception Exp)
            {

            }
        }
    }
}
