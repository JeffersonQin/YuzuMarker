using YuzuMarker.Common;

namespace YuzuMarker.Model
{
    public class ShapeData : NotifyObject
    {
        private float _x;

        public float X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }
        
        private float _y;
        
        public float Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }
        
        private float _width;
        
        public float Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }
        
        private float _height;
        
        public float Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public ShapeData()
        {
            X = Y = Width = Height = 0;
        }
    }
}