using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ImageTools;
using ImageTools.IO;
using ImageTools.IO.Png;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace HazeWP
{
    public partial class DynamicTile : UserControl
    {
        /// <summary>
        /// Identifies <see cref="TextProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty APIProperty = DependencyProperty.Register("API", typeof(string), typeof(DynamicTile), null);

        public static readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(string), typeof(DynamicTile), null);

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(string), typeof(DynamicTile), null);

        /// <summary>
        /// Gets or sets the text displayed in the tile.
        /// This is a dependency property.
        /// </summary>
        public string API
        {
            get { return (string)GetValue(APIProperty); }
            set { SetValue(APIProperty, value); }
        }

        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public DynamicTile()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// Used to render the contents to a tile
        /// </summary>
        /// <returns>a <see cref="StandardTileData"/> with the contents of this control</returns>
        public StandardTileData ToTile()
        {
            // Need to call these, otherwise the contents aren't rendered correctly.
            this.Measure(new Size(173, 173));
            this.Arrange(new Rect(0, 0, 173, 173));

            // The png encoder is the work of the ImageTools API. http://imagetools.codeplex.com/
            ExtendedImage tileImaged = this.ToImage();

            Encoders.AddEncoder<PngEncoder>();

            var p = new PngEncoder();

            const string tempFileName = "/Shared/ShellContent/tileImage.png";

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(tempFileName))
                {
                    myIsolatedStorage.DeleteFile(tempFileName);
                }

                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(tempFileName);
                p.Encode(tileImaged, fileStream);
                fileStream.Close();
            }

            var newTile = new StandardTileData
            {
                Title = "",
                BackgroundImage =
                    new Uri("isostore:" + tempFileName, UriKind.RelativeOrAbsolute)
            };

            return newTile;
        }
    }
}
