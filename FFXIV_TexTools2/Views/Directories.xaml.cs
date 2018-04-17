﻿// FFXIV TexTools
// Copyright © 2017 Rafael Gonzalez - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using FFXIV_TexTools2.Helpers;
using FolderSelect;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace FFXIV_TexTools2.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DirectoriesView : Window
    {
        public DirectoriesView()
        {
            InitializeComponent();
            DataContext = Properties.Settings.Default;

            saveDir.Text = Path.GetFullPath(Properties.Settings.Default.Save_Directory);

            FFXIVDir.Text = Path.GetFullPath(Properties.Settings.Default.FFXIV_Directory);

            modListDir.Text = Path.GetFullPath(Properties.Settings.Default.Modlist_Directory);

            indexBackupsDir.Text = Path.GetFullPath(Properties.Settings.Default.IndexBackups_Directory);

            ModPackDir.Text = Path.GetFullPath(Properties.Settings.Default.ModPack_Directory);
        }

        private void FFXIVDirButton_Click(object sender, RoutedEventArgs e)
        {
            FolderSelectDialog folderSelect = new FolderSelectDialog()
            {
                Title = "Select ffxiv folder"
            };
            var result = folderSelect.ShowDialog();
            if (result)
            {
                Properties.Settings.Default.FFXIV_Directory = folderSelect.FileName;
                Properties.Settings.Default.Save();

                FFXIVDir.Text = folderSelect.FileName;
            }
        }

        private void SaveDirButton_Click(object sender, RoutedEventArgs e)
        {
            var oldSaveLocation = saveDir.Text;
            FolderSelectDialog folderSelect = new FolderSelectDialog()
            {
                Title = "Select new location of save folder"
            };
            folderSelect.InitialDirectory = oldSaveLocation;

            var result = folderSelect.ShowDialog();
            if (result)
            {
                if (FlexibleMessageBox.Show("Would you like to move the existing data to the new location?", "Move Data?",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        Directory.Move(oldSaveLocation, folderSelect.FileName);
                    }
                    catch
                    {
                        var newLoc = folderSelect.FileName;
                        Directory.CreateDirectory(newLoc);

                        foreach (string dirPath in Directory.GetDirectories(oldSaveLocation, "*", SearchOption.AllDirectories))
                        {
                            Directory.CreateDirectory(dirPath.Replace(oldSaveLocation, newLoc));
                        }

                        foreach (string newPath in Directory.GetFiles(oldSaveLocation, "*.*", SearchOption.AllDirectories))
                        {
                            File.Copy(newPath, newPath.Replace(oldSaveLocation, newLoc), true);
                        }

                        DeleteDirectory(oldSaveLocation);
                    }
                }
                else
                {
                    Directory.CreateDirectory(folderSelect.FileName);
                }

                Properties.Settings.Default.Save_Directory = folderSelect.FileName;
                Properties.Settings.Default.Save();

               FlexibleMessageBox.Show("Location of Saved folder changed.\n\n" +
                    "New Location: " + folderSelect.FileName, "New Directory",MessageBoxButtons.OK,MessageBoxIcon.Information);
                saveDir.Text = folderSelect.FileName;
            }
        }

        private void indexbackupDirButton_Click(object sender, RoutedEventArgs e)
        {
            var oldIndexBackupLocation = indexBackupsDir.Text;
            FolderSelectDialog folderSelect = new FolderSelectDialog()
            {
                Title = "Select new location of Index Backup folder"
            };
            folderSelect.InitialDirectory = oldIndexBackupLocation;
            var result = folderSelect.ShowDialog();
            if (result)
            {
                Properties.Settings.Default.IndexBackups_Directory = folderSelect.FileName + "\\Index_Backups";
                Properties.Settings.Default.Save();

                if (FlexibleMessageBox.Show("Would you like to move the existing data to the new location?", "Move Data?",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        Directory.Move(oldIndexBackupLocation, folderSelect.FileName);

                    }
                    catch
                    {
                        var newLoc = folderSelect.FileName + "\\Index_Backups";
                        Directory.CreateDirectory(newLoc);

                        foreach (string dirPath in Directory.GetDirectories(oldIndexBackupLocation, "*", SearchOption.AllDirectories))
                        {
                            Directory.CreateDirectory(dirPath.Replace(oldIndexBackupLocation, newLoc));
                        }

                        foreach (string newPath in Directory.GetFiles(oldIndexBackupLocation, "*.*", SearchOption.AllDirectories))
                        {
                            File.Copy(newPath, newPath.Replace(oldIndexBackupLocation, newLoc), true);
                        }

                        DeleteDirectory(oldIndexBackupLocation);
                    }
                }

               FlexibleMessageBox.Show("Location of Index Backup folder changed.\n\n" +
                    "New Location: " + folderSelect.FileName + "\\Index_Backups", "New Directory",MessageBoxButtons.OK,MessageBoxIcon.Information);
                indexBackupsDir.Text = folderSelect.FileName + "\\Index_Backups";
            }
        }

        private void modlistDirButton_Click(object sender, RoutedEventArgs e)
        {
            var oldModListLocation = modListDir.Text;
            FolderSelectDialog folderSelect = new FolderSelectDialog()
            {
                Title = "Select new location of Mod List file"
            };
            folderSelect.InitialDirectory = oldModListLocation;
            var result = folderSelect.ShowDialog();
            if (result)
            {
                var fullLoc = folderSelect.FileName + "\\TexTools.modlist";

                Properties.Settings.Default.Modlist_Directory = fullLoc;
                Properties.Settings.Default.Save();

                try
                {
                    File.Move(oldModListLocation, fullLoc);
                }
                catch
                {
                    File.Copy(oldModListLocation, fullLoc, true);
                    File.Delete(oldModListLocation);
                }

               FlexibleMessageBox.Show("Location of ModList file changed.\n\n" +
                    "New Location: " + fullLoc, "New Directory",MessageBoxButtons.OK,MessageBoxIcon.Information);
                modListDir.Text = fullLoc;
            }
        }

        private void ModPackButton_Click(object sender, RoutedEventArgs e)
        {
            var oldSaveLocation = ModPackDir.Text;
            FolderSelectDialog folderSelect = new FolderSelectDialog()
            {
                Title = "Select new location of ModPack folder"
            };
            folderSelect.InitialDirectory = oldSaveLocation;

            var result = folderSelect.ShowDialog();
            if (result)
            {
                if (FlexibleMessageBox.Show("Would you like to move the existing data to the new location?", "Move Data?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        Directory.Move(oldSaveLocation, folderSelect.FileName);
                    }
                    catch
                    {
                        var newLoc = folderSelect.FileName + "\\ModPacks";
                        Directory.CreateDirectory(newLoc);

                        foreach (string dirPath in Directory.GetDirectories(oldSaveLocation, "*", SearchOption.AllDirectories))
                        {
                            Directory.CreateDirectory(dirPath.Replace(oldSaveLocation, newLoc));
                        }

                        foreach (string newPath in Directory.GetFiles(oldSaveLocation, "*.*", SearchOption.AllDirectories))
                        {
                            File.Copy(newPath, newPath.Replace(oldSaveLocation, newLoc), true);
                        }

                        DeleteDirectory(oldSaveLocation);
                    }
                }
                else
                {
                    Directory.CreateDirectory(folderSelect.FileName + "\\ModPacks");
                }

                Properties.Settings.Default.ModPack_Directory = folderSelect.FileName + "\\ModPacks";
                Properties.Settings.Default.Save();

                FlexibleMessageBox.Show("Location of ModPacks folder changed.\n\n" +
                     "New Location: " + folderSelect.FileName + "\\ModPacks", "New Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ModPackDir.Text = folderSelect.FileName + "\\ModPacks";
            }
        }

        private static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
