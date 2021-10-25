using System.Collections.ObjectModel;
using YuzuMarker.BasicDataFormat;

namespace YuzuMarker.DataFormat
{
    public class YuzuProject : BasicYuzuProject
    {
        public YuzuProject(string path, string fileName, string projectName) : base(path, fileName, projectName) { }

        public YuzuProject(string path, string fileName, string projectName,
            ObservableCollection<BasicYuzuImage> images) : base(path, fileName, projectName, images) { }

        public override string CreateNewImageAt(int index, string imagePath)
        {
            var imageFileName = CopyImage(imagePath);

            Images.Insert(index, new YuzuImage(this.Path, imageFileName, false));
            return imageFileName;
        }

        public override string CreateNewImage(string imagePath)
        {
            var imageFileName = CopyImage(imagePath);

            Images.Add(new YuzuImage(this.Path, imageFileName, false));
            return imageFileName;
        }
    }
}
