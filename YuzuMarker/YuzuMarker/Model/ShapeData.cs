using YuzuMarker.Common;

namespace YuzuMarker.Model
{
    public class ShapeData : NotifyObject
    {
        private float _x;

        public float X
        {
            get => _x;
            set => SetProperty(value);
        }
        
        private float _y;
        
        public float Y
        {
            get => _y;
            set => SetProperty(value);
        }
        
        private float _width;
        
        public float Width
        {
            get => _width;
            set => SetProperty(value);
        }
        
        private float _height;
        
        public float Height
        {
            get => _height;
            set => SetProperty(value);
        }

        public ShapeData()
        {
            X = Y = Width = Height = 0;
        }
    }
}