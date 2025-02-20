using Ceitcon_Data.Model;
using Ceitcon_Data.Model.Playlist;
using Ceitcon_Data.Model.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Controls;
using log4net;
using System.Windows.Media.Effects;
using System.Text;

namespace Ceitcon_Data.Utilities
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public static partial class IOManagerProject
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string ProjectFileExtension = ".cdp";
        public static string ProjectFileFilter = "Project Files (*.cdp)|*.cdp";
        public static string ProjectDirectory;

        #region Save Project
        public static bool SaveProject(ProjectModel pm, string path)
        {
            bool result = false;
            try
            {
                logger.Info("Save Project started");
                string projectDir = System.IO.Path.GetDirectoryName(path);
                if (!Directory.Exists(projectDir))
                    Directory.CreateDirectory(projectDir);

                Dictionary<string, string> filelist = new Dictionary<string, string>();

                //Delete old files
                //try
                //{
                //    string[] list = GetContents(path);
                //    foreach (string fileName in Directory.GetFiles(Path.GetDirectoryName(path)))
                //    {
                //        if (Path.GetExtension(fileName) != ".cdp" && !list.Contains(Path.GetFileName(fileName)))
                //            File.Delete(fileName);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    logger.Error("Delete file", ex);
                //}
                var x_settings = new XmlWriterSettings();
                x_settings.NewLineChars = Environment.NewLine;
                x_settings.NewLineOnAttributes = true;
                x_settings.NewLineHandling = NewLineHandling.None;
                x_settings.CloseOutput = true;
                x_settings.Indent = true;

                using (var writer = XmlWriter.Create(path, x_settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Project");
                    writer.WriteElementString("Id", pm.Id);

                    writer.WriteStartElement("Information");
                    writer.WriteElementString("Name", pm.Information.ProjectName);
                    writer.WriteElementString("Creator", pm.Information.CreatorName);
                    writer.WriteElementString("MainContact", pm.Information.MainContact);
                    writer.WriteElementString("Owner", pm.Information.OwnerName);
                    writer.WriteElementString("Phone", pm.Information.Phone);
                    writer.WriteElementString("Interval", pm.Information.Interval.TotalSeconds.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("Resolution");
                    writer.WriteElementString("Id", pm.SelectedResolution.Id);
                    writer.WriteElementString("Name", pm.SelectedResolution.Name);
                    writer.WriteElementString("Width", pm.SelectedResolution.Width.ToString());
                    writer.WriteElementString("Height", pm.SelectedResolution.Height.ToString());
                    writer.WriteElementString("IsInitial", pm.SelectedResolution.IsInitial.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("Monitor");
                    writer.WriteElementString("Id", pm.SelectedMonitor.Id);
                    writer.WriteElementString("Name", pm.SelectedMonitor.Name);
                    writer.WriteElementString("Horizontal", pm.SelectedMonitor.Horizontal.ToString());
                    writer.WriteElementString("Vertical", pm.SelectedMonitor.Vertical.ToString());
                    writer.WriteElementString("IsInitial", pm.SelectedMonitor.IsInitial.ToString());
                    writer.WriteEndElement();

                    writer.WriteElementString("Orientation", pm.SelectedOrientation.ToString());

                    writer.WriteStartElement("Regions");
                    foreach (var region in pm.Regions)
                    {
                        writer.WriteStartElement("Region");
                        writer.WriteElementString("Id", region.Id);
                        writer.WriteElementString("Name", region.Name);
                        writer.WriteElementString("Width", region.Width.ToString());
                        writer.WriteElementString("Height", region.Height.ToString());
                        writer.WriteElementString("X", region.X.ToString());
                        writer.WriteElementString("Y", region.Y.ToString());

                        writer.WriteStartElement("Slides");
                        foreach (var slide in region.Slides)
                        {
                            writer.WriteStartElement("Slide");
                            writer.WriteElementString("Id", slide.Id);
                            writer.WriteElementString("Name", slide.Name);
                            writer.WriteElementString("Duration", slide.Duration.TotalSeconds.ToString());
                            writer.WriteElementString("Forever", slide.Forever.ToString());
                            writer.WriteElementString("EnableSchedule", slide.EnableSchedule.ToString());
                            writer.WriteElementString("StartDate", slide.StartDate.ToString());
                            writer.WriteElementString("EndDate", slide.EndDate.ToString());
                            writer.WriteElementString("StartTime", slide.StartTime.ToString());
                            writer.WriteElementString("EndTime", slide.EndTime.ToString());

                            writer.WriteStartElement("Layers");
                            foreach (var layer in slide.Layers)
                            {
                                writer.WriteStartElement("Layer");
                                writer.WriteElementString("Id", layer.Id);
                                writer.WriteElementString("Name", layer.Name);
                                writer.WriteElementString("ZIndex", layer.ZIndex.ToString());
                                writer.WriteElementString("IsVisible", layer.IsVisible.ToString());
                                writer.WriteElementString("IsLocked", layer.IsLocked.ToString());

                                writer.WriteStartElement("Controls");
                                foreach (var control in layer.Controls)
                                {
                                    writer.WriteStartElement("Control");
                                    writer.WriteAttributeString("Type", control.Type.ToString());
                                    writer.WriteElementString("Id", control.Id);
                                    writer.WriteElementString("Name", control.Name);
                                    writer.WriteElementString("Width", control.Width.ToString());
                                    writer.WriteElementString("Height", control.Height.ToString());
                                    writer.WriteElementString("X", control.X.ToString());
                                    writer.WriteElementString("Y", control.Y.ToString());
                                    writer.WriteElementString("Margion", new Thickness(control.X, control.Y, control.W, control.Z).ToString());
                                    writer.WriteElementString("Opacity", control.Opacity.ToString());
                                    writer.WriteStartElement("Background");
                                    writer.WriteRaw(BrushToXML(control.Background));
                                    writer.WriteEndElement();
                                    writer.WriteStartElement("BorderBrush");
                                    writer.WriteRaw(BrushToXML(control.BorderBrush));
                                    writer.WriteEndElement();
                                    writer.WriteElementString("BorderThickness", control.BorderThickness.ToString());
                                    writer.WriteElementString("CornerRadius", control.CornerRadius.ToString());
                                    writer.WriteElementString("HorizontalAlignment", control.HorizontalAlignment.ToString());
                                    writer.WriteElementString("VerticalAlignment", control.VerticalAlignment.ToString());
                                    writer.WriteElementString("Stretch", control.Stretch.ToString());
                                    writer.WriteElementString("HorizontalFlip", control.HorizontalFlip.ToString());
                                    writer.WriteElementString("VerticalFlip", control.VerticalFlip.ToString());
                                    writer.WriteElementString("Rotate", control.Rotate.ToString());
                                    if (control.FontFamily != null) writer.WriteElementString("FontFamily", control.FontFamily.ToString());
                                    if (control.FontSize != null) writer.WriteElementString("FontSize", control.FontSize.ToString());
                                    if (control.Foreground != null)
                                    {
                                        writer.WriteStartElement("Foreground");
                                        writer.WriteRaw(BrushToXML(control.Foreground));
                                        writer.WriteEndElement();
                                    }
                                    if (control.Url != null) writer.WriteElementString("Url", control.Url.ToString());
                                    if (control.FontWeight != null) writer.WriteElementString("FontWeight", control.FontWeight.ToString());
                                    if (control.FontStyle != null) writer.WriteElementString("FontStyle", control.FontStyle.ToString());
                                    if (control.TextDecorationText != null) writer.WriteElementString("TextDecorationText", control.TextDecorationText);
                                    if (control.InvertDirection != null) writer.WriteElementString("InvertDirection", control.InvertDirection.ToString());
                                    if (control.Duration != null) writer.WriteElementString("Duration", control.Duration.TotalSeconds.ToString());
                                    writer.WriteElementString("Type", control.Type.ToString());
                                    if (control.SelectedSocialMedia != null) writer.WriteElementString("SelectedSocialMedia", control.SelectedSocialMedia.Name);
                                    writer.WriteElementString("MediaAccountId", control.MediaAccountId);
                                    writer.WriteElementString("MediaPageName", control.MediaPageName);
                                    writer.WriteElementString("FlowDirection", control.FlowDirection.ToString());
                                    writer.WriteElementString("DateTimeFormat", control.DateTimeFormat.ToString());
                                    writer.WriteElementString("CustomDateTimeFormat", control.CustomDateTimeFormat);
                                    writer.WriteElementString("ItemCount", control.ItemCount.ToString());
                                    writer.WriteElementString("Type", control.Type.ToString());
                                    writer.WriteElementString("ZIndex", control.ZIndex.ToString());
                                    writer.WriteElementString("IsVisible", control.IsVisible.ToString());
                                    writer.WriteElementString("IsLocked", control.IsLocked.ToString());
                                    writer.WriteStartElement("Playlists");
                                    foreach (var playlist in control.Playlist)
                                    {
                                        writer.WriteStartElement("Playlist");
                                        writer.WriteElementString("Id", playlist.Id);
                                        writer.WriteElementString("Name", playlist.Name);
                                        writer.WriteElementString("StartTime", playlist.StartTime.TotalSeconds.ToString());
                                        writer.WriteElementString("Duration", playlist.Duration.TotalSeconds.ToString());
                                        writer.WriteElementString("Forever", playlist.Forever.ToString());
                                        if (playlist.Depends != null) writer.WriteElementString("Depends", playlist.Depends.Id);
                                        writer.WriteElementString("Type", playlist.Type.ToString());
                                        switch (playlist.Type)
                                        {
                                            case PlaylistType.SetContent:
                                                if (control.Type == ControlType.Image
                                                    || control.Type == ControlType.Video
                                                    || control.Type == ControlType.GifAnim
                                                    || control.Type == ControlType.PDF
                                                    || control.Type == ControlType.PPT)
                                                {
                                                    var scm = playlist as SetContentModel;
                                                    string output = String.Empty;
                                                    string res = StoreSource("Content", true, scm.Content, scm.ContentSize, filelist, projectDir, out output);
                                                    writer.WriteRaw(res);
                                                    if (!String.IsNullOrEmpty(output))
                                                        scm.Content = output;

                                                    if (control.Type == ControlType.Video)
                                                        writer.WriteElementString("IsMuted", scm.IsMuted.ToString());

                                                    if (control.Type == ControlType.PDF
                                                        || control.Type == ControlType.PPT)
                                                    {
                                                        writer.WriteElementString("PageDuration", scm.PageDuration.ToString());
                                                        writer.WriteElementString("DocumentFit", scm.DocumentFit);
                                                    }
                                                }
                                                else
                                                {
                                                    var scm = playlist as SetContentModel;
                                                    writer.WriteElementString("Content", scm.Content == null ? null : (playlist as SetContentModel).Content);
                                                    writer.WriteElementString("ContentSize", scm.ContentSize.ToString());
                                                    if (control.Type == ControlType.Video)
                                                        writer.WriteElementString("IsMuted", scm.IsMuted.ToString());
                                                }


                                                if (control.Type == ControlType.DataGrid && (playlist as SetContentModel).DataGrid != null)
                                                {
                                                    var dg = (playlist as SetContentModel).DataGrid;
                                                    writer.WriteStartElement("DataGrid");
                                                    writer.WriteElementString("Id", dg.Id);

                                                    writer.WriteStartElement("RowBackground");
                                                    writer.WriteRaw(BrushToXML(dg.RowBackground));
                                                    writer.WriteEndElement();

                                                    writer.WriteStartElement("BorderBrush");
                                                    writer.WriteRaw(BrushToXML(dg.BorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("BorderThickness", dg.BorderThickness.ToString());
                                                    writer.WriteElementString("RowCornerRadius", dg.RowCornerRadius.ToString());
                                                    writer.WriteElementString("RowMargin", dg.RowMargin.ToString());
                                                    writer.WriteElementString("LinesVisibility", dg.LinesVisibility.ToString());

                                                    writer.WriteStartElement("VerticalLineColour");
                                                    writer.WriteRaw(BrushToXML(dg.VerticalLineColour));
                                                    writer.WriteEndElement();

                                                    writer.WriteStartElement("HorizontalLineColour");
                                                    writer.WriteRaw(BrushToXML(dg.HorizontalLineColour));
                                                    writer.WriteEndElement();

                                                    writer.WriteElementString("AlternationCount", dg.AlternationCount.ToString());

                                                    writer.WriteStartElement("AlternatingRowBackground");
                                                    writer.WriteRaw(BrushToXML(dg.AlternatingRowBackground));
                                                    writer.WriteEndElement();

                                                    writer.WriteElementString("IsVisibleShadow", dg.IsVisibleShadow.ToString());
                                                    writer.WriteElementString("RowShadowEffect", (dg.RowShadowEffect == null) ? null : DropShadowEffectToXML(dg.RowShadowEffect));
                                                    writer.WriteElementString("HeaderSize", dg.HeaderSize.ToString());
                                                    writer.WriteElementString("HeaderHeight", dg.HeaderHeight.ToString());
                                                    writer.WriteStartElement("HeaderBackground");
                                                    writer.WriteRaw(BrushToXML(dg.HeaderBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeaderForeground");
                                                    writer.WriteRaw(BrushToXML(dg.HeaderForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeaderBorderBrush");
                                                    writer.WriteRaw(BrushToXML(dg.HeaderBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("HeaderBorderThickness", dg.HeaderBorderThickness.ToString());
                                                    writer.WriteElementString("HeaderCornerRadius", dg.HeaderCornerRadius.ToString());
                                                    writer.WriteElementString("HeaderMargin", dg.HeaderMargin.ToString());
                                                    if (dg.HeaderFontFamily != null) writer.WriteElementString("HeaderFontFamily", dg.HeaderFontFamily.ToString());
                                                    if (dg.HeaderFontWeight != null) writer.WriteElementString("HeaderFontWeight", dg.HeaderFontWeight.ToString());
                                                    if (dg.HeaderFontStyle != null) writer.WriteElementString("HeaderFontStyle", dg.HeaderFontStyle.ToString());
                                                    writer.WriteElementString("HeaderHorizontalAlignment", dg.HeaderHorizontalAlignment.ToString());
                                                    writer.WriteElementString("HeaderVerticalAlignment", dg.HeaderVerticalAlignment.ToString());
                                                    writer.WriteElementString("HeaderIsVisibleShadow", dg.HeaderIsVisibleShadow.ToString());
                                                    writer.WriteElementString("HeaderShadowEffect", (dg.HeaderShadowEffect == null) ? null : DropShadowEffectToXML(dg.HeaderShadowEffect));
                                                    writer.WriteElementString("MaxRows", dg.MaxRows.ToString());
                                                    writer.WriteElementString("RowHeight", dg.RowHeight.ToString());
                                                    writer.WriteElementString("RefreshTime", dg.RefreshTime.ToString());
                                                    if (dg.SelectedSource != null)
                                                    {
                                                        writer.WriteStartElement("Source");
                                                        writer.WriteElementString("Id", dg.SelectedSource.Id);
                                                        writer.WriteElementString("Name", dg.SelectedSource.Name);
                                                        writer.WriteStartElement("Columns");
                                                        foreach (var column in dg.SelectedSource.Columns)
                                                        {
                                                            writer.WriteStartElement("Column");
                                                            writer.WriteElementString("Id", column.Id);
                                                            writer.WriteElementString("Name", column.Name);
                                                            writer.WriteElementString("Title", column.Title);
                                                            writer.WriteElementString("Width", column.Width.ToString());
                                                            writer.WriteElementString("Type", column.Type.ToString());
                                                            writer.WriteStartElement("Background");
                                                            writer.WriteRaw(BrushToXML(column.Background));
                                                            writer.WriteEndElement();
                                                            writer.WriteStartElement("Foreground");
                                                            writer.WriteRaw(BrushToXML(column.Foreground));
                                                            writer.WriteEndElement();
                                                            writer.WriteElementString("TextAlignment", column.TextAlignment.ToString());
                                                            writer.WriteElementString("VerticalAlignment", column.VerticalAlignment.ToString());
                                                            if (column.FontFamily != null) writer.WriteElementString("FontFamily", column.FontFamily.ToString());
                                                            if (column.FontSize != null) writer.WriteElementString("FontSize", column.FontSize.ToString());
                                                            if (column.FontWeight != null) writer.WriteElementString("FontWeight", column.FontWeight.ToString());
                                                            if (column.FontStyle != null) writer.WriteElementString("FontStyle", column.FontStyle.ToString());
                                                            writer.WriteElementString("WhereOperator", column.WhereOperator);
                                                            writer.WriteElementString("WhereValue", column.WhereValue);
                                                            writer.WriteElementString("MergeColumn", column.MergeColumn);
                                                            writer.WriteElementString("Sort", column.Sort.ToString());
                                                            writer.WriteElementString("ImageStretch", column.ImageStretch.ToString());
                                                            writer.WriteElementString("IsVisible", column.IsVisible.ToString());
                                                            writer.WriteStartElement("SpecialCells");
                                                            foreach (var specialCell in column.SpecialCells)
                                                            {
                                                                writer.WriteStartElement("SpecialCell");
                                                                writer.WriteElementString("Id", specialCell.Id);
                                                                writer.WriteElementString("Text", specialCell.Text);
                                                                writer.WriteElementString("IsRow", specialCell.IsRow.ToString());
                                                                writer.WriteElementString("IsBlink", specialCell.IsBlink.ToString());
                                                                if (specialCell.Foreground != null)
                                                                {
                                                                    writer.WriteStartElement("Foreground");
                                                                    writer.WriteRaw(BrushToXML(specialCell.Foreground));
                                                                    writer.WriteEndElement();
                                                                }

                                                                if (specialCell.Foreground != null)
                                                                {
                                                                    writer.WriteStartElement("Background");
                                                                    writer.WriteRaw(BrushToXML(specialCell.Background));
                                                                    writer.WriteEndElement();
                                                                }
                                                                writer.WriteEndElement();
                                                            }
                                                            writer.WriteEndElement();

                                                            writer.WriteStartElement("TimeFilters");
                                                            foreach (var timeFilter in column.TimeFilters)
                                                            {
                                                                writer.WriteStartElement("TimeFilter");
                                                                writer.WriteElementString("Id", timeFilter.Id);
                                                                writer.WriteElementString("Name", timeFilter.Name);
                                                                writer.WriteElementString("BeforeDuration", timeFilter.BeforeDuration.ToString());
                                                                writer.WriteElementString("AfterDuration", timeFilter.AfterDuration.ToString());
                                                                writer.WriteEndElement();
                                                            }
                                                            writer.WriteEndElement();

                                                            writer.WriteEndElement();
                                                        }
                                                        writer.WriteEndElement();
                                                        writer.WriteEndElement();
                                                    }
                                                    writer.WriteEndElement();
                                                }
                                                else if (control.Type == ControlType.Weather && (playlist as SetContentModel).Weather != null)
                                                {
                                                    var w = (playlist as SetContentModel).Weather;
                                                    writer.WriteStartElement("Weather");
                                                    writer.WriteElementString("Id", w.Id);
                                                    writer.WriteElementString("TitleIsVisible", w.TitleIsVisible.ToString());
                                                    writer.WriteElementString("TitleSize", w.TitleSize.ToString());
                                                    writer.WriteStartElement("TitleBackground");
                                                    writer.WriteRaw(BrushToXML(w.TitleBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("TitleForeground");
                                                    writer.WriteRaw(BrushToXML(w.TitleForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("TitleBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.TitleBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("TitleBorderThickness", w.TitleBorderThickness.ToString());
                                                    if (w.TitleFontFamily != null) writer.WriteElementString("TitleFontFamily", w.TitleFontFamily.ToString());
                                                    if (w.TitleFontWeight != null) writer.WriteElementString("TitleFontWeight", w.TitleFontWeight.ToString());
                                                    if (w.TitleFontStyle != null) writer.WriteElementString("TitleFontStyle", w.TitleFontStyle.ToString());
                                                    writer.WriteElementString("TitleCornerRadius", w.TitleCornerRadius.ToString());
                                                    writer.WriteElementString("TitleText", w.TitleText);

                                                    writer.WriteElementString("DayTextIsVisible", w.DayTextIsVisible.ToString());
                                                    writer.WriteElementString("DayTextSize", w.DayTextSize.ToString());
                                                    writer.WriteStartElement("DayTextBackground");
                                                    writer.WriteRaw(BrushToXML(w.DayTextBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("DayTextForeground");
                                                    writer.WriteRaw(BrushToXML(w.DayTextForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("DayTextBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.DayTextBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("DayTextBorderThickness", w.DayTextBorderThickness.ToString());
                                                    if (w.DayTextFontFamily != null) writer.WriteElementString("DayTextFontFamily", w.DayTextFontFamily.ToString());
                                                    if (w.DayTextFontWeight != null) writer.WriteElementString("DayTextFontWeight", w.DayTextFontWeight.ToString());
                                                    if (w.DayTextFontStyle != null) writer.WriteElementString("DayTextFontStyle", w.DayTextFontStyle.ToString());
                                                    writer.WriteElementString("DayTextCornerRadius", w.DayTextCornerRadius.ToString());

                                                    writer.WriteElementString("WeatherTextIsVisible", w.WeatherTextIsVisible.ToString());
                                                    writer.WriteElementString("WeatherTextSize", w.WeatherTextSize.ToString());
                                                    writer.WriteStartElement("WeatherTextBackground");
                                                    writer.WriteRaw(BrushToXML(w.WeatherTextBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("WeatherTextForeground");
                                                    writer.WriteRaw(BrushToXML(w.WeatherTextForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("WeatherTextBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.WeatherTextBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("WeatherTextBorderThickness", w.WeatherTextBorderThickness.ToString());
                                                    if (w.WeatherTextFontFamily != null) writer.WriteElementString("WeatherTextFontFamily", w.WeatherTextFontFamily.ToString());
                                                    if (w.WeatherTextFontWeight != null) writer.WriteElementString("WeatherTextFontWeight", w.WeatherTextFontWeight.ToString());
                                                    if (w.WeatherTextFontStyle != null) writer.WriteElementString("WeatherTextFontStyle", w.WeatherTextFontStyle.ToString());
                                                    writer.WriteElementString("WeatherTextCornerRadius", w.WeatherTextCornerRadius.ToString());

                                                    writer.WriteElementString("HeightTextIsVisible", w.HeightTextIsVisible.ToString());
                                                    writer.WriteElementString("HeightTextSize", w.HeightTextSize.ToString());
                                                    writer.WriteStartElement("HeightTextBackground");
                                                    writer.WriteRaw(BrushToXML(w.HeightTextBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeightTextForeground");
                                                    writer.WriteRaw(BrushToXML(w.HeightTextForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeightTextBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.HeightTextBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("HeightTextBorderThickness", w.HeightTextBorderThickness.ToString());
                                                    if (w.HeightTextFontFamily != null) writer.WriteElementString("HeightTextFontFamily", w.HeightTextFontFamily.ToString());
                                                    if (w.HeightTextFontWeight != null) writer.WriteElementString("HeightTextFontWeight", w.HeightTextFontWeight.ToString());
                                                    if (w.HeightTextFontStyle != null) writer.WriteElementString("HeightTextFontStyle", w.HeightTextFontStyle.ToString());
                                                    writer.WriteElementString("HeightTextCornerRadius", w.HeightTextCornerRadius.ToString());

                                                    writer.WriteElementString("HeightValueIsVisible", w.HeightValueIsVisible.ToString());
                                                    writer.WriteElementString("HeightValueSize", w.HeightValueSize.ToString());
                                                    writer.WriteStartElement("HeightValueBackground");
                                                    writer.WriteRaw(BrushToXML(w.HeightValueBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeightValueForeground");
                                                    writer.WriteRaw(BrushToXML(w.HeightValueForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeightValueBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.HeightValueBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("HeightValueBorderThickness", w.HeightValueBorderThickness.ToString());
                                                    if (w.HeightValueFontFamily != null) writer.WriteElementString("HeightValueFontFamily", w.HeightValueFontFamily.ToString());
                                                    if (w.HeightValueFontWeight != null) writer.WriteElementString("HeightValueFontWeight", w.HeightValueFontWeight.ToString());
                                                    if (w.HeightValueFontStyle != null) writer.WriteElementString("HeightValueFontStyle", w.HeightValueFontStyle.ToString());
                                                    writer.WriteElementString("HeightValueCornerRadius", w.HeightValueCornerRadius.ToString());

                                                    writer.WriteElementString("LowTextIsVisible", w.LowTextIsVisible.ToString());
                                                    writer.WriteElementString("LowTextSize", w.LowTextSize.ToString());
                                                    writer.WriteStartElement("LowTextBackground");
                                                    writer.WriteRaw(BrushToXML(w.LowTextBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("LowTextForeground");
                                                    writer.WriteRaw(BrushToXML(w.LowTextForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("LowTextBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.LowTextBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("LowTextBorderThickness", w.LowTextBorderThickness.ToString());
                                                    if (w.LowTextFontFamily != null) writer.WriteElementString("LowTextFontFamily", w.LowTextFontFamily.ToString());
                                                    if (w.LowTextFontWeight != null) writer.WriteElementString("LowTextFontWeight", w.LowTextFontWeight.ToString());
                                                    if (w.LowTextFontStyle != null) writer.WriteElementString("LowTextFontStyle", w.LowTextFontStyle.ToString());
                                                    writer.WriteElementString("LowTextCornerRadius", w.LowTextCornerRadius.ToString());

                                                    writer.WriteElementString("LowValueIsVisible", w.LowValueIsVisible.ToString());
                                                    writer.WriteElementString("LowValueSize", w.LowValueSize.ToString());
                                                    writer.WriteStartElement("LowValueBackground");
                                                    writer.WriteRaw(BrushToXML(w.LowValueBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("LowValueForeground");
                                                    writer.WriteRaw(BrushToXML(w.LowValueForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("LowValueBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.LowValueBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("LowValueBorderThickness", w.LowValueBorderThickness.ToString());
                                                    if (w.LowValueFontFamily != null) writer.WriteElementString("LowValueFontFamily", w.LowValueFontFamily.ToString());
                                                    if (w.LowValueFontWeight != null) writer.WriteElementString("LowValueFontWeight", w.LowValueFontWeight.ToString());
                                                    if (w.LowValueFontStyle != null) writer.WriteElementString("LowValueFontStyle", w.LowValueFontStyle.ToString());
                                                    writer.WriteElementString("LowValueCornerRadius", w.LowValueCornerRadius.ToString());

                                                    writer.WriteEndElement();
                                                }
                                                break;
                                            case PlaylistType.Delay:
                                                break;
                                            case PlaylistType.AnimateBorder:
                                                writer.WriteElementString("BorderFrom", (playlist as AnimateBorderModel).BorderThicknessFrom.ToString());
                                                writer.WriteElementString("BorderTo", (playlist as AnimateBorderModel).BorderThicknessTo.ToString());
                                                break;
                                            case PlaylistType.AnimateHeight:
                                                writer.WriteElementString("HeightFrom", (playlist as AnimateHeightModel).HeightFrom.ToString());
                                                writer.WriteElementString("HeightTo", (playlist as AnimateHeightModel).HeightTo.ToString());
                                                break;
                                            case PlaylistType.AnimateMargin:
                                                writer.WriteElementString("MarginFrom", (playlist as AnimateMarginModel).MarginThicknessFrom.ToString());
                                                writer.WriteElementString("MarginTo", (playlist as AnimateMarginModel).MarginThicknessTo.ToString());
                                                break;
                                            case PlaylistType.AnimateOpacity:
                                                writer.WriteElementString("OpacityFrom", (playlist as AnimateOpacityModel).OpacityFrom.ToString());
                                                writer.WriteElementString("OpacityTo", (playlist as AnimateOpacityModel).OpacityTo.ToString());
                                                break;
                                            case PlaylistType.AnimateWidth:
                                                writer.WriteElementString("WidthFrom", (playlist as AnimateWidthModel).WidthFrom.ToString());
                                                writer.WriteElementString("WidthTo", (playlist as AnimateWidthModel).WidthTo.ToString());
                                                break;
                                            case PlaylistType.ResumePlayback:
                                                break;
                                            case PlaylistType.SuspendPlayback:
                                                break;
                                        }

                                        writer.WriteEndElement();
                                    }
                                    writer.WriteEndElement();
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    //Alarm
                    writer.WriteStartElement("Alerts");
                    foreach (var alert in pm.Alerts)
                    {
                        writer.WriteStartElement("Alert");
                        writer.WriteElementString("Id", alert.Id);
                        writer.WriteElementString("Name", alert.Name);
                        writer.WriteElementString("Type", alert.Type.ToString());

                        writer.WriteStartElement("Controls");
                        foreach (var control in alert.Controls)
                        {
                            writer.WriteStartElement("Control");
                            writer.WriteAttributeString("Type", control.Type.ToString());
                            writer.WriteElementString("Id", control.Id);
                            writer.WriteElementString("Name", control.Name);
                            writer.WriteElementString("Width", control.Width.ToString());
                            writer.WriteElementString("Height", control.Height.ToString());
                            writer.WriteElementString("X", control.X.ToString());
                            writer.WriteElementString("Y", control.Y.ToString());
                            writer.WriteElementString("Margion", new Thickness(control.X, control.Y, control.W, control.Z).ToString());
                            writer.WriteElementString("Opacity", control.Opacity.ToString());
                            writer.WriteStartElement("Background");
                            writer.WriteRaw(BrushToXML(control.Background));
                            writer.WriteEndElement();
                            writer.WriteStartElement("BorderBrush");
                            writer.WriteRaw(BrushToXML(control.BorderBrush));
                            writer.WriteEndElement();
                            writer.WriteElementString("BorderThickness", control.BorderThickness.ToString());
                            writer.WriteElementString("CornerRadius", control.CornerRadius.ToString());
                            writer.WriteElementString("HorizontalAlignment", control.HorizontalAlignment.ToString());
                            writer.WriteElementString("VerticalAlignment", control.VerticalAlignment.ToString());
                            writer.WriteElementString("Stretch", control.Stretch.ToString());
                            writer.WriteElementString("HorizontalFlip", control.HorizontalFlip.ToString());
                            writer.WriteElementString("VerticalFlip", control.VerticalFlip.ToString());
                            writer.WriteElementString("Rotate", control.Rotate.ToString());
                            if (control.FontFamily != null) writer.WriteElementString("FontFamily", control.FontFamily.ToString());
                            if (control.FontSize != null) writer.WriteElementString("FontSize", control.FontSize.ToString());
                            if (control.Foreground != null)
                            {
                                writer.WriteStartElement("Foreground");
                                writer.WriteRaw(BrushToXML(control.Foreground));
                                writer.WriteEndElement();
                            }
                            if (control.Url != null) writer.WriteElementString("Url", control.Url.ToString());
                            if (control.FontWeight != null) writer.WriteElementString("FontWeight", control.FontWeight.ToString());
                            if (control.FontStyle != null) writer.WriteElementString("FontStyle", control.FontStyle.ToString());
                            if (control.TextDecorationText != null) writer.WriteElementString("TextDecorationText", control.TextDecorationText);
                            if (control.InvertDirection != null) writer.WriteElementString("InvertDirection", control.InvertDirection.ToString());
                            if (control.Duration != null) writer.WriteElementString("Duration", control.Duration.TotalSeconds.ToString());
                            writer.WriteElementString("Type", control.Type.ToString());
                            if (control.SelectedSocialMedia != null) writer.WriteElementString("SelectedSocialMedia", control.SelectedSocialMedia.Name);
                            writer.WriteElementString("MediaAccountId", control.MediaAccountId);
                            writer.WriteElementString("MediaPageName", control.MediaPageName);
                            writer.WriteElementString("Type", control.Type.ToString());
                            writer.WriteElementString("ZIndex", control.ZIndex.ToString());
                            writer.WriteElementString("IsVisible", control.IsVisible.ToString());
                            writer.WriteElementString("IsLocked", control.IsLocked.ToString());
                            writer.WriteStartElement("Playlists");
                            foreach (var playlist in control.Playlist)
                            {
                                writer.WriteStartElement("Playlist");
                                writer.WriteElementString("Id", playlist.Id);
                                writer.WriteElementString("Name", playlist.Name);
                                writer.WriteElementString("StartTime", playlist.StartTime.TotalSeconds.ToString());
                                writer.WriteElementString("Duration", playlist.Duration.TotalSeconds.ToString());
                                writer.WriteElementString("Forever", playlist.Forever.ToString());
                                if (playlist.Depends != null) writer.WriteElementString("Depends", playlist.Depends.Id);
                                writer.WriteElementString("Type", playlist.Type.ToString());
                                switch (playlist.Type)
                                {
                                    case PlaylistType.SetContent:
                                        if (control.Type == ControlType.PrayerText)
                                        {
                                            var scm = playlist as SetContentModel;
                                            writer.WriteElementString("Fajr", scm.Fajr == null ? null : scm.Fajr);
                                            writer.WriteElementString("Dhuhr", scm.Fajr == null ? null : scm.Dhuhr);
                                            writer.WriteElementString("Asr", scm.Fajr == null ? null : scm.Asr);
                                            writer.WriteElementString("Maghrib", scm.Fajr == null ? null : scm.Maghrib);
                                            writer.WriteElementString("Isha", scm.Fajr == null ? null : scm.Isha);
                                        }
                                        else if (control.Type == ControlType.PrayerImage || control.Type == ControlType.PrayerVideo)
                                        {
                                            var scm = playlist as SetContentModel;
                                            logger.Info($"Arif Fajr {scm.Fajr} Size : {scm.FajrSize}");
                                            string output = String.Empty;
                                            writer.WriteRaw(StoreSource("Fajr", true, scm.Fajr, scm.FajrSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Fajr = output;



                                            //var scm = playlist as SetContentModel;
                                            //string output = String.Empty;
                                            //string res = StoreSource("Content", true, scm.Content, scm.ContentSize, filelist, projectDir, out output);
                                            //writer.WriteRaw(res);
                                            //if (!String.IsNullOrEmpty(output))
                                            //    scm.Content = output;

                                            output = String.Empty;
                                            logger.Info($"Arif Fajr {scm.DhuhrSize} Size : {scm.DhuhrSize}");
                                            writer.WriteRaw(StoreSource("Dhuhr", true, scm.Dhuhr, scm.DhuhrSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Dhuhr = output;

                                            output = String.Empty;
                                            logger.Info($"Arif Asr {scm.Asr} Size : {scm.AsrSize}");
                                            writer.WriteRaw(StoreSource("Asr", true, scm.Asr, scm.AsrSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Asr = output;

                                            output = String.Empty;
                                            logger.Info($"Arif Maghrib {scm.Maghrib} Size : {scm.MaghribSize}");
                                            writer.WriteRaw(StoreSource("Maghrib", true, scm.Maghrib, scm.MaghribSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Maghrib = output;

                                            output = String.Empty;
                                            logger.Info($"Arif Isha {scm.Isha} Size : {scm.IshaSize}");
                                            writer.WriteRaw(StoreSource("Isha", true, scm.Isha, scm.IshaSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Isha = output;
                                        }
                                        else if (control.Type == ControlType.Image || control.Type == ControlType.Video)
                                        {
                                            string output = String.Empty;
                                            var scm = playlist as SetContentModel;
                                            writer.WriteRaw(StoreSource("Content", true, scm.Content, scm.ContentSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Content = output;

                                            if (control.Type == ControlType.Video)
                                                writer.WriteElementString("IsMuted", scm.IsMuted.ToString());
                                        }
                                        else
                                        {
                                            var scm = playlist as SetContentModel;
                                            writer.WriteElementString("Content", scm.Content == null ? null : (playlist as SetContentModel).Content);
                                            writer.WriteElementString("ContentSize", scm.ContentSize.ToString());
                                            writer.WriteElementString("IsMuted", scm.IsMuted.ToString());
                                        }
                                        break;
                                    case PlaylistType.Delay:
                                        break;
                                    case PlaylistType.AnimateBorder:
                                        writer.WriteElementString("BorderFrom", (playlist as AnimateBorderModel).BorderThicknessFrom.ToString());
                                        writer.WriteElementString("BorderTo", (playlist as AnimateBorderModel).BorderThicknessTo.ToString());
                                        break;
                                    case PlaylistType.AnimateHeight:
                                        writer.WriteElementString("HeightFrom", (playlist as AnimateHeightModel).HeightFrom.ToString());
                                        writer.WriteElementString("HeightTo", (playlist as AnimateHeightModel).HeightTo.ToString());
                                        break;
                                    case PlaylistType.AnimateMargin:
                                        writer.WriteElementString("MarginFrom", (playlist as AnimateMarginModel).MarginThicknessFrom.ToString());
                                        writer.WriteElementString("MarginTo", (playlist as AnimateMarginModel).MarginThicknessTo.ToString());
                                        break;
                                    case PlaylistType.AnimateOpacity:
                                        writer.WriteElementString("OpacityFrom", (playlist as AnimateOpacityModel).OpacityFrom.ToString());
                                        writer.WriteElementString("OpacityTo", (playlist as AnimateOpacityModel).OpacityTo.ToString());
                                        break;
                                    case PlaylistType.AnimateWidth:
                                        writer.WriteElementString("WidthFrom", (playlist as AnimateWidthModel).WidthFrom.ToString());
                                        writer.WriteElementString("WidthTo", (playlist as AnimateWidthModel).WidthTo.ToString());
                                        break;
                                    case PlaylistType.ResumePlayback:
                                        break;
                                    case PlaylistType.SuspendPlayback:
                                        break;
                                }

                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("Slots");
                        foreach (var slot in alert.Slots)
                        {
                            writer.WriteStartElement("Slot");
                            writer.WriteElementString("Id", slot.Id);
                            writer.WriteElementString("Name", slot.Name);
                            writer.WriteElementString("StartTime", slot.Start.Ticks.ToString());
                            writer.WriteStartElement("Location");
                            writer.WriteElementString("Id", slot.Location.Id);
                            writer.WriteElementString("Country", slot.Location.Country);
                            writer.WriteElementString("City", slot.Location.City);
                            writer.WriteElementString("Latitude", slot.Location.Latitude.ToString());
                            writer.WriteElementString("Longnitude", slot.Location.Longnitude.ToString());
                            writer.WriteEndElement();
                            writer.WriteElementString("Duration", slot.Duration.TotalSeconds.ToString());
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                //try
                //{
                //    foreach (string fileName in Directory.GetFiles(Path.GetDirectoryName(path)))
                //    {
                //        if (Path.GetExtension(fileName) != ".cdp" && !filelist.ContainsValue(Path.GetFileName(fileName)))
                //            File.Delete(fileName);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    logger.Error("Delete file", ex);
                //}

                //Crypt text
                XElement text = XElement.Load(path);
                File.Delete(path);
                string cryptedText = Crypt.Encrypt(text.ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
                System.IO.File.WriteAllText(path, cryptedText);
                result = true;
            }
            catch (Exception e)
            {
                logger.Error("Problem with saving project", e);
                return false;
            }
            logger.Info("Project is saved");
            return result;
        }

        public static string DropShadowEffectToXML(DropShadowEffect shedow)
        {
            string result = String.Empty;

            using (var sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                using (var writer = XmlWriter.Create(sw, settings))
                {
                    writer.WriteStartElement("DropShadowEffect");
                    writer.WriteElementString("BlurRadius", shedow.BlurRadius.ToString());
                    writer.WriteElementString("Opacity", shedow.Opacity.ToString());
                    writer.WriteElementString("Color", shedow.Color.ToString());
                    writer.WriteElementString("ShadowDepth", shedow.ShadowDepth.ToString());
                    writer.WriteElementString("Direction", shedow.Direction.ToString());
                    writer.WriteEndElement();
                }
                result = sw.ToString();

                return result;
            }
        }

        public static string StoreSource(string name, bool sizeCheck, string content, long contentSize, Dictionary<string, string> filelist, string projectDir, out string output)
        {
            using (var sw = new StringWriter())
            {
                string result = String.Empty;
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                using (var writer = XmlWriter.Create(sw, settings))
                {
                    if (String.IsNullOrWhiteSpace(content) || !File.Exists(content))
                    {
                        writer.WriteElementString(name, null);
                        if (sizeCheck)
                            writer.WriteElementString($"{name}Size", null);

                        output = String.Empty;
                        result = sw.ToString();
                    }
                    else if (filelist.ContainsKey(content))
                    {
                        writer.WriteElementString(name, filelist[content]);
                        output = System.IO.Path.Combine(projectDir, filelist[content]);
                        result = sw.ToString();
                    }
                    else
                    {
                        //check if file exist in project folder
                        string fileName = String.Empty;
                        string fullPath = String.Empty;
                        if (Path.GetDirectoryName(content) == projectDir)
                        {
                            fileName = Path.GetFileName(content);
                            fullPath = content;
                        }
                        else
                        {
                            //Copy to project folder;
                            fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(content);
                            fullPath = System.IO.Path.Combine(projectDir, fileName);
                            if (!File.Exists(fullPath) || (sizeCheck && contentSize > 0 && contentSize != (new System.IO.FileInfo(fullPath).Length)))
                                File.Copy(content, fullPath, true);
                        }

                        writer.WriteElementString(name, fileName);
                        if (sizeCheck) writer.WriteElementString($"{name}Size", contentSize.ToString());
                        filelist.Add(content, fileName);
                        output = fullPath;
                        result = sw.ToString();
                    }
                }
                return sw.ToString();
            }
        }

        public static string BrushToXML(Brush brush)
        {
            string result = String.Empty;
            if (brush == null)
            {

            }
            else if (brush is LinearGradientBrush)
            {
                using (var sw = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = true;
                    using (var writer = XmlWriter.Create(sw, settings))
                    {
                        writer.WriteStartElement("LinearGradientBrush");
                        //writer.WriteElementString("Type", "LinearGradientBrush");
                        writer.WriteStartElement("StartPoint");
                        writer.WriteElementString("X", (brush as LinearGradientBrush).StartPoint.X.ToString());
                        writer.WriteElementString("Y", (brush as LinearGradientBrush).StartPoint.Y.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("EndPoint");
                        writer.WriteElementString("X", (brush as LinearGradientBrush).EndPoint.X.ToString());
                        writer.WriteElementString("Y", (brush as LinearGradientBrush).EndPoint.Y.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("GradientStops");
                        foreach (var gs in (brush as LinearGradientBrush).GradientStops)
                        {
                            writer.WriteStartElement("GradientStop");
                            writer.WriteElementString("Offset", gs.Offset.ToString());
                            writer.WriteElementString("Color", gs.Color.ToString());
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteElementString("MappingMode", (brush as LinearGradientBrush).MappingMode.ToString());
                        writer.WriteElementString("SpreadMethod", (brush as LinearGradientBrush).SpreadMethod.ToString());
                        writer.WriteElementString("Opacity", (brush as LinearGradientBrush).Opacity.ToString());
                        writer.WriteEndElement();
                    }
                    result = sw.ToString();
                }
            }
            else if (brush is RadialGradientBrush)
            {
                using (var sw = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = true;
                    using (var writer = XmlWriter.Create(sw, settings))
                    {
                        writer.WriteStartElement("RadialGradientBrush");
                        //writer.WriteElementString("Type", "RadialGradientBrush");
                        writer.WriteStartElement("GradientOrigin");
                        writer.WriteElementString("X", (brush as RadialGradientBrush).GradientOrigin.X.ToString());
                        writer.WriteElementString("Y", (brush as RadialGradientBrush).GradientOrigin.Y.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("Center");
                        writer.WriteElementString("X", (brush as RadialGradientBrush).Center.X.ToString());
                        writer.WriteElementString("Y", (brush as RadialGradientBrush).Center.Y.ToString());
                        writer.WriteEndElement();
                        writer.WriteElementString("RadiusX", (brush as RadialGradientBrush).RadiusX.ToString());
                        writer.WriteElementString("RadiusY", (brush as RadialGradientBrush).RadiusY.ToString());
                        writer.WriteStartElement("GradientStops");
                        foreach (var gs in (brush as RadialGradientBrush).GradientStops)
                        {
                            writer.WriteStartElement("GradientStop");
                            writer.WriteElementString("Offset", gs.Offset.ToString());
                            writer.WriteElementString("Color", gs.Color.ToString());
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteElementString("MappingMode", (brush as RadialGradientBrush).MappingMode.ToString());
                        writer.WriteElementString("SpreadMethod", (brush as RadialGradientBrush).SpreadMethod.ToString());
                        writer.WriteElementString("Opacity", (brush as RadialGradientBrush).Opacity.ToString());
                        writer.WriteEndElement();
                    }
                    result = sw.ToString();
                }
            }
            else if (brush is SolidColorBrush)
            {
                using (var sw = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = true;
                    using (var writer = XmlWriter.Create(sw, settings))
                    {
                        writer.WriteStartElement("SolidColorBrush");
                        //writer.WriteElementString("Type", "SolidColorBrush");
                        writer.WriteElementString("Color", (brush as SolidColorBrush).Color.ToString());
                        writer.WriteElementString("Opacity", (brush as SolidColorBrush).Opacity.ToString());
                        writer.WriteEndElement();
                    }
                    result = sw.ToString();
                }
            }
            return result;
        }

        public static void ClearOldFiles(ProjectModel project, string path)
        {
            logger.Info("Clear Old Files");
            try
            {
                string[] list = GetContents(project, true);
                foreach (string fileName in Directory.GetFiles(Path.GetDirectoryName(path)))
                {
                    if (Path.GetExtension(fileName) != ".cdp" && !list.Contains(Path.GetFileName(fileName)))
                        File.Delete(fileName);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Delete file", ex);
            }
        }
        #endregion

        #region Load
        public static ProjectModel LoadProject(string path)
        {
            logger.Info("Loading project");
            //path = @"C:\Users\DELL 1\Desktop\Test\Project_767bb7.cdp";
            if (!File.Exists(path))
            {
                logger.Error(String.Format("File don't exist {0}", path));
                return null;
            }

            ProjectModel result = null;

            try
            {
                ProjectDirectory = Path.GetDirectoryName(path);
                System.Threading.Thread.Sleep(200);//Slow loading

                //Encrypt text
                string cryptedText = System.IO.File.ReadAllText(path);
                logger.Info($"CDPFile:{path}");
                string openText = Crypt.Decrypt(cryptedText);
                //logger.Info($"CDPFile:Content:{openText}");
                XDocument xdoc = XDocument.Parse(openText);
                XElement item = xdoc.Descendants("Project").First();
                ProjectModel project = new ProjectModel();
                project.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                project.Information = LoadInformation(item.Descendants("Information").First(), project);
                project.SelectedResolution = LoadResolution(item.Descendants("Resolution").First());
                project.SelectedMonitor = LoadMonitor(item.Descendants("Monitor").First());
                project.SelectedOrientation = (System.Windows.Forms.ScreenOrientation)Enum.Parse(typeof(System.Windows.Forms.ScreenOrientation), item.Element("Orientation").Value);
                project.Regions = LoadRegions(item.Descendants("Regions").First(), project);
                project.SelectedRegion = project.Regions.FirstOrDefault();
                project.Alerts = LoadAlerts(item.Descendants("Alerts").First(), project);
                project.GlobalAlerts = new ObservableCollection<AlertModel>(project.Alerts.Where(_ => _.Type == AlertType.Global));
                project.PrayerAlerts = new ObservableCollection<AlertModel>(project.Alerts.Where(_ => _.Type == AlertType.Prayer));
                logger.Info($"project.PrayerAlerts:{project.PrayerAlerts.Count.ToString()}");
                project.SelectedAlert = project.Alerts.FirstOrDefault();
                logger.Info("Project is loaded");
                result = project;
            }
            catch (Exception e)
            {
                logger.Error("LoadProject", e);
            }

            return result;

        }

        private static InformationModel LoadInformation(XElement xml, ProjectModel parent)
        {
            logger.Info("Load Information");
            return new InformationModel(parent)
            {
                ProjectName = xml.Element("Name").Value,
                CreatorName = xml.Element("Creator").Value,
                MainContact = xml.Element("MainContact").Value,
                OwnerName = xml.Element("Owner").Value,
                Phone = xml.Element("Phone").Value,
                Interval = new TimeSpan(0, 0, Convert.ToInt32(xml.Element("Interval").Value))
            };
        }

        private static ResolutionModel LoadResolution(XElement xml)
        {
            logger.Info("Load Resolution");
            return new ResolutionModel(xml.Element("Name").Value, Convert.ToInt32(xml.Element("Width").Value), Convert.ToInt32(xml.Element("Height").Value), Convert.ToBoolean(xml.Element("IsInitial").Value))
            {
                Id = xml.Element("Id").Value,
            };
        }

        private static MonitorModel LoadMonitor(XElement xml)
        {
            logger.Info("Load Monitor");
            return new MonitorModel(xml.Element("Name").Value, Convert.ToInt32(xml.Element("Horizontal").Value), Convert.ToInt32(xml.Element("Vertical").Value), Convert.ToBoolean(xml.Element("IsInitial").Value))
            {
                Id = xml.Element("Id").Value,
            };
        }

        private static ObservableCollection<RegionModel> LoadRegions(XElement xml, ProjectModel parent)
        {
            logger.Info("Load Regions");
            var collection = new ObservableCollection<RegionModel>();
            foreach (var item in xml.Descendants("Region"))
            {
                RegionModel region = new RegionModel(parent);
                region.Id = item.Element("Id").Value;
                region.Name = item.Element("Name").Value;
                region.Width = Convert.ToDouble(item.Element("Width").Value);
                region.Height = Convert.ToDouble(item.Element("Height").Value);
                region.X = Convert.ToDouble(item.Element("X").Value);
                region.Y = Convert.ToDouble(item.Element("Y").Value);
                region.Slides = LoadSlides(item.Descendants("Slides").First(), region);
                region.SelectedSlide = region.Slides.FirstOrDefault();

                collection.Add(region);
            }
            return collection;
        }

        private static ObservableCollection<SlideModel> LoadSlides(XElement xml, RegionModel parent)
        {
            logger.Info("Load Slides");
            var collection = new ObservableCollection<SlideModel>();
            foreach (var item in xml.Descendants("Slide"))
            {
                SlideModel slide = new SlideModel(parent);
                slide.Id = item.Element("Id").Value;
                slide.Name = item.Element("Name").Value;
                slide.Duration = new TimeSpan(0, 0, Convert.ToInt32(item.Element("Duration").Value));
                slide.Forever = Convert.ToBoolean(item.Element("Forever").Value);
                try
                {
                    if (item.Element("EnableSchedule") != null && item.Element("EnableSchedule").Value != null)
                        slide.EnableSchedule = Convert.ToBoolean(item.Element("EnableSchedule").Value);
                    else
                        slide.EnableSchedule = false;
                    logger.Info("Enable Schulde: " + slide.EnableSchedule);
                    if (slide.EnableSchedule)
                    {
                        slide.StartDate = Convert.ToDateTime(item.Element("StartDate").Value);
                        slide.EndDate = Convert.ToDateTime(item.Element("EndDate").Value);
                        slide.StartTime = item.Element("StartTime").Value;
                        slide.EndTime = item.Element("EndTime").Value;
                        logger.Info($"Enable Schulde : {slide.EnableSchedule} Slide Name: {slide.Name} Start Date : {slide.StartDate} End Date: {slide.EndDate} Start Time: {slide.StartTime} End Time: {slide.EndTime}");
                    }
                    else
                    {
                        slide.StartDate = DateTime.Now;
                        slide.EndDate = DateTime.Now.AddDays(3);
                        slide.StartTime = DateTime.Now.ToShortTimeString();
                        slide.EndTime = DateTime.Now.AddHours(6).ToShortTimeString();
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Load Slides Schedule Exception Slide Name: " + slide.Name, e);
                }

                slide.Layers = LoadLayers(item.Descendants("Layers").First(), slide);
                slide.SelectedLayer = slide.Layers.FirstOrDefault();

                //if (slide.EnableSchedule)
                //{
                //    if (DateTime.Now.Date >= slide.StartDate.Value.Date && DateTime.Now.Date <= slide.EndDate.Value.Date)
                //    {
                //        DateTime startTime = Convert.ToDateTime(DateTime.Now.Date.ToString("M/dd/yyyy") + " " + slide.StartTime);
                //        DateTime endTime = Convert.ToDateTime(DateTime.Now.Date.ToString("M/dd/yyyy") + " " + slide.EndTime);
                //        if (DateTime.Now >= startTime && DateTime.Now <= endTime)
                //        {
                //            collection.Add(slide);
                //            logger.Info("Scheduled Slide Added for start time " + startTime.ToString() + " till end time: " + endTime.ToString());
                //        }
                //    }
                //}
                //else
                collection.Add(slide);
            }
            return collection;
        }

        private static ObservableCollection<LayerModel> LoadLayers(XElement xml, SlideModel parent)
        {
            logger.Info("Load Layers");
            var collection = new ObservableCollection<LayerModel>();
            foreach (var item in xml.Descendants("Layer"))
            {
                LayerModel layer = new LayerModel(parent);
                layer.Id = item.Element("Id").Value;
                layer.Name = item.Element("Name").Value;
                layer.ZIndex = Convert.ToInt32(item.Element("ZIndex").Value);
                layer.IsVisible = Convert.ToBoolean(item.Element("IsVisible").Value);
                layer.IsLocked = Convert.ToBoolean(item.Element("IsLocked").Value);
                layer.Controls = LoadControls(item.Descendants("Controls").First(), layer);
                layer.SelectedControl = layer.Controls.FirstOrDefault();

                collection.Add(layer);
            }
            return collection;
        }

        private static ObservableCollection<AlertModel> LoadAlerts(XElement xml, ProjectModel parent)
        {
            logger.Info("Load Alerts");
            var collection = new ObservableCollection<AlertModel>();
            foreach (var item in xml.Descendants("Alert"))
            {
                AlertModel alert = new AlertModel(parent);
                alert.Id = item.Element("Id").Value;
                alert.Name = item.Element("Name").Value;
                alert.Type = (AlertType)Enum.Parse(typeof(AlertType), item.Element("Type").Value);
                alert.Controls = LoadControls(item.Descendants("Controls").First(), alert);
                alert.SelectedControl = alert.Controls.FirstOrDefault();
                alert.Slots = LoadSlots(item.Descendants("Slots").First(), alert);
                alert.SelectedSlot = alert.Slots.FirstOrDefault();
                collection.Add(alert);
            }
            return collection;
        }

        private static ObservableCollection<SlotModel> LoadSlots(XElement xml, AlertModel parent)
        {
            logger.Info("Load Slots");
            var collection = new ObservableCollection<SlotModel>();
            foreach (var item in xml.Descendants("Slot"))
            {
                SlotModel slot = new SlotModel(parent);
                slot.Id = item.Element("Id").Value;
                slot.Name = item.Element("Name").Value;
                slot.Start = (item.Element("StartTime") == null || String.IsNullOrEmpty(item.Element("StartTime").Value)) ? new DateTime() : new DateTime(Convert.ToInt64(item.Element("StartTime").Value));
                slot.Location = new LocationModel();
                slot.Location.Id = item.Element("Location").Element("Id").Value;
                slot.Location.Country = item.Element("Location").Element("Country").Value;
                slot.Location.City = item.Element("Location").Element("City").Value;
                slot.Location.Latitude = Convert.ToDouble(item.Element("Location").Element("Latitude").Value);
                slot.Location.Longnitude = Convert.ToDouble(item.Element("Location").Element("Longnitude").Value);
                slot.Duration = new TimeSpan(0, 0, Convert.ToInt32(item.Element("Duration").Value));

                collection.Add(slot);
            }
            return collection;
        }

        private static ObservableCollection<ControlModel> LoadControls(XElement xml, object parent)
        {
            logger.Info("Load Controls");
            ThicknessConverter thicknessConverter = new ThicknessConverter();

            var collection = new ObservableCollection<ControlModel>();
            foreach (var item in xml.Descendants("Control"))
            {
                try
                {
                    ControlType ct = (ControlType)Enum.Parse(typeof(ControlType), item.Element("Type").Value);
                    ControlModel control = new ControlModel(ct,
                        Convert.ToDouble(item.Element("X").Value),
                        Convert.ToDouble(item.Element("Y").Value),
                        Convert.ToDouble(item.Element("Width").Value),
                        Convert.ToDouble(item.Element("Height").Value),
                        parent);
                    control.Id = item.Element("Id").Value;
                    control.Name = item.Element("Name").Value;
                    control.Opacity = Convert.ToDouble(item.Element("Opacity").Value);
                    control.Background = XMLToBrush(item.Element("Background"));
                    control.BorderBrush = XMLToBrush(item.Element("BorderBrush"));
                    control.BorderThickness = (Thickness)thicknessConverter.ConvertFromString(item.Element("BorderThickness").Value);
                    control.CornerRadius = GetCornerRadius(item.Element("CornerRadius").Value);
                    control.HorizontalAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), item.Element("HorizontalAlignment").Value);
                    control.VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), item.Element("VerticalAlignment").Value);
                    control.Stretch = (Stretch)Enum.Parse(typeof(Stretch), item.Element("Stretch").Value);
                    control.HorizontalFlip = Convert.ToBoolean(item.Element("HorizontalFlip").Value);
                    control.VerticalFlip = Convert.ToBoolean(item.Element("VerticalFlip").Value);
                    control.Rotate = Convert.ToInt32(item.Element("Rotate").Value);
                    control.FontFamily = (item.Element("FontFamily") == null || String.IsNullOrEmpty(item.Element("FontFamily").Value)) ? null : new FontFamily(item.Element("FontFamily").Value);
                    control.FontSize = Convert.ToInt32(item.Element("FontSize").Value);
                    control.Foreground = XMLToBrush(item.Element("Foreground"));
                    control.Url = (item.Element("Url") == null || String.IsNullOrEmpty(item.Element("Url").Value)) ? null : new Uri(item.Element("Url").Value);
                    control.FontWeight = (item.Element("FontWeight") == null || String.IsNullOrEmpty(item.Element("FontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(item.Element("FontWeight").Value);
                    control.FontStyle = (item.Element("FontStyle") == null || String.IsNullOrEmpty(item.Element("FontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(item.Element("FontStyle").Value);
                    control.TextDecorationText = (item.Element("TextDecorationText") == null || String.IsNullOrEmpty(item.Element("TextDecorationText").Value)) ? null : item.Element("TextDecorationText").Value;
                    control.InvertDirection = Convert.ToBoolean(item.Element("InvertDirection").Value);
                    control.Duration = new TimeSpan(0, 0, Convert.ToInt32(item.Element("Duration").Value));
                    control.MediaAccountId = (item.Element("MediaAccountId") == null || String.IsNullOrEmpty(item.Element("MediaAccountId").Value) ? String.Empty : item.Element("MediaAccountId").Value);
                    control.MediaPageName = (item.Element("MediaPageName") == null || String.IsNullOrEmpty(item.Element("MediaPageName").Value) ? String.Empty : item.Element("MediaPageName").Value);
                    control.FlowDirection = (item.Element("FlowDirection") == null || String.IsNullOrEmpty(item.Element("FlowDirection").Value) ? false : Convert.ToBoolean(item.Element("FlowDirection").Value));
                    control.DateTimeFormat = (item.Element("DateTimeFormat") == null || String.IsNullOrEmpty(item.Element("DateTimeFormat").Value) ? 0 : Convert.ToInt32(item.Element("DateTimeFormat").Value));
                    control.CustomDateTimeFormat = item.Element("CustomDateTimeFormat") == null || String.IsNullOrEmpty(item.Element("CustomDateTimeFormat").Value) ? String.Empty : item.Element("CustomDateTimeFormat").Value;
                    control.ItemCount = (item.Element("ItemCount") == null || String.IsNullOrEmpty(item.Element("ItemCount").Value) ? 1 : Convert.ToInt32(item.Element("ItemCount").Value));
                    control.ZIndex = Convert.ToInt32(item.Element("ZIndex").Value);
                    control.IsVisible = Convert.ToBoolean(item.Element("IsVisible").Value);
                    control.IsLocked = Convert.ToBoolean(item.Element("IsLocked").Value);
                    control.Playlist = LoadPlaylist(item.Descendants("Playlists").First(), control);
                    control.SelectedPlaylist = control.Playlist.FirstOrDefault();
                    control.SelectedSocialMedia = (item.Element("Source") == null || String.IsNullOrEmpty(item.Element("Source").Value) || item.Element("SelectedSocialMedia") == null || String.IsNullOrEmpty(item.Element("SelectedSocialMedia").Value)) ?
                        null : GetSocialMedia(item.Element("SelectedSocialMedia").Value, item.Element("Source").Value);

                    collection.Add(control);
                }
                catch (Exception e)
                {
                    logger.Error("Control is not loaded", e);
                }
            }
            return collection;
        }

        private static ObservableCollection<PlaylistModel> LoadPlaylist(XElement xml, ControlModel parent)
        {
            logger.Info("Load Playlist");
            ThicknessConverter thicknessConverter = new ThicknessConverter();

            var collection = new ObservableCollection<PlaylistModel>();
            foreach (var item in xml.Descendants("Playlist"))
            {
                PlaylistModel playlist = null;
                PlaylistType ct = (PlaylistType)Enum.Parse(typeof(PlaylistType), item.Element("Type").Value);
                switch (ct)
                {
                    case PlaylistType.AnimateBorder:
                        playlist = new AnimateBorderModel(parent)
                        {
                            BorderThicknessFrom = (Thickness)thicknessConverter.ConvertFromString(item.Element("BorderFrom").Value),
                            BorderThicknessTo = (Thickness)thicknessConverter.ConvertFromString(item.Element("BordeTor").Value),
                        };
                        break;
                    case PlaylistType.AnimateHeight:
                        playlist = new AnimateHeightModel(parent)
                        {
                            HeightFrom = Convert.ToDouble(item.Element("HeightFrom").Value),
                            HeightTo = Convert.ToDouble(item.Element("HeightTo").Value),
                        };
                        break;
                    case PlaylistType.AnimateMargin:
                        playlist = new AnimateMarginModel(parent)
                        {
                            MarginThicknessFrom = (Thickness)thicknessConverter.ConvertFromString(item.Element("MarginFrom").Value),
                            MarginThicknessTo = (Thickness)thicknessConverter.ConvertFromString(item.Element("MarginTo").Value),
                        };
                        break;
                    case PlaylistType.AnimateOpacity:
                        playlist = new AnimateOpacityModel(parent)
                        {
                            OpacityFrom = Convert.ToDouble(item.Element("OpacityFrom").Value),
                            OpacityTo = Convert.ToDouble(item.Element("OpacityTo").Value),
                        };
                        break;
                    case PlaylistType.AnimateWidth:
                        playlist = new AnimateWidthModel(parent)
                        {
                            WidthFrom = Convert.ToDouble(item.Element("WidthFrom").Value),
                            WidthTo = Convert.ToDouble(item.Element("WidthTo").Value),
                        };
                        break;
                    case PlaylistType.Delay:
                        playlist = new DelayModel(parent);
                        break;
                    case PlaylistType.ResumePlayback:
                        playlist = new ResumePlaybackModel(parent);
                        break;
                    case PlaylistType.SetContent:
                        playlist = new SetContentModel(parent)
                        {
                            Content = (item.Element("Content") == null || String.IsNullOrEmpty(item.Element("Content").Value)) ? String.Empty : item.Element("Content").Value,
                            ContentSize = (item.Element("ContentSize") == null || String.IsNullOrEmpty(item.Element("ContentSize").Value)) ? 0 : Convert.ToInt64(item.Element("ContentSize").Value)
                        };
                        switch (parent.Type)
                        {
                            case ControlType.Image:
                            case ControlType.GifAnim:
                                (playlist as SetContentModel).Content = Path.Combine(ProjectDirectory, item.Element("Content").Value);
                                break;
                            case ControlType.Video:
                                (playlist as SetContentModel).Content = Path.Combine(ProjectDirectory, item.Element("Content").Value);
                                (playlist as SetContentModel).IsMuted = (item.Element("IsMuted") == null || String.IsNullOrEmpty(item.Element("IsMuted").Value)) ? false : Convert.ToBoolean(item.Element("IsMuted").Value);
                                break;
                            case ControlType.DataGrid:
                                (playlist as SetContentModel).DataGrid = LoadDataGrid(item.Element("DataGrid"));
                                break;
                            case ControlType.Weather:
                                (playlist as SetContentModel).Weather = LoadWeather(item.Element("Weather"));
                                break;
                            case ControlType.PrayerImage:
                            case ControlType.PrayerVideo:
                                //(playlist as SetContentModel).Content= (playlist as SetContentModel).Fajr = (item.Element("Fajr") == null || String.IsNullOrEmpty(item.Element("Fajr").Value)) ? null : Path.Combine(ProjectDirectory, item.Element("Fajr").Value);
                                //(playlist as SetContentModel).ContentSize = (playlist as SetContentModel).FajrSize = (item.Element("FajrSize") == null || String.IsNullOrEmpty(item.Element("FajrSize").Value)) ? 0 : Convert.ToInt64(item.Element("FajrSize").Value);

                                (playlist as SetContentModel).Fajr = (item.Element("Fajr") == null || String.IsNullOrEmpty(item.Element("Fajr").Value)) ? null : Path.Combine(ProjectDirectory, item.Element("Fajr").Value);
                                (playlist as SetContentModel).FajrSize = (item.Element("FajrSize") == null || String.IsNullOrEmpty(item.Element("FajrSize").Value)) ? 0 : Convert.ToInt64(item.Element("FajrSize").Value);

                                (playlist as SetContentModel).Dhuhr = (item.Element("Dhuhr") == null || String.IsNullOrEmpty(item.Element("Dhuhr").Value)) ? null : Path.Combine(ProjectDirectory, item.Element("Dhuhr").Value);
                                (playlist as SetContentModel).DhuhrSize = (item.Element("DhuhrSize") == null || String.IsNullOrEmpty(item.Element("DhuhrSize").Value)) ? 0 : Convert.ToInt64(item.Element("DhuhrSize").Value);
                                (playlist as SetContentModel).Asr = (item.Element("Asr") == null || String.IsNullOrEmpty(item.Element("Asr").Value)) ? null : Path.Combine(ProjectDirectory, item.Element("Asr").Value);
                                (playlist as SetContentModel).AsrSize = (item.Element("AsrSize") == null || String.IsNullOrEmpty(item.Element("AsrSize").Value)) ? 0 : Convert.ToInt64(item.Element("AsrSize").Value);
                                (playlist as SetContentModel).Maghrib = (item.Element("Maghrib") == null || String.IsNullOrEmpty(item.Element("Maghrib").Value)) ? null : Path.Combine(ProjectDirectory, item.Element("Maghrib").Value);
                                (playlist as SetContentModel).MaghribSize = (item.Element("MaghribSize") == null || String.IsNullOrEmpty(item.Element("MaghribSize").Value)) ? 0 : Convert.ToInt64(item.Element("MaghribSize").Value);
                                (playlist as SetContentModel).Isha = (item.Element("Isha") == null || String.IsNullOrEmpty(item.Element("Isha").Value)) ? null : Path.Combine(ProjectDirectory, item.Element("Isha").Value);
                                (playlist as SetContentModel).IshaSize = (item.Element("IshaSize") == null || String.IsNullOrEmpty(item.Element("IshaSize").Value)) ? 0 : Convert.ToInt64(item.Element("IshaSize").Value);
                                break;
                            case ControlType.PrayerText:
                                (playlist as SetContentModel).Fajr = (item.Element("Fajr") == null || String.IsNullOrEmpty(item.Element("Fajr").Value)) ? String.Empty : item.Element("Fajr").Value;
                                (playlist as SetContentModel).Dhuhr = (item.Element("Dhuhr") == null || String.IsNullOrEmpty(item.Element("Dhuhr").Value)) ? String.Empty : item.Element("Dhuhr").Value;
                                (playlist as SetContentModel).Asr = (item.Element("Asr") == null || String.IsNullOrEmpty(item.Element("Asr").Value)) ? String.Empty : item.Element("Asr").Value;
                                (playlist as SetContentModel).Maghrib = (item.Element("Maghrib") == null || String.IsNullOrEmpty(item.Element("Maghrib").Value)) ? String.Empty : item.Element("Maghrib").Value;
                                (playlist as SetContentModel).Isha = (item.Element("Isha") == null || String.IsNullOrEmpty(item.Element("Isha").Value)) ? String.Empty : item.Element("Isha").Value;
                                break;
                            case ControlType.PDF:
                            case ControlType.PPT:
                                (playlist as SetContentModel).Content = Path.Combine(ProjectDirectory, item.Element("Content").Value);
                                (playlist as SetContentModel).PageDuration = (item.Element("PageDuration") == null || String.IsNullOrEmpty(item.Element("PageDuration").Value)) ? 0 : Convert.ToInt32(item.Element("PageDuration").Value);
                                (playlist as SetContentModel).DocumentFit = (item.Element("DocumentFit") == null || String.IsNullOrEmpty(item.Element("DocumentFit").Value)) ? String.Empty : item.Element("DocumentFit").Value;
                                break;
                            default:
                                break;
                        }
                        break;
                    case PlaylistType.SuspendPlayback:
                        playlist = new SuspendPlaybackModel(parent);
                        break;
                }
                playlist.Id = item.Element("Id").Value;
                playlist.Name = item.Element("Name").Value;
                playlist.StartTime = new TimeSpan(0, 0, Convert.ToInt32(item.Element("StartTime").Value));
                playlist.Duration = new TimeSpan(0, 0, Convert.ToInt32(item.Element("Duration").Value));
                playlist.Forever = Convert.ToBoolean(item.Element("Forever").Value);
                playlist.Depends = (item.Element("Depends") == null || String.IsNullOrEmpty(item.Element("Depends").Value)) ? null : new PlaylistModel(null) { Id = item.Element("Depends").Value };

                collection.Add(playlist);
            }

            //Change Id for Depends
            foreach (var item in collection)
            {
                if (item.Depends != null)
                    item.Depends = collection.Where(_ => _.Id == item.Depends.Id).FirstOrDefault();
            }

            return collection;
        }

        private static DataGridModel LoadDataGrid(XElement xml)
        {
            ThicknessConverter thicknessConverter = new ThicknessConverter();
            logger.Info("Load DataGrid");
            var dg = new DataGridModel();
            if (xml == null)
                return dg;
            dg.Id = xml.Element("Id").Value;
            dg.LinesVisibility = (DataGridGridLinesVisibility)Enum.Parse(typeof(DataGridGridLinesVisibility), xml.Element("LinesVisibility").Value);
            dg.RowBackground = (xml.Element("RowBackground") == null || String.IsNullOrEmpty(xml.Element("RowBackground").Value)) ? null : XMLToBrush(xml.Element("RowBackground"));
            dg.BorderBrush = (xml.Element("BorderBrush") == null || String.IsNullOrEmpty(xml.Element("BorderBrush").Value)) ? null : XMLToBrush(xml.Element("BorderBrush"));
            dg.BorderThickness = (Thickness)thicknessConverter.ConvertFromString(xml.Element("BorderThickness").Value);
            dg.RowCornerRadius = (xml.Element("RowCornerRadius") == null || String.IsNullOrEmpty(xml.Element("RowCornerRadius").Value)) ? new CornerRadius(0) : GetCornerRadius(xml.Element("RowCornerRadius").Value);
            dg.RowMargin = (xml.Element("RowMargin") == null || String.IsNullOrEmpty(xml.Element("RowMargin").Value)) ? new Thickness(0) : (Thickness)thicknessConverter.ConvertFromString(xml.Element("RowMargin").Value);
            dg.VerticalLineColour = (xml.Element("VerticalLineColour") == null || String.IsNullOrEmpty(xml.Element("VerticalLineColour").Value)) ? null : XMLToBrush(xml.Element("VerticalLineColour"));
            dg.HorizontalLineColour = (xml.Element("VerticalLineColour") == null || String.IsNullOrEmpty(xml.Element("VerticalLineColour").Value)) ? null : XMLToBrush(xml.Element("HorizontalLineColour"));
            dg.AlternationCount = (xml.Element("AlternationCount") == null || String.IsNullOrEmpty(xml.Element("AlternationCount").Value)) ? 0 : Convert.ToInt32(xml.Element("AlternationCount").Value);
            dg.AlternatingRowBackground = (xml.Element("AlternatingRowBackground") == null || String.IsNullOrEmpty(xml.Element("AlternatingRowBackground").Value)) ? null : XMLToBrush(xml.Element("AlternatingRowBackground"));
            dg.IsVisibleShadow = (xml.Element("IsVisibleShadow") == null || String.IsNullOrEmpty(xml.Element("IsVisibleShadow").Value)) ? false : Convert.ToBoolean(xml.Element("IsVisibleShadow").Value);
            dg.RowShadowEffect = (xml.Element("RowShadowEffect") == null || String.IsNullOrEmpty(xml.Element("RowShadowEffect").Value)) ? null : XMLToDropShadowEffect(xml.Element("RowShadowEffect"));
            dg.HeaderHeight = (xml.Element("HeaderHeight") == null || String.IsNullOrEmpty(xml.Element("HeaderHeight").Value)) ? 65 : Convert.ToInt32(xml.Element("HeaderHeight").Value);
            dg.HeaderSize = Convert.ToInt32(xml.Element("HeaderSize").Value);
            dg.HeaderBackground = XMLToBrush(xml.Element("HeaderBackground"));
            dg.HeaderForeground = XMLToBrush(xml.Element("HeaderForeground"));
            dg.HeaderBorderBrush = (xml.Element("HeaderBorderBrush") == null || String.IsNullOrEmpty(xml.Element("HeaderBorderBrush").Value)) ? null : XMLToBrush(xml.Element("HeaderBorderBrush"));
            dg.HeaderBorderThickness = (Thickness)thicknessConverter.ConvertFromString(xml.Element("HeaderBorderThickness").Value);
            dg.HeaderCornerRadius = (xml.Element("HeaderCornerRadius") == null || String.IsNullOrEmpty(xml.Element("HeaderCornerRadius").Value)) ? new CornerRadius(0) : GetCornerRadius(xml.Element("HeaderCornerRadius").Value);
            dg.HeaderMargin = (xml.Element("HeaderMargin") == null || String.IsNullOrEmpty(xml.Element("HeaderMargin").Value)) ? new Thickness(0) : (Thickness)thicknessConverter.ConvertFromString(xml.Element("HeaderMargin").Value);
            dg.HeaderFontFamily = (xml.Element("HeaderFontFamily") == null || String.IsNullOrEmpty(xml.Element("HeaderFontFamily").Value)) ? null : new FontFamily(xml.Element("HeaderFontFamily").Value);
            dg.HeaderFontWeight = (xml.Element("HeaderFontWeight") == null || String.IsNullOrEmpty(xml.Element("HeaderFontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(xml.Element("HeaderFontWeight").Value);
            dg.HeaderFontStyle = (xml.Element("HeaderFontStyle") == null || String.IsNullOrEmpty(xml.Element("HeaderFontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(xml.Element("HeaderFontStyle").Value);
            dg.HeaderHorizontalAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), xml.Element("HeaderHorizontalAlignment").Value);
            dg.HeaderVerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), xml.Element("HeaderVerticalAlignment").Value);
            dg.HeaderIsVisibleShadow = (xml.Element("HeaderIsVisibleShadow") == null || String.IsNullOrEmpty(xml.Element("HeaderIsVisibleShadow").Value)) ? false : Convert.ToBoolean(xml.Element("HeaderIsVisibleShadow").Value);
            dg.HeaderShadowEffect = (xml.Element("HeaderShadowEffect") == null || String.IsNullOrEmpty(xml.Element("HeaderShadowEffect").Value)) ? null : XMLToDropShadowEffect(xml.Element("HeaderShadowEffect"));
            dg.MaxRows = Convert.ToInt32(xml.Element("MaxRows").Value);
            dg.RowHeight = (xml.Element("RowHeight") == null || String.IsNullOrEmpty(xml.Element("RowHeight").Value)) ? 50 : Convert.ToDouble(xml.Element("RowHeight").Value);
            dg.RefreshTime = Convert.ToInt32(xml.Element("RefreshTime").Value);
            dg.SelectedSource = LoadDataSources(xml.Element("Source"));
            return dg;
        }

        private static WeatherModel LoadWeather(XElement xml)
        {
            ThicknessConverter thicknessConverter = new ThicknessConverter();
            logger.Info("Load Weather");
            var item = new WeatherModel();
            if (xml == null)
                return item;
            item.Id = xml.Element("Id").Value;
            item.TitleIsVisible = (xml.Element("TitleIsVisible") == null || String.IsNullOrEmpty(xml.Element("TitleIsVisible").Value)) ? true : Convert.ToBoolean(xml.Element("TitleIsVisible").Value);
            item.TitleSize = Convert.ToInt32(xml.Element("TitleSize").Value);
            item.TitleBackground = XMLToBrush(xml.Element("TitleBackground"));
            item.TitleForeground = XMLToBrush(xml.Element("TitleForeground"));
            item.TitleBorderBrush = (xml.Element("TitleBorderBrush") == null || String.IsNullOrEmpty(xml.Element("TitleBorderBrush").Value)) ? Brushes.Black : XMLToBrush(xml.Element("TitleBorderBrush"));
            item.TitleBorderThickness = (Thickness)thicknessConverter.ConvertFromString(xml.Element("TitleBorderThickness").Value);
            item.TitleFontFamily = (xml.Element("TitleFontFamily") == null || String.IsNullOrEmpty(xml.Element("TitleFontFamily").Value)) ? null : new FontFamily(xml.Element("TitleFontFamily").Value);
            item.TitleFontWeight = (xml.Element("TitleFontWeight") == null || String.IsNullOrEmpty(xml.Element("TitleFontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(xml.Element("TitleFontWeight").Value);
            item.TitleFontStyle = (xml.Element("TitleFontStyle") == null || String.IsNullOrEmpty(xml.Element("TitleFontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(xml.Element("TitleFontStyle").Value);
            item.TitleCornerRadius = GetCornerRadius(xml.Element("TitleCornerRadius").Value);
            item.TitleText = xml.Element("TitleText").Value;

            item.DayTextIsVisible = (xml.Element("DayTextIsVisible") == null || String.IsNullOrEmpty(xml.Element("DayTextIsVisible").Value)) ? true : Convert.ToBoolean(xml.Element("DayTextIsVisible").Value);
            item.DayTextSize = Convert.ToInt32(xml.Element("DayTextSize").Value);
            item.DayTextBackground = XMLToBrush(xml.Element("DayTextBackground"));
            item.DayTextForeground = XMLToBrush(xml.Element("DayTextForeground"));
            item.DayTextBorderBrush = (xml.Element("DayTextBorderBrush") == null || String.IsNullOrEmpty(xml.Element("DayTextBorderBrush").Value)) ? Brushes.Black : XMLToBrush(xml.Element("DayTextBorderBrush"));
            item.DayTextBorderThickness = (Thickness)thicknessConverter.ConvertFromString(xml.Element("DayTextBorderThickness").Value);
            item.DayTextFontFamily = (xml.Element("DayTextFontFamily") == null || String.IsNullOrEmpty(xml.Element("DayTextFontFamily").Value)) ? null : new FontFamily(xml.Element("DayTextFontFamily").Value);
            item.DayTextFontWeight = (xml.Element("DayTextFontWeight") == null || String.IsNullOrEmpty(xml.Element("DayTextFontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(xml.Element("DayTextFontWeight").Value);
            item.DayTextFontStyle = (xml.Element("DayTextFontStyle") == null || String.IsNullOrEmpty(xml.Element("DayTextFontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(xml.Element("DayTextFontStyle").Value);
            item.DayTextCornerRadius = GetCornerRadius(xml.Element("DayTextCornerRadius").Value);

            item.WeatherTextIsVisible = (xml.Element("WeatherTextIsVisible") == null || String.IsNullOrEmpty(xml.Element("WeatherTextIsVisible").Value)) ? true : Convert.ToBoolean(xml.Element("WeatherTextIsVisible").Value);
            item.WeatherTextSize = Convert.ToInt32(xml.Element("WeatherTextSize").Value);
            item.WeatherTextBackground = XMLToBrush(xml.Element("WeatherTextBackground"));
            item.WeatherTextForeground = XMLToBrush(xml.Element("WeatherTextForeground"));
            item.WeatherTextBorderBrush = (xml.Element("WeatherTextBorderBrush") == null || String.IsNullOrEmpty(xml.Element("WeatherTextBorderBrush").Value)) ? Brushes.Black : XMLToBrush(xml.Element("WeatherTextBorderBrush"));
            item.WeatherTextBorderThickness = (Thickness)thicknessConverter.ConvertFromString(xml.Element("WeatherTextBorderThickness").Value);
            item.WeatherTextFontFamily = (xml.Element("WeatherTextFontFamily") == null || String.IsNullOrEmpty(xml.Element("WeatherTextFontFamily").Value)) ? null : new FontFamily(xml.Element("WeatherTextFontFamily").Value);
            item.WeatherTextFontWeight = (xml.Element("WeatherTextFontWeight") == null || String.IsNullOrEmpty(xml.Element("WeatherTextFontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(xml.Element("WeatherTextFontWeight").Value);
            item.WeatherTextFontStyle = (xml.Element("WeatherTextFontStyle") == null || String.IsNullOrEmpty(xml.Element("WeatherTextFontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(xml.Element("WeatherTextFontStyle").Value);
            item.WeatherTextCornerRadius = GetCornerRadius(xml.Element("WeatherTextCornerRadius").Value);

            item.HeightTextIsVisible = (xml.Element("HeightTextIsVisible") == null || String.IsNullOrEmpty(xml.Element("HeightTextIsVisible").Value)) ? true : Convert.ToBoolean(xml.Element("HeightTextIsVisible").Value);
            item.HeightTextSize = Convert.ToInt32(xml.Element("HeightTextSize").Value);
            item.HeightTextBackground = XMLToBrush(xml.Element("HeightTextBackground"));
            item.HeightTextForeground = XMLToBrush(xml.Element("HeightTextForeground"));
            item.HeightTextBorderBrush = (xml.Element("HeightTextBorderBrush") == null || String.IsNullOrEmpty(xml.Element("HeightTextBorderBrush").Value)) ? Brushes.Black : XMLToBrush(xml.Element("HeightTextBorderBrush"));
            item.HeightTextBorderThickness = (Thickness)thicknessConverter.ConvertFromString(xml.Element("HeightTextBorderThickness").Value);
            item.HeightTextFontFamily = (xml.Element("HeightTextFontFamily") == null || String.IsNullOrEmpty(xml.Element("HeightTextFontFamily").Value)) ? null : new FontFamily(xml.Element("HeightTextFontFamily").Value);
            item.HeightTextFontWeight = (xml.Element("HeightTextFontWeight") == null || String.IsNullOrEmpty(xml.Element("HeightTextFontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(xml.Element("HeightTextFontWeight").Value);
            item.HeightTextFontStyle = (xml.Element("HeightTextFontStyle") == null || String.IsNullOrEmpty(xml.Element("HeightTextFontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(xml.Element("HeightTextFontStyle").Value);
            item.HeightTextCornerRadius = GetCornerRadius(xml.Element("HeightTextCornerRadius").Value);

            item.HeightValueIsVisible = (xml.Element("HeightValueIsVisible") == null || String.IsNullOrEmpty(xml.Element("HeightValueIsVisible").Value)) ? true : Convert.ToBoolean(xml.Element("HeightValueIsVisible").Value);
            item.HeightValueSize = Convert.ToInt32(xml.Element("HeightValueSize").Value);
            item.HeightValueBackground = XMLToBrush(xml.Element("HeightValueBackground"));
            item.HeightValueForeground = XMLToBrush(xml.Element("HeightValueForeground"));
            item.HeightValueBorderBrush = (xml.Element("HeightValueBorderBrush") == null || String.IsNullOrEmpty(xml.Element("HeightValueBorderBrush").Value)) ? Brushes.Black : XMLToBrush(xml.Element("HeightValueBorderBrush"));
            item.HeightValueBorderThickness = (Thickness)thicknessConverter.ConvertFromString(xml.Element("HeightValueBorderThickness").Value);
            item.HeightValueFontFamily = (xml.Element("HeightValueFontFamily") == null || String.IsNullOrEmpty(xml.Element("HeightValueFontFamily").Value)) ? null : new FontFamily(xml.Element("HeightValueFontFamily").Value);
            item.HeightValueFontWeight = (xml.Element("HeightValueFontWeight") == null || String.IsNullOrEmpty(xml.Element("HeightValueFontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(xml.Element("HeightValueFontWeight").Value);
            item.HeightValueFontStyle = (xml.Element("HeightValueFontStyle") == null || String.IsNullOrEmpty(xml.Element("HeightValueFontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(xml.Element("HeightValueFontStyle").Value);
            item.HeightValueCornerRadius = GetCornerRadius(xml.Element("HeightValueCornerRadius").Value);

            item.LowTextIsVisible = (xml.Element("LowTextIsVisible") == null || String.IsNullOrEmpty(xml.Element("LowTextIsVisible").Value)) ? true : Convert.ToBoolean(xml.Element("LowTextIsVisible").Value);
            item.LowTextSize = Convert.ToInt32(xml.Element("LowTextSize").Value);
            item.LowTextBackground = XMLToBrush(xml.Element("LowTextBackground"));
            item.LowTextForeground = XMLToBrush(xml.Element("LowTextForeground"));
            item.LowTextBorderBrush = (xml.Element("LowTextBorderBrush") == null || String.IsNullOrEmpty(xml.Element("LowTextBorderBrush").Value)) ? Brushes.Black : XMLToBrush(xml.Element("LowTextBorderBrush"));
            item.LowTextBorderThickness = (Thickness)thicknessConverter.ConvertFromString(xml.Element("LowTextBorderThickness").Value);
            item.LowTextFontFamily = (xml.Element("LowTextFontFamily") == null || String.IsNullOrEmpty(xml.Element("LowTextFontFamily").Value)) ? null : new FontFamily(xml.Element("LowTextFontFamily").Value);
            item.LowTextFontWeight = (xml.Element("LowTextFontWeight") == null || String.IsNullOrEmpty(xml.Element("LowTextFontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(xml.Element("LowTextFontWeight").Value);
            item.LowTextFontStyle = (xml.Element("LowTextFontStyle") == null || String.IsNullOrEmpty(xml.Element("LowTextFontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(xml.Element("LowTextFontStyle").Value);
            item.LowTextCornerRadius = GetCornerRadius(xml.Element("LowTextCornerRadius").Value);

            item.LowValueIsVisible = (xml.Element("LowValueIsVisible") == null || String.IsNullOrEmpty(xml.Element("LowValueIsVisible").Value)) ? true : Convert.ToBoolean(xml.Element("LowValueIsVisible").Value);
            item.LowValueSize = Convert.ToInt32(xml.Element("LowValueSize").Value);
            item.LowValueBackground = XMLToBrush(xml.Element("LowValueBackground"));
            item.LowValueForeground = XMLToBrush(xml.Element("LowValueForeground"));
            item.LowValueBorderBrush = (xml.Element("LowValueBorderBrush") == null || String.IsNullOrEmpty(xml.Element("LowValueBorderBrush").Value)) ? Brushes.Black : XMLToBrush(xml.Element("LowValueBorderBrush"));
            item.LowValueBorderThickness = (Thickness)thicknessConverter.ConvertFromString(xml.Element("LowValueBorderThickness").Value);
            item.LowValueFontFamily = (xml.Element("LowValueFontFamily") == null || String.IsNullOrEmpty(xml.Element("LowValueFontFamily").Value)) ? null : new FontFamily(xml.Element("LowValueFontFamily").Value);
            item.LowValueFontWeight = (xml.Element("LowValueFontWeight") == null || String.IsNullOrEmpty(xml.Element("LowValueFontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(xml.Element("LowValueFontWeight").Value);
            item.LowValueFontStyle = (xml.Element("LowValueFontStyle") == null || String.IsNullOrEmpty(xml.Element("LowValueFontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(xml.Element("LowValueFontStyle").Value);
            item.LowValueCornerRadius = GetCornerRadius(xml.Element("LowValueCornerRadius").Value);

            return item;
        }

        private static DataSourceModel LoadDataSources(XElement xml)
        {
            logger.Info("Load DataSources");
            var ds = new DataSourceModel();
            if (xml == null)
                return ds;
            ds.Id = xml.Element("Id").Value;
            ds.Name = xml.Element("Name").Value;
            ds.Columns = LoadDataColumns(xml.Descendants("Columns").First());
            return ds;
        }

        private static ObservableCollection<DataColumnModel> LoadDataColumns(XElement xml)
        {
            logger.Info("Load DataColumns");
            var collection = new ObservableCollection<DataColumnModel>();
            if (xml == null)
                return collection;
            foreach (var item in xml.Descendants("Column"))
            {
                DataColumnModel ds = new DataColumnModel();
                ds.Id = item.Element("Id").Value;
                ds.Name = item.Element("Name").Value;
                ds.Title = item.Element("Title").Value;
                ds.Width = Convert.ToDouble(item.Element("Width").Value);
                ds.Type = Convert.ToInt32(item.Element("Type").Value);
                ds.Background = XMLToBrush(item.Element("Background"));
                ds.Foreground = XMLToBrush(item.Element("Foreground"));
                ds.TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), item.Element("TextAlignment").Value);
                ds.VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), item.Element("VerticalAlignment").Value);
                ds.FontFamily = (item.Element("FontFamily") == null || String.IsNullOrEmpty(item.Element("FontFamily").Value)) ? null : new FontFamily(item.Element("FontFamily").Value);
                ds.FontSize = Convert.ToInt32(item.Element("FontSize").Value);
                ds.FontWeight = (item.Element("FontWeight") == null || String.IsNullOrEmpty(item.Element("FontWeight").Value)) ? FontWeights.Normal : (FontWeight)new FontWeightConverter().ConvertFromString(item.Element("FontWeight").Value);
                ds.FontStyle = (item.Element("FontStyle") == null || String.IsNullOrEmpty(item.Element("FontStyle").Value)) ? FontStyles.Normal : (FontStyle)new FontStyleConverter().ConvertFromString(item.Element("FontStyle").Value);
                ds.WhereOperator = item.Element("WhereOperator").Value;
                ds.WhereValue = item.Element("WhereValue").Value;
                ds.MergeColumn = item.Element("MergeColumn").Value;
                ds.Sort = Convert.ToInt32(item.Element("Sort").Value);
                ds.ImageStretch = (item.Element("ImageStretch") == null || String.IsNullOrEmpty(item.Element("ImageStretch").Value)) ? Stretch.Fill : (Stretch)Enum.Parse(typeof(Stretch), item.Element("ImageStretch").Value);
                ds.IsVisible = Convert.ToBoolean(item.Element("IsVisible").Value);
                ds.SpecialCells = LoadSpecialCell(item.Element("SpecialCells"));
                ds.TimeFilters = LoadTimeFilter(item.Element("TimeFilters"));
                collection.Add(ds);
            }
            return collection;
        }

        private static ObservableCollection<SpecialCellModel> LoadSpecialCell(XElement xml)
        {
            logger.Info("Load Special Cell");
            var collection = new ObservableCollection<SpecialCellModel>();
            if (xml == null)
                return collection;
            foreach (var item in xml.Descendants("SpecialCell"))
            {
                SpecialCellModel ds = new SpecialCellModel();
                ds.Id = item.Element("Id").Value;
                ds.Text = item.Element("Text").Value;
                ds.IsRow = Convert.ToBoolean(item.Element("IsRow").Value);
                ds.IsBlink = Convert.ToBoolean(item.Element("IsBlink").Value);
                ds.Background = XMLToBrush(item.Element("Background"));
                ds.Foreground = XMLToBrush(item.Element("Foreground"));
                collection.Add(ds);
            }
            return collection;
        }

        private static ObservableCollection<TimeFilterModel> LoadTimeFilter(XElement xml)
        {
            logger.Info("Load Time Filter");
            var collection = new ObservableCollection<TimeFilterModel>();
            if (xml == null)
                return collection;
            foreach (var item in xml.Descendants("TimeFilter"))
            {
                TimeFilterModel ds = new TimeFilterModel();
                ds.Id = item.Element("Id").Value;
                ds.Name = item.Element("Name").Value;
                ds.BeforeDuration = Convert.ToInt32(item.Element("BeforeDuration").Value);
                ds.AfterDuration = Convert.ToInt32(item.Element("AfterDuration").Value);
                collection.Add(ds);
            }
            return collection;
        }

        public static DropShadowEffect XMLToDropShadowEffect(XElement xml)
        {
            DropShadowEffect result = null;
            if (xml != null && xml.Descendants("DropShadowEffect").FirstOrDefault() != null)
            {
                var item = xml.Descendants("DropShadowEffect").First();
                result = new DropShadowEffect();
                result.BlurRadius = Convert.ToDouble(item.Element("BlurRadius").Value);
                result.Opacity = Convert.ToDouble(item.Element("Opacity").Value);
                result.Color = (Color)ColorConverter.ConvertFromString(item.Element("Color").Value);
                result.ShadowDepth = Convert.ToDouble(item.Element("ShadowDepth").Value);
                result.Direction = Convert.ToDouble(item.Element("Direction").Value);
            }
            return result;

        }

        public static Brush XMLToBrush(XElement xml)
        {
            Brush result = null;
            if (xml == null || xml.Descendants().FirstOrDefault() == null)
            {
                result = null;
            }
            else
            {
                string type = xml.Descendants().FirstOrDefault().Name.ToString();
                switch (xml.Descendants().FirstOrDefault().Name.ToString())
                {
                    case "SolidColorBrush":
                        {
                            var item = xml.Descendants("SolidColorBrush").First();
                            SolidColorBrush brush = new SolidColorBrush();
                            brush.Color = (Color)ColorConverter.ConvertFromString(item.Element("Color").Value);
                            brush.Opacity = Convert.ToDouble(item.Element("Opacity").Value);
                            result = brush;
                        }
                        break;
                    case "LinearGradientBrush":
                        {
                            var item = xml.Descendants("LinearGradientBrush").First();
                            LinearGradientBrush brush = new LinearGradientBrush();
                            var point = item.Element("StartPoint");
                            brush.StartPoint = new Point(Convert.ToDouble(point.Element("X").Value), Convert.ToDouble(point.Element("Y").Value));
                            point = item.Element("EndPoint");
                            brush.EndPoint = new Point(Convert.ToDouble(point.Element("X").Value), Convert.ToDouble(point.Element("Y").Value));
                            foreach (var gradientSpot in item.Element("GradientStops").Descendants("GradientStop"))
                            {
                                brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(gradientSpot.Element("Color").Value), Convert.ToDouble(gradientSpot.Element("Offset").Value)));
                            }
                            brush.MappingMode = (BrushMappingMode)Enum.Parse(typeof(BrushMappingMode), item.Element("MappingMode").Value);
                            brush.SpreadMethod = (GradientSpreadMethod)Enum.Parse(typeof(GradientSpreadMethod), item.Element("SpreadMethod").Value);
                            brush.Opacity = Convert.ToDouble(item.Element("Opacity").Value);
                            result = brush;
                        }
                        break;
                    case "RadialGradientBrush":
                        {

                            var item = xml.Element("RadialGradientBrush");
                            RadialGradientBrush brush = new RadialGradientBrush();
                            var point = item.Element("GradientOrigin");
                            brush.GradientOrigin = new Point(Convert.ToDouble(point.Element("X").Value), Convert.ToDouble(point.Element("Y").Value));
                            point = item.Element("Center");
                            brush.Center = new Point(Convert.ToDouble(point.Element("X").Value), Convert.ToDouble(point.Element("Y").Value));
                            brush.RadiusX = Convert.ToDouble(item.Element("RadiusX").Value);
                            brush.RadiusY = Convert.ToDouble(item.Element("RadiusY").Value);
                            foreach (var gradientSpot in item.Element("GradientStops").Descendants("GradientStop"))
                            {
                                brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(gradientSpot.Element("Color").Value), Convert.ToDouble(gradientSpot.Element("Offset").Value)));
                            }
                            brush.MappingMode = (BrushMappingMode)Enum.Parse(typeof(BrushMappingMode), item.Element("MappingMode").Value);
                            brush.SpreadMethod = (GradientSpreadMethod)Enum.Parse(typeof(GradientSpreadMethod), item.Element("SpreadMethod").Value);
                            brush.Opacity = Convert.ToDouble(item.Element("Opacity").Value);
                            result = brush;
                            //RadialGradientBrush fiveColorRGB = new RadialGradientBrush();
                            //fiveColorRGB.GradientOrigin = new Point(0.5, 0.5);
                            //fiveColorRGB.Center = new Point(0.5, 0.5);

                            //GradientStop blueGS = new GradientStop();
                            //blueGS.Color = Colors.Blue;
                            //blueGS.Offset = 0.0;
                            //fiveColorRGB.GradientStops.Add(blueGS);

                            //GradientStop orangeGS = new GradientStop();
                            //orangeGS.Color = Colors.Orange;
                            //orangeGS.Offset = 0.25;
                            //fiveColorRGB.GradientStops.Add(orangeGS);
                        }
                        break;
                    default:
                        result = null;
                        break;
                }
            }
            return result;
        }

        private static CornerRadius GetCornerRadius(string text)
        {
            ThicknessConverter thicknessConverter = new ThicknessConverter();
            var value = (Thickness)thicknessConverter.ConvertFromString(text);
            return new CornerRadius(value.Left, value.Top, value.Right, value.Bottom);
        }

        private static SocialMediaModel GetSocialMedia(string text, string source)
        {
            return new SocialMediaModel(text, new Uri(source, UriKind.Relative));
        }
        #endregion

        #region Flight
        public static TimeSpan GetInterval(string path)
        {
            var project = LoadProject(path);
            TimeSpan result = new TimeSpan(0, 1, 0); // one minute
            if (project.Information.Interval.TotalSeconds > 10)
                result = project.Information.Interval;
            return result;
        }

        public static string[] GetFlightId(string path)
        {
            var project = LoadProject(path);
            string[] result = null;
            if (project != null)
            {
                result = GetFlightId(project.SelectedRegion);
            }
            return result;
        }

        public static string[] GetFlightId(RegionModel rm)
        {
            List<string> list = new List<string>();
            try
            {
                foreach (SlideModel s in rm.Slides)
                {
                    foreach (LayerModel l in s.Layers)
                    {
                        foreach (ControlModel c in l.Controls)
                        {
                            if (c.Type == ControlType.DataGrid)
                            {
                                SetContentModel pl = (SetContentModel)c.Playlist.Where(_ => _.Type == PlaylistType.SetContent).FirstOrDefault();
                                list.Add(pl.DataGrid.SelectedSource.Id);

                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return list.Select(_ => _).Distinct().ToArray();
        }

        public static string[] GetContents(string path)
        {
            string[] result = null;
            try
            {
                var project = LoadProject(path);
                result = GetContents(project);
            }
            catch (Exception)
            {

            }
            return result.ToArray();
        }

        public static string GetContentsString(string[] items)
        {
            const char separator = ';';
            var sb = new StringBuilder();
            foreach (string item in items)
            {
                if (sb.Length > 0)
                    sb.Append(separator);

                int index = item.IndexOf(separator);
                if (index > 0)
                {
                    //remove size, only name save
                    sb.Append(item.Substring(0, index));
                }
                else
                {
                    sb.Append(item);
                }
            }
            return sb.ToString();
        }

        public static string[] GetContents(ProjectModel project, bool onlyName = false)
        {
            List<string> list = new List<string>();
            try
            {
                foreach (var region in project.Regions)
                {
                    //foreach (var slide in region.Slides)
                    foreach (var slide in region.SlidesDownload)
                    {
                        foreach (var layer in slide.Layers)
                        {
                            foreach (var control in layer.Controls)
                            {
                                foreach (var playlist in control.Playlist)
                                {
                                    if (playlist.Type == PlaylistType.SetContent)
                                    {
                                        string content = String.Empty;
                                        long contentSize = 0;

                                        content = (playlist as SetContentModel).Content;
                                        contentSize = (playlist as SetContentModel).ContentSize;

                                        if (!String.IsNullOrWhiteSpace(content) && contentSize > 0)
                                        {
                                            content = Path.GetFileName(content);
                                            if (onlyName)
                                            {
                                                list.Add(content);
                                            }
                                            else
                                            {
                                                list.Add($"{content};{contentSize}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var alert in project.Alerts)
                {
                    foreach (var control in alert.Controls)
                    {
                        foreach (var playlist in control.Playlist)
                        {
                            if (playlist.Type == PlaylistType.SetContent)
                            {
                                if (control.Type == ControlType.Image || control.Type == ControlType.Video || control.Type == ControlType.GifAnim)
                                {
                                    string content = (playlist as SetContentModel).Content;
                                    long contentSize = (playlist as SetContentModel).ContentSize;
                                    if (!String.IsNullOrWhiteSpace(content) && contentSize > 0)
                                    {
                                        content = Path.GetFileName(content);
                                        list.Add($"{content};{contentSize}");
                                    }
                                }
                                else if (control.Type == ControlType.PrayerImage || control.Type == ControlType.PrayerVideo)
                                {
                                    string[] prayers = new string[]
                                    {
                                        ((playlist as SetContentModel).FajrSize != 0 && !onlyName) ? $"{(playlist as SetContentModel).Fajr};{(playlist as SetContentModel).FajrSize}" : (playlist as SetContentModel).Fajr,
                                        ((playlist as SetContentModel).DhuhrSize != 0 && !onlyName) ? $"{(playlist as SetContentModel).Dhuhr};{(playlist as SetContentModel).DhuhrSize}" : (playlist as SetContentModel).Dhuhr,
                                        ((playlist as SetContentModel).AsrSize != 0 && !onlyName) ? $"{(playlist as SetContentModel).Asr};{(playlist as SetContentModel).AsrSize}" : (playlist as SetContentModel).Asr,
                                        ((playlist as SetContentModel).MaghribSize != 0 && !onlyName) ? $"{(playlist as SetContentModel).Maghrib};{(playlist as SetContentModel).MaghribSize}" : (playlist as SetContentModel).Maghrib,
                                        ((playlist as SetContentModel).IshaSize != 0 && !onlyName) ? $"{(playlist as SetContentModel).Isha};{(playlist as SetContentModel).IshaSize}" : (playlist as SetContentModel).Isha
                                    };

                                    foreach (var prayer in prayers)
                                    {
                                        if (!String.IsNullOrWhiteSpace(prayer))
                                        {
                                            if (onlyName)
                                            {

                                                list.Add(Path.GetFileName(prayer));
                                            }
                                            else
                                            {
                                                //list.Add($"{Path.GetFileName(prayer)};0");
                                                list.Add($"{Path.GetFileName(prayer)}");
                                            }
                                        }
                                    }
                                    //string content = (playlist as SetContentModel).Content;
                                    //long contentSize = (playlist as SetContentModel).ContentSize;
                                    //if (!String.IsNullOrWhiteSpace(content) && contentSize > 0)
                                    //{
                                    //    content = Path.GetFileName(content);
                                    //    list.Add(String.Format("{0};{1}", content, contentSize));
                                    //}
                                }
                                //string content = (playlist as SetContentModel).Content;
                                //long contentSize = (playlist as SetContentModel).ContentSize;
                                //if (!String.IsNullOrWhiteSpace(content) && contentSize > 0)
                                //{
                                //    content = Path.GetFileName(content);
                                //    list.Add(String.Format("{0};{1}", content, contentSize));
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return list.Select(_ => _).Distinct().ToArray();
        }
        #endregion

        #region Weather
        public static string[] GetWeatherLocations(string path)
        {
            string[] result = null;
            try
            {
                var project = LoadProject(path);
                result = GetWeatherLocations(project);
            }
            catch (Exception)
            {

            }
            return result.ToArray();
        }

        public static string[] GetWeatherLocations(ProjectModel project)
        {
            List<string> list = new List<string>();
            try
            {
                foreach (var region in project.Regions)
                {
                    foreach (var slide in region.Slides)
                    {
                        foreach (var layer in slide.Layers)
                        {
                            foreach (var control in layer.Controls)
                            {
                                if (control.Type == ControlType.Weather)
                                {
                                    foreach (var playlist in control.Playlist)
                                    {
                                        if (playlist.Type == PlaylistType.SetContent)
                                        {
                                            list.Add((playlist as SetContentModel).Content);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return list.Select(_ => _).Distinct().ToArray();
        }
        #endregion

        public static string[] GetTopics(string path)
        {
            List<string> list = new List<string>();
            try
            {
                var project = LoadProject(path);

                foreach (var region in project.Regions)
                {
                    foreach (var slide in region.Slides)
                    {
                        foreach (var layer in slide.Layers)
                        {
                            foreach (var control in layer.Controls)
                            {
                                if (control.Type == ControlType.Ticker || control.Type == ControlType.Likebox ||
                                    control.Type == ControlType.Facebook || control.Type == ControlType.Instagram ||
                                    control.Type == ControlType.Twitter)
                                    foreach (var playlist in control.Playlist)
                                    {
                                        if (playlist.Type == PlaylistType.SetContent)
                                        {
                                            string content = (playlist as SetContentModel).Content;
                                            if (content.StartsWith("topic:"))
                                                list.Add(content.Substring(6));
                                        }
                                    }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return list.ToArray();
        }

        #region[New Methods]
        public static ProjectModel LoadProjectFromString(string text, bool IsEncrypted)
        {
            logger.Info("Loading Project From String");
            ProjectModel result = null;
            try
            {
                string openText = text;
                if (IsEncrypted)
                    openText = Crypt.Decrypt(text);

                XDocument xdoc = XDocument.Parse(openText);
                XElement item = xdoc.Descendants("Project").First();
                ProjectModel project = new ProjectModel();
                project.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                project.Information = LoadInformation(item.Descendants("Information").First(), project);
                project.SelectedResolution = LoadResolution(item.Descendants("Resolution").First());
                project.SelectedMonitor = LoadMonitor(item.Descendants("Monitor").First());
                project.SelectedOrientation = (System.Windows.Forms.ScreenOrientation)Enum.Parse(typeof(System.Windows.Forms.ScreenOrientation), item.Element("Orientation").Value);
                project.Regions = LoadRegions(item.Descendants("Regions").First(), project);
                project.SelectedRegion = project.Regions.FirstOrDefault();
                project.Alerts = LoadAlerts(item.Descendants("Alerts").First(), project);
                project.GlobalAlerts = new ObservableCollection<AlertModel>(project.Alerts.Where(_ => _.Type == AlertType.Global));
                project.PrayerAlerts = new ObservableCollection<AlertModel>(project.Alerts.Where(_ => _.Type == AlertType.Prayer));
                logger.Info($"project.PrayerAlerts:{project.PrayerAlerts.Count.ToString()}");
                project.SelectedAlert = project.Alerts.FirstOrDefault();
                logger.Info("Project From String is loaded");
                result = project;
            }
            catch (Exception e)
            {
                logger.Error("LoadProjectFromString", e);
            }
            return result;
        }

        public static string ProjectToXML(ProjectModel pm, bool IsEncrypted, string path)
        {
            string result = string.Empty;
            try
            {
                logger.Info("Project To XML started");
                string projectDir = System.IO.Path.GetDirectoryName(path);
                if (!Directory.Exists(projectDir))
                    Directory.CreateDirectory(projectDir);

                //Stream sb = new MemoryStream();
                //string filename = Environment.CurrentDirectory + "\test.txt";
                //if (File.Exists(filename))
                //{
                //    File.Delete(filename);
                //}

                //FileStream fs = new FileStream(filename, FileMode.CreateNew);

                //StringBuilder sb = new StringBuilder();

                StringWriter sw = new StringWriter();

                Dictionary<string, string> filelist = new Dictionary<string, string>();

                var x_settings = new XmlWriterSettings();
                x_settings.NewLineChars = Environment.NewLine;
                x_settings.NewLineOnAttributes = true;
                x_settings.NewLineHandling = NewLineHandling.None;
                x_settings.CloseOutput = true;
                x_settings.Indent = true;
                logger.Info((object)"Project To XML started 1");
                using (var writer = XmlWriter.Create(sw, x_settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Project");
                    writer.WriteElementString("Id", pm.Id);

                    writer.WriteStartElement("Information");
                    writer.WriteElementString("Name", pm.Information.ProjectName);
                    writer.WriteElementString("Creator", pm.Information.CreatorName);
                    writer.WriteElementString("MainContact", pm.Information.MainContact);
                    writer.WriteElementString("Owner", pm.Information.OwnerName);
                    writer.WriteElementString("Phone", pm.Information.Phone);
                    writer.WriteElementString("Interval", pm.Information.Interval.TotalSeconds.ToString());
                    writer.WriteEndElement();
                    logger.Info((object)"Project To XML started 2");
                    writer.WriteStartElement("Resolution");
                    writer.WriteElementString("Id", pm.SelectedResolution.Id);
                    writer.WriteElementString("Name", pm.SelectedResolution.Name);
                    writer.WriteElementString("Width", pm.SelectedResolution.Width.ToString());
                    writer.WriteElementString("Height", pm.SelectedResolution.Height.ToString());
                    writer.WriteElementString("IsInitial", pm.SelectedResolution.IsInitial.ToString());
                    writer.WriteEndElement();
                    logger.Info((object)"Project To XML started 3");
                    writer.WriteStartElement("Monitor");
                    writer.WriteElementString("Id", pm.SelectedMonitor.Id);
                    writer.WriteElementString("Name", pm.SelectedMonitor.Name);
                    writer.WriteElementString("Horizontal", pm.SelectedMonitor.Horizontal.ToString());
                    writer.WriteElementString("Vertical", pm.SelectedMonitor.Vertical.ToString());
                    writer.WriteElementString("IsInitial", pm.SelectedMonitor.IsInitial.ToString());
                    writer.WriteEndElement();

                    writer.WriteElementString("Orientation", pm.SelectedOrientation.ToString());
                    logger.Info((object)"Project To XML started 4");
                    writer.WriteStartElement("Regions");
                    foreach (var region in pm.Regions)
                    {
                        writer.WriteStartElement("Region");
                        writer.WriteElementString("Id", region.Id);
                        writer.WriteElementString("Name", region.Name);
                        writer.WriteElementString("Width", region.Width.ToString());
                        writer.WriteElementString("Height", region.Height.ToString());
                        writer.WriteElementString("X", region.X.ToString());
                        writer.WriteElementString("Y", region.Y.ToString());

                        writer.WriteStartElement("Slides");
                        foreach (var slide in region.SlidesDownload)
                        {
                            writer.WriteStartElement("Slide");
                            writer.WriteElementString("Id", slide.Id);
                            writer.WriteElementString("Name", slide.Name);
                            writer.WriteElementString("Duration", slide.Duration.TotalSeconds.ToString());
                            writer.WriteElementString("Forever", slide.Forever.ToString());

                            writer.WriteStartElement("Layers");
                            foreach (var layer in slide.Layers)
                            {
                                writer.WriteStartElement("Layer");
                                writer.WriteElementString("Id", layer.Id);
                                writer.WriteElementString("Name", layer.Name);
                                writer.WriteElementString("ZIndex", layer.ZIndex.ToString());
                                writer.WriteElementString("IsVisible", layer.IsVisible.ToString());
                                writer.WriteElementString("IsLocked", layer.IsLocked.ToString());

                                writer.WriteStartElement("Controls");
                                foreach (var control in layer.Controls)
                                {
                                    writer.WriteStartElement("Control");
                                    writer.WriteAttributeString("Type", control.Type.ToString());
                                    writer.WriteElementString("Id", control.Id);
                                    writer.WriteElementString("Name", control.Name);
                                    writer.WriteElementString("Width", control.Width.ToString());
                                    writer.WriteElementString("Height", control.Height.ToString());
                                    writer.WriteElementString("X", control.X.ToString());
                                    writer.WriteElementString("Y", control.Y.ToString());
                                    writer.WriteElementString("Margion", new Thickness(control.X, control.Y, control.W, control.Z).ToString());
                                    writer.WriteElementString("Opacity", control.Opacity.ToString());
                                    writer.WriteStartElement("Background");
                                    writer.WriteRaw(BrushToXML(control.Background));
                                    writer.WriteEndElement();
                                    writer.WriteStartElement("BorderBrush");
                                    writer.WriteRaw(BrushToXML(control.BorderBrush));
                                    writer.WriteEndElement();
                                    writer.WriteElementString("BorderThickness", control.BorderThickness.ToString());
                                    writer.WriteElementString("CornerRadius", control.CornerRadius.ToString());
                                    writer.WriteElementString("HorizontalAlignment", control.HorizontalAlignment.ToString());
                                    writer.WriteElementString("VerticalAlignment", control.VerticalAlignment.ToString());
                                    writer.WriteElementString("Stretch", control.Stretch.ToString());
                                    writer.WriteElementString("HorizontalFlip", control.HorizontalFlip.ToString());
                                    writer.WriteElementString("VerticalFlip", control.VerticalFlip.ToString());
                                    writer.WriteElementString("Rotate", control.Rotate.ToString());
                                    if (control.FontFamily != null) writer.WriteElementString("FontFamily", control.FontFamily.ToString());
                                    if (control.FontSize != null) writer.WriteElementString("FontSize", control.FontSize.ToString());
                                    if (control.Foreground != null)
                                    {
                                        writer.WriteStartElement("Foreground");
                                        writer.WriteRaw(BrushToXML(control.Foreground));
                                        writer.WriteEndElement();
                                    }
                                    if (control.Url != null) writer.WriteElementString("Url", control.Url.ToString());
                                    if (control.FontWeight != null) writer.WriteElementString("FontWeight", control.FontWeight.ToString());
                                    if (control.FontStyle != null) writer.WriteElementString("FontStyle", control.FontStyle.ToString());
                                    if (control.TextDecorationText != null) writer.WriteElementString("TextDecorationText", control.TextDecorationText);
                                    if (control.InvertDirection != null) writer.WriteElementString("InvertDirection", control.InvertDirection.ToString());
                                    if (control.Duration != null) writer.WriteElementString("Duration", control.Duration.TotalSeconds.ToString());
                                    writer.WriteElementString("Type", control.Type.ToString());
                                    if (control.SelectedSocialMedia != null) writer.WriteElementString("SelectedSocialMedia", control.SelectedSocialMedia.Name);
                                    writer.WriteElementString("MediaAccountId", control.MediaAccountId);
                                    writer.WriteElementString("MediaPageName", control.MediaPageName);
                                    writer.WriteElementString("FlowDirection", control.FlowDirection.ToString());
                                    writer.WriteElementString("DateTimeFormat", control.DateTimeFormat.ToString());
                                    writer.WriteElementString("CustomDateTimeFormat", control.CustomDateTimeFormat);
                                    writer.WriteElementString("ItemCount", control.ItemCount.ToString());
                                    writer.WriteElementString("Type", control.Type.ToString());
                                    writer.WriteElementString("ZIndex", control.ZIndex.ToString());
                                    writer.WriteElementString("IsVisible", control.IsVisible.ToString());
                                    writer.WriteElementString("IsLocked", control.IsLocked.ToString());
                                    writer.WriteStartElement("Playlists");
                                    foreach (var playlist in control.Playlist)
                                    {
                                        writer.WriteStartElement("Playlist");
                                        writer.WriteElementString("Id", playlist.Id);
                                        writer.WriteElementString("Name", playlist.Name);
                                        writer.WriteElementString("StartTime", playlist.StartTime.TotalSeconds.ToString());
                                        writer.WriteElementString("Duration", playlist.Duration.TotalSeconds.ToString());
                                        writer.WriteElementString("Forever", playlist.Forever.ToString());
                                        if (playlist.Depends != null) writer.WriteElementString("Depends", playlist.Depends.Id);
                                        writer.WriteElementString("Type", playlist.Type.ToString());
                                        switch (playlist.Type)
                                        {
                                            case PlaylistType.SetContent:
                                                if (control.Type == ControlType.Image
                                                    || control.Type == ControlType.Video
                                                    || control.Type == ControlType.GifAnim
                                                    || control.Type == ControlType.PDF
                                                    || control.Type == ControlType.PPT)
                                                {
                                                    var scm = playlist as SetContentModel;
                                                    string output = String.Empty;
                                                    string res = "<Content>" + scm.Content + "</Content><ContentSize>" + scm.ContentSize + "</ContentSize>";
                                                    //string res = StoreSourceXML("Content", true, scm.Content, scm.ContentSize, filelist, projectDir, out output);
                                                    writer.WriteRaw(res);
                                                    if (!String.IsNullOrEmpty(output))
                                                        scm.Content = output;

                                                    if (control.Type == ControlType.Video)
                                                        writer.WriteElementString("IsMuted", scm.IsMuted.ToString());

                                                    if (control.Type == ControlType.PDF
                                                        || control.Type == ControlType.PPT)
                                                    {
                                                        writer.WriteElementString("PageDuration", scm.PageDuration.ToString());
                                                        writer.WriteElementString("DocumentFit", scm.DocumentFit);
                                                    }
                                                }
                                                else
                                                {
                                                    var scm = playlist as SetContentModel;
                                                    writer.WriteElementString("Content", scm.Content == null ? null : (playlist as SetContentModel).Content);
                                                    writer.WriteElementString("ContentSize", scm.ContentSize.ToString());
                                                    if (control.Type == ControlType.Video)
                                                        writer.WriteElementString("IsMuted", scm.IsMuted.ToString());
                                                }


                                                if (control.Type == ControlType.DataGrid && (playlist as SetContentModel).DataGrid != null)
                                                {
                                                    var dg = (playlist as SetContentModel).DataGrid;
                                                    writer.WriteStartElement("DataGrid");
                                                    writer.WriteElementString("Id", dg.Id);

                                                    writer.WriteStartElement("RowBackground");
                                                    writer.WriteRaw(BrushToXML(dg.RowBackground));
                                                    writer.WriteEndElement();

                                                    writer.WriteStartElement("BorderBrush");
                                                    writer.WriteRaw(BrushToXML(dg.BorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("BorderThickness", dg.BorderThickness.ToString());
                                                    writer.WriteElementString("RowCornerRadius", dg.RowCornerRadius.ToString());
                                                    writer.WriteElementString("RowMargin", dg.RowMargin.ToString());
                                                    writer.WriteElementString("LinesVisibility", dg.LinesVisibility.ToString());

                                                    writer.WriteStartElement("VerticalLineColour");
                                                    writer.WriteRaw(BrushToXML(dg.VerticalLineColour));
                                                    writer.WriteEndElement();

                                                    writer.WriteStartElement("HorizontalLineColour");
                                                    writer.WriteRaw(BrushToXML(dg.HorizontalLineColour));
                                                    writer.WriteEndElement();

                                                    writer.WriteElementString("AlternationCount", dg.AlternationCount.ToString());

                                                    writer.WriteStartElement("AlternatingRowBackground");
                                                    writer.WriteRaw(BrushToXML(dg.AlternatingRowBackground));
                                                    writer.WriteEndElement();

                                                    writer.WriteElementString("IsVisibleShadow", dg.IsVisibleShadow.ToString());
                                                    writer.WriteElementString("RowShadowEffect", (dg.RowShadowEffect == null) ? null : DropShadowEffectToXML(dg.RowShadowEffect));
                                                    writer.WriteElementString("HeaderSize", dg.HeaderSize.ToString());
                                                    writer.WriteElementString("HeaderHeight", dg.HeaderHeight.ToString());
                                                    writer.WriteStartElement("HeaderBackground");
                                                    writer.WriteRaw(BrushToXML(dg.HeaderBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeaderForeground");
                                                    writer.WriteRaw(BrushToXML(dg.HeaderForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeaderBorderBrush");
                                                    writer.WriteRaw(BrushToXML(dg.HeaderBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("HeaderBorderThickness", dg.HeaderBorderThickness.ToString());
                                                    writer.WriteElementString("HeaderCornerRadius", dg.HeaderCornerRadius.ToString());
                                                    writer.WriteElementString("HeaderMargin", dg.HeaderMargin.ToString());
                                                    if (dg.HeaderFontFamily != null) writer.WriteElementString("HeaderFontFamily", dg.HeaderFontFamily.ToString());
                                                    if (dg.HeaderFontWeight != null) writer.WriteElementString("HeaderFontWeight", dg.HeaderFontWeight.ToString());
                                                    if (dg.HeaderFontStyle != null) writer.WriteElementString("HeaderFontStyle", dg.HeaderFontStyle.ToString());
                                                    writer.WriteElementString("HeaderHorizontalAlignment", dg.HeaderHorizontalAlignment.ToString());
                                                    writer.WriteElementString("HeaderVerticalAlignment", dg.HeaderVerticalAlignment.ToString());
                                                    writer.WriteElementString("HeaderIsVisibleShadow", dg.HeaderIsVisibleShadow.ToString());
                                                    writer.WriteElementString("HeaderShadowEffect", (dg.HeaderShadowEffect == null) ? null : DropShadowEffectToXML(dg.HeaderShadowEffect));
                                                    writer.WriteElementString("MaxRows", dg.MaxRows.ToString());
                                                    writer.WriteElementString("RowHeight", dg.RowHeight.ToString());
                                                    writer.WriteElementString("RefreshTime", dg.RefreshTime.ToString());
                                                    if (dg.SelectedSource != null)
                                                    {
                                                        writer.WriteStartElement("Source");
                                                        writer.WriteElementString("Id", dg.SelectedSource.Id);
                                                        writer.WriteElementString("Name", dg.SelectedSource.Name);
                                                        writer.WriteStartElement("Columns");
                                                        foreach (var column in dg.SelectedSource.Columns)
                                                        {
                                                            writer.WriteStartElement("Column");
                                                            writer.WriteElementString("Id", column.Id);
                                                            writer.WriteElementString("Name", column.Name);
                                                            writer.WriteElementString("Title", column.Title);
                                                            writer.WriteElementString("Width", column.Width.ToString());
                                                            writer.WriteElementString("Type", column.Type.ToString());
                                                            writer.WriteStartElement("Background");
                                                            writer.WriteRaw(BrushToXML(column.Background));
                                                            writer.WriteEndElement();
                                                            writer.WriteStartElement("Foreground");
                                                            writer.WriteRaw(BrushToXML(column.Foreground));
                                                            writer.WriteEndElement();
                                                            writer.WriteElementString("TextAlignment", column.TextAlignment.ToString());
                                                            writer.WriteElementString("VerticalAlignment", column.VerticalAlignment.ToString());
                                                            if (column.FontFamily != null) writer.WriteElementString("FontFamily", column.FontFamily.ToString());
                                                            if (column.FontSize != null) writer.WriteElementString("FontSize", column.FontSize.ToString());
                                                            if (column.FontWeight != null) writer.WriteElementString("FontWeight", column.FontWeight.ToString());
                                                            if (column.FontStyle != null) writer.WriteElementString("FontStyle", column.FontStyle.ToString());
                                                            writer.WriteElementString("WhereOperator", column.WhereOperator);
                                                            writer.WriteElementString("WhereValue", column.WhereValue);
                                                            writer.WriteElementString("MergeColumn", column.MergeColumn);
                                                            writer.WriteElementString("Sort", column.Sort.ToString());
                                                            writer.WriteElementString("ImageStretch", column.ImageStretch.ToString());
                                                            writer.WriteElementString("IsVisible", column.IsVisible.ToString());
                                                            writer.WriteStartElement("SpecialCells");
                                                            foreach (var specialCell in column.SpecialCells)
                                                            {
                                                                writer.WriteStartElement("SpecialCell");
                                                                writer.WriteElementString("Id", specialCell.Id);
                                                                writer.WriteElementString("Text", specialCell.Text);
                                                                writer.WriteElementString("IsRow", specialCell.IsRow.ToString());
                                                                writer.WriteElementString("IsBlink", specialCell.IsBlink.ToString());
                                                                if (specialCell.Foreground != null)
                                                                {
                                                                    writer.WriteStartElement("Foreground");
                                                                    writer.WriteRaw(BrushToXML(specialCell.Foreground));
                                                                    writer.WriteEndElement();
                                                                }

                                                                if (specialCell.Foreground != null)
                                                                {
                                                                    writer.WriteStartElement("Background");
                                                                    writer.WriteRaw(BrushToXML(specialCell.Background));
                                                                    writer.WriteEndElement();
                                                                }
                                                                writer.WriteEndElement();
                                                            }
                                                            writer.WriteEndElement();

                                                            writer.WriteStartElement("TimeFilters");
                                                            foreach (var timeFilter in column.TimeFilters)
                                                            {
                                                                writer.WriteStartElement("TimeFilter");
                                                                writer.WriteElementString("Id", timeFilter.Id);
                                                                writer.WriteElementString("Name", timeFilter.Name);
                                                                writer.WriteElementString("BeforeDuration", timeFilter.BeforeDuration.ToString());
                                                                writer.WriteElementString("AfterDuration", timeFilter.AfterDuration.ToString());
                                                                writer.WriteEndElement();
                                                            }
                                                            writer.WriteEndElement();

                                                            writer.WriteEndElement();
                                                        }
                                                        writer.WriteEndElement();
                                                        writer.WriteEndElement();
                                                    }
                                                    writer.WriteEndElement();
                                                }
                                                else if (control.Type == ControlType.Weather && (playlist as SetContentModel).Weather != null)
                                                {
                                                    var w = (playlist as SetContentModel).Weather;
                                                    writer.WriteStartElement("Weather");
                                                    writer.WriteElementString("Id", w.Id);
                                                    writer.WriteElementString("TitleIsVisible", w.TitleIsVisible.ToString());
                                                    writer.WriteElementString("TitleSize", w.TitleSize.ToString());
                                                    writer.WriteStartElement("TitleBackground");
                                                    writer.WriteRaw(BrushToXML(w.TitleBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("TitleForeground");
                                                    writer.WriteRaw(BrushToXML(w.TitleForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("TitleBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.TitleBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("TitleBorderThickness", w.TitleBorderThickness.ToString());
                                                    if (w.TitleFontFamily != null) writer.WriteElementString("TitleFontFamily", w.TitleFontFamily.ToString());
                                                    if (w.TitleFontWeight != null) writer.WriteElementString("TitleFontWeight", w.TitleFontWeight.ToString());
                                                    if (w.TitleFontStyle != null) writer.WriteElementString("TitleFontStyle", w.TitleFontStyle.ToString());
                                                    writer.WriteElementString("TitleCornerRadius", w.TitleCornerRadius.ToString());
                                                    writer.WriteElementString("TitleText", w.TitleText);

                                                    writer.WriteElementString("DayTextIsVisible", w.DayTextIsVisible.ToString());
                                                    writer.WriteElementString("DayTextSize", w.DayTextSize.ToString());
                                                    writer.WriteStartElement("DayTextBackground");
                                                    writer.WriteRaw(BrushToXML(w.DayTextBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("DayTextForeground");
                                                    writer.WriteRaw(BrushToXML(w.DayTextForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("DayTextBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.DayTextBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("DayTextBorderThickness", w.DayTextBorderThickness.ToString());
                                                    if (w.DayTextFontFamily != null) writer.WriteElementString("DayTextFontFamily", w.DayTextFontFamily.ToString());
                                                    if (w.DayTextFontWeight != null) writer.WriteElementString("DayTextFontWeight", w.DayTextFontWeight.ToString());
                                                    if (w.DayTextFontStyle != null) writer.WriteElementString("DayTextFontStyle", w.DayTextFontStyle.ToString());
                                                    writer.WriteElementString("DayTextCornerRadius", w.DayTextCornerRadius.ToString());

                                                    writer.WriteElementString("WeatherTextIsVisible", w.WeatherTextIsVisible.ToString());
                                                    writer.WriteElementString("WeatherTextSize", w.WeatherTextSize.ToString());
                                                    writer.WriteStartElement("WeatherTextBackground");
                                                    writer.WriteRaw(BrushToXML(w.WeatherTextBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("WeatherTextForeground");
                                                    writer.WriteRaw(BrushToXML(w.WeatherTextForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("WeatherTextBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.WeatherTextBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("WeatherTextBorderThickness", w.WeatherTextBorderThickness.ToString());
                                                    if (w.WeatherTextFontFamily != null) writer.WriteElementString("WeatherTextFontFamily", w.WeatherTextFontFamily.ToString());
                                                    if (w.WeatherTextFontWeight != null) writer.WriteElementString("WeatherTextFontWeight", w.WeatherTextFontWeight.ToString());
                                                    if (w.WeatherTextFontStyle != null) writer.WriteElementString("WeatherTextFontStyle", w.WeatherTextFontStyle.ToString());
                                                    writer.WriteElementString("WeatherTextCornerRadius", w.WeatherTextCornerRadius.ToString());

                                                    writer.WriteElementString("HeightTextIsVisible", w.HeightTextIsVisible.ToString());
                                                    writer.WriteElementString("HeightTextSize", w.HeightTextSize.ToString());
                                                    writer.WriteStartElement("HeightTextBackground");
                                                    writer.WriteRaw(BrushToXML(w.HeightTextBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeightTextForeground");
                                                    writer.WriteRaw(BrushToXML(w.HeightTextForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeightTextBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.HeightTextBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("HeightTextBorderThickness", w.HeightTextBorderThickness.ToString());
                                                    if (w.HeightTextFontFamily != null) writer.WriteElementString("HeightTextFontFamily", w.HeightTextFontFamily.ToString());
                                                    if (w.HeightTextFontWeight != null) writer.WriteElementString("HeightTextFontWeight", w.HeightTextFontWeight.ToString());
                                                    if (w.HeightTextFontStyle != null) writer.WriteElementString("HeightTextFontStyle", w.HeightTextFontStyle.ToString());
                                                    writer.WriteElementString("HeightTextCornerRadius", w.HeightTextCornerRadius.ToString());

                                                    writer.WriteElementString("HeightValueIsVisible", w.HeightValueIsVisible.ToString());
                                                    writer.WriteElementString("HeightValueSize", w.HeightValueSize.ToString());
                                                    writer.WriteStartElement("HeightValueBackground");
                                                    writer.WriteRaw(BrushToXML(w.HeightValueBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeightValueForeground");
                                                    writer.WriteRaw(BrushToXML(w.HeightValueForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("HeightValueBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.HeightValueBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("HeightValueBorderThickness", w.HeightValueBorderThickness.ToString());
                                                    if (w.HeightValueFontFamily != null) writer.WriteElementString("HeightValueFontFamily", w.HeightValueFontFamily.ToString());
                                                    if (w.HeightValueFontWeight != null) writer.WriteElementString("HeightValueFontWeight", w.HeightValueFontWeight.ToString());
                                                    if (w.HeightValueFontStyle != null) writer.WriteElementString("HeightValueFontStyle", w.HeightValueFontStyle.ToString());
                                                    writer.WriteElementString("HeightValueCornerRadius", w.HeightValueCornerRadius.ToString());

                                                    writer.WriteElementString("LowTextIsVisible", w.LowTextIsVisible.ToString());
                                                    writer.WriteElementString("LowTextSize", w.LowTextSize.ToString());
                                                    writer.WriteStartElement("LowTextBackground");
                                                    writer.WriteRaw(BrushToXML(w.LowTextBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("LowTextForeground");
                                                    writer.WriteRaw(BrushToXML(w.LowTextForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("LowTextBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.LowTextBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("LowTextBorderThickness", w.LowTextBorderThickness.ToString());
                                                    if (w.LowTextFontFamily != null) writer.WriteElementString("LowTextFontFamily", w.LowTextFontFamily.ToString());
                                                    if (w.LowTextFontWeight != null) writer.WriteElementString("LowTextFontWeight", w.LowTextFontWeight.ToString());
                                                    if (w.LowTextFontStyle != null) writer.WriteElementString("LowTextFontStyle", w.LowTextFontStyle.ToString());
                                                    writer.WriteElementString("LowTextCornerRadius", w.LowTextCornerRadius.ToString());

                                                    writer.WriteElementString("LowValueIsVisible", w.LowValueIsVisible.ToString());
                                                    writer.WriteElementString("LowValueSize", w.LowValueSize.ToString());
                                                    writer.WriteStartElement("LowValueBackground");
                                                    writer.WriteRaw(BrushToXML(w.LowValueBackground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("LowValueForeground");
                                                    writer.WriteRaw(BrushToXML(w.LowValueForeground));
                                                    writer.WriteEndElement();
                                                    writer.WriteStartElement("LowValueBorderBrush");
                                                    writer.WriteRaw(BrushToXML(w.LowValueBorderBrush));
                                                    writer.WriteEndElement();
                                                    writer.WriteElementString("LowValueBorderThickness", w.LowValueBorderThickness.ToString());
                                                    if (w.LowValueFontFamily != null) writer.WriteElementString("LowValueFontFamily", w.LowValueFontFamily.ToString());
                                                    if (w.LowValueFontWeight != null) writer.WriteElementString("LowValueFontWeight", w.LowValueFontWeight.ToString());
                                                    if (w.LowValueFontStyle != null) writer.WriteElementString("LowValueFontStyle", w.LowValueFontStyle.ToString());
                                                    writer.WriteElementString("LowValueCornerRadius", w.LowValueCornerRadius.ToString());

                                                    writer.WriteEndElement();
                                                }
                                                break;
                                            case PlaylistType.Delay:
                                                break;
                                            case PlaylistType.AnimateBorder:
                                                writer.WriteElementString("BorderFrom", (playlist as AnimateBorderModel).BorderThicknessFrom.ToString());
                                                writer.WriteElementString("BorderTo", (playlist as AnimateBorderModel).BorderThicknessTo.ToString());
                                                break;
                                            case PlaylistType.AnimateHeight:
                                                writer.WriteElementString("HeightFrom", (playlist as AnimateHeightModel).HeightFrom.ToString());
                                                writer.WriteElementString("HeightTo", (playlist as AnimateHeightModel).HeightTo.ToString());
                                                break;
                                            case PlaylistType.AnimateMargin:
                                                writer.WriteElementString("MarginFrom", (playlist as AnimateMarginModel).MarginThicknessFrom.ToString());
                                                writer.WriteElementString("MarginTo", (playlist as AnimateMarginModel).MarginThicknessTo.ToString());
                                                break;
                                            case PlaylistType.AnimateOpacity:
                                                writer.WriteElementString("OpacityFrom", (playlist as AnimateOpacityModel).OpacityFrom.ToString());
                                                writer.WriteElementString("OpacityTo", (playlist as AnimateOpacityModel).OpacityTo.ToString());
                                                break;
                                            case PlaylistType.AnimateWidth:
                                                writer.WriteElementString("WidthFrom", (playlist as AnimateWidthModel).WidthFrom.ToString());
                                                writer.WriteElementString("WidthTo", (playlist as AnimateWidthModel).WidthTo.ToString());
                                                break;
                                            case PlaylistType.ResumePlayback:
                                                break;
                                            case PlaylistType.SuspendPlayback:
                                                break;
                                        }

                                        writer.WriteEndElement();
                                    }
                                    writer.WriteEndElement();
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    logger.Info((object)"Project To XML started 5");
                    //Alarm
                    writer.WriteStartElement("Alerts");
                    foreach (var alert in pm.Alerts)
                    {
                        writer.WriteStartElement("Alert");
                        writer.WriteElementString("Id", alert.Id);
                        writer.WriteElementString("Name", alert.Name);
                        writer.WriteElementString("Type", alert.Type.ToString());

                        writer.WriteStartElement("Controls");
                        foreach (var control in alert.Controls)
                        {
                            writer.WriteStartElement("Control");
                            writer.WriteAttributeString("Type", control.Type.ToString());
                            writer.WriteElementString("Id", control.Id);
                            writer.WriteElementString("Name", control.Name);
                            writer.WriteElementString("Width", control.Width.ToString());
                            writer.WriteElementString("Height", control.Height.ToString());
                            writer.WriteElementString("X", control.X.ToString());
                            writer.WriteElementString("Y", control.Y.ToString());
                            writer.WriteElementString("Margion", new Thickness(control.X, control.Y, control.W, control.Z).ToString());
                            writer.WriteElementString("Opacity", control.Opacity.ToString());
                            writer.WriteStartElement("Background");
                            writer.WriteRaw(BrushToXML(control.Background));
                            writer.WriteEndElement();
                            writer.WriteStartElement("BorderBrush");
                            writer.WriteRaw(BrushToXML(control.BorderBrush));
                            writer.WriteEndElement();
                            writer.WriteElementString("BorderThickness", control.BorderThickness.ToString());
                            writer.WriteElementString("CornerRadius", control.CornerRadius.ToString());
                            writer.WriteElementString("HorizontalAlignment", control.HorizontalAlignment.ToString());
                            writer.WriteElementString("VerticalAlignment", control.VerticalAlignment.ToString());
                            writer.WriteElementString("Stretch", control.Stretch.ToString());
                            writer.WriteElementString("HorizontalFlip", control.HorizontalFlip.ToString());
                            writer.WriteElementString("VerticalFlip", control.VerticalFlip.ToString());
                            writer.WriteElementString("Rotate", control.Rotate.ToString());
                            if (control.FontFamily != null) writer.WriteElementString("FontFamily", control.FontFamily.ToString());
                            if (control.FontSize != null) writer.WriteElementString("FontSize", control.FontSize.ToString());
                            if (control.Foreground != null)
                            {
                                writer.WriteStartElement("Foreground");
                                writer.WriteRaw(BrushToXML(control.Foreground));
                                writer.WriteEndElement();
                            }
                            if (control.Url != null) writer.WriteElementString("Url", control.Url.ToString());
                            if (control.FontWeight != null) writer.WriteElementString("FontWeight", control.FontWeight.ToString());
                            if (control.FontStyle != null) writer.WriteElementString("FontStyle", control.FontStyle.ToString());
                            if (control.TextDecorationText != null) writer.WriteElementString("TextDecorationText", control.TextDecorationText);
                            if (control.InvertDirection != null) writer.WriteElementString("InvertDirection", control.InvertDirection.ToString());
                            if (control.Duration != null) writer.WriteElementString("Duration", control.Duration.TotalSeconds.ToString());
                            writer.WriteElementString("Type", control.Type.ToString());
                            if (control.SelectedSocialMedia != null) writer.WriteElementString("SelectedSocialMedia", control.SelectedSocialMedia.Name);
                            writer.WriteElementString("MediaAccountId", control.MediaAccountId);
                            writer.WriteElementString("MediaPageName", control.MediaPageName);
                            writer.WriteElementString("Type", control.Type.ToString());
                            writer.WriteElementString("ZIndex", control.ZIndex.ToString());
                            writer.WriteElementString("IsVisible", control.IsVisible.ToString());
                            writer.WriteElementString("IsLocked", control.IsLocked.ToString());
                            writer.WriteStartElement("Playlists");
                            foreach (var playlist in control.Playlist)
                            {
                                writer.WriteStartElement("Playlist");
                                writer.WriteElementString("Id", playlist.Id);
                                writer.WriteElementString("Name", playlist.Name);
                                writer.WriteElementString("StartTime", playlist.StartTime.TotalSeconds.ToString());
                                writer.WriteElementString("Duration", playlist.Duration.TotalSeconds.ToString());
                                writer.WriteElementString("Forever", playlist.Forever.ToString());
                                if (playlist.Depends != null) writer.WriteElementString("Depends", playlist.Depends.Id);
                                writer.WriteElementString("Type", playlist.Type.ToString());
                                switch (playlist.Type)
                                {
                                    case PlaylistType.SetContent:
                                        if (control.Type == ControlType.PrayerText)
                                        {
                                            var scm = playlist as SetContentModel;
                                            writer.WriteElementString("Fajr", scm.Fajr == null ? null : scm.Fajr);
                                            writer.WriteElementString("Dhuhr", scm.Fajr == null ? null : scm.Dhuhr);
                                            writer.WriteElementString("Asr", scm.Fajr == null ? null : scm.Asr);
                                            writer.WriteElementString("Maghrib", scm.Fajr == null ? null : scm.Maghrib);
                                            writer.WriteElementString("Isha", scm.Fajr == null ? null : scm.Isha);
                                        }
                                        else if (control.Type == ControlType.PrayerImage || control.Type == ControlType.PrayerVideo)
                                        {
                                            var scm = playlist as SetContentModel;
                                            logger.Info($"Arif Fajr {scm.Fajr} Size : {scm.FajrSize}");
                                            string output = String.Empty;
                                            string res = "<Fajr>" + scm.Fajr + "</Fajr><FajrSize>" + scm.FajrSize + "</FajrSize>";
                                            writer.WriteRaw(res);
                                            //writer.WriteRaw(StoreSourceXML("Fajr", true, scm.Fajr, scm.FajrSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Fajr = output;

                                            output = String.Empty;
                                            logger.Info($"Arif Fajr {scm.DhuhrSize} Size : {scm.DhuhrSize}");
                                            res = "<Dhuhr>" + scm.Dhuhr + "</Dhuhr><DhuhrSize>" + scm.DhuhrSize + "</DhuhrSize>";
                                            writer.WriteRaw(res);
                                            //writer.WriteRaw(StoreSourceXML("Dhuhr", true, scm.Dhuhr, scm.DhuhrSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Dhuhr = output;

                                            output = String.Empty;
                                            logger.Info($"Arif Asr {scm.Asr} Size : {scm.AsrSize}");
                                            res = "<Asr>" + scm.Asr + "</Asr><AsrSize>" + scm.AsrSize + "</AsrSize>";
                                            writer.WriteRaw(res);
                                            //writer.WriteRaw(StoreSourceXML("Asr", true, scm.Asr, scm.AsrSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Asr = output;

                                            output = String.Empty;
                                            logger.Info($"Arif Maghrib {scm.Maghrib} Size : {scm.MaghribSize}");
                                            res = "<Maghrib>" + scm.Maghrib + "</Maghrib><MaghribSize>" + scm.MaghribSize + "</MaghribSize>";
                                            writer.WriteRaw(res);
                                            //writer.WriteRaw(StoreSourceXML("Maghrib", true, scm.Maghrib, scm.MaghribSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Maghrib = output;

                                            output = String.Empty;
                                            logger.Info($"Arif Isha {scm.Isha} Size : {scm.IshaSize}");
                                            res = "<Isha>" + scm.Isha + "</Isha><IshaSize>" + scm.IshaSize + "</IshaSize>";
                                            writer.WriteRaw(res);
                                            //writer.WriteRaw(StoreSourceXML("Isha", true, scm.Isha, scm.IshaSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Isha = output;
                                        }
                                        else if (control.Type == ControlType.Image || control.Type == ControlType.Video)
                                        {
                                            string output = String.Empty;
                                            var scm = playlist as SetContentModel;
                                            string res = "<Content>" + scm.Content + "</Content><ContentSize>" + scm.ContentSize + "</ContentSize>";
                                            writer.WriteRaw(res);

                                            //writer.WriteRaw(StoreSourceXML("Content", true, scm.Content, scm.ContentSize, filelist, projectDir, out output));
                                            if (!String.IsNullOrEmpty(output))
                                                scm.Content = output;

                                            if (control.Type == ControlType.Video)
                                                writer.WriteElementString("IsMuted", scm.IsMuted.ToString());
                                        }
                                        else
                                        {
                                            var scm = playlist as SetContentModel;
                                            writer.WriteElementString("Content", scm.Content == null ? null : (playlist as SetContentModel).Content);
                                            writer.WriteElementString("ContentSize", scm.ContentSize.ToString());
                                            writer.WriteElementString("IsMuted", scm.IsMuted.ToString());
                                        }
                                        break;
                                    case PlaylistType.Delay:
                                        break;
                                    case PlaylistType.AnimateBorder:
                                        writer.WriteElementString("BorderFrom", (playlist as AnimateBorderModel).BorderThicknessFrom.ToString());
                                        writer.WriteElementString("BorderTo", (playlist as AnimateBorderModel).BorderThicknessTo.ToString());
                                        break;
                                    case PlaylistType.AnimateHeight:
                                        writer.WriteElementString("HeightFrom", (playlist as AnimateHeightModel).HeightFrom.ToString());
                                        writer.WriteElementString("HeightTo", (playlist as AnimateHeightModel).HeightTo.ToString());
                                        break;
                                    case PlaylistType.AnimateMargin:
                                        writer.WriteElementString("MarginFrom", (playlist as AnimateMarginModel).MarginThicknessFrom.ToString());
                                        writer.WriteElementString("MarginTo", (playlist as AnimateMarginModel).MarginThicknessTo.ToString());
                                        break;
                                    case PlaylistType.AnimateOpacity:
                                        writer.WriteElementString("OpacityFrom", (playlist as AnimateOpacityModel).OpacityFrom.ToString());
                                        writer.WriteElementString("OpacityTo", (playlist as AnimateOpacityModel).OpacityTo.ToString());
                                        break;
                                    case PlaylistType.AnimateWidth:
                                        writer.WriteElementString("WidthFrom", (playlist as AnimateWidthModel).WidthFrom.ToString());
                                        writer.WriteElementString("WidthTo", (playlist as AnimateWidthModel).WidthTo.ToString());
                                        break;
                                    case PlaylistType.ResumePlayback:
                                        break;
                                    case PlaylistType.SuspendPlayback:
                                        break;
                                }

                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        logger.Info((object)"Project To XML started 6");
                        writer.WriteStartElement("Slots");
                        foreach (var slot in alert.Slots)
                        {
                            writer.WriteStartElement("Slot");
                            writer.WriteElementString("Id", slot.Id);
                            writer.WriteElementString("Name", slot.Name);
                            writer.WriteElementString("StartTime", slot.Start.Ticks.ToString());
                            writer.WriteStartElement("Location");
                            writer.WriteElementString("Id", slot.Location.Id);
                            writer.WriteElementString("Country", slot.Location.Country);
                            writer.WriteElementString("City", slot.Location.City);
                            writer.WriteElementString("Latitude", slot.Location.Latitude.ToString());
                            writer.WriteElementString("Longnitude", slot.Location.Longnitude.ToString());
                            writer.WriteEndElement();
                            writer.WriteElementString("Duration", slot.Duration.TotalSeconds.ToString());
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    sw.Flush();
                }
                logger.Info((object)"Project To XML started 7");
                string txt = sw.ToString();
                XElement text = XElement.Load("");
                //File.Delete(path);
                if (IsEncrypted)
                {
                    string cryptedText = Crypt.Encrypt(text.ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
                    result = cryptedText;
                }
                else
                    result = text.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
            }
            catch (Exception e)
            {
                logger.Error("Problem with converting project to XML", e);
                return string.Empty;
            }
            logger.Info("Project converted to XML");
            return result;
        }

        public static string StoreSourceXML2(string name, bool sizeCheck, string content, long contentSize, Dictionary<string, string> filelist, out string output)
        {
            using (var sw = new StringWriter())
            {
                string result = String.Empty;
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                using (var writer = XmlWriter.Create(sw, settings))
                {
                    if (String.IsNullOrWhiteSpace(content) || !File.Exists(content))
                    {
                        writer.WriteElementString(name, null);
                        if (sizeCheck)
                            writer.WriteElementString($"{name}Size", null);

                        output = String.Empty;
                        result = sw.ToString();
                    }
                    else if (filelist.ContainsKey(content))
                    {
                        writer.WriteElementString(name, filelist[content]);
                        //output = Path.Combine(projectDir, filelist[content]);
                        output = filelist[content];
                        result = sw.ToString();
                    }
                    else
                    {
                        //check if file exist in project folder
                        string fileName = String.Empty;
                        string fullPath = String.Empty;
                        //if (Path.GetDirectoryName(content) == projectDir)
                        //{
                        fileName = Path.GetFileName(content);
                        fullPath = content;
                        //}
                        //else
                        //{
                        //    //Copy to project folder;
                        //    fileName = Guid.NewGuid().ToString() + Path.GetExtension(content);
                        //    fullPath = Path.Combine(projectDir, fileName);
                        //    if (!File.Exists(fullPath) || (sizeCheck && contentSize > 0 && contentSize != (new System.IO.FileInfo(fullPath).Length)))
                        //        File.Copy(content, fullPath, true);
                        //}

                        writer.WriteElementString(name, fileName);
                        if (sizeCheck) writer.WriteElementString($"{name}Size", contentSize.ToString());
                        filelist.Add(content, fileName);
                        output = fullPath;
                        result = sw.ToString();
                    }
                }
                return sw.ToString();
            }
        }

        public static string StoreSourceXML(string name, bool sizeCheck, string content, long contentSize, Dictionary<string, string> filelist, string projectDir, out string output)
        {
            using (var sw = new StringWriter())
            {
                string result = String.Empty;
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                using (var writer = XmlWriter.Create(sw, settings))
                {
                    if (String.IsNullOrWhiteSpace(content) || !File.Exists(content))
                    {
                        writer.WriteElementString(name, null);
                        if (sizeCheck)
                            writer.WriteElementString($"{name}Size", null);

                        output = String.Empty;
                        result = sw.ToString();
                    }
                    else if (filelist.ContainsKey(content))
                    {
                        writer.WriteElementString(name, filelist[content]);
                        output = Path.Combine(projectDir, filelist[content]);
                        result = sw.ToString();
                    }
                    else
                    {
                        //check if file exist in project folder
                        string fileName = String.Empty;
                        string fullPath = String.Empty;
                        if (Path.GetDirectoryName(content) == projectDir)
                        {
                            fileName = Path.GetFileName(content);
                            fullPath = content;
                        }
                        else
                        {
                            //Copy to project folder;
                            fileName = Guid.NewGuid().ToString() + Path.GetExtension(content);
                            fullPath = Path.Combine(projectDir, fileName);
                            if (!File.Exists(fullPath) || (sizeCheck && contentSize > 0 && contentSize != (new System.IO.FileInfo(fullPath).Length)))
                                File.Copy(content, fullPath, true);
                        }

                        writer.WriteElementString(name, fileName);
                        if (sizeCheck) writer.WriteElementString($"{name}Size", contentSize.ToString());
                        filelist.Add(content, fileName);
                        output = fullPath;
                        result = sw.ToString();
                    }
                }
                return sw.ToString();
            }
        }
        #endregion
    }
}